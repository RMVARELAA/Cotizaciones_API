namespace Cotizaciones_API.Models
{
    public class TipoSeguro
    {
        public int IdTipoSeguro { get; set; }
        public string? NombreSeguro { get; set; }
        public string? Codigo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Estado { get; set; }
        public string? UsuarioCreacion { get; set; }
        public string? UsuarioModificacion { get; set; }
    }
}
