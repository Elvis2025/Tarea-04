using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas _repositorioTiposCuentas;
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly IRepositorioCuentas _repositorioCuentas;
        private readonly IMapper _mapper;
        private readonly IRepositorioTrasacciones _repositorioTrasacciones;
        private readonly IServicioReportes _servicioReportes;

        public CuentasController
            (
            IRepositorioTiposCuentas repositorioTiposCuentas,
            IServicioUsuarios servicioUsuarios,IRepositorioCuentas repositorioCuentas,
            IMapper mapper,IRepositorioTrasacciones repositorioTrasacciones,
            IServicioReportes servicioReportes           
            )
        {
            _repositorioTiposCuentas = repositorioTiposCuentas;
            _servicioUsuarios = servicioUsuarios;
            _repositorioCuentas = repositorioCuentas;
            _mapper = mapper;
            _repositorioTrasacciones = repositorioTrasacciones;
            _servicioReportes = servicioReportes;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var cuentasConTipoCuenta = await _repositorioCuentas.Buscar(usuarioId);

            var modelo = cuentasConTipoCuenta
                   .GroupBy(x => x.TipoCuenta)
                   .Select(grupo => new IndiceCuentasViewModel
                   {
                       TipoCuenta = grupo.Key,
                       Cuentas = grupo.AsEnumerable()
                   }).ToList();


            var modelosPrueba = modelo;

            return View(modelo);

        }

        public async Task<IActionResult> Detalle(int id, int mes, int year)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await _repositorioCuentas.ObtenerPorId(id, usuarioId);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");

            }

            ViewBag.Cuenta = cuenta.Nombre;

            var modelo = await _servicioReportes.ObtenerReporteTransaccionesDetalladasPorCuenta(usuarioId, id, mes, year, ViewBag);

            return View(modelo);

        }
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();     
            var modelo = new CuentaCreacionViewModel();


            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);

            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuenta);
            }

            await _repositorioCuentas.Crear(cuenta);

            return RedirectToAction("Index");
        }

       
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId(); 
            var cuenta = await _repositorioCuentas.ObtenerPorId(id, usuarioId);

            if(cuenta is null){
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = _mapper.Map<CuentaCreacionViewModel>(cuenta);

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(modelo);
        }


        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await _repositorioCuentas.ObtenerPorId(cuentaEditar.Id, usuarioId);


            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var tipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(cuentaEditar.TipoCuentaId, usuarioId);

            
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioCuentas.Actualizar(cuentaEditar);


            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult>Borrar(int id,string urlRetrun = null)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var cuentas = await _repositorioCuentas.ObtenerPorId(id,usuarioId);

            if(cuentas is null)
            {
                RedirectToAction("NoEncontrado", "Home");
            }

            return View(cuentas);

        }

        [HttpPost]
        public async Task<IActionResult>BorrarCuenta(int id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var cuentas = await _repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuentas is null)
            {
                RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioCuentas.Borrar(id);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposCuentas = await _repositorioTiposCuentas.Obtener(usuarioId);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}
