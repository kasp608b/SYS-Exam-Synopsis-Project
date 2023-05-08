using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Infrastructure;
using OrderApi.Models;
using Prometheus;
using SharedModels;


var builder = WebApplication.CreateBuilder(args);


// Base URL for the product service when the solution is executed using docker-compose.
// The product service (running as a container) listens on this URL for HTTP requests
// from other services specified in the docker compose file (which in this solution is
// the order service).
string productServiceBaseUrl = "http://productapi/products/";

string customerServiceBaseUrl = "http://customerapi/Customers/";


// RabbitMQ connection string (I use CloudAMQP as a RabbitMQ server).
// Remember to replace this connectionstring with your own.
string cloudAMQPConnectionString = "host=rabbitmq";

// Use this connection string if you want to run RabbitMQ server as a container
// (see docker-compose.yml)
//string cloudAMQPConnectionString = "host=rabbitmq";

// Add services to the container.

builder.Services.AddDbContext<OrderApiContext>(opt => opt.UseInMemoryDatabase("OrdersDb"));

// Register repositories for dependency injection
builder.Services.AddScoped<IRepository<Order>, OrderRepository>();

// Register database initializer for dependency injection
builder.Services.AddTransient<IDbInitializer, DbInitializer>();

// Register OrderConverter for dependency injection
builder.Services.AddSingleton<IConverter<Order, OrderDto>, OrderConverter>();


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
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

// Initialize the database.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetService<OrderApiContext>();
    var dbInitializer = services.GetService<IDbInitializer>();
    dbInitializer.Initialize(dbContext);
}

//app.UseHttpsRedirection();

app.UseHttpMetrics();

app.UseAuthorization();

app.MapControllers();

app.MapMetrics();

app.Run();
