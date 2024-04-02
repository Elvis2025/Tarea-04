using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Actualizar(Categoria categoria);
        Task Borrar(int id);
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> Obtener(int id);
        Task<IEnumerable<Categoria>> Obtener(int id, TipoOperacion tipoOperacion);
        Task<Categoria> ObtenerPorId(int id, int usuarioId);
    }
}
