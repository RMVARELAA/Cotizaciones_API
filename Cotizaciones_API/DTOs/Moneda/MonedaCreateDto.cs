namespace Cotizaciones_API.DTOs.Moneda
{
    public class MonedaCreateDto
    {
        public string? CodigoISO { get; set; } // 3 letras
        public string? Nombre { get; set; }
        public string? Simbolo { get; set; }
    }
}
