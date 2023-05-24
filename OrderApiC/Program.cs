using Common.EventStoreCQRS;
using OrderApiC;
using OrderApiC.CommandHandlers;
using OrderApiC.Commands;
using OrderApiC.Infrastructure;
using OrderApiC.Models.Converters;
using SharedModels;
using SharedModels.EventStoreCQRS;

// EventStoreDB connection string.
string gRpcConnectionString = "esdb://ordereventstore.db:2115?tls=false";

// RabbitMQ connection string (I use CloudAMQP as a RabbitMQ server).
// Remember to replace this connectionstring with your own.
string cloudAMQPConnectionString = "host=rabbitmq";

// Base URL for the product service when the solution is executed using docker-compose.
// The product service (running as a container) listens on this URL for HTTP requests
// from other services specified in the docker compose file (which in this solution is
// the order service).
string productServiceBaseUrl = "http://productapiq/products/";

string customerServiceBaseUrl = "http://customerapiq/Customers/";


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add the eventstore client to the service container.
builder.Services.AddEventStoreClient(gRpcConnectionString);

builder.Services.AddScoped<EventSerializer>();

builder.Services.AddScoped<EventDeserializer>();

// Register OrderConverter for dependency injection
builder.Services.AddSingleton<IConverter<Order, OrderDto>, OrderConverter>();

// Add all the command handlers to the service container
builder.Services.AddScoped<ICommandHandler<CancelOrder>, CancelOrderCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CompleteOrder>, CompleteOrderCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CreateOrder>, CreateOrderCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteOrder>, DeleteOrderrCommandHandler>();
builder.Services.AddScoped<ICommandHandler<PayforOrder>, PayforOrderCommandHandler>();
builder.Services.AddScoped<ICommandHandler<ShipOrder>, ShipOrderCommandHandler>();


// Register product service gateway for dependency injection
builder.Services.AddSingleton<IServiceGateway<ProductDto>>(new
    ProductServiceGateway(productServiceBaseUrl));

builder.Services.AddSingleton<IServiceGateway<CustomerDto>>(new CustomerServiceGateway(customerServiceBaseUrl));


// Register MessagePublisher (a messaging gateway) for dependency injection
builder.Services.AddSingleton<IMessagePublisher>(new
    MessagePublisher(cloudAMQPConnectionString));


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

app.UseAuthorization();

app.MapControllers();

app.Run();
