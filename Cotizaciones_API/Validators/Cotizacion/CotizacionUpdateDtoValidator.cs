using FluentValidation;
using Cotizaciones_API.DTOs.Cotizacion;

namespace Cotizaciones_API.Validators.Cotizacion
{
    public class CotizacionUpdateDtoValidator : AbstractValidator<CotizacionUpdateDto>
    {
        public CotizacionUpdateDtoValidator()
        {
            RuleFor(x => x.IdCotizacion)
                .GreaterThan(0).WithMessage("IdCotizacion es requerido.");

            RuleFor(x => x.IdTipoSeguro).GreaterThan(0);
            RuleFor(x => x.IdMoneda).GreaterThan(0);
            RuleFor(x => x.IdCliente).GreaterThan(0);
            RuleFor(x => x.SumaAsegurada).GreaterThan(0);
            RuleFor(x => x.Tasa).InclusiveBetween(0m, 1m)
                .WithMessage("Tasa debe estar entre 0 y 1 (decimal).");

            RuleFor(x => x.UsuarioModificacion).NotEmpty();
        }
    }
}
