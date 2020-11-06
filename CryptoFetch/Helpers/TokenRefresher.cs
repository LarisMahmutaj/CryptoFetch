using CryptoFetch.Data;
using CryptoFetch.Interfaces;
using CryptoFetch.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CryptoFetch.Helpers {
    public class TokenRefresher : ITokenRefresher{

        private readonly string key;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IServiceScopeFactory _scopeFactory;

        public TokenRefresher(string key, IJwtAuthManager jwtAuthManager, IServiceScopeFactory scopeFactory) {
            this.key = key;
            _jwtAuthManager = jwtAuthManager;
            _scopeFactory = scopeFactory;
        }

        public async Task<AuthenticationResponse> Refresh(RefreshCredentials refreshCredentials) {
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;

            var principal = tokenHandler.ValidateToken(refreshCredentials.JwtToken, new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            }, out validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;

            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
                throw new SecurityTokenException("Invalid token passed!"); 
            }

            var email = principal.FindFirst(ClaimTypes.Email).Value;
            using(var scope = _scopeFactory.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var dbToken = await context.RefreshTokens.Where(x => x.UserEmail == email).FirstOrDefaultAsync();
                if(dbToken.Token != refreshCredentials.RefreshToken) {
                    throw new SecurityTokenException("Invalid token passed!");
                }
                return await _jwtAuthManager.Authenticate(email, principal.Claims.ToArray());
            }
            
        }
    }
}
