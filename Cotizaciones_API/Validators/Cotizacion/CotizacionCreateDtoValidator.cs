using FluentValidation;
using Cotizaciones_API.DTOs.Cotizacion;

namespace Cotizaciones_API.Validators.Cotizacion
{
    public class CotizacionCreateDtoValidator : AbstractValidator<CotizacionCreateDto>
    {
        public CotizacionCreateDtoValidator()
        {
            RuleFor(x => x.IdTipoSeguro)
                .GreaterThan(0).WithMessage("IdTipoSeguro inválido.");

            RuleFor(x => x.IdMoneda)
                .GreaterThan(0).WithMessage("IdMoneda inválido.");

            RuleFor(x => x.IdCliente)
                .GreaterThan(0).WithMessage("IdCliente inválido.");

            RuleFor(x => x.SumaAsegurada)
                .GreaterThan(0).WithMessage("SumaAsegurada debe ser mayor que 0.");

            RuleFor(x => x.Tasa)
                .InclusiveBetween(0m, 1m)
                .WithMessage("Tasa debe estar entre 0 y 1 (decimal). Ej: 0.05 = 5%.");

            RuleFor(x => x.DescripcionBien)
                .MaximumLength(1000).WithMessage("Descripción máximo 1000 caracteres.");

            RuleFor(x => x.Observaciones)
                .MaximumLength(1000).WithMessage("Observaciones máximo 1000 caracteres.");

            RuleFor(x => x.UsuarioCreacion)
                .NotEmpty().WithMessage("UsuarioCreacion es requerido.");
        }
    }
}
