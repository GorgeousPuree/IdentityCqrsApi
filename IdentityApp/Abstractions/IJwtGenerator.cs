using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityApp.Abstractions
{
    public interface IJwtGenerator
    {
        string GenerateJwt(IEnumerable<Claim> claims = null);
    }
}
