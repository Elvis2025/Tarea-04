using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Servicios
{
    public class ServicioReportes:IServicioReportes
    {
        private readonly IRepositorioTrasacciones _repositorioTrasacciones;
        private readonly HttpContext _httpContext;

        public ServicioReportes(IRepositorioTrasacciones repositorioTrasacciones,IHttpContextAccessor httpContextAccessor)
        {
            _repositorioTrasacciones = repositorioTrasacciones;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<ReporteTransaccionesDetalladas>ObtenerReporteTransaccionesDetalladas
             (int usuarioId, int mes, int year, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechaInicioYFin(mes, year);
            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
            };

            var transacciones = await _repositorioTrasacciones.ObtenerPorUausarioId(parametro);
            var modelo = GenerarReporteTransaccionesDetalladas(fechaInicio, fechaFin, transacciones);
            asignarValoresAlViewBag(ViewBag, fechaInicio);

            return modelo;

        }

        public async Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladasPorCuenta
            (int usuarioId, int cuentaId, int mes, int year, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechaInicioYFin(mes, year);


            var obtenerTransaccionesPorCuenta = new ObtenerTransaccionesPorCuenta()
            {
                CuentaId = cuentaId,
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var transacciones = await _repositorioTrasacciones.ObtenerPorCuentaId(obtenerTransaccionesPorCuenta);
            var modelo = GenerarReporteTransaccionesDetalladas(fechaInicio, fechaFin, transacciones);

            asignarValoresAlViewBag(ViewBag, fechaInicio);

            return modelo;
        }

        private void asignarValoresAlViewBag(dynamic ViewBag, DateTime fechaInicio)
        {
            ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            ViewBag.yearAnterior = fechaInicio.AddMonths(-1).Year;

            ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;
            ViewBag.yearPosterior = fechaInicio.AddMonths(1).Year;

            ViewBag.urlReturn = _httpContext.Request.Path + _httpContext.Request.QueryString;
        }

        private static ReporteTransaccionesDetalladas GenerarReporteTransaccionesDetalladas(DateTime fechaInicio, DateTime fechaFin, IEnumerable<Transaccion> transacciones)
        {
            var modelo = new ReporteTransaccionesDetalladas();


            var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion)
                                                     .GroupBy(x => x.FechaTransaccion)
                                                     .Select(gr => new ReporteTransaccionesDetalladas.TransaccionesPorFecha()
                                                     {
                                                         FehcaTransaccion = gr.Key,
                                                         Transacciones = gr.AsEnumerable()
                                                     });
            modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = fechaInicio;
            modelo.FechaFin = fechaFin;
            return modelo;
        }

        private (DateTime fechaInicio, DateTime fechaFin) GenerarFechaInicioYFin(int mes,int year)
        {
            DateTime fechaInicio;
            DateTime fechaFin;

            if (mes <= 0 || mes > 12 || year <= 1900)
            {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            }
            else
            {
                fechaInicio = new DateTime(year, mes, 1);
            }

            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            return (fechaInicio, fechaFin);
        }
    }
}
