namespace Cotizaciones_API.DTOs.Cotizacion
{
    public class CotizacionReportRowDto
    {
        public long IdCotizacion { get; set; }
        public string? NumeroCotizacion { get; set; }
        public DateTime FechaCotizacion { get; set; }
        public int IdTipoSeguro { get; set; }
        public string? NombreTipoSeguro { get; set; }
        public int IdMoneda { get; set; }
        public string? MonedaCodigoISO { get; set; }
        public string? MonedaNombre { get; set; }
        public int IdCliente { get; set; }
        public string? ClienteNombre { get; set; }
        public string? DescripcionBien { get; set; }
        public decimal SumaAsegurada { get; set; }
        public decimal Tasa { get; set; }
        public decimal PrimaNeta { get; set; }
        public string? Observaciones { get; set; }

        // Campo extra solo para la paginación
        public int TotalRows { get; set; }
    }

}
