using EC_ProductService.Models;
using Newtonsoft.Json;

namespace EC_ProductService.Services
{
    public class ServiceApiExterne
    {
        private readonly HttpClient _httpClient;

        public ServiceApiExterne(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProduitFakeStore>> ObtenirProduitsFakeStore()
        {
            var reponse = await _httpClient.GetStringAsync("https://fakestoreapi.com/products");
            return JsonConvert.DeserializeObject<List<ProduitFakeStore>>(reponse) ?? new List<ProduitFakeStore>();
        }

        public async Task<List<ProduitDummyJson>> ObtenirProduitsDummyJson()
        {
            var reponse = await _httpClient.GetStringAsync("https://dummyjson.com/products?limit=30");
            var resultat = JsonConvert.DeserializeObject<ReponseDummyJson>(reponse);
            return resultat?.Produits ?? new List<ProduitDummyJson>();
        }
    }
}
