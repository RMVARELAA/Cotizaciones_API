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
            CreateMap<ClienteCreateDtO, Cliente>()
                .ForMember(d => d.UsuarioCreacion, o => o.MapFrom(s => s.UsuarioCreacion));
            CreateMap<ClienteUpdateDto, Cliente>()
                .ForMember(d => d.UsuarioModificacion, o => o.MapFrom(s => s.UsuarioModificacion));
            CreateMap<Cliente, ClienteReadDto>();

            // Cotizacion
            CreateMap<CotizacionCreateDto, Cotizacion>();
            CreateMap<CotizacionUpdateDto, Cotizacion>();
            CreateMap<Cotizacion, CotizacionReadDto>();

            // TipoSeguro
            CreateMap<TipoSeguroCreateDto, TipoSeguro>();
            CreateMap<TipoSeguroUpdateDto, TipoSeguro>();
            CreateMap<TipoSeguro, TipoSeguroReadDto>();

            // TipoCliente
            CreateMap<TipoClienteCreateDto, TipoCliente>();
            CreateMap<TipoClienteUpdateDto, TipoCliente>();
            CreateMap<TipoCliente, TipoClienteReadDto>();

            // Moneda
            CreateMap<MonedaCreateDto, Moneda>();
            CreateMap<MonedaUpdateDto, Moneda>();
            CreateMap<Moneda, MonedaReadDto>();
        }
    }
}
