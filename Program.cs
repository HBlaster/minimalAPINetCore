using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinimalApiMovies;
using MinimalApiMovies.Entidades;

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

app.MapGet("/generos", () =>
{

    var generos = new List<Genero>
    {
        new Genero { Id = 1, Nombre = "Drama" },
        new Genero { Id = 2, Nombre = "Accion" },
        new Genero { Id = 3, Nombre = "Comedia" },
    };

    return generos;

}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)));

app.Run();
