using Microsoft.EntityFrameworkCore;
using ProyectoASPNET;
using ProyectoASPNET.Data;
using ProyectoASPNET.Helpers;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("SqlServer");
builder.Services.AddTransient<RepositoryRestaurantes>();
builder.Services.AddDbContext<RestaurantesContext>
    (options => options.UseSqlServer(connectionString));

builder.Services.AddSingleton<HelperPathProvider>();
builder.Services.AddSingleton<HelperCesta>();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});

var app = builder.Build();

app.UseStaticFiles();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}");

app.Run();
