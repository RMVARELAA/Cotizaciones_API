namespace Cotizaciones_API.DTOs.Cliente
{
    /// <summary>
    /// Datos para actualizar un cliente (PUT).
    /// </summary>
    public class ClienteUpdateDto
    {
        public int IdCliente { get; set; }
        public string? Nombre { get; set; }
        public string? Identidad { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int IdTipoCliente { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public string? UsuarioModificacion { get; set; }
    }
}
