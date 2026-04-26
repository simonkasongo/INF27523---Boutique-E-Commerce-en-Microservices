using EC_CartService.Data;
using EC_CartService.Models;
using EC_CartService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EC_CartService.Controllers
{
    [ApiController]
    [Route("api/panier")]
    public class PanierController : ControllerBase
    {
        private readonly CartDbContext _contexte;
        private readonly ServiceProduitClient _serviceProduit;

        public PanierController(CartDbContext contexte, ServiceProduitClient serviceProduit)
        {
            _contexte = contexte;
            _serviceProduit = serviceProduit;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticlePanier>>> ObtenirPanier([FromQuery] int idUtilisateur)
        {
            var articles = await _contexte.ArticlesPanier
                .Where(a => a.IdUtilisateur == idUtilisateur)
                .ToListAsync();
            return Ok(articles);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ArticlePanier>> Ajouter([FromBody] AjouterPanierDto dto)
        {
            // Appel inter-service au EC_ProductService pour récupérer les infos du produit
            var produit = await _serviceProduit.ObtenirProduit(dto.IdProduit);
            if (produit == null)
                return NotFound(new { message = "Produit introuvable." });

            var articleExistant = await _contexte.ArticlesPanier
                .FirstOrDefaultAsync(a => a.IdUtilisateur == dto.IdUtilisateur && a.IdProduit == dto.IdProduit);

            if (articleExistant != null)
            {
                articleExistant.Quantite += dto.Quantite;
                await _contexte.SaveChangesAsync();
                return Ok(articleExistant);
            }

            var article = new ArticlePanier
            {
                IdUtilisateur = dto.IdUtilisateur,
                IdProduit = dto.IdProduit,
                Quantite = dto.Quantite,
                TitreProduit = produit.Titre,
                PrixUnitaire = produit.Prix,
                UrlImage = produit.UrlImage,
                IdVendeur = produit.IdVendeur,
                AjouteLe = DateTime.UtcNow
            };

            _contexte.ArticlesPanier.Add(article);
            await _contexte.SaveChangesAsync();
            return CreatedAtAction(nameof(ObtenirPanier), new { idUtilisateur = dto.IdUtilisateur }, article);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> MettreAJourQuantite(int id, [FromQuery] int quantite)
        {
            var article = await _contexte.ArticlesPanier.FindAsync(id);
            if (article == null) return NotFound();

            if (quantite <= 0)
                _contexte.ArticlesPanier.Remove(article);
            else
                article.Quantite = quantite;

            await _contexte.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Retirer(int id)
        {
            var article = await _contexte.ArticlesPanier.FindAsync(id);
            if (article == null) return NotFound();
            _contexte.ArticlesPanier.Remove(article);
            await _contexte.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Vider([FromQuery] int idUtilisateur)
        {
            var articles = await _contexte.ArticlesPanier
                .Where(a => a.IdUtilisateur == idUtilisateur)
                .ToListAsync();
            _contexte.ArticlesPanier.RemoveRange(articles);
            await _contexte.SaveChangesAsync();
            return NoContent();
        }
    }
}
