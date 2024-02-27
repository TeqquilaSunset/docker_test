using Consul;
using ConsulService2.Models;
using ConsulService2.Services;
using MassTransit;
using System.Text;

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

var consulClient = new ConsulClient(x => x.Address = new Uri(urlConsul)); // адрес вашего Consul
var kvPairs = consulClient.KV.List("service1").Result.Response; // ваш префикс в Consul
var dict = new Dictionary<string, string>();
foreach (var kvPair in kvPairs)
{
    dict.Add(kvPair.Key, Encoding.UTF8.GetString(kvPair.Value));
}
builder.Configuration.AddInMemoryCollection(dict.Select(kv => new KeyValuePair<string, string?>(kv.Key, kv.Value)));

var serviceQueryResult = consulClient.Health.Service("rabbitmq").Result;
var nbServices = serviceQueryResult?.Response?.Length;
var rabbiturl = "rabbitmq://localhost";
if (nbServices > 0)
{
    Console.WriteLine($"{nbServices} service(s) found");
    var service = serviceQueryResult?.Response[0]!;
    rabbiturl = service.Service.Address;
}

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbiturl", c =>
        {
            c.Username("rmuser");
            c.Password("rmpassword");
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
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseAuthorization();

app.MapControllers();

app.Run();

