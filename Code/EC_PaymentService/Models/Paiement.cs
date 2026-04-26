using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EC_PaymentService.Models
{
    public class Paiement
    {
        public int Id { get; set; }

        public int IdUtilisateur { get; set; }

        public int? IdCommande { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Montant { get; set; }

        [MaxLength(10)]
        public string Devise { get; set; } = "cad";

        [MaxLength(200)]
        public string IdTransactionStripe { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Statut { get; set; } = "En_Attente";

        public string? MessageErreur { get; set; }

        public DateTime CreeLe { get; set; } = DateTime.UtcNow;
    }

    public class DemandePaiementDto
    {
        public int IdUtilisateur { get; set; }
        public int? IdCommande { get; set; }
        public decimal Montant { get; set; }
        public string Devise { get; set; } = "cad";
        public string JetonStripe { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class ReponsePaiementDto
    {
        public bool Succes { get; set; }
        public int IdPaiement { get; set; }
        public string IdTransactionStripe { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public string? Message { get; set; }
    }

    public class ConfigStripe
    {
        public string ClePublique { get; set; } = string.Empty;
        public string CleSecrete { get; set; } = string.Empty;
    }
}
