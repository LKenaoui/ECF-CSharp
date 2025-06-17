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

namespace ECF.Controllers
{
    public class AnimalController : Controller
    {
        private readonly EcfDbContext _context;

        public AnimalController(EcfDbContext context)
        {
            _context = context;
        }

        // GET: Animals
        // Affiche la liste de tous les animaux
        public async Task<IActionResult> Index()
        {
            try
            {
                // Pr�pare la requ�te pour r�cup�rer les animaux avec leurs relations
                var query = _context.Animals
                    .Include(a => a.Breed)         // Charge la race de chaque animal
                    .AsQueryable();                // Transforme en requ�te modifiable

                // Ex�cute la requ�te et trie les animaux par nom
                var animals = await query.OrderBy(a => a.Name).ToListAsync();
                return View(animals);
            }
            catch
            {
                return View("Error"); // Affiche une page d'erreur
            }
        }

        // GET: Animals/Create
        // Affiche le formulaire de cr�ation d'un animal
        public async Task<IActionResult> Create(string? returnUrl)
        {
            try
            {
                // Pr�pare la liste d�roulante des races
                ViewData["BreedId"] = new SelectList(
                    await _context.Breeds
                        .OrderBy(b => b.BreedName) // Puis par nom de race
                        .ToListAsync(),
                    "BreedId",    // La valeur qui sera envoy�e au serveur
                    "BreedName"   // Le texte qui sera affich� dans la liste
                );

                // On garde l'URL de retour pour rediriger apr�s la cr�ation
                ViewData["ReturnUrl"] = returnUrl;
                return View(); // On affiche le formulaire
            }
            catch
            {
                // En cas d'erreur, on l'enregistre
                return View("Error");
            }
        }

        // POST: Animals/Create
        // Traite le formulaire de cr�ation d'un animal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            // [Bind] sp�cifie quels champs du formulaire on accepte
            [Bind("Name,Description,BreedId")] Animal animal,
            string? returnUrl)
        {
            try
            {
                // On v�rifie si les donn�es du formulaire sont valides
                if (ModelState.IsValid)
                {
                    // On ajoute l'animal � la base de donn�es
                    _context.Add(animal);
                    // On sauvegarde les changements
                    await _context.SaveChangesAsync();

                    // Si une URL de retour est sp�cifi�e et est valide
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl); // On redirige vers cette URL
                    }
                    // Sinon on redirige vers la liste des animaux
                    return RedirectToAction(nameof(Index));
                }

                // Si le formulaire n'est pas valide, on recharge les listes d�roulantes
                ViewData["BreedId"] = new SelectList(
                    await _context.Breeds
                        .OrderBy(b => b.BreedName)
                        .ToListAsync(),
                    "BreedId",
                    "BreedName",
                    animal.BreedId);

                // On garde l'URL de retour
                ViewData["ReturnUrl"] = returnUrl;
                // On r�affiche le formulaire avec les erreurs
                return View(animal);
            }
            catch
            {
                // On ajoute un message d'erreur
                ModelState.AddModelError("", "Une erreur est survenue lors de la cr�ation de l'animal.");
                // On r�affiche le formulaire
                return View(animal);
            }
        }

        // GET: Animals/Edit/5
        // Affiche le formulaire de modification d'un animal
        public async Task<IActionResult> Edit(int? id, Animal animal)
        {
            // V�rifie si un ID a �t� fourni
            if (id == null)
            {
                return NotFound(); // Si pas d'ID, on renvoie une erreur 404
            }

            try
            {
                // V�rifie si l'animal existe
                if (animal == null)
                {
                    return NotFound(); // Si non, on renvoie une erreur 404
                }

                // Pr�pare la liste des races pour le formulaire
                ViewData["BreedId"] = new SelectList(
                    await _context.Breeds
                        .OrderBy(b => b.BreedName)
                        .ToListAsync(),
                    "BreedId",
                    "BreedName",
                    animal.BreedId);

                return View(animal);
            }
            catch
            {
                return View("Error");
            }
        }

        // POST: Animals/Edit/5
        // Traite le formulaire de modification d'un animal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("AnimalId,Name,Description,BreedId")] Animal animal)
        {
            // V�rifie la coh�rence des IDs
            if (id != animal.AnimalId)
            {
                return NotFound();
            }

            try
            {
                // R�cup�re l'animal existant avec toutes ses relations
                var existingAnimal = await _context.Animals
                    .FirstOrDefaultAsync(a => a.AnimalId == id);

                // V�rifie si l'animal existe
                if (existingAnimal == null)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        // Met � jour l'animal dans la base de donn�es
                        _context.Update(animal);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AnimalExists(animal.AnimalId))
                        {
                            return NotFound();
                        }
                        throw;
                    }
                }

                // Recharge les listes en cas d'erreur de validation
                ViewData["BreedId"] = new SelectList(await _context.Breeds.ToListAsync(), "BreedId", "BreedName", animal.BreedId);
                return View(animal);
            }
            catch
            {
                ModelState.AddModelError("", "Une erreur est survenue lors de la modification de l'animal.");
                return View(animal);
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
                // R�cup�re l'animal avec ses relations
                var animal = await _context.Animals
                    .Include(a => a.Breed)
                    .FirstOrDefaultAsync(m => m.AnimalId == id);

                if (animal == null)
                {
                    return NotFound();
                }

                return View(animal);
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
                // R�cup�re l'animal avec ses inscriptions futures
                var animal = await _context.Animals
                    .FirstOrDefaultAsync(a => a.AnimalId == id);

                if (animal == null)
                {
                    return NotFound();
                }

                // Supprime l'animal
                _context.Animals.Remove(animal);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Error");
            }
        }

        // V�rifie si un animal existe dans la base de donn�es
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
                // Pr�pare la liste d�roulante des races
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
            [Bind("Name,Description,BreedId")] Animal animal)
        {
            try
            {
                // Enregistre les donn�es re�ues dans les logs
                LogInformation("Donn�es re�ues : {Animal}", JsonSerializer.Serialize(animal));

                // Validation manuelle des champs
                var errors = new List<string>();

                // V�rifie le nom
                if (string.IsNullOrWhiteSpace(animal.Name))
                    errors.Add("Le nom est obligatoire");
                else if (animal.Name.Length > 50)
                    errors.Add("Le nom ne peut pas d�passer 50 caract�res");

                // V�rifie la description
                if (animal.Description == default)
                    errors.Add("La date de naissance est obligatoire");
                else if (animal.Description.Length > 2000)
                    errors.Add("La description ne peut pas d�passer 2000 caract�res");

                // V�rifie la race
                if (animal.BreedId <= 0)
                    errors.Add("La race est obligatoire");

                // Si des erreurs sont trouv�es, on les renvoie
                if (errors.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Erreur de validation du formulaire",
                        errors = errors
                    });
                }

                // V�rifie que la race existe
                var breed = await _context.Breeds.FindAsync(animal.BreedId);
                if (breed == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Erreur de validation du formulaire",
                        errors = new[] { "La race s�lectionn�e n'existe pas" }
                    });
                }

                // Sauvegarde l'animal dans la base de donn�es
                _context.Add(animal);
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
    }
}