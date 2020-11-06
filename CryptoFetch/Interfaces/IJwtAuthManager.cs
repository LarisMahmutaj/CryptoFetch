using CryptoFetch.Data;
using CryptoFetch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CryptoFetch.Interfaces {
    public interface IJwtAuthManager {
        Task<AuthenticationResponse> Authenticate(string email);
        Task<AuthenticationResponse> Authenticate(string email, Claim[] claims);
    }
}
