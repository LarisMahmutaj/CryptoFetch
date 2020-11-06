using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoFetch.Models {
    public class AssetList {
        public Coin[] data { get; set; }
    }

    public class Asset {
        public Coin data { get; set; }
    }
}
