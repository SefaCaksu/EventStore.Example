//using ProductEvent.Handler.Service;

//IHost host = Host.CreateDefaultBuilder(args)
//    .ConfigureServices(services =>
//    {
//        services.AddHostedService<Worker>();
//    })
//    .Build();

using ProductEvent.Handler.Service.Services;
using Shared.Services;
using Shared.Services.Abstractions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IEventstoreService, EventstoreService>();
builder.Services.AddSingleton<IMongoDBService, MongoDBService>();

builder.Services.AddHostedService<EventStoreBackgroundService>();

var host = builder.Build();
host.Run();

