using CryptoFetch.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CryptoFetch.Helpers {
    public class RefreshTokenGenerator: IRefreshTokenGenerator {
        public string GenerateToken() {
            var rand = new byte[32];

            using( var randomNumberGenerator = RandomNumberGenerator.Create()) {
                randomNumberGenerator.GetBytes(rand);
                return Convert.ToBase64String(rand);
            }
        }
    }
}
