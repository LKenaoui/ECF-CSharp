using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using ECF.Controllers;
using ECF.Database.Context;
using ECF.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestECF
{
    [TestClass]
    public class AnimalControllerTests
    {
        public required EcfDbContext _context;
        public required AnimalController _controller;
        public required ILogger<AnimalController> _logger;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<EcfDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new EcfDbContext(options);
            _logger = Mock.Of<ILogger<AnimalController>>();
            _controller = new AnimalController(_context, _logger);
        }

        [TestMethod]
        public async Task Index_ReturnsViewResult_WithListOfAnimals()
        {
            // Créer les races nécessaires
            var breeds = new List<Breed>
            {
                new Breed { BreedName = "TestBreed1", Description = "TestDescription1" },
                new Breed { BreedName = "TestBreed2", Description = "TestDescription2" }
            };

            await _context.Breeds.AddRangeAsync(breeds);
            await _context.SaveChangesAsync();

            var animals = new List<Animal>
            {
                new Animal { Name = "Test1", Description = "Description1", BreedId = breeds[0].BreedId },
                new Animal { Name = "Test2", Description = "Description2", BreedId = breeds[1].BreedId }
            };

            await _context.Animals.AddRangeAsync(animals);
            await _context.SaveChangesAsync();

            var result = await _controller.Index();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as List<Animal>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
        }

        [TestMethod]
        public async Task Create_WithValidData_RedirectsToIndex()
        {
            var breed = new Breed { BreedName = "TestBreed", Description = "TestDescription" };
            await _context.Breeds.AddAsync(breed);
            await _context.SaveChangesAsync();

            var animal = new Animal
            {
                Name = "TestAnimal",
                Description = "TestDescription",
                BreedId = breed.BreedId
            };

            var result = await _controller.Create(animal, null);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [TestMethod]
        public async Task Edit_WithValidData_RedirectsToIndex()
        {
            var breed = new Breed { BreedName = "TestBreed", Description = "TestDescription" };
            await _context.Breeds.AddAsync(breed);
            await _context.SaveChangesAsync();

            var animal = new Animal
            {
                Name = "TestAnimal",
                Description = "TestDescription",
                BreedId = breed.BreedId
            };

            await _context.Animals.AddAsync(animal);
            await _context.SaveChangesAsync();

            animal.Name = "UpdatedName";
            var result = await _controller.Edit(animal.AnimalId, animal);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [TestMethod]
        public async Task Delete_WithValidId_RedirectsToIndex()
        {
            var breed = new Breed { BreedName = "TestBreed", Description = "TestDescription" };
            await _context.Breeds.AddAsync(breed);
            await _context.SaveChangesAsync();

            var animal = new Animal
            {
                Name = "TestAnimal",
                Description = "TestDescription",
                BreedId = breed.BreedId
            };

            await _context.Animals.AddAsync(animal);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteConfirmed(animal.AnimalId);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.IsFalse(await _context.Animals.AnyAsync(a => a.AnimalId == animal.AnimalId));
        }

        [TestMethod]
        public async Task Search_WithValidTerm_ReturnsMatchingAnimals()
        {
            var breed = new Breed { BreedName = "TestBreed", Description = "TestDescription" };
            await _context.Breeds.AddAsync(breed);
            await _context.SaveChangesAsync();

            var animals = new List<Animal>
            {
                new Animal { Name = "Test1", Description = "Description1", BreedId = breed.BreedId },
                new Animal { Name = "Test2", Description = "Description2", BreedId = breed.BreedId }
            };

            await _context.Animals.AddRangeAsync(animals);
            await _context.SaveChangesAsync();

            var result = await _controller.Search("Test1");

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as List<Animal>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Test1", model[0].Name);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
} 