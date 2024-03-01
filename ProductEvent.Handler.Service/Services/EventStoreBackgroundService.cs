using System.Reflection;
using System.Text.Json;
using MongoDB.Driver;
using Shared.Events;
using Shared.Models;
using Shared.Services.Abstractions;

namespace ProductEvent.Handler.Service.Services
{
    public class EventStoreBackgroundService : BackgroundService
    {
        readonly IEventstoreService _eventStoreService;
        readonly IMongoDBService _mongoDbService;

        public EventStoreBackgroundService(IEventstoreService eventStoreService, IMongoDBService mongoDbService)
        {
            _eventStoreService = eventStoreService;
            _mongoDbService = mongoDbService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _eventStoreService.SubscribeToStreamAsync(
                "product-stream", async (streamSubscription, resolvedEvent, cancellationToken) =>
                {
                    string eventType = resolvedEvent.Event.EventType;
                    object @event = JsonSerializer.Deserialize(resolvedEvent.Event.Data.ToArray(), Assembly.Load("Shared").GetTypes().FirstOrDefault(c => c.Name == eventType));

                    var productCollection = _mongoDbService.GetCollection<Product>("Products");
                    Shared.Models.Product product = null;

                    switch (@event)
                    {
                        case NewProductAddedEvent e:
                            var hasProduct = await (await productCollection.FindAsync(p =>
                            p.Id.ToString() == e.ProductId)).AnyAsync();
                            if (!hasProduct)
                            {
                                await productCollection.InsertOneAsync(new()
                                {
                                    Id = e.ProductId,
                                    ProductName = e.ProductName,
                                    Count = e.InitialCount,
                                    IsAvilable = e.IsAvailable,
                                    Price = e.InitialPrice
                                });
                            }

                            break;
                        case CountDecreasedEvent e:
                            product = await GetProductAsync(e.ProductId, productCollection); ;

                            if (product != null)
                            {
                                product.Count -= e.DecrementAmount;
                                await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product);
                            }

                            break;
                        case CountIncreasedEvent e:
                            product = await GetProductAsync(e.ProductId, productCollection);

                            if (product != null)
                            {
                                product.Count += e.incrementAmount;
                                await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product);
                            }
                            break;
                        case PriceDecreasedEvent e:
                            product = await GetProductAsync(e.ProductId, productCollection);

                            if (product != null)
                            {
                                product.Price -= e.DecrementAmount;
                                await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product);
                            }
                            break;
                        case PriceIncreasedEvent e:
                            product = await GetProductAsync(e.ProductId, productCollection);

                            if (product != null)
                            {
                                product.Price += e.incrementAmount;
                                await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product);
                            }
                            break;
                        case AvailableChangedEvent e:
                            product = await GetProductAsync(e.ProductId, productCollection);

                            if (product != null)
                            {
                                product.IsAvilable = e.IsAvailable;
                                await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product);
                            }
                            break;
                    }
                });
        }

        private async Task<Product> GetProductAsync(string productNumber, IMongoCollection<Product> collections)
        {
            return await (await collections.FindAsync(c => c.Id == productNumber)).FirstOrDefaultAsync();
        }
    }
}

