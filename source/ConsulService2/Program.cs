using Consul;
using ConsulService2.Models;
using ConsulService2.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();

string url = Environment.GetEnvironmentVariable("ASPNETCORE_URL") ?? "http://localhost:5775";
builder.WebHost.UseUrls(url);

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpRequest, ConsulService2.Services.HttpRequest>();

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


var configuration = builder.Configuration;
var rabbitUrl = configuration["Rabbit:Url"];
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration["Rabbit:Url"], c =>
        {
            c.Username(configuration["Rabbit:User"]);
            c.Password(configuration["Rabbit:Password"]);
        });

        cfg.ClearSerialization();
        cfg.UseRawJsonSerializer();
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseAuthorization();

app.MapControllers();

app.Run();

