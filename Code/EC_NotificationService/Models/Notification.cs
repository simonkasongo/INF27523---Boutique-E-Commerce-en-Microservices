using System.ComponentModel.DataAnnotations;

namespace EC_NotificationService.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public int IdUtilisateur { get; set; }

        [Required, MaxLength(20)]
        public string Type { get; set; } = "Email";

        [Required, MaxLength(500)]
        public string Sujet { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Statut { get; set; } = "Envoyee";

        public DateTime EnvoyeeLe { get; set; } = DateTime.UtcNow;
    }

    public class EnvoyerNotificationDto
    {
        public int IdUtilisateur { get; set; }
        // Email ou SMS
        public string Type { get; set; } = "Email";
        public string Sujet { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
