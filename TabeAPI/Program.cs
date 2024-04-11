using Microsoft.EntityFrameworkCore;
using ProyectoASPNET;
using ProyectoASPNET.Data;
using Microsoft.OpenApi.Models;
using ProyectoASPNET.Helpers;
using TabeAPI.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

HelperActionServicesOAuth helper =
    new HelperActionServicesOAuth(builder.Configuration);
builder.Services
    .AddSingleton<HelperActionServicesOAuth>(helper);
// Habilitamos los servicios de Authentication que
// hemos creado en el helper con Action<>
builder.Services.AddAuthentication
    (helper.GetAuthenticateSchema())
    .AddJwtBearer(helper.GetJwtBearerOptions());


string connectionString =
    builder.Configuration.GetConnectionString("SqlAzure");
builder.Services.AddTransient<RepositoryRestaurantes>();
builder.Services.AddDbContext<RestaurantesContext>
    (options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<HelperPathProvider>();
builder.Services.AddTransient<HelperUploadFiles>();

string googleApiKey = builder.Configuration.GetValue<string>("GoogleApiKey");
builder.Services.AddTransient
    (h => new HelperGoogleApiDirections(googleApiKey, h.GetRequiredService<IHttpClientFactory>()));

builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tabe API",
        Description = "API de la app de Tabe"
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "Tabe API");
    options.RoutePrefix = "";
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
