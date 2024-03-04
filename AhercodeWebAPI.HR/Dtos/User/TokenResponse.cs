using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AhercodeWebAPI.HR.Dtos.User
{
    public class TokenResponse
    {
        public string JWTToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
