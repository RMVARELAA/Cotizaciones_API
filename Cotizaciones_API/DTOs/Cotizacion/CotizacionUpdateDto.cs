namespace Cotizaciones_API.DTOs.Cotizacion
{
    public class CotizacionUpdateDto
    {
        public long IdCotizacion { get; set; }
        public int IdTipoSeguro { get; set; }
        public int IdMoneda { get; set; }
        public int IdCliente { get; set; }
        public string? DescripcionBien { get; set; }
        public decimal SumaAsegurada { get; set; }
        public decimal Tasa { get; set; }
        public string? Observaciones { get; set; }
        public string? UsuarioModificacion { get; set; }
    }
}
