using Microsoft.Extensions.Options;
using MinimalApiMovies.Entidades;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!;

//builder.Services.AddCors( options => 
//    options.AddDefaultPolicy(
//        config => {
//            config.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
//        }
//    )
//);

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

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/generos", () =>
{

    var generos = new List<Genero>
    {
        new Genero { Id = 1, Nombre = "Drama" },
        new Genero { Id = 2, Nombre = "Accion" },
        new Genero { Id = 3, Nombre = "Comedia" },
    };

    return generos;

});

app.Run();
