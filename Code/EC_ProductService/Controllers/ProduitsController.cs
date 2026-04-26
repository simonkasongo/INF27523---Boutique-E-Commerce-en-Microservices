using EC_ProductService.Data;
using EC_ProductService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EC_ProductService.Controllers
{
    [ApiController]
    [Route("api/produits")]
    public class ProduitsController : ControllerBase
    {
        private readonly ProductDbContext _contexte;

        public ProduitsController(ProductDbContext contexte)
        {
            _contexte = contexte;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produit>>> ListerTous(
            [FromQuery] string? recherche = null,
            [FromQuery] string? categorie = null,
            [FromQuery] int? idVendeur = null)
        {
            var requete = _contexte.Produits.AsQueryable();

            if (!string.IsNullOrWhiteSpace(recherche))
                requete = requete.Where(p => p.Titre.Contains(recherche) || p.Description.Contains(recherche));

            if (!string.IsNullOrWhiteSpace(categorie))
                requete = requete.Where(p => p.Categorie == categorie);

            if (idVendeur.HasValue)
                requete = requete.Where(p => p.IdVendeur == idVendeur.Value);

            var produits = await requete.OrderByDescending(p => p.AjouteLe).ToListAsync();
            return Ok(produits);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Produit>> Obtenir(int id)
        {
            var produit = await _contexte.Produits.FindAsync(id);
            if (produit == null) return NotFound();
            return Ok(produit);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Produit>> Creer([FromBody] CreerProduitDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Titre) || dto.Prix <= 0)
                return BadRequest(new { message = "Le titre et un prix valide sont obligatoires." });

            var produit = new Produit
            {
                Titre = dto.Titre.Trim(),
                Description = dto.Description?.Trim() ?? string.Empty,
                Prix = dto.Prix,
                Categorie = dto.Categorie?.Trim() ?? "Autre",
                UrlImage = dto.UrlImage?.Trim() ?? string.Empty,
                Source = "Manuel",
                IdVendeur = dto.IdVendeur,
                AjouteLe = DateTime.UtcNow
            };

            _contexte.Produits.Add(produit);
            await _contexte.SaveChangesAsync();
            return CreatedAtAction(nameof(Obtenir), new { id = produit.Id }, produit);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> MettreAJour(int id, [FromBody] CreerProduitDto dto)
        {
            var produit = await _contexte.Produits.FindAsync(id);
            if (produit == null) return NotFound();
            if (produit.IdVendeur != dto.IdVendeur)
                return Forbid();

            produit.Titre = dto.Titre.Trim();
            produit.Description = dto.Description?.Trim() ?? string.Empty;
            produit.Prix = dto.Prix;
            produit.Categorie = dto.Categorie?.Trim() ?? "Autre";
            produit.UrlImage = dto.UrlImage?.Trim() ?? string.Empty;

            await _contexte.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Supprimer(int id)
        {
            var produit = await _contexte.Produits.FindAsync(id);
            if (produit == null) return NotFound();

            _contexte.Produits.Remove(produit);
            await _contexte.SaveChangesAsync();
            return NoContent();
        }
    }
}
