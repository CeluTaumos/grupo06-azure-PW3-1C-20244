using Azurecito.Data.Entidades;

namespace Azurecito.Web.Models
{
    public class SesionLayoutViewModel
    {
        public Usuario Usuario { get; set; }
        public Foto Foto { get; set; }
        public List<Foto> Fotos { get; set; }

        public SesionLayoutViewModel()
        {
            Fotos = new List<Foto>();
            Usuario = new Usuario();
            Foto = new Foto();
        }
    }
}