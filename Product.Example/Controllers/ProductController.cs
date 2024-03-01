using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Product.App.Models.ViewModels;
using Shared.Events;
using Shared.Services.Abstractions;
using System.Linq;


namespace Product.App.Controllers
{
    public class ProductController : Controller
    {
        readonly IEventstoreService _eventstoreService;
        readonly IMongoDBService _mongoDBService;

        public ProductController(IEventstoreService eventstoreService, IMongoDBService mongoDBService)
        {
            _eventstoreService = eventstoreService;
            _mongoDBService = mongoDBService;
        }

        // GET: Product
        public async Task<ActionResult> Index()
        {
            var collections = _mongoDBService.GetCollection<Shared.Models.Product>("Products");
            var products = await (await collections.FindAsync(_ => true)).ToListAsync();

            var result = products.Select(c => new ListProductVM
            {
                Id = c.Id,
                Count = c.Count,
                IsAvailable = c.IsAvilable,
                Price = c.Price,
                ProductName = c.ProductName
            });

            return View(result);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateProductVM model)
        {
            NewProductAddedEvent newProductAddedEvent = new()
            {
                ProductId = Guid.NewGuid().ToString(),
                InitialCount = model.Count,
                InitialPrice = model.Price,
                IsAvailable = model.IsAvailable,
                ProductName = model.ProductName
            };

            await _eventstoreService.AppendToStreamAsync("product-stream", new[] { _eventstoreService.GenerateEventData(newProductAddedEvent) });
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(string id)
        {
            var collections = _mongoDBService.GetCollection<Shared.Models.Product>("Products");
            var product = await (await collections.FindAsync(c => c.Id == id)).FirstOrDefaultAsync();

            var model = new EditProductVM
            {
                Id = product.Id,
                Count = product.Count,
                IsAvailable = product.IsAvilable,
                Price = product.Price,
                ProductName = product.ProductName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditCount(EditProductVM model)
        {
            var collections = _mongoDBService.GetCollection<Shared.Models.Product>("Products");
            var product = await (await collections.FindAsync(c => c.Id == model.Id)).FirstOrDefaultAsync();



            if (product.Count > model.Count)
            {
                CountDecreasedEvent countDecreasedEvent = new()
                {
                    ProductId = model.Id,
                    DecrementAmount = model.Count
                };

                await _eventstoreService.AppendToStreamAsync("product-stream", new[] { _eventstoreService.GenerateEventData(countDecreasedEvent) });
            }
            else if (product.Count < model.Count)
            {
                CountIncreasedEvent countIncreasedEvent = new()
                {
                    ProductId = model.Id,
                    incrementAmount = model.Count
                };

                await _eventstoreService.AppendToStreamAsync("product-stream", new[] { _eventstoreService.GenerateEventData(countIncreasedEvent) });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> EditPrice(EditProductVM model)
        {
            var collections = _mongoDBService.GetCollection<Shared.Models.Product>("Products");
            var product = await (await collections.FindAsync(c => c.Id == model.Id)).FirstOrDefaultAsync();

            if (product.Price > model.Price)
            {
                PriceDecreasedEvent priceDecreasedEvent = new()
                {
                    ProductId = model.Id,
                    DecrementAmount = model.Price
                };

                await _eventstoreService.AppendToStreamAsync("product-stream", new[] { _eventstoreService.GenerateEventData(priceDecreasedEvent) });
            }
            else if (product.Price < model.Price)
            {
                PriceIncreasedEvent priceIncreasedEvent = new()
                {
                    ProductId = model.Id,
                    incrementAmount = model.Price
                };

                await _eventstoreService.AppendToStreamAsync("product-stream", new[] { _eventstoreService.GenerateEventData(priceIncreasedEvent) });
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<ActionResult> EditIsAvailable(EditProductVM model)
        {
            var collections = _mongoDBService.GetCollection<Shared.Models.Product>("Products");
            var product = await (await collections.FindAsync(c => c.Id == model.Id)).FirstOrDefaultAsync();

            if (product.IsAvilable != model.IsAvailable)
            {
                AvailableChangedEvent availableChangedEvent = new()
                {
                    ProductId = model.Id,
                    IsAvailable = model.IsAvailable
                };

                await _eventstoreService.AppendToStreamAsync("product-stream", new[] { _eventstoreService.GenerateEventData(availableChangedEvent) });
            }

            return RedirectToAction("Index");
        }

    }
}