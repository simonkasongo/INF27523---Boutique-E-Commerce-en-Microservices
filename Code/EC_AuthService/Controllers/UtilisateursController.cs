using EC_AuthService.Data;
using EC_AuthService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EC_AuthService.Controllers
{
    [ApiController]
    [Route("api/utilisateurs")]
    public class UtilisateursController : ControllerBase
    {
        private readonly AuthDbContext _contexte;

        public UtilisateursController(AuthDbContext contexte)
        {
            _contexte = contexte;
        }

        // Permet aux autres microservices (OrderService, etc.) de récupérer l'info d'un utilisateur
        [HttpGet("{id}")]
        public async Task<ActionResult<UtilisateurDto>> Obtenir(int id)
        {
            var u = await _contexte.Utilisateurs.FindAsync(id);
            if (u == null) return NotFound();
            return Ok(new UtilisateurDto
            {
                Id = u.Id,
                Prenom = u.Prenom,
                Nom = u.Nom,
                Email = u.Email,
                Role = u.Role,
                CreeLe = u.CreeLe
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UtilisateurDto>>> ListerTous()
        {
            var liste = await _contexte.Utilisateurs
                .OrderBy(u => u.Id)
                .Select(u => new UtilisateurDto
                {
                    Id = u.Id,
                    Prenom = u.Prenom,
                    Nom = u.Nom,
                    Email = u.Email,
                    Role = u.Role,
                    CreeLe = u.CreeLe
                })
                .ToListAsync();
            return Ok(liste);
        }
    }
}
