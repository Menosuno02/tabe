using Amazon.S3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProyectoASPNET.Data;
using ProyectoASPNET.Helpers;
using ProyectoASPNET.Hubs;
using ProyectoASPNET.Services;
using TabeNuget;


var builder = WebApplication.CreateBuilder(args);


string secret = GetSecretAsync().GetAwaiter().GetResult();
async Task<string> GetSecretAsync()
{
    return await HelperSecretManager.GetSecretsAsync();
}
KeysModel model = JsonConvert.DeserializeObject<KeysModel>(secret);
builder.Services.AddTransient<KeysModel>(x => model);


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

string signalRKey = model.SignalRKey;
builder.Services.AddSignalR().AddAzureSignalR(signalRKey);


builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddTransient<ServiceStorageAWS>();

builder.Services.AddTransient<IServiceRestaurantes, ServiceApiRestaurantes>
    (s => new ServiceApiRestaurantes(model, s.GetRequiredService<HelperCryptography>(), s.GetRequiredService<ServiceStorageAWS>(), s.GetRequiredService<IHttpContextAccessor>(), s.GetRequiredService<RestaurantesContext>()));
builder.Services.AddTransient<ServiceCacheRedis>();
builder.Services.AddTransient<ServiceLogicApps>();

string connectionString = model.MySql;
builder.Services.AddDbContext<RestaurantesContext>
    (options => options.UseSqlServer(connectionString));

builder.Services.AddSingleton<HelperCryptography>();
builder.Services.AddTransient<HelperMails>();

string googleApiKey = model.GoogleApiKey;
builder.Services.AddTransient<HelperGoogleApiDirections>
    (h => new HelperGoogleApiDirections(googleApiKey, h.GetRequiredService<IHttpClientFactory>()));

builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();



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