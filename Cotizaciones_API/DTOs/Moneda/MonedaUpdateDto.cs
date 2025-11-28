namespace Cotizaciones_API.DTOs.Moneda
{
    public class MonedaUpdateDto
    {
        public int IdMoneda { get; set; }
        public string? CodigoISO { get; set; }
        public string? Nombre { get; set; }
        public string? Simbolo { get; set; }
    }
}
