using IdentityApp.Infrastructure.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityApp.Infrastructure.Helpers.Auth
{
    /// <summary>
    /// Generates a user jwt.
    /// </summary>
    public static class JwtGenerator
    {
        public static string Generate(AuthOptions authOptions)
        {
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: authOptions.Issuer,
                //audience: _authOptions.Audience,
                notBefore: now,
                expires: now.Add(TimeSpan.FromMinutes(authOptions.Lifetime)),
                signingCredentials: new SigningCredentials(authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }
    }
}
