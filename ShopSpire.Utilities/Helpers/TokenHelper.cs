using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopSpire.Utilities.DTO;

namespace ShopSpire.Utilities.Helpers
{
    public class TokenHelper
    {
        private readonly IConfiguration _configuration;

        public TokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(TokenDTO tokenDTO)
        {
            var JWTSection = _configuration.GetSection("JWT");
            var secretKey = JWTSection["Key"];
            var issuer = JWTSection["ValidIssuer"];
            var audience = JWTSection["ValidAudience"];
            var expirationIn = JWTSection["expirationIn"];
            var claims = new List<Claim>
            {
                    new Claim("email", tokenDTO.Email),
                    new Claim(ClaimTypes.Role, tokenDTO.Role),
                    new Claim("id", tokenDTO.Id),
                     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)); // Use a secure key
            var Token = new JwtSecurityToken(
          issuer:issuer, // Issuer of the token
          audience:audience, // Audience for the token
          claims: claims, // Claims to include in the token
          expires: DateTime.Now.AddDays(double.Parse(expirationIn)), // Token expiration time
          signingCredentials: new SigningCredentials(AuthKey, SecurityAlgorithms.HmacSha256) // Signing credentials using HMAC SHA256
          );
            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
    }
}
