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

        public AuthController(ApplicationDbContext context, IJwtAuthManager jwtAuthManager) {
            _context = context;
            _jwtAuthManager = jwtAuthManager;
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
                var token = _jwtAuthManager.Authenticate(credentials.Email);
                return Ok(token);
            }
            else {
                return Unauthorized();
            }
        }
    }
}
