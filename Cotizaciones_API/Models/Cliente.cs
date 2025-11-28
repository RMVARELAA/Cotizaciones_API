
namespace Cotizaciones_API.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string? Nombre { get; set; }
        public string? Identidad { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int IdTipoCliente { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? UsuarioCreacion { get; set; }
        public string? UsuarioModificacion { get; set; }
        public bool Estado { get; set; }
    }
}
