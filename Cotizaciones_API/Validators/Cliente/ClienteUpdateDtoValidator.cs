using FluentValidation;
using Cotizaciones_API.DTOs.Cliente;

namespace Cotizaciones_API.Validators.Cliente
{
    public class ClienteUpdateDtoValidator : AbstractValidator<ClienteUpdateDto>
    {
        public ClienteUpdateDtoValidator()
        {
            RuleFor(x => x.IdCliente)
                .GreaterThan(0).WithMessage("IdCliente es requerido.");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(180);

            RuleFor(x => x.Identidad)
                .NotEmpty().WithMessage("La identidad es requerida.")
                .MaximumLength(50);

            RuleFor(x => x.IdTipoCliente)
                .GreaterThan(0).WithMessage("IdTipoCliente inválido.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.UsuarioModificacion)
                .NotEmpty().WithMessage("UsuarioModificacion es requerido.");
        }
    }
}
