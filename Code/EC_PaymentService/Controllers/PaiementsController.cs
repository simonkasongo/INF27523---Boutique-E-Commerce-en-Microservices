using EC_PaymentService.Data;
using EC_PaymentService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;

namespace EC_PaymentService.Controllers
{
    [ApiController]
    [Route("api/paiements")]
    public class PaiementsController : ControllerBase
    {
        private readonly PaymentDbContext _contexte;
        private readonly ConfigStripe _configStripe;

        public PaiementsController(PaymentDbContext contexte, IOptions<ConfigStripe> configStripe)
        {
            _contexte = contexte;
            _configStripe = configStripe.Value;
        }

        [HttpGet("cle-publique")]
        public ActionResult<object> ObtenirClePublique()
        {
            return Ok(new { clePublique = _configStripe.ClePublique });
        }

        [Authorize]
        [HttpPost("debiter")]
        public async Task<ActionResult<ReponsePaiementDto>> Debiter([FromBody] DemandePaiementDto dto)
        {
            // Enregistre la tentative de paiement
            var paiement = new Paiement
            {
                IdUtilisateur = dto.IdUtilisateur,
                IdCommande = dto.IdCommande,
                Montant = dto.Montant,
                Devise = dto.Devise,
                Statut = "En_Cours",
                CreeLe = DateTime.UtcNow
            };
            _contexte.Paiements.Add(paiement);
            await _contexte.SaveChangesAsync();

            try
            {
                StripeConfiguration.ApiKey = _configStripe.CleSecrete;

                var options = new ChargeCreateOptions
                {
                    Amount = (long)(dto.Montant * 100),
                    Currency = dto.Devise,
                    Description = string.IsNullOrWhiteSpace(dto.Description)
                        ? $"Commande Kasongo Shop — Client #{dto.IdUtilisateur}"
                        : dto.Description,
                    Source = dto.JetonStripe
                };

                var service = new ChargeService();
                var charge = await service.CreateAsync(options);

                paiement.IdTransactionStripe = charge.Id;
                paiement.Statut = charge.Status == "succeeded" ? "Payee" : "Refusee";
                await _contexte.SaveChangesAsync();

                return Ok(new ReponsePaiementDto
                {
                    Succes = charge.Status == "succeeded",
                    IdPaiement = paiement.Id,
                    IdTransactionStripe = charge.Id,
                    Statut = paiement.Statut
                });
            }
            catch (StripeException ex)
            {
                paiement.Statut = "Echec";
                paiement.MessageErreur = ex.StripeError?.Message ?? ex.Message;
                await _contexte.SaveChangesAsync();

                return BadRequest(new ReponsePaiementDto
                {
                    Succes = false,
                    IdPaiement = paiement.Id,
                    Statut = "Echec",
                    Message = paiement.MessageErreur
                });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Paiement>>> Lister([FromQuery] int? idUtilisateur = null)
        {
            var requete = _contexte.Paiements.AsQueryable();
            if (idUtilisateur.HasValue)
                requete = requete.Where(p => p.IdUtilisateur == idUtilisateur.Value);

            var paiements = await requete.OrderByDescending(p => p.CreeLe).ToListAsync();
            return Ok(paiements);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Paiement>> Obtenir(int id)
        {
            var paiement = await _contexte.Paiements.FindAsync(id);
            if (paiement == null) return NotFound();
            return Ok(paiement);
        }
    }
}
