using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECF.Database.Context;
using ECF.Models;

namespace ECF.Controllers
{
    public class BreedsController : Controller
    {
        private readonly EcfDbContext _context;

        public BreedsController(EcfDbContext context)
        {
            _context = context;
        }

        // GET: Breeds
        // Affiche la liste de toutes les races
        public async Task<IActionResult> Index()
        {
            // Récupère toutes les races
            var EcfDbContext = _context.Breeds;
            return View(await EcfDbContext.ToListAsync());
        }

        // GET: Breeds/Details/5
        // Affiche les détails d'une race spécifique
        public async Task<IActionResult> Details(int? id)
        {
            // Vérifie si un ID a été fourni
            if (id == null)
            {
                return NotFound();
            }

            // Récupère la race
            var breed = await _context.Breeds
                .FirstOrDefaultAsync(m => m.BreedId == id);
            if (breed == null)
            {
                return NotFound();
            }

            return View(breed);
        }

        // GET: Breeds/Create
        // Affiche le formulaire de création d'une race
        public IActionResult Create()
        {
            return View();
        }

        // POST: Breeds/Create
        // Traite le formulaire de création d'une race
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BreedId,BreedName")] Breed breed)
        {
            // Vérifie si les données du formulaire sont valides
            if (ModelState.IsValid)
            {
                // Ajoute la nouvelle race à la base de données
                _context.Add(breed);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(breed);
        }

        // GET: Breeds/Edit/5
        // Affiche le formulaire de modification d'une race
        public async Task<IActionResult> Edit(int? id)
        {
            // Vérifie si un ID a été fourni
            if (id == null)
            {
                return NotFound();
            }

            // Récupère la race à modifier
            var breed = await _context.Breeds.FindAsync(id);
            if (breed == null)
            {
                return NotFound();
            }
            return View(breed);
        }

        // POST: Breeds/Edit/5
        // Traite le formulaire de modification d'une race
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BreedId,BreedName")] Breed breed)
        {
            // Vérifie la cohérence des IDs
            if (id != breed.BreedId)
            {
                return NotFound();
            }

            // Vérifie si les données du formulaire sont valides
            if (ModelState.IsValid)
            {
                try
                {
                    // Met à jour la race dans la base de données
                    _context.Update(breed);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Gère les erreurs de concurrence
                    if (!BreedExists(breed.BreedId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(breed);
        }

        // GET: Breeds/Delete/5
        // Affiche la page de confirmation de suppression
        public async Task<IActionResult> Delete(int? id)
        {
            // Vérifie si un ID a été fourni
            if (id == null)
            {
                return NotFound();
            }

            // Récupère la race
            var breed = await _context.Breeds
                .FirstOrDefaultAsync(m => m.BreedId == id);
            if (breed == null)
            {
                return NotFound();
            }

            return View(breed);
        }

        // POST: Breeds/Delete/5
        // Effectue la suppression de la race
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Récupère la race à supprimer
            var breed = await _context.Breeds.FindAsync(id);
            if (breed != null)
            {
                // Supprime la race de la base de données
                _context.Breeds.Remove(breed);
            }

            // Sauvegarde les changements
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Vérifie si une race existe dans la base de données
        private bool BreedExists(int id)
        {
            return _context.Breeds.Any(e => e.BreedId == id);
        }
    }
}
