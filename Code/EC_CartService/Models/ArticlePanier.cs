using System.ComponentModel.DataAnnotations.Schema;

namespace EC_CartService.Models
{
    public class ArticlePanier
    {
        public int Id { get; set; }

        // Référence au client (géré par EC_AuthService)
        public int IdUtilisateur { get; set; }

        // Référence au produit (géré par EC_ProductService)
        public int IdProduit { get; set; }

        public int Quantite { get; set; } = 1;

        // Champs dénormalisés (copiés à l'ajout) pour éviter un appel au ProductService à chaque lecture
        public string TitreProduit { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrixUnitaire { get; set; }

        public string UrlImage { get; set; } = string.Empty;

        public int IdVendeur { get; set; }

        public DateTime AjouteLe { get; set; } = DateTime.UtcNow;
    }

    public class AjouterPanierDto
    {
        public int IdUtilisateur { get; set; }
        public int IdProduit { get; set; }
        public int Quantite { get; set; } = 1;
    }

    public class ProduitDto
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public decimal Prix { get; set; }
        public string UrlImage { get; set; } = string.Empty;
        public int IdVendeur { get; set; }
    }
}
