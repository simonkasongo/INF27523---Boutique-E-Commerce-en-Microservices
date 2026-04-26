using EC_AuthService.Data;
using EC_AuthService.Models;
using EC_AuthService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace EC_AuthService.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _contexte;
        private readonly ServiceJwt _serviceJwt;

        public AuthController(AuthDbContext contexte, ServiceJwt serviceJwt)
        {
            _contexte = contexte;
            _serviceJwt = serviceJwt;
        }

        [HttpPost("inscription")]
        public async Task<ActionResult<ReponseJetonDto>> Inscription([FromBody] InscriptionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var email = dto.Email.Trim().ToLower();
            if (await _contexte.Utilisateurs.AnyAsync(u => u.Email == email))
                return Conflict(new { message = "Un compte avec cet email existe déjà." });

            var utilisateur = new Utilisateur
            {
                Prenom = dto.Prenom.Trim(),
                Nom = dto.Nom.Trim(),
                Email = email,
                MotDePasseHash = HacherMotDePasse(dto.MotDePasse),
                Role = dto.Role == "Vendeur" ? "Vendeur" : "Client",
                CreeLe = DateTime.UtcNow
            };

            _contexte.Utilisateurs.Add(utilisateur);
            await _contexte.SaveChangesAsync();

            var (jeton, expiration) = _serviceJwt.GenererJeton(utilisateur);
            return Ok(new ReponseJetonDto
            {
                Jeton = jeton,
                IdUtilisateur = utilisateur.Id,
                Prenom = utilisateur.Prenom,
                Nom = utilisateur.Nom,
                Email = utilisateur.Email,
                Role = utilisateur.Role,
                Expiration = expiration
            });
        }

        [HttpPost("connexion")]
        public async Task<ActionResult<ReponseJetonDto>> Connexion([FromBody] ConnexionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var email = dto.Email.Trim().ToLower();
            var hash = HacherMotDePasse(dto.MotDePasse);

            var utilisateur = await _contexte.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == email && u.MotDePasseHash == hash);

            if (utilisateur == null)
                return Unauthorized(new { message = "Email ou mot de passe incorrect." });

            var (jeton, expiration) = _serviceJwt.GenererJeton(utilisateur);
            return Ok(new ReponseJetonDto
            {
                Jeton = jeton,
                IdUtilisateur = utilisateur.Id,
                Prenom = utilisateur.Prenom,
                Nom = utilisateur.Nom,
                Email = utilisateur.Email,
                Role = utilisateur.Role,
                Expiration = expiration
            });
        }

        private static string HacherMotDePasse(string motDePasse)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(motDePasse));
            return Convert.ToHexString(bytes);
        }
    }
}
