using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProyectoASPNET;
using ProyectoASPNET.Data;
using ProyectoASPNET.Helpers;

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
/*
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
*/

string connectionString = builder.Configuration.GetConnectionString("SqlServer");
builder.Services.AddTransient<RepositoryRestaurantes>();
builder.Services.AddDbContext<RestaurantesContext>
    (options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<HelperPathProvider>();
builder.Services.AddTransient<HelperCesta>();
builder.Services.AddTransient<HelperUploadFiles>();

string googleApiKey = builder.Configuration.GetValue<string>("GoogleApiKey");
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

// app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}");

app.Run();
