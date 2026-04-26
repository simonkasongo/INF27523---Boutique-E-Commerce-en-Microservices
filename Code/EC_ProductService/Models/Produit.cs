using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EC_ProductService.Models
{
    public class Produit
    {
        public int Id { get; set; }

        public int? IdExterne { get; set; }

        [Required, MaxLength(500)]
        public string Titre { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Prix { get; set; }

        [MaxLength(200)]
        public string Categorie { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string UrlImage { get; set; } = string.Empty;

        // "FakeStore", "DummyJSON" ou "Manuel"
        [MaxLength(20)]
        public string Source { get; set; } = "Manuel";

        // Identifiant du vendeur (géré par EC_AuthService)
        public int IdVendeur { get; set; }

        public DateTime AjouteLe { get; set; } = DateTime.UtcNow;
    }
}
