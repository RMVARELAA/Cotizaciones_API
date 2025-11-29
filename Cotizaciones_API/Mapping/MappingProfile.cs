// Mapping/MappingProfile.cs
using AutoMapper;
using Cotizaciones_API.DTOs.Cliente;
using Cotizaciones_API.DTOs.Cotizacion;
using Cotizaciones_API.DTOs.Moneda;
using Cotizaciones_API.DTOs.TipoCliente;
using Cotizaciones_API.DTOs.TipoSeguro;
using Cotizaciones_API.Models;

namespace Cotizaciones_API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Cliente
            CreateMap<ClienteCreateDto, Cliente>()
                .ForMember(d => d.UsuarioCreacion, o => o.MapFrom(s => s.UsuarioCreacion));
            CreateMap<ClienteUpdateDto, Cliente>()
                .ForMember(d => d.UsuarioModificacion, o => o.MapFrom(s => s.UsuarioModificacion));
            CreateMap<Cliente, ClienteReadDto>();

            // Cotizacion
            CreateMap<CotizacionCreateDto, Cotizacion>();
            CreateMap<CotizacionUpdateDto, Cotizacion>();
            CreateMap<Cotizacion, CotizacionReadDto>();

            // TipoCliente
            CreateMap<TipoCliente, TipoClienteReadDto>();
            CreateMap<TipoClienteCreateDto, TipoCliente>();
            CreateMap<TipoClienteUpdateDto, TipoCliente>();

            // TipoSeguro
            CreateMap<TipoSeguro, TipoSeguroReadDto>();
            CreateMap<TipoSeguroCreateDto, TipoSeguro>();
            CreateMap<TipoSeguroUpdateDto, TipoSeguro>();

            // Moneda
            CreateMap<Moneda, MonedaReadDto>();
            CreateMap<MonedaCreateDto, Moneda>();
            CreateMap<MonedaUpdateDto, Moneda>();

        }
    }
}
