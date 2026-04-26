using Newtonsoft.Json;

namespace EC_ProductService.Models
{
    // DTO reçu depuis FakeStoreAPI
    public class ProduitFakeStore
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("title")] public string Titre { get; set; } = string.Empty;
        [JsonProperty("price")] public decimal Prix { get; set; }
        [JsonProperty("description")] public string Description { get; set; } = string.Empty;
        [JsonProperty("category")] public string Categorie { get; set; } = string.Empty;
        [JsonProperty("image")] public string Image { get; set; } = string.Empty;
    }

    // DTO reçu depuis DummyJSON
    public class ProduitDummyJson
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("title")] public string Titre { get; set; } = string.Empty;
        [JsonProperty("price")] public decimal Prix { get; set; }
        [JsonProperty("description")] public string Description { get; set; } = string.Empty;
        [JsonProperty("category")] public string Categorie { get; set; } = string.Empty;
        [JsonProperty("thumbnail")] public string Miniature { get; set; } = string.Empty;
    }

    public class ReponseDummyJson
    {
        [JsonProperty("products")]
        public List<ProduitDummyJson> Produits { get; set; } = new();
    }

    // DTO d'import unifié
    public class ImportProduitDto
    {
        public int IdExterne { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Prix { get; set; }
        public string Categorie { get; set; } = string.Empty;
        public string UrlImage { get; set; } = string.Empty;
        public string Source { get; set; } = "FakeStore";
        public int IdVendeur { get; set; }
    }

    public class CreerProduitDto
    {
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Prix { get; set; }
        public string Categorie { get; set; } = string.Empty;
        public string UrlImage { get; set; } = string.Empty;
        public int IdVendeur { get; set; }
    }
}
