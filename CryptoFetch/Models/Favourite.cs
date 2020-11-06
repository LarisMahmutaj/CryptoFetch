using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoFetch.Models {
    public class Favourite {
        public int Id { get; set; }
        public string CoinId { get; set; }
        public string UserEmail { get; set; }
    }
}
