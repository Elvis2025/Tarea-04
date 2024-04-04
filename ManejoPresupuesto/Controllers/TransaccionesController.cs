using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    public class TransaccionesController : Controller
    {
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly IRepositorioCuentas _repositorioCuentas;
        private readonly IRepositorioCategorias _repositorioCategorias;
        private readonly IRepositorioTrasacciones _repositorioTrasacciones;
        private readonly IMapper _mapper;
        private readonly IServicioReportes _servicioReportes;

        public TransaccionesController(IServicioUsuarios servicioUsuarios,
            IRepositorioCuentas repositorioCuentas,
            IRepositorioCategorias repositorioCategorias,
            IRepositorioTrasacciones repositorioTrasacciones,
            IMapper mapper,
            IServicioReportes servicioReportes)
        {
            _servicioUsuarios = servicioUsuarios;
            _repositorioCuentas = repositorioCuentas;
            _repositorioCategorias = repositorioCategorias;
            _repositorioTrasacciones = repositorioTrasacciones;
            _mapper = mapper;
            _servicioReportes = servicioReportes;
        }

        public async Task<IActionResult> Index(int mes, int year)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
                      
            var modelo = await _servicioReportes.ObtenerReporteTransaccionesDetalladas(usuarioId, mes, year, ViewBag);

            return View(modelo);
        }

        public IActionResult Semanal()
        {
            return View();
        }
        public IActionResult Calendario()
        {
            return View();
        }
        public IActionResult Excel()
        {
            return View();
        }
        public IActionResult Mensual()
        {
            return View();
        }

        public async Task<IActionResult> Crear()
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var modelo = new TransaccionesCreacionViewModel();
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);


            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TransaccionesCreacionViewModel modelo)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                return View(modelo);
            }

            var cuenta = await _repositorioCuentas.ObtenerPorId(modelo.CuentaId, usuarioId);



            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await _repositorioCategorias.ObtenerPorId(modelo.CategoriaId, usuarioId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            modelo.UsuarioId = usuarioId;

            if (modelo.TipoOperacionId == TipoOperacion.Gastos)
            {
                modelo.Monto *= -1;
            }

            await _repositorioTrasacciones.Crear(modelo);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult>Editar(int id, string urlReturn = null)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var transaccion = await _repositorioTrasacciones.ObtenerPorId(id, usuarioId);

            if(transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var modelo = _mapper.Map<TransaccionActualizacionViewModel>(transaccion);

            modelo.MontoAnteriorId = modelo.Monto;

            if (modelo.TipoOperacionId == TipoOperacion.Gastos)
            {
                modelo.MontoAnteriorId = modelo.Monto * -1;
            }

            modelo.CuentaAnteriorId = transaccion.CuentaId;
            modelo.Categorias = await ObtenerCategorias(usuarioId, transaccion.TipoOperacionId);
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.urlReturn = urlReturn;
            return View(modelo);

        }

        [HttpPost]
        public async Task<IActionResult>Editar(TransaccionActualizacionViewModel modelo )
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                return View(modelo);
            }

            var cuenta = await _repositorioCuentas.ObtenerPorId(modelo.CuentaId, usuarioId);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await _repositorioCategorias.ObtenerPorId(modelo.CategoriaId, usuarioId);

            if(categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var transaccion = _mapper.Map<Transaccion>(modelo);

            if(modelo.TipoOperacionId == TipoOperacion.Gastos)
            {
                transaccion.Monto *= -1;
            }
            await _repositorioTrasacciones.Actualizar(transaccion, modelo.MontoAnteriorId, modelo.CuentaAnteriorId);

            if (string.IsNullOrEmpty(modelo.urlReturn))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(modelo.urlReturn);
            }
        }

        [HttpPost]
        public async Task<IActionResult>Borrar(int id,string urlReturn = null)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var transaccion = await _repositorioTrasacciones.ObtenerPorId(id, usuarioId);

            if (transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioTrasacciones.Borrar(id);
            if (string.IsNullOrEmpty(urlReturn))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(urlReturn);
            }
        }


        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId)
        {
            var cuentas = await _repositorioCuentas.Buscar(usuarioId); 
            return cuentas.Select(x => new SelectListItem(x.Nombre,x.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId,TipoOperacion tipoOperacion)
        {
            var categorias = await _repositorioCategorias.Obtener(usuarioId, tipoOperacion);


            return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var categorias = await ObtenerCategorias(usuarioId,tipoOperacion);
            return Ok(categorias);

        }
    }
}
