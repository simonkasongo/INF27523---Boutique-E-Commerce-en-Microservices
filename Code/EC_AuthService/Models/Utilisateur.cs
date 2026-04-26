using System.ComponentModel.DataAnnotations;

namespace EC_AuthService.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Prenom { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string MotDePasseHash { get; set; } = string.Empty;

        // "Client" ou "Vendeur"
        [Required, MaxLength(20)]
        public string Role { get; set; } = "Client";

        public DateTime CreeLe { get; set; } = DateTime.UtcNow;
    }
}
