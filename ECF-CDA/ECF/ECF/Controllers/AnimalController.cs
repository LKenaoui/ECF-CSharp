using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECF.Database.Context;
using ECF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace ECF.Controllers
{
    public class AnimalController : Controller
    {
        private readonly EcfDbContext _context;
        private readonly ILogger<AnimalController> _logger;

        public AnimalController(EcfDbContext context, ILogger<AnimalController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Animals
        // Affiche la liste de tous les animaux
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<Animal> animalsWithBreeds = _context.Animals
                    .Include(a => a.Breed)
                    .AsQueryable();

                List<Animal> sortedAnimals = await animalsWithBreeds.OrderBy(a => a.Name).ToListAsync();
                return View(sortedAnimals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la récupération des animaux");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        // GET: Animals/Create
        // Affiche le formulaire de création d'un animal
        public async Task<IActionResult> Create(string? returnUrl)
        {
            try
            {
                ViewData["BreedId"] = new SelectList(
                    await _context.Breeds
                        .OrderBy(b => b.BreedName)
                        .ToListAsync(),
                    "BreedId",
                    "BreedName"
                );

                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de l'affichage du formulaire de création");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        // POST: Animals/Create
        // Traite le formulaire de création d'un animal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Description,BreedId")] Animal newAnimal,
            string? returnUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newAnimal.Name))
                {
                    ModelState.AddModelError("Name", "Le nom est obligatoire");
                }
                if (string.IsNullOrWhiteSpace(newAnimal.Description))
                {
                    ModelState.AddModelError("Description", "La description est obligatoire");
                }

                if (newAnimal.BreedId <= 0)
                {
                    ModelState.AddModelError("BreedId", "La race est obligatoire");
                }
                else
                {
                    var breedExists = await _context.Breeds.AnyAsync(b => b.BreedId == newAnimal.BreedId);
                    if (!breedExists)
                    {
                        ModelState.AddModelError("BreedId", "La race sélectionnée n'existe pas");
                    }
                }

                if (ModelState.IsValid)
                {
                    _context.Add(newAnimal);
                    await _context.SaveChangesAsync();

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction(nameof(Index));
                }

                ViewData["BreedId"] = new SelectList(
                    await _context.Breeds
                        .OrderBy(b => b.BreedName)
                        .ToListAsync(),
                    "BreedId",
                    "BreedName",
                    newAnimal.BreedId);

                ViewData["ReturnUrl"] = returnUrl;
                return View(newAnimal);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Une erreur est survenue lors de la création de l'animal : {ex.Message}");
                return View(newAnimal);
            }
        }

        // GET: Animals/Edit/5
        // Affiche le formulaire de modification d'un animal
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var animal = await _context.Animals
                    .Include(a => a.Breed)
                    .FirstOrDefaultAsync(a => a.AnimalId == id);

                if (animal == null)
                {
                    return NotFound();
                }

                ViewData["BreedId"] = new SelectList(
                    await _context.Breeds
                        .OrderBy(b => b.BreedName)
                        .ToListAsync(),
                    "BreedId",
                    "BreedName",
                    animal.BreedId);

                return View(animal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de l'affichage du formulaire de modification");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        // POST: Animals/Edit/5
        // Traite le formulaire de modification d'un animal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("AnimalId,Name,Description,BreedId")] Animal updatedAnimal)
        {
            if (id != updatedAnimal.AnimalId)
            {
                return NotFound();
            }

            try
            {
                var currentAnimal = await _context.Animals
                    .Include(a => a.Breed)
                    .FirstOrDefaultAsync(a => a.AnimalId == id);

                if (currentAnimal == null)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        currentAnimal.Name = updatedAnimal.Name;
                        currentAnimal.Description = updatedAnimal.Description;
                        currentAnimal.BreedId = updatedAnimal.BreedId;

                        _context.Update(currentAnimal);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AnimalExists(updatedAnimal.AnimalId))
                        {
                            return NotFound();
                        }
                        throw;
                    }
                }

                ViewData["BreedId"] = new SelectList(
                    await _context.Breeds
                        .OrderBy(b => b.BreedName)
                        .ToListAsync(),
                    "BreedId",
                    "BreedName",
                    updatedAnimal.BreedId);
                return View(updatedAnimal);
            }
            catch
            {
                ModelState.AddModelError("", "Une erreur est survenue lors de la modification de l'animal.");
                return View(updatedAnimal);
            }
        }

        // GET: Animals/Delete/5
        // Affiche la page de confirmation de suppression
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var animalToDelete = await _context.Animals
                    .Include(a => a.Breed)
                    .FirstOrDefaultAsync(m => m.AnimalId == id);

                if (animalToDelete == null)
                {
                    return NotFound();
                }

                return View(animalToDelete);
            }
            catch
            {
                return View("Error");
            }
        }

        // POST: Animals/Delete/5
        // Effectue la suppression de l'animal
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var animalToDelete = await _context.Animals
                    .FirstOrDefaultAsync(a => a.AnimalId == id);

                if (animalToDelete == null)
                {
                    return NotFound();
                }

                _context.Animals.Remove(animalToDelete);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Error");
            }
        }

        // Vérifie si un animal existe dans la base de données
        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(e => e.AnimalId == id);
        }

        // GET: Animals/Register
        // Affiche le formulaire d'inscription d'un animal
        public async Task<IActionResult> Register()
        {
            try
            {
                // Prépare la liste déroulante des races
                ViewData["BreedId"] = new SelectList(
                    await _context.Breeds
                        .OrderBy(b => b.BreedName) // Puis par nom de race
                        .ToListAsync(),
                    "BreedId",
                    "BreedName");

                return View();
            }
            catch
            {
                return View("Error");
            }
        }

        // POST: Animals/Register
        // Traite l'inscription d'un animal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            [Bind("Name,Description,BreedId")] Animal newAnimal)
        {
            try
            {
                List<string> validationErrors = new();

                // Vérifie le nom
                if (string.IsNullOrWhiteSpace(newAnimal.Name))
                    validationErrors.Add("Le nom est obligatoire");
                else if (newAnimal.Name.Length > 50)
                    validationErrors.Add("Le nom ne peut pas dépasser 50 caractères");

                // Vérifie la description
                if (newAnimal.Description == default)
                    validationErrors.Add("La date de naissance est obligatoire");
                else if (newAnimal.Description.Length > 2000)
                    validationErrors.Add("La description ne peut pas dépasser 2000 caractères");

                // Vérifie la race
                if (newAnimal.BreedId <= 0)
                    validationErrors.Add("La race est obligatoire");

                // Si des erreurs sont trouvées, on les renvoie
                if (validationErrors.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Erreur de validation du formulaire",
                        errors = validationErrors
                    });
                }

                // Vérifie que la race existe
                Breed? selectedBreed = await _context.Breeds.FindAsync(newAnimal.BreedId);
                if (selectedBreed == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Erreur de validation du formulaire",
                        errors = new[] { "La race sélectionnée n'existe pas" }
                    });
                }

                // Sauvegarde l'animal dans la base de données
                _context.Add(newAnimal);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Une erreur est survenue lors de l'inscription de l'animal.",
                    error = ex.Message
                });
            }
        }

        // GET: Animals/Search
        // Recherche des animaux par nom ou race
        public async Task<IActionResult> Search(string searchTerm)
        {
            try
            {
                IQueryable<Animal> animalsWithBreeds = _context.Animals
                    .Include(a => a.Breed)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    animalsWithBreeds = animalsWithBreeds.Where(a => 
                        a.Name.Contains(searchTerm) || 
                        (a.Breed != null && a.Breed.BreedName.Contains(searchTerm)));
                }

                List<Animal> matchingAnimals = await animalsWithBreeds
                    .OrderBy(a => a.Name)
                    .ToListAsync();

                return View("Index", matchingAnimals);
            }
            catch
            {
                return View("Error");
            }
        }
    }
}