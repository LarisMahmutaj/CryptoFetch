using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using CryptoFetch.Data;
using CryptoFetch.Interfaces;
using CryptoFetch.Models;
using CryptoFetch.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CryptoFetch.Controllers {
    
    [Authorize]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase {

        private readonly ApplicationDbContext _context;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly ITokenRefresher _tokenRefresher;

        public AuthController(ApplicationDbContext context, IJwtAuthManager jwtAuthManager, ITokenRefresher tokenRefresher) {
            _context = context;
            _jwtAuthManager = jwtAuthManager;
            _tokenRefresher = tokenRefresher;
        }
        
        [HttpGet]
        public string Get() {
            return "value";
        } 

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]UserCredentials credentials) {

            var user = await _context.Users.Where(u => u.Email == credentials.Email).FirstOrDefaultAsync();

            if (user == null) {
                return NotFound();
            }

            if(user.Password == credentials.Password) {
                var token = await _jwtAuthManager.Authenticate(credentials.Email);
                return Ok(token);
            }
            else {
                return Unauthorized();
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshCredentials refreshCredentials) {
            var token = await _tokenRefresher.Refresh(refreshCredentials);
            if(token == null) {
                return Unauthorized();
            }
            return Ok(token);
        }
    }
}
