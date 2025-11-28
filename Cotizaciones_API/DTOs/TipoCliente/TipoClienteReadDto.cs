namespace Cotizaciones_API.DTOs.TipoCliente
{
    public class TipoClienteReadDto
    {
        public int IdTipoCliente { get; set; }
        public string? NombreTipoCliente { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Estado { get; set; }
    }
}
