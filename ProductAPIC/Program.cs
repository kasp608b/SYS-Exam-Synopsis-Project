using Common.EventStoreCQRS;
using EventStore.Client;
using ProductAPIC.Command;
using ProductAPIC.CommandHandlers;
using ProductAPIC.Commands;
using ProductAPIC.Infrastructure;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

var builder = WebApplication.CreateBuilder(args);

// Connect to EventStoreDB.
string gRpcConnectionString = "esdb://producteventstore.db:2113?tls=false";


string cloudAMQPConnectionString = "host=rabbitmq";

// Add services to the container.
builder.Services.AddEventStoreClient(gRpcConnectionString);

builder.Services.AddScoped<EventSerializer>();

builder.Services.AddScoped<EventDeserializer>();

builder.Services.AddScoped<ICommandHandler<AddItemsToStock>, AddItemsToStockCommandHandler>();
builder.Services.AddScoped<ICommandHandler<RemoveItemsFromStock>, RemoveItemsFromStockCommandHandler>();
builder.Services.AddScoped<ICommandHandler<ChangeProductCategory>, ChangeProductCategoryCommandHandler>();
builder.Services.AddScoped<ICommandHandler<ChangeProductName>, ChangeProductNameCommandHandler>();
builder.Services.AddScoped<ICommandHandler<ChangeProductPrice>, ChangeProductPriceCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CreateProduct>, CreateProductCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DecreaseReservedItems>, DecreaseReservedItemsCommandHandler>();
builder.Services.AddScoped<ICommandHandler<IncreaseReservedItems>, IncreaseReservedItemsCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteProduct>, DeleteProductCommandHandler>();
builder.Services.AddScoped<ICommandHandler<ShipProduct>, ShipProductCommandHandler>();





builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Create a message listener in a separate thread.
Console.WriteLine("Started listening program wow this should work");
Task.Factory.StartNew(() =>
    new MessageListener(app.Services, cloudAMQPConnectionString).StartAsync());

app.UseAuthorization();

app.MapControllers();

app.Run();
