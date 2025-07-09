using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinimalApiMovies;
using MinimalApiMovies.Entidades;
using MinimalApiMovies.Migrations;
using MinimalApiMovies.Repositorios;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!;

// Add services to the container.
builder.Services.AddDbContext<AplicationDbContext>(options => options.UseSqlServer("name=DefaultConnection"));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        config =>
        {
            config.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
        });
    options.AddPolicy("libre", config =>
    {

        config.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();

    });
});

builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositorioGenero, RepositorioGeneros>();

var app = builder.Build();

//if (builder.Environment.IsDevelopment()) {
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseOutputCache();

app.MapGet("/", [EnableCors(policyName: "libre")] () => "Hello World!");

var endpointsGeneros = app.MapGroup("/generos");

endpointsGeneros.MapGet("/", obtenerGeneros)
    .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"));

endpointsGeneros.MapGet("/{id:int}", obtenerGeneroId);

endpointsGeneros.MapPost("/", crearGenero);

endpointsGeneros.MapPut("/{id:int}", actualizarGenero);

endpointsGeneros.MapDelete("/{id:int}", eliminarGenero);

app.Run();

static async Task<Ok<List<Genero>>> obtenerGeneros(IRepositorioGenero repositorio)
{
    var generos = await repositorio.GetGeneros();
    return TypedResults.Ok(generos);
}

static async Task<Results<Ok<Genero>, NotFound>> obtenerGeneroId(IRepositorioGenero repositorio, int id)
{
    var genero = await repositorio.GetGenero(id);
    if (genero == null)
    {
        return TypedResults.NotFound();
    }
    return TypedResults.Ok(genero);
}

static async Task<Created<Genero>> crearGenero(Genero genero, IRepositorioGenero repositorio,
    IOutputCacheStore outputCacheStore)
{
    var id = await repositorio.Crear(genero);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return TypedResults.Created($"/generos/{id}", genero);
}

static async Task<Results<NoContent, NotFound>> actualizarGenero(int id, Genero genero, IRepositorioGenero repositorio, IOutputCacheStore outputCacheStore)
{

    var exists = await repositorio.exists(id);
    if (!exists)
    {
        return TypedResults.NotFound();
    }

    await repositorio.Actualizar(genero);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return TypedResults.NoContent();
}

static async Task<Results<NotFound, NoContent>> eliminarGenero(int id, IRepositorioGenero repositorio, IOutputCacheStore outputCacheStore)
{

    var exists = await repositorio.exists(id);
    if (!exists)
    {
        return TypedResults.NotFound();
    }

    await repositorio.Eliminar(id);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return TypedResults.NoContent();
}