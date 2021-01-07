using IdentityApp.Abstractions;
using IdentityApp.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityApp.Services
{
    /// <summary>
    /// Generates a user jwt.
    /// </summary>
    public class JwtGenerator : IJwtGenerator
    {
        private readonly JwtOptions _jwtOptionsMonitor;

        public JwtGenerator(IOptionsMonitor<JwtOptions> jwtOptionsMonitor)
        {
            _jwtOptionsMonitor = jwtOptionsMonitor.CurrentValue;
        }

        public string GenerateJwt(IEnumerable<Claim> claims = null)
        {
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptionsMonitor.Issuer,
                //audience: _jwtOptionsMonitor.Audience,
                notBefore: now,
                claims: claims,
                expires: now.Add(TimeSpan.FromMinutes(_jwtOptionsMonitor.Lifetime)),
                signingCredentials: new SigningCredentials(_jwtOptionsMonitor.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }
    }
}
