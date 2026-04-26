using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EC_OrderService.Models
{
    public class Commande
    {
        public int Id { get; set; }

        public int IdClient { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MontantTotal { get; set; }

        [MaxLength(200)]
        public string IdTransactionStripe { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Statut { get; set; } = "En_Attente";

        public DateTime CreeLe { get; set; } = DateTime.UtcNow;

        public List<LigneCommande> Lignes { get; set; } = new();
    }

    public class LigneCommande
    {
        public int Id { get; set; }

        public int IdCommande { get; set; }
        public Commande? Commande { get; set; }

        public int IdProduit { get; set; }

        public string TitreProduit { get; set; } = string.Empty;

        public int Quantite { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrixUnitaire { get; set; }

        public int IdVendeur { get; set; }
    }

    // DTOs

    public class CheckoutDto
    {
        public int IdUtilisateur { get; set; }
        public string JetonStripe { get; set; } = string.Empty;
    }

    public class ArticlePanierDto
    {
        public int Id { get; set; }
        public int IdUtilisateur { get; set; }
        public int IdProduit { get; set; }
        public int Quantite { get; set; }
        public string TitreProduit { get; set; } = string.Empty;
        public decimal PrixUnitaire { get; set; }
        public string UrlImage { get; set; } = string.Empty;
        public int IdVendeur { get; set; }
    }

    public class ReponseCheckoutDto
    {
        public bool Succes { get; set; }
        public int? IdCommande { get; set; }
        public string Statut { get; set; } = string.Empty;
        public decimal MontantTotal { get; set; }
        public string? Message { get; set; }
    }
}
