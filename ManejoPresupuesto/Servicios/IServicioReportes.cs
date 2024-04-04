using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Servicios
{
    public interface IServicioReportes
    {
        Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladas(int usuarioId, int mes, int year, dynamic ViewBag);
        Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladasPorCuenta(int usuarioId, int cuentaId, int mes, int year, dynamic ViewBag);
        Task<IEnumerable<ResultadoObtenerPorSemana>> obtenerSemanal(int usuarioId, int mes, int year, dynamic ViewBag);
    }
}
