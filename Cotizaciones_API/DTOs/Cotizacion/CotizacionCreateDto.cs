namespace Cotizaciones_API.DTOs.Cotizacion
{
    public class CotizacionCreateDto
    {
        public int IdTipoSeguro { get; set; }
        public int IdMoneda { get; set; }
        public int IdCliente { get; set; }
        public string? DescripcionBien { get; set; }
        public decimal SumaAsegurada { get; set; }
        public decimal Tasa { get; set; } // ej: 0.05 = 5%
        public string? Observaciones { get; set; }
        public string? UsuarioCreacion { get; set; }
    }
}
