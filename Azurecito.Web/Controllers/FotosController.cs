using Azurecito.Data.Entidades;
using Azurecito.Logica.Servicios;
using Azurecito.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Azurecito.Web.Controllers
{
    public class FotosController : Controller
    {
        private readonly IFotoService _fotoService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FotosController(IFotoService fotoService, IWebHostEnvironment webHostEnvironment)
        {
            _fotoService = fotoService;
            _webHostEnvironment = webHostEnvironment;
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
                if (usuario.EsAdmin)
                {
                    return RedirectToAction("AprobarFotos");
                }
                else
                {
                    TempData["UserId"] = usuario.Id;
                    return RedirectToAction("SubirFoto", new { userId = usuario.Id });
                }
            }

            ModelState.AddModelError(string.Empty, "Usuario no válido.");
            return View();
        }

        [HttpGet]
        public IActionResult SubirFoto(int userId)
        {
            if (userId == 0)
            {
                return RedirectToAction("IniciarSesion");
            }

            TempData["UserId"] = userId;
            ViewBag.UserId = userId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubirFoto(IFormFile file, int userId)
        {
            if (file != null && file.Length > 0)
            {
                string root = Path.Combine("wwwroot", "TempUploadRoot");
                var result = await _fotoService.SubirFotoTemporalAsync(file, userId, root);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Usuario no válido o falta la foto.");
                ViewBag.UserId = userId;
                return View();
            }

            TempData["UserId"] = userId;
            return RedirectToAction("Index", new { id = userId });
        }

        [HttpGet]
        public IActionResult Index(int id)
        {
            SesionLayoutViewModel viewModel = new();
            Usuario usuario = _fotoService.ObtenerUsuarioPorId(id);
            var fotos = _fotoService.VerFotos(usuario.Id);
            viewModel.Fotos = fotos;
            viewModel.Usuario = usuario;
            return View(viewModel);
        }


        [HttpGet]
        public IActionResult AprobarFotos()
        {
            var fotosPendientes = _fotoService.ObtenerFotosPendientesDeAprobacion();

           
            foreach (var foto in fotosPendientes)
            {
                foto.User = _fotoService.ObtenerUsuarioPorId(foto.UserId);
            }

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

            try
            {

                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "TempUploadRoot");

                await _fotoService.AprobarFotoAsync(photoId, uploadPath);
            }
            catch (Exception ex)
            {
                
                ModelState.AddModelError(string.Empty, "Error al aprobar la foto: " + ex.Message);
                return View("AprobarFotos", _fotoService.ObtenerFotosPendientesDeAprobacion());
            }

            return RedirectToAction("AprobarFotos");
        }

        [HttpGet]
        public async Task<IActionResult> Ver()
        {
            var fotosUrls = await _fotoService.ObtenerTodasLasFotosDeBlobAsync();
            return View(fotosUrls);
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

