using Azurecito.Data.Entidades;
using Azurecito.Logica.Servicios;
using Azurecito.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Azurecito.Web.Controllers
{
    public class FotosController : Controller
    {
        private readonly IFotoService _fotoService;

        public FotosController(IFotoService fotoService)
        {
            _fotoService = fotoService;
        }

        [HttpGet]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IniciarSesion(string nombreUsuario, string password)
        {
            var usuario = _fotoService.ObtenerUsuarioEnBDD(nombreUsuario, password);

            if (usuario != null)
            {
                // Verificar si el usuario es admin
                if (usuario.EsAdmin)
                {
                    return RedirectToAction("AprobarFotos");
                }
                else
                {
                    return RedirectToAction("SubirFoto");
                }
            }

            ModelState.AddModelError(string.Empty, "Usuario no válido.");
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var fotos = _fotoService.VerFotos();
            return View(fotos);
        }

        [HttpGet]
        public IActionResult SubirFoto()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubirFoto(IFormFile file, int userId)
        {
            var usuario = _fotoService.ObtenerUsuarioPorId(userId);

            if (usuario != null && file != null && file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var fileName = Path.GetFileName(file.FileName);
                await _fotoService.SubirFotoAsync(stream, fileName, userId);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Usuario no válido o falta la foto.");
                return View();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AprobarFotos()
        {
            var fotosPendientes = _fotoService.ObtenerFotosPendientesDeAprobacion();
            return View(fotosPendientes);
        }

        [HttpPost]
        public async Task<IActionResult> AprobarFoto(int photoId)
        {
            var foto = _fotoService.ObtenerFotoPorId(photoId);
            if (foto == null)
            {
                return NotFound();
            }

            await _fotoService.AprobarFotoAsync(photoId);

            return RedirectToAction("AprobarFotos");
        }

        [HttpPost]
        public async Task<IActionResult> RechazarFoto(int photoId)
        {
            var foto = _fotoService.ObtenerFotoPorId(photoId);
            if (foto == null)
            {
                return NotFound();
            }

            await _fotoService.RechazarFotoAsync(photoId);

            return RedirectToAction("AprobarFotos");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
