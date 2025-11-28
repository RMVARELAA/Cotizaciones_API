namespace Cotizaciones_API.DTOs.TipoSeguro
{
    public class TipoSeguroUpdateDto
    {
        public int IdTipoSeguro { get; set; }
        public string? NombreSeguro { get; set; }
        public string? Codigo { get; set; }
        public string? Descripcion { get; set; }
    }
}
