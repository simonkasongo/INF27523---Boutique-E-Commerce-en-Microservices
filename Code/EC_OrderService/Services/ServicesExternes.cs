using EC_OrderService.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EC_OrderService.Services
{
    // Client HTTP vers EC_CartService
    public class ServiceCartClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ServiceCartClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<ArticlePanierDto>> ObtenirPanier(int idUtilisateur, string? jetonJwt = null)
        {
            var urlBase = _configuration["Services:CartService"]!;
            var req = new HttpRequestMessage(HttpMethod.Get, $"{urlBase}/api/panier?idUtilisateur={idUtilisateur}");
            if (!string.IsNullOrEmpty(jetonJwt))
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jetonJwt);

            var reponse = await _httpClient.SendAsync(req);
            if (!reponse.IsSuccessStatusCode) return new List<ArticlePanierDto>();

            var contenu = await reponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<ArticlePanierDto>>(contenu, options) ?? new();
        }

        public async Task ViderPanier(int idUtilisateur, string? jetonJwt = null)
        {
            var urlBase = _configuration["Services:CartService"]!;
            var req = new HttpRequestMessage(HttpMethod.Delete, $"{urlBase}/api/panier?idUtilisateur={idUtilisateur}");
            if (!string.IsNullOrEmpty(jetonJwt))
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jetonJwt);
            await _httpClient.SendAsync(req);
        }
    }

    // Client HTTP vers EC_PaymentService
    public class ServicePaymentClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ServicePaymentClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<(bool succes, string idTransaction, string statut, string? message)> Debiter(
            int idUtilisateur, int? idCommande, decimal montant, string jetonStripe, string? jetonJwt = null)
        {
            var urlBase = _configuration["Services:PaymentService"]!;
            var payload = new
            {
                idUtilisateur,
                idCommande,
                montant,
                devise = "cad",
                jetonStripe,
                description = $"Commande Kasongo Shop — Client #{idUtilisateur}"
            };

            var req = new HttpRequestMessage(HttpMethod.Post, $"{urlBase}/api/paiements/debiter")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(jetonJwt))
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jetonJwt);

            var reponse = await _httpClient.SendAsync(req);
            var contenu = await reponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            try
            {
                var resultat = JsonSerializer.Deserialize<ReponsePaiementDto>(contenu, options);
                if (resultat == null)
                    return (false, string.Empty, "Echec", "Réponse vide du service de paiement.");
                return (resultat.Succes, resultat.IdTransactionStripe, resultat.Statut, resultat.Message);
            }
            catch
            {
                return (false, string.Empty, "Echec", "Erreur de désérialisation.");
            }
        }

        private class ReponsePaiementDto
        {
            public bool Succes { get; set; }
            public int IdPaiement { get; set; }
            public string IdTransactionStripe { get; set; } = string.Empty;
            public string Statut { get; set; } = string.Empty;
            public string? Message { get; set; }
        }
    }

    // Client HTTP vers EC_NotificationService
    public class ServiceNotificationClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ServiceNotificationClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task Envoyer(int idUtilisateur, string sujet, string message, string? jetonJwt = null)
        {
            var urlBase = _configuration["Services:NotificationService"];
            if (string.IsNullOrEmpty(urlBase)) return;

            var payload = new
            {
                idUtilisateur,
                type = "Email",
                sujet,
                message
            };

            var req = new HttpRequestMessage(HttpMethod.Post, $"{urlBase}/api/notifications")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(jetonJwt))
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jetonJwt);

            try { await _httpClient.SendAsync(req); }
            catch { /* notification non bloquante */ }
        }
    }
}
