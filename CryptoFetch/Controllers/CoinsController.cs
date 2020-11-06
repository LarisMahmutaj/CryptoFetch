using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CryptoFetch.Data;
using CryptoFetch.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CryptoFetch.Controllers {
    [Authorize]
    [Route("api/coins")]
    [ApiController]
    public class CoinsController : ControllerBase {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ApplicationDbContext _context;

        public CoinsController(IHttpClientFactory clientFactory, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) {
            _clientFactory = clientFactory;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<ActionResult<Coin[]>> GetCoins() {
            var client = _clientFactory.CreateClient();
            try {
                HttpResponseMessage response = await client.GetAsync("https://api.coincap.io/v2/assets");
                if (response.IsSuccessStatusCode) {
                    var assetList = response.Content.ReadFromJsonAsync<AssetList>().Result.data;
                    return assetList;
                }
            }catch(Exception e) {
                return NotFound($"Error: {e}");
            }

            return BadRequest();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Coin>> GetCoin(string id) {
            var client = _clientFactory.CreateClient();
            try{
                var response = await client.GetAsync($"https://api.coincap.io/v2/assets/{id}");

                if (response.IsSuccessStatusCode) {
                    var asset = response.Content.ReadFromJsonAsync<Asset>().Result.data;
                    return asset;
                }

            }catch(Exception e) {
                return NotFound($"Error: {e}");
            }

            return BadRequest();
        }

        [HttpGet("favourites")]
        public async Task<ActionResult<List<Favourite>>> GetFavourites() {
            var userEmail = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            Console.WriteLine(userEmail);

            var list = await _context.Favourites.Where(x => x.UserEmail == userEmail).ToListAsync();
            return list;
        }

        [HttpPost("favourites")]
        public async Task<ActionResult<Favourite>> MarkFavourite([FromBody] string id) {
            var client = _clientFactory.CreateClient();
            try {
                var response = await client.GetAsync($"https://api.coincap.io/v2/assets/{id}");

                if (response.IsSuccessStatusCode) {
                    var asset = response.Content.ReadFromJsonAsync<Asset>().Result.data;
                    if(asset == null) {
                        return NotFound();
                    }
                    Favourite fav = new Favourite
                    {
                        CoinId = id,
                        UserEmail = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value
                    };

                    if(_context.Favourites.Any(x => x.CoinId == fav.CoinId && x.UserEmail == fav.UserEmail)) {
                        return BadRequest();
                    }

                    _context.Favourites.Add(fav);
                    await _context.SaveChangesAsync();
                    return Ok(fav);
                }

            }
            catch (Exception e) {
                return NotFound($"Error: {e}");
            }

            return BadRequest();
        }

        [HttpDelete("favourites/{id}")]
        public async Task<ActionResult<Favourite>> UnmarkFavourite(string id) {
            var userEmail = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            var fav = await _context.Favourites.Where(x => x.CoinId == id && x.UserEmail == userEmail).FirstOrDefaultAsync();
            if(fav == null) {
                return NotFound();
            }

            _context.Favourites.Remove(fav);
            await _context.SaveChangesAsync();
            return Ok(fav);
        }
    }
}
