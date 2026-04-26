using System.ComponentModel.DataAnnotations;

namespace EC_AuthService.Models
{
    public class InscriptionDto
    {
        [Required] public string Prenom { get; set; } = string.Empty;
        [Required] public string Nom { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)] public string MotDePasse { get; set; } = string.Empty;
        // "Client" ou "Vendeur"
        public string Role { get; set; } = "Client";
    }

    public class ConnexionDto
    {
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string MotDePasse { get; set; } = string.Empty;
    }

    public class ReponseJetonDto
    {
        public string Jeton { get; set; } = string.Empty;
        public int IdUtilisateur { get; set; }
        public string Prenom { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }

    public class UtilisateurDto
    {
        public int Id { get; set; }
        public string Prenom { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreeLe { get; set; }
    }
}
