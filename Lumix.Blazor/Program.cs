using System.Net.Http.Headers;
using System.Net.Security;
using Lumix.Blazor.Data;
using Lumix.Blazor.Services;
using Lumix.Blazor.Services.IServices;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Service 
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddMudServices();

// HTTP and API 
builder.Services.AddHttpClient<HttpService>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5207/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
            KeepAlivePingPolicy = HttpKeepAlivePingPolicy.WithActiveRequests,
            EnableMultipleHttp2Connections = true,
            SslOptions = new SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (sender, cert, chain, errors) => true
            }
        };
    });

builder.Services.AddScoped<HttpService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Logging 
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// CORS 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Middleware 
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll"); 

// Endpoint 
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();