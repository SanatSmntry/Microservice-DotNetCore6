using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//adding to client factory and managed by http client factory
////Doubt
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
// add rabbitmq service
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

// add grpc
builder.Services.AddGrpc();

// Add services to the container.

// getting some issue while doing ef migration
if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using SQL Server Db");
    builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else
{
    Console.WriteLine("--> Using InMem Db");
    builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("InMem"));
}

Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

PrepDb.PrepPopulation(app, app.Environment.IsProduction()); // initial seeding data

//app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

// map grpc service
app.MapGrpcService<GrpcPlatformService>();

app.MapGet("/protos/platforms.proto", async context => { await context.Response.WriteAsync(System.IO.File.ReadAllText("Protos/platforms.proto")); }) ;

app.Run();
