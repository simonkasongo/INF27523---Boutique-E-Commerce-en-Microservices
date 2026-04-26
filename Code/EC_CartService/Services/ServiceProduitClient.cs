using EC_CartService.Models;
using System.Text.Json;

namespace EC_CartService.Services
{
    // Client HTTP qui communique avec EC_ProductService pour obtenir les infos d'un produit
    public class ServiceProduitClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ServiceProduitClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ProduitDto?> ObtenirProduit(int idProduit)
        {
            var urlBase = _configuration["Services:ProductService"]!;
            var reponse = await _httpClient.GetAsync($"{urlBase}/api/produits/{idProduit}");
            if (!reponse.IsSuccessStatusCode) return null;

            var contenu = await reponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<ProduitDto>(contenu, options);
        }
    }
}
