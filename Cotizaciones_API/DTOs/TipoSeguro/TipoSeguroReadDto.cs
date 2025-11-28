namespace Cotizaciones_API.DTOs.TipoSeguro
{
    public class TipoSeguroReadDto
    {
        public int IdTipoSeguro { get; set; }
        public string? NombreSeguro { get; set; }
        public string? Codigo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Estado { get; set; }
    }
}
