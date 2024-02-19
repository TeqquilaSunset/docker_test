using ConsulService1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();



// ��������� ������ ����� ����� ��� ������������� ������������
string url = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://localhost:6000";
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
// ������������� ������������� ��������� � �������������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// �������� ��������� �������
var consulHttpClient = app.Services.GetRequiredService<IConsulHttpClient>();

// ����������� ��� �������
try
{
    await consulHttpClient.RegisterServiceAsync();
}
catch (Exception e) { }


// �������� ��������� IHostApplicationLifetime 
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

// �������� ����� ��� ������������� ������� ��� ���������
lifetime.ApplicationStopping.Register(async () =>
{
    await consulHttpClient.DeregisterServiceAsync();
});

app.Run();

