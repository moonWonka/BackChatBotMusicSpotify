using MediatR;

var builder = WebApplication.CreateBuilder(args);

// 1. Servicios API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. Swagger configuración básica
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SpotifyMusicChatBot API",
        Version = "v1",
        Description = "API para el ChatBot de Música de Spotify"
    });
});

// 3. MediatR
builder.Services.AddMediatR(typeof(Program));

// 4. Inyecciones separadas
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
