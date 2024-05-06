using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using ProyectoASPNET.Data;
using ProyectoASPNET.Helpers;
using ProyectoASPNET.Hubs;
using ProyectoASPNET.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient
        (builder.Configuration.GetSection("KeyVault"));
});

SecretClient secretClient = builder.Services.BuildServiceProvider()
    .GetService<SecretClient>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie();

builder.Services.AddAntiforgery();

builder.Services.AddControllersWithViews(options => options.EnableEndpointRouting = false);

builder.Services.AddHttpClient();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

KeyVaultSecret secretSignalR = await secretClient.GetSecretAsync("SignalRKey");
string signalRKey = secretSignalR.Value;
builder.Services.AddSignalR().AddAzureSignalR(signalRKey);

KeyVaultSecret secretCacheRedis = await secretClient.GetSecretAsync("CacheRedisKey");
string cacheRedisKey = secretCacheRedis.Value;
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = cacheRedisKey;
});

KeyVaultSecret secretConnectionString = await secretClient.GetSecretAsync("SqlAzure");
string connectionString = secretConnectionString.Value;
builder.Services.AddTransient<IServiceRestaurantes, ServiceApiRestaurantes>
    (s => new ServiceApiRestaurantes(secretClient, s.GetRequiredService<HelperCryptography>(), s.GetRequiredService<IHttpContextAccessor>(), s.GetRequiredService<RestaurantesContext>(), s.GetRequiredService<ServiceStorageBlobs>()));
builder.Services.AddTransient<ServiceStorageBlobs>();
builder.Services.AddTransient<ServiceCacheRedis>();
builder.Services.AddTransient<ServiceLogicApps>();
builder.Services.AddDbContext<RestaurantesContext>
    (options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<HelperMails>();
builder.Services.AddSingleton<HelperCryptography>();

KeyVaultSecret secretGoogleApi = await secretClient.GetSecretAsync("GoogleApiKey");
string googleApiKey = secretGoogleApi.Value;
builder.Services.AddTransient<HelperGoogleApiDirections>
    (h => new HelperGoogleApiDirections(googleApiKey, h.GetRequiredService<IHttpClientFactory>()));

builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

KeyVaultSecret secretStorageAccount = await secretClient.GetSecretAsync("StorageAccountKey");
string storageAccountKey = secretStorageAccount.Value;
BlobServiceClient blobServiceClient = new BlobServiceClient(storageAccountKey);
builder.Services.AddTransient<BlobServiceClient>(x => blobServiceClient);

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=Home}/{action=Index}");
});
app.MapHub<EstadoPedidoHub>("/estadoPedidoHub");

app.Run();
