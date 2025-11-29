namespace Cotizaciones_API.Interfaces.TipoCliente
{
    public interface ITipoClienteRepository
    {
        Task<int> CreateAsync(Models.TipoCliente item);
        Task<Models.TipoCliente> GetByIdAsync(int id);
        Task<IEnumerable<Models.TipoCliente>> GetAllAsync();
        Task UpdateAsync(Models.TipoCliente item);
        Task DeleteAsync(int id, string usuarioModificacion);
    }
}
