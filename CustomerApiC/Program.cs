using Common.EventStoreCQRS;
using CustomerApi.Infrastructure;
using CustomerApiC.CommandHandlers;
using CustomerApiC.Commands;
using SharedModels;
using SharedModels.EventStoreCQRS;

var builder = WebApplication.CreateBuilder(args);

// Connect to EventStoreDB.
string gRpcConnectionString = "esdb://customereventstore.db:2114?tls=false";

string cloudAMQPConnectionString = "host=rabbitmq";

string productServiceBaseUrl = "http://productapiq/products/";

// Add services to the container.

// Register product service gateway for dependency injection
builder.Services.AddSingleton<IServiceGateway<ProductDto>>(new
    ProductServiceGateway(productServiceBaseUrl));

builder.Services.AddEventStoreClient(gRpcConnectionString);

builder.Services.AddScoped<EventSerializer>();

builder.Services.AddScoped<EventDeserializer>();

builder.Services.AddScoped<ICommandHandler<CreateCustomer>, CreateCustomerCommandHandler>();
builder.Services.AddScoped<ICommandHandler<ChangeCustomerInfo>, ChangeCustomerInfoCommandHandler>();
builder.Services.AddScoped<ICommandHandler<ChangeCustomerCreditStanding>, ChangeCustomerCreditStandingCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteCustomer>, DeleteCustomerCommandHandler>();

// Regiser email service for dependency injection
builder.Services.AddScoped<IEmailService, EmailServiceStub>();


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

Console.WriteLine("Started listening program version after pushv3");
Task.Factory.StartNew(() =>
    new MessageListener(app.Services, cloudAMQPConnectionString).Start());

app.UseAuthorization();

app.MapControllers();

app.Run();
