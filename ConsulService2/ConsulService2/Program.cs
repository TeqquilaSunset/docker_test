using ConsulService1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();



// Получение адреса хоста извне или использования стандартного
<<<<<<< HEAD
string url = Environment.GetEnvironmentVariable("ASPNETCORE_URL") ?? "http://localhost:57400";
=======
string url = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://0.0.0.0:6000";
>>>>>>> c5bb339b3740bee56ecd58f72c2056a59a95fb37
builder.WebHost.UseUrls(url);

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IConsulHttpClient, ConsulHttpClientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// устанавливаем сопоставление маршрутов с контроллерами
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Получите экземпляр сервиса
var consulHttpClient = app.Services.GetRequiredService<IConsulHttpClient>();

// Регистрация при запуске
try
{
    await consulHttpClient.RegisterServiceAsync();
}
catch (Exception e) { }


// Получите экземпляр IHostApplicationLifetime 
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

// Обратный вызов для дерегистрации сервиса при остановке
lifetime.ApplicationStopping.Register(async () =>
{
    await consulHttpClient.DeregisterServiceAsync();
});

app.Run();

