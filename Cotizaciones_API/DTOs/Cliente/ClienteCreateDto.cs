namespace Cotizaciones_API.DTOs.Cliente
{
    public class ClienteCreateDto
    {
        public string? Nombre { get; set; }
        public string? Identidad { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int IdTipoCliente { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public string? UsuarioCreacion { get; set; }
    }
}
