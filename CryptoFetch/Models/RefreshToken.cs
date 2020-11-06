using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoFetch.Models {
    public class RefreshToken {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string Token { get; set; }
    }
}
