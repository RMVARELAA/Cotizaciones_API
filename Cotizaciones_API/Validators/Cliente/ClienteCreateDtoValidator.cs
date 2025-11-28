using FluentValidation;
using Cotizaciones_API.DTOs.Cliente;

namespace Cotizaciones_API.Validators.Cliente
{
    public class ClienteCreateDtoValidator : AbstractValidator<ClienteCreateDto>
    {
        public ClienteCreateDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(180).WithMessage("Nombre máximo 180 caracteres.");

            RuleFor(x => x.Identidad)
                .NotEmpty().WithMessage("La identidad es requerida.")
                .MaximumLength(50).WithMessage("Identidad máximo 50 caracteres.");

            RuleFor(x => x.IdTipoCliente)
                .GreaterThan(0).WithMessage("IdTipoCliente inválido.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("El correo no tiene formato válido.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Telefono)
                .MaximumLength(30).WithMessage("Teléfono máximo 30 caracteres.");

            RuleFor(x => x.Direccion)
                .MaximumLength(300).WithMessage("Dirección máximo 300 caracteres.");

            RuleFor(x => x.UsuarioCreacion)
                .NotEmpty().WithMessage("UsuarioCreacion es requerido.");
        }
    }
}
