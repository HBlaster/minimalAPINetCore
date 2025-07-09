using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinimalApiMovies;
using MinimalApiMovies.Entidades;
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

app.MapGet("/generos", async (IRepositorioGenero repositorio) =>
{
    return await repositorio.GetGeneros();

}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)));

app.MapGet("/generos/{id:int}", async (IRepositorioGenero repositorio, int id) => { 
    var genero = await repositorio.GetGenero(id);
    if (genero == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(genero);
});

app.MapPost("/generos", async (Genero genero, IRepositorioGenero repositorio) =>
{
    if (string.IsNullOrWhiteSpace(genero.Nombre))
    {
        return Results.BadRequest("El nombre del género es obligatorio.");
    }
    var id = await repositorio.Crear(genero);
    return Results.Created($"/generos/{id}", genero);
});

app.Run();
