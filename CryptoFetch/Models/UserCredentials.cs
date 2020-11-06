using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoFetch.Models {
    public class UserCredentials {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RefreshCredentials {
        public string JwtToken { get; set; }
        public string RefreshToken  { get; set; }
    }
}
