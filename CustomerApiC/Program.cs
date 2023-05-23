using Common.EventStoreCQRS;

var builder = WebApplication.CreateBuilder(args);

// Connect to EventStoreDB.
string gRpcConnectionString = "esdb://customereventstore.db:2114?tls=false";

string cloudAMQPConnectionString = "host=rabbitmq";

// Add services to the container.

builder.Services.AddEventStoreClient(gRpcConnectionString);

builder.Services.AddScoped<EventSerializer>();

builder.Services.AddScoped<EventDeserializer>();

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
