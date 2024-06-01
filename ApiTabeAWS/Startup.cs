using ApiTabeAWS.Data;
using ApiTabeAWS.Helpers;
using ApiTabeAWS.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using TabeNuget;

namespace ApiTabeAWS;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    private async Task<string> GetSecretAsync()
    {
        return await HelperSecretManager.GetSecretAsync();
    }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        string secret = GetSecretAsync().GetAwaiter().GetResult();

        KeysModel model = JsonConvert.DeserializeObject<KeysModel>(secret);
        string connectionString = model.MySql;

        services.AddControllers();
        services.AddSingleton<KeysModel>(x => model);
        services.AddTransient<RepositoryRestaurantes>();
        HelperActionServicesOAuth helper = new HelperActionServicesOAuth(model);
        services.AddSingleton<HelperActionServicesOAuth>(helper);
        services.AddAuthentication(helper.GetAuthenticateSchema())
        .AddJwtBearer(helper.GetJwtBearerOptions());
        services.AddDbContext<RestaurantesContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });
        string googleApiKey = model.GoogleApiKey;
        services.AddHttpClient();
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient
            (h => new HelperGoogleApiDirections(googleApiKey, h.GetRequiredService<IHttpClientFactory>()));
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        services.AddEndpointsApiExplorer();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowOrigin", x => x.AllowAnyOrigin());
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Api Tabe AWS",
                Version = "v1"
            });
        });

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(url: "swagger/v1/swagger.json",
                "Api Tabe AWS");
            options.RoutePrefix = "";
        });


        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors(options => options.AllowAnyOrigin());
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}