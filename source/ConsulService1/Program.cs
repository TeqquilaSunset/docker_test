using Consul;
using ConsulService1.Consumers;
using ConsulService1.Services;
using Core.Models;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Получение адреса хоста извне или использования стандартного
string url = Environment.GetEnvironmentVariable("ASPNETCORE_URL") ?? "http://localhost:57369";
builder.WebHost.UseUrls(url);

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IPredictionsGenerator, PredictionService>();

string urlConsul = Environment.GetEnvironmentVariable("CONSUL_URL") ?? "http://localhost:8500";
builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    consulConfig.Address = new Uri(urlConsul);
}));
builder.Services.AddSingleton<IHostedService, ConsulHostedService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PredictionConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("host.docker.internal", c =>
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();



app.Run();
