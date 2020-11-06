using CryptoFetch.Data;
using CryptoFetch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoFetch.Interfaces {
    public interface ITokenRefresher {
        Task<AuthenticationResponse> Refresh(RefreshCredentials refreshCredentials);
    }
}
