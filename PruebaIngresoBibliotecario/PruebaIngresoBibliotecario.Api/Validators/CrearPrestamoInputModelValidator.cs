using FluentValidation;
using PruebaIngresoBibliotecario.Api.InputModels;

namespace PruebaIngresoBibliotecario.Api.Validators
{
    public class CrearPrestamoInputModelValidator : AbstractValidator<CrearPrestamoInputModel>
    {
        public CrearPrestamoInputModelValidator()
        {
            RuleFor(x => x.Isbn).NotEmpty();
            RuleFor(x => x.IdentificacionUsuario).MaximumLength(10);
            RuleFor(x => x.TipoUsuario).IsInEnum().NotNull();
        }
    }
}