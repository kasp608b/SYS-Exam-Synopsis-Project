using Common.EventStoreCQRS;
using CustomerApi.Data;
using CustomerApi.Infrastructure;
using CustomerApi.Models;
using CustomerApiQ.EventHandlers;
using CustomerApiQ.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using SharedModels;
using SharedModels.CustomerAPICommon.Events;
using SharedModels.EventStoreCQRS;


// EventStoreDB connection string.
string gRpcConnectionString = "esdb://customereventstore.db:2114?tls=false";

var builder = WebApplication.CreateBuilder(args);

// Base URL for the product service when the solution is executed using docker-compose.
// The product service (running as a container) listens on this URL for HTTP requests
// from other services specified in the docker compose file (which in this solution is
// the order service).
string productServiceBaseUrl = "http://productapiq/products/";


// RabbitMQ connection string (I use CloudAMQP as a RabbitMQ server).
// Remember to replace this connectionstring with your own.
string cloudAMQPConnectionString = "host=rabbitmq";

// Add services to the container.

builder.Services.AddDbContext<CustomerApiContext>(opt => opt.UseInMemoryDatabase("CustomersDb"));

// Add the eventstore client to the service container.
builder.Services.AddEventStoreClient(gRpcConnectionString);

builder.Services.AddScoped<EventSerializer>();

builder.Services.AddScoped<EventDeserializer>();

// Add all the event handlers to the service container
builder.Services.AddScoped<IEventHandler<CustomerCreated>, CustomerCreatedEventHandler>();
builder.Services.AddScoped<IEventHandler<CustomerDeleted>, CustomerDeletedEventHandler>();
builder.Services.AddScoped<IEventHandler<CustomerCreditStandingChanged>, CustomerCreditStandingChangedEventHandler>();
builder.Services.AddScoped<IEventHandler<CustomerInfoChanged>, CustomerInfoChangedEventHandler>();

// Register repositories for dependenct injection
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Register database initializer for dependency injection
builder.Services.AddTransient<IDbInitializer, DbInitializer>();


// Register product service gateway for dependency injection
builder.Services.AddSingleton<IServiceGateway<ProductDto>>(new
    ProductServiceGateway(productServiceBaseUrl));

// Regiser email service for dependency injection
builder.Services.AddScoped<IEmailService, EmailServiceStub>();

// Register CustomerConverter for dependency injection
builder.Services.AddSingleton<IConverter<Customer, CustomerDto>, CustomerConverter>();

// Register MessagePublisher (a messaging gateway) for dependency injection
builder.Services.AddSingleton<IMessagePublisher>(new
    MessagePublisher(cloudAMQPConnectionString));

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
    var dbContext = services.GetService<CustomerApiContext>();
    var dbInitializer = services.GetService<IDbInitializer>();
    dbInitializer.Initialize(dbContext);
}

// Create a message listener in a separate thread.
Console.WriteLine("Started listening program version after pushv3");
Task.Factory.StartNew(() =>
    new MessageListener(app.Services, cloudAMQPConnectionString).Start());

Console.WriteLine("EventSubscriber connected to EventStoreDB");
Task.Factory.StartNew(() =>
    new EventSubscriberTask(app.Services).StartAsync());

//app.UseHttpsRedirection();

//app.UseHttpsRedirection();

app.UseHttpMetrics();

app.UseAuthorization();

app.MapControllers();

app.MapMetrics();

app.Run();
