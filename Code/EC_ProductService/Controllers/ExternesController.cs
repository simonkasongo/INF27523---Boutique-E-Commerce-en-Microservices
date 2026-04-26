using EC_ProductService.Data;
using EC_ProductService.Models;
using EC_ProductService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EC_ProductService.Controllers
{
    // Intègre les APIs externes FakeStore et DummyJSON (reprise du TP1)
    [ApiController]
    [Route("api/externes")]
    public class ExternesController : ControllerBase
    {
        private readonly ProductDbContext _contexte;
        private readonly ServiceApiExterne _serviceApi;

        public ExternesController(ProductDbContext contexte, ServiceApiExterne serviceApi)
        {
            _contexte = contexte;
            _serviceApi = serviceApi;
        }

        [HttpGet("fakestore")]
        public async Task<ActionResult<IEnumerable<ProduitFakeStore>>> ListerFakeStore()
        {
            var produits = await _serviceApi.ObtenirProduitsFakeStore();
            return Ok(produits);
        }

        [HttpGet("dummyjson")]
        public async Task<ActionResult<IEnumerable<ProduitDummyJson>>> ListerDummyJson()
        {
            var produits = await _serviceApi.ObtenirProduitsDummyJson();
            return Ok(produits);
        }

        [Authorize]
        [HttpPost("importer")]
        public async Task<ActionResult<Produit>> Importer([FromBody] ImportProduitDto dto)
        {
            var dejaImporte = await _contexte.Produits.AnyAsync(p =>
                p.IdExterne == dto.IdExterne &&
                p.Source == dto.Source &&
                p.IdVendeur == dto.IdVendeur);

            if (dejaImporte)
                return Conflict(new { message = "Ce produit est déjà dans votre boutique." });

            var produit = new Produit
            {
                IdExterne = dto.IdExterne,
                Titre = dto.Titre,
                Description = dto.Description,
                Prix = dto.Prix,
                Categorie = dto.Categorie,
                UrlImage = dto.UrlImage,
                Source = dto.Source,
                IdVendeur = dto.IdVendeur,
                AjouteLe = DateTime.UtcNow
            };

            _contexte.Produits.Add(produit);
            await _contexte.SaveChangesAsync();
            return CreatedAtAction("Obtenir", "Produits", new { id = produit.Id }, produit);
        }
    }
}
