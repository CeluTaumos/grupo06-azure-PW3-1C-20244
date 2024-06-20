using Azurecito.Logica.Servicios;
using Microsoft.AspNetCore.Mvc;
using System.IO;
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
            if (file != null && file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var fileName = Path.GetFileName(file.FileName);
                await _fotoService.SubirFotoAsync(stream, fileName, userId);
            }
            return RedirectToAction("Index");
        }
    }
}
