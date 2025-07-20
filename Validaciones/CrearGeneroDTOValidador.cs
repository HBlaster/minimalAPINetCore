using FluentValidation;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Repositorios;

namespace MinimalApiMovies.Validaciones
{
    public class CrearGeneroDTOValidador:AbstractValidator<CrearGeneroDTO>
    {
        public CrearGeneroDTOValidador(IRepositorioGenero repositorioGenero)
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50).WithMessage("El campo {PropertyName} debe tener menos de {MaxLength} caracteres.")
                .Must(PrimeraLetraEnMayusculas).WithMessage("El campo {PropertyName} debe comenzar con mayuscula")
                .MustAsync(async (nombre, _) =>
                {

                    var existe = await repositorioGenero.exists(id: 0, nombre);
                    return !existe;
                }).WithMessage(g=> $"Ya existe un genero con el nombre {g.Nombre}");

        }

        private bool PrimeraLetraEnMayusculas(string valor) { 
            if(string.IsNullOrWhiteSpace(valor)){
                return true;
            }
            var primeraLetra = valor[0].ToString();
            return primeraLetra == primeraLetra.ToUpper();
        }
    }
}
