using Microsoft.EntityFrameworkCore;
using ProyectoASPNET;
using ProyectoASPNET.Data;
using ProyectoASPNET.Helpers;
using TabeAPI.Helpers;
using NSwag.Generation.Processors.Security;
using NSwag;
using Azure.Storage.Blobs;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Azure;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient
        (builder.Configuration.GetSection("KeyVault"));
});
SecretClient secretClient = builder.Services.BuildServiceProvider().GetService<SecretClient>();


builder.Services.AddHttpClient();

HelperActionServicesOAuth helper = new HelperActionServicesOAuth(secretClient);
builder.Services
    .AddSingleton<HelperActionServicesOAuth>(helper);
builder.Services.AddAuthentication
    (helper.GetAuthenticateSchema()).AddJwtBearer(helper.GetJwtBearerOptions());

KeyVaultSecret secretStorageAccount = await secretClient.GetSecretAsync("StorageAccountKey");
string storageAccountKey = secretStorageAccount.Value;
BlobServiceClient blobServiceClient = new BlobServiceClient(storageAccountKey);
builder.Services.AddTransient<BlobServiceClient>(x => blobServiceClient);

KeyVaultSecret secretConnectionString = await secretClient.GetSecretAsync("SqlAzure");
string connectionString = secretConnectionString.Value;
builder.Services.AddTransient<RepositoryRestaurantes>();
builder.Services.AddDbContext<RestaurantesContext>
    (options => options.UseSqlServer(connectionString));

KeyVaultSecret secretGoogleApiKey = await secretClient.GetSecretAsync("GoogleApiKey");
string googleApiKey = secretGoogleApiKey.Value;
builder.Services.AddTransient
    (h => new HelperGoogleApiDirections(googleApiKey, h.GetRequiredService<IHttpClientFactory>()));

builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(document =>
{
    document.Title = "Tabe API";
    document.Description = "API de la app de Tabe";
    document.AddSecurity("JWT", Enumerable.Empty<string>(),
        new NSwag.OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Copia y pega el Token en el campo 'Value:' así: Bearer {Token JWT}."
        }
    );
    document.OperationProcessors.Add(
    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUI(options =>
{
    options.InjectStylesheet("/css/theme-material.css");
    options.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "Tabe API");
    options.RoutePrefix = "";
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
