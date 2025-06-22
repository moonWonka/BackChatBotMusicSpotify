using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// 1. Servicios API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. Versionado de API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("version"),
        new HeaderApiVersionReader("X-Version"),
        new UrlSegmentApiVersionReader()
    );
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // genera v1, v2, etc.
    options.SubstituteApiVersionInUrl = true;
});

// 3. Swagger configuración básica
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SpotifyMusicChatBot API",
        Version = "v1",
        Description = "API para el ChatBot de Música de Spotify"
    });
});

// 4. MediatR
builder.Services.AddMediatR(typeof(Program));

// 5. Inyecciones separadas
ConfigureServiceDependencies(builder.Services);
ConfigureRepositoryDependencies(builder.Services);

var app = builder.Build();

// Middleware y pipeline
app.UseHttpsRedirection();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SpotifyMusicChatBot API v1");
    });
}

app.MapControllers();
app.Run();


// ----------- Métodos privados para DI -----------
static void ConfigureServiceDependencies(IServiceCollection services)
{
    // services.AddScoped<ISpotifyService, SpotifyService>();
}

static void ConfigureRepositoryDependencies(IServiceCollection services)
{
    // services.AddScoped<IUserRepository, UserRepository>();
}
