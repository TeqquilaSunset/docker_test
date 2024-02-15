using ConsulService1.Services;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Получение адреса хоста из вне или использования стандартного
string url = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://0.0.0.0:5000";
builder.WebHost.UseUrls(url);

builder.Services.AddHttpClient();

builder.Services.AddScoped<IPredictionsGenerator, PredictionService>();
builder.Services.AddSingleton<IConsulHttpClient, ConsulHttpClientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Получите экземпляр сервиса
var consulHttpClient = app.Services.GetRequiredService<IConsulHttpClient>();

// Регистрация при запуске
try
{
    await consulHttpClient.RegisterServiceAsync();
}
catch (Exception e) { }


app.UseAuthorization();
app.MapControllers();

// Получите экземпляр IHostApplicationLifetime 
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

// Обратный вызов для дерегистрации сервиса при остановке
lifetime.ApplicationStopping.Register(async () =>
{
    await consulHttpClient.DeregisterServiceAsync();
});

app.Run();
