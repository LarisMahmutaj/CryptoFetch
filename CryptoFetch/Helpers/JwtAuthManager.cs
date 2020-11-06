using CryptoFetch.Data;
using CryptoFetch.Helpers;
using CryptoFetch.Interfaces;
using CryptoFetch.Models;
using Microsoft.AspNetCore.Mvc;
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

namespace CryptoFetch.Services {
    
    public class JwtAuthManager : IJwtAuthManager{
        private readonly string key;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IServiceScopeFactory _scopeFactory;

        public JwtAuthManager(string key, IRefreshTokenGenerator refreshTokenGenerator, IServiceScopeFactory scopeFactory) {
            this.key = key;
            _refreshTokenGenerator = refreshTokenGenerator;
            _scopeFactory = scopeFactory;
        }

        public async Task<AuthenticationResponse> Authenticate(string email, Claim[] claims) {
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var jwtSecurityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            var refreshToken = _refreshTokenGenerator.GenerateToken();
            using(var scope = _scopeFactory.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var exists = context.RefreshTokens.Any(x => x.UserEmail == email);
                if (exists) {
                    var rf = await context.RefreshTokens.Where(x => x.UserEmail == email).FirstOrDefaultAsync();
                    rf.Token = refreshToken;
                    context.Update(rf);
                    await context.SaveChangesAsync();
                }
                else {
                    context.RefreshTokens.Add(new RefreshToken
                    {
                        Token = refreshToken,
                        UserEmail = email
                    });

                    await context.SaveChangesAsync();
                }

            }

            return new AuthenticationResponse
            {
                JwtToken = token,
                RefreshToken = refreshToken
            };

        }

        public async Task<AuthenticationResponse> Authenticate(string email) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = _refreshTokenGenerator.GenerateToken();

            using(var scope = _scopeFactory.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var exists = context.RefreshTokens.Any(x => x.UserEmail == email);
                if (exists) {
                    var rf = await context.RefreshTokens.Where(x => x.UserEmail == email).FirstOrDefaultAsync();
                    rf.Token = refreshToken;
                    context.Update(rf);
                    await context.SaveChangesAsync();
                }
                else {
                    context.RefreshTokens.Add(new RefreshToken
                    {
                        Token = refreshToken,
                        UserEmail = email
                    });

                    await context.SaveChangesAsync();
                }

            }
            return new AuthenticationResponse {
                JwtToken = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken
            };

        }
    }
}
