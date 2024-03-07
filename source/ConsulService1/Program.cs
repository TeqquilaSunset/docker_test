using Consul;
using ConsulService1;
using ConsulService1.Consumers;
using ConsulService1.Models;
using ConsulService1.Services;
using Core.Models;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using ConsulService2.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Получение адреса хоста извне или использования стандартного
string url = Environment.GetEnvironmentVariable("ASPNETCORE_URL") ?? "http://localhost:57369";
builder.WebHost.UseUrls(url);

builder.Services.AddHttpClient();
builder.Services.AddScoped<IPredictionsGenerator, PredictionService>();

string urlConsul = Environment.GetEnvironmentVariable("CONSUL_URL") ?? "http://localhost:8500";
builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    consulConfig.Address = new Uri(urlConsul);
}));
builder.Services.AddSingleton<IHostedService, ConsulHostedService>();



var consulClient = new ConsulClient(configuration =>
{
    configuration.Address = new Uri(urlConsul);
});

var getPairRabbit = consulClient.KV.Get("rabbit").Result;
if (getPairRabbit?.Response != null)
{
    var value = Encoding.UTF8.GetString(getPairRabbit.Response.Value, 0, getPairRabbit.Response.Value.Length);
    var consulConfig2 = JsonSerializer.Deserialize<RabbitConfig>(value);

    builder.Configuration["Rabbit:Password"] = consulConfig2.Password;
    builder.Configuration["Rabbit:User"] = consulConfig2.User;
    builder.Configuration["Rabbit:Url"] = consulConfig2.Url;
}

var getPairFabio = consulClient.KV.Get("fabio").Result;
if (getPairRabbit?.Response != null)
{
    var value = Encoding.UTF8.GetString(getPairRabbit.Response.Value, 0, getPairRabbit.Response.Value.Length);
    var consulConfig3 = JsonSerializer.Deserialize<FabioConfig>(value);

    builder.Configuration["Fabio:Adress"] = consulConfig3.Adress;
    builder.Configuration["Fabio:Port"] = consulConfig3.Port.ToString();
}

var getPairDatabase = consulClient.KV.Get("database").Result;
if (getPairRabbit?.Response != null)
{
    var value = Encoding.UTF8.GetString(getPairDatabase.Response.Value, 0, getPairDatabase.Response.Value.Length);
    var consulConfig3 = JsonSerializer.Deserialize<DbConfig>(value);

    builder.Configuration["ConnectionStrings:DefaultConnection"] = consulConfig3.DefaultConnection;
}
var fgfgfg = builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseNpgsql(fgfgfg));

var configuration = builder.Configuration;
var rabbitUrl = configuration["Rabbit:Url"];
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PredictionConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitUrl, c =>
        {
            c.Username("rmuser");
            c.Password("rmpassword");
        });

        cfg.ReceiveEndpoint("PredictionQueue", e =>
        {
            e.ConfigureConsumer<PredictionConsumer>(context);
        });

        cfg.Publish<NewPrediction>(e =>
        {
            e.ExchangeType = ExchangeType.Fanout;
        });

        cfg.ClearSerialization();
        cfg.UseRawJsonSerializer();
        cfg.ConfigureEndpoints(context);
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();



app.Run();
