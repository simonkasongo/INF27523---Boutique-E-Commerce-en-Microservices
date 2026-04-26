using EC_AuthService.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EC_AuthService.Services
{
    public class ServiceJwt
    {
        private readonly IConfiguration _configuration;

        public ServiceJwt(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string jeton, DateTime expiration) GenererJeton(Utilisateur utilisateur)
        {
            var cle = _configuration["Jwt:Cle"]!;
            var emetteur = _configuration["Jwt:Emetteur"]!;
            var audience = _configuration["Jwt:Audience"]!;
            var dureeMinutes = int.Parse(_configuration["Jwt:DureeMinutes"] ?? "120");

            var cleSymetrique = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cle));
            var credentials = new SigningCredentials(cleSymetrique, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(dureeMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, utilisateur.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, utilisateur.Email),
                new Claim("role", utilisateur.Role),
                new Claim("prenom", utilisateur.Prenom),
                new Claim("nom", utilisateur.Nom),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: emetteur,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            var jeton = new JwtSecurityTokenHandler().WriteToken(token);
            return (jeton, expiration);
        }
    }
}
