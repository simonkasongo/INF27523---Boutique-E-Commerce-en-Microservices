using EC_NotificationService.Data;
using EC_NotificationService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EC_NotificationService.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationDbContext _contexte;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(NotificationDbContext contexte, ILogger<NotificationsController> logger)
        {
            _contexte = contexte;
            _logger = logger;
        }

        // Envoie une notification fictive (journalise + enregistre en base)
        [HttpPost]
        public async Task<ActionResult<Notification>> Envoyer([FromBody] EnvoyerNotificationDto dto)
        {
            var notification = new Notification
            {
                IdUtilisateur = dto.IdUtilisateur,
                Type = dto.Type == "SMS" ? "SMS" : "Email",
                Sujet = dto.Sujet,
                Message = dto.Message,
                Statut = "Envoyee",
                EnvoyeeLe = DateTime.UtcNow
            };

            _contexte.Notifications.Add(notification);
            await _contexte.SaveChangesAsync();

            // Simulation d'envoi (log console)
            _logger.LogInformation("[NOTIFICATION {Type}] Utilisateur #{Id} — {Sujet}", notification.Type, notification.IdUtilisateur, notification.Sujet);

            return CreatedAtAction(nameof(Obtenir), new { id = notification.Id }, notification);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> Lister([FromQuery] int? idUtilisateur = null)
        {
            var requete = _contexte.Notifications.AsQueryable();
            if (idUtilisateur.HasValue)
                requete = requete.Where(n => n.IdUtilisateur == idUtilisateur.Value);

            var notifications = await requete.OrderByDescending(n => n.EnvoyeeLe).ToListAsync();
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> Obtenir(int id)
        {
            var notification = await _contexte.Notifications.FindAsync(id);
            if (notification == null) return NotFound();
            return Ok(notification);
        }
    }
}
