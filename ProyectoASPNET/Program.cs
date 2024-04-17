using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProyectoASPNET.Data;
using ProyectoASPNET.Helpers;
using ProyectoASPNET.Services;

var builder = WebApplication.CreateBuilder(args);

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

string cacheRedisKeys =
    builder.Configuration.GetValue<string>("AzureKeys:CacheRedis");
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = cacheRedisKeys;
});

string connectionString = builder.Configuration.GetConnectionString("SqlServer");
builder.Services.AddTransient<IServiceRestaurantes, ServiceApiRestaurantes>();
builder.Services.AddTransient<ServiceStorageBlobs>();
builder.Services.AddTransient<ServiceCacheRedis>();
builder.Services.AddDbContext<RestaurantesContext>
    (options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<HelperMails>();
builder.Services.AddSingleton<HelperCryptography>();

string googleApiKey = builder.Configuration.GetValue<string>("GoogleApiKey");
builder.Services.AddTransient<HelperGoogleApiDirections>
    (h => new HelperGoogleApiDirections(googleApiKey, h.GetRequiredService<IHttpClientFactory>()));

builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

string azureKeys = builder.Configuration.GetValue<string>("AzureKeys:StorageAccount");
BlobServiceClient blobServiceClient = new BlobServiceClient(azureKeys);
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

app.Run();
