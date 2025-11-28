namespace Cotizaciones_API.Models
{
    public class Moneda
    {
        public int IdMoneda { get; set; }
        public string? CodigoISO { get; set; }   // HNL, USD, EUR
        public string? Nombre { get; set; }
        public string? Simbolo { get; set; }     // L, $, €
        public DateTime FechaCreacion { get; set; }
        public bool Estado { get; set; }
    }
}
