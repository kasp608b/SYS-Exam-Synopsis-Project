using Common.EventStoreCQRS;
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Infrastructure;
using ProductApi.Models;
using ProductApiQ.EventHandlers;
using ProductApiQ.Infrastructure;
using Prometheus;
using SharedModels;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

// EventStoreDB connection string.
string gRpcConnectionString = "esdb://producteventstore.db:2113?tls=false";

var builder = WebApplication.CreateBuilder(args);

//string cloudAMQPConnectionString = "host=hawk-01.rmq.cloudamqp.com;virtualHost=npaprqop;username=npaprqop;password=type your password here";

string cloudAMQPConnectionString = "host=rabbitmq";

// Add services to the container.

builder.Services.AddDbContext<ProductApiContext>(opt => opt.UseInMemoryDatabase("ProductsDb"));

// Add the eventstore client to the service container.
builder.Services.AddEventStoreClient(gRpcConnectionString);

builder.Services.AddScoped<EventSerializer>();

builder.Services.AddScoped<EventDeserializer>();

// Add all the event handlers to the service container
builder.Services.AddScoped<IEventHandler<ItemsAddedToStock>, ItemsAddedToStockEventHandler>();
builder.Services.AddScoped<IEventHandler<ItemsRemovedFromStock>, ItemsRemovedFromStockEventHandler>();
builder.Services.AddScoped<IEventHandler<ProductCategoryChanged>, ProductCategoryChangedEventHandler>();
builder.Services.AddScoped<IEventHandler<ProductCreated>, ProductCreatedEventHandler>();
builder.Services.AddScoped<IEventHandler<ProductDeleted>, ProductDeletedEventHandler>();
builder.Services.AddScoped<IEventHandler<ProductNameChanged>, ProductNameChangedEventHandler>();
builder.Services.AddScoped<IEventHandler<ProductPriceChanged>, ProductPriceChangedEventHandler>();
builder.Services.AddScoped<IEventHandler<ProductShipped>, ProductShippedEventHandler>();
builder.Services.AddScoped<IEventHandler<ReservedItemsDecreased>, ReservedItemsDecreasedEventHandler>();
builder.Services.AddScoped<IEventHandler<ReservedItemsIncreased>, ReservedItemsIncreasedEventHandler>();

// Register repositories for dependency injection
builder.Services.AddScoped<IRepository<Product>, ProductRepository>();

// Register database initializer for dependency injection
builder.Services.AddTransient<IDbInitializer, DbInitializer>();

// Register ProductConverter for dependency injection
builder.Services.AddSingleton<IConverter<Product, ProductDto>, ProductConverter>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

// Initialize the database.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetService<ProductApiContext>();
    var dbInitializer = services.GetService<IDbInitializer>();
    dbInitializer.Initialize(dbContext);
}
// Create a message listener in a separate thread.
Console.WriteLine("Started listening program wow this should work");
Task.Factory.StartNew(() =>
    new MessageListener(app.Services, cloudAMQPConnectionString).StartAsync());
//app.UseHttpsRedirection();

Console.WriteLine("EventSubscriber connected to EventStoreDB");
Task.Factory.StartNew(() =>
    new EventSubscriberTask(app.Services).StartAsync());

app.UseHttpMetrics();

app.UseAuthorization();

app.MapControllers();

app.MapMetrics();

app.Run();