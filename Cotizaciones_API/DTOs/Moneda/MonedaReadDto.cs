namespace Cotizaciones_API.DTOs.Moneda
{
    public class MonedaReadDto
    {
        public int IdMoneda { get; set; }
        public string? CodigoISO { get; set; }
        public string? Nombre { get; set; }
        public string? Simbolo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Estado { get; set; }
    }
}
