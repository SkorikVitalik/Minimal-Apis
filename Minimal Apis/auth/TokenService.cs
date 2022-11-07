using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Minimal_Apis.auth
{
    public class TokenService : ITokenService
    {
        private readonly TimeSpan ExpiryTime = new TimeSpan(0, 30, 0);
        public string BuildToken(string key, string issuer, UserDto userDto)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userDto.UserName),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescrioption = new JwtSecurityToken(issuer, issuer, claims, 
                expires: DateTime.Now.Add(ExpiryTime), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescrioption);
        }
    }
}
