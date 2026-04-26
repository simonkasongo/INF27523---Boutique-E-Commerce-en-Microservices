using EC_OrderService.Data;
using EC_OrderService.Models;
using EC_OrderService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EC_OrderService.Controllers
{
    [ApiController]
    [Route("api/commandes")]
    public class CommandesController : ControllerBase
    {
        private readonly OrderDbContext _contexte;
        private readonly ServiceCartClient _cart;
        private readonly ServicePaymentClient _payment;
        private readonly ServiceNotificationClient _notif;

        public CommandesController(OrderDbContext contexte,
            ServiceCartClient cart,
            ServicePaymentClient payment,
            ServiceNotificationClient notif)
        {
            _contexte = contexte;
            _cart = cart;
            _payment = payment;
            _notif = notif;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Commande>>> Lister([FromQuery] int? idClient = null, [FromQuery] int? idVendeur = null)
        {
            var requete = _contexte.Commandes
                .Include(c => c.Lignes)
                .AsQueryable();

            if (idClient.HasValue)
                requete = requete.Where(c => c.IdClient == idClient.Value);

            if (idVendeur.HasValue)
                requete = requete.Where(c => c.Lignes.Any(l => l.IdVendeur == idVendeur.Value));

            var commandes = await requete.OrderByDescending(c => c.CreeLe).ToListAsync();
            return Ok(commandes);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Commande>> Obtenir(int id)
        {
            var commande = await _contexte.Commandes
                .Include(c => c.Lignes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (commande == null) return NotFound();
            return Ok(commande);
        }

        // Orchestration du checkout : récupère le panier via EC_CartService,
        // déclenche le paiement via EC_PaymentService, enregistre la commande,
        // vide le panier et envoie une notification (bonus).
        [Authorize]
        [HttpPost("checkout")]
        public async Task<ActionResult<ReponseCheckoutDto>> Checkout([FromBody] CheckoutDto dto)
        {
            // Récupère le JWT courant pour l'injecter dans les appels inter-services
            var jetonJwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var articles = await _cart.ObtenirPanier(dto.IdUtilisateur, jetonJwt);
            if (articles.Count == 0)
                return BadRequest(new ReponseCheckoutDto { Succes = false, Statut = "Panier_Vide", Message = "Votre panier est vide." });

            var montantTotal = articles.Sum(a => a.PrixUnitaire * a.Quantite);

            // Crée la commande en attente
            var commande = new Commande
            {
                IdClient = dto.IdUtilisateur,
                MontantTotal = montantTotal,
                Statut = "En_Attente",
                CreeLe = DateTime.UtcNow,
                Lignes = articles.Select(a => new LigneCommande
                {
                    IdProduit = a.IdProduit,
                    TitreProduit = a.TitreProduit,
                    Quantite = a.Quantite,
                    PrixUnitaire = a.PrixUnitaire,
                    IdVendeur = a.IdVendeur
                }).ToList()
            };

            _contexte.Commandes.Add(commande);
            await _contexte.SaveChangesAsync();

            // Lance le paiement via EC_PaymentService
            var (succes, idTransaction, statut, message) = await _payment.Debiter(
                dto.IdUtilisateur, commande.Id, montantTotal, dto.JetonStripe, jetonJwt);

            commande.IdTransactionStripe = idTransaction;
            commande.Statut = succes ? "Payee" : statut;
            await _contexte.SaveChangesAsync();

            if (!succes)
            {
                return BadRequest(new ReponseCheckoutDto
                {
                    Succes = false,
                    IdCommande = commande.Id,
                    Statut = commande.Statut,
                    MontantTotal = montantTotal,
                    Message = message
                });
            }

            // Vide le panier
            await _cart.ViderPanier(dto.IdUtilisateur, jetonJwt);

            // Envoie la notification (bonus)
            await _notif.Envoyer(
                dto.IdUtilisateur,
                $"Confirmation de commande #{commande.Id}",
                $"Votre paiement de {montantTotal:N2} CAD a été accepté. Merci pour votre achat sur Kasongo Shop.",
                jetonJwt);

            return Ok(new ReponseCheckoutDto
            {
                Succes = true,
                IdCommande = commande.Id,
                Statut = "Payee",
                MontantTotal = montantTotal,
                Message = "Commande confirmée."
            });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Supprimer(int id)
        {
            var commande = await _contexte.Commandes.FindAsync(id);
            if (commande == null) return NotFound();
            _contexte.Commandes.Remove(commande);
            await _contexte.SaveChangesAsync();
            return NoContent();
        }
    }
}
