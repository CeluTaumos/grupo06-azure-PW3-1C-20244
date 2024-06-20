using Azurecito.Data.Entidades;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;

namespace Azurecito.Logica.Servicios
{
    public interface IFotoService
    {
        Task<string> SubirFotoAsync(Stream photoStream, string fileName, int userId);
        Task AprobarFotoAsync(int photoId);
        List<Foto> VerFotos();
        List<Foto> ObtenerFotosPendientesDeAprobacion();
        List<Usuario> ObtenerUsuariosEnBDD();
        Usuario ObtenerUsuarioEnBDD(string nombreUsuario, string password);
        Usuario ObtenerUsuarioPorId(int id);
        Foto ObtenerFotoPorId(int photoId);
        Task RechazarFotoAsync(int photoId);
    }

    public class AzurecitoServicio : IFotoService
    {
        private readonly AzurecitoFotosContext _ctx;
        private readonly BlobServiceClient _blobServiceClient;

        public AzurecitoServicio(AzurecitoFotosContext context, IConfiguration confi)
        {
            var connectionString = confi.GetConnectionString("AzureBlobStorage");
            _blobServiceClient = new BlobServiceClient(connectionString);
            _ctx = context;
        }

        public async Task<string> SubirFotoAsync(Stream photoStream, string fileName, int userId)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("photos");
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(photoStream, true);

            var foto = new Foto
            {
                UserId = userId,
                FotoUrl = blobClient.Uri.ToString(),
                EstaAprobada = false
            };
            _ctx.Fotos.Add(foto);
            await _ctx.SaveChangesAsync();

            return blobClient.Uri.ToString();
        }

        public async Task AprobarFotoAsync(int photoId)
        {
            var photo = await _ctx.Fotos.FindAsync(photoId);
            if (photo != null)
            {
                photo.EstaAprobada = true;
                await _ctx.SaveChangesAsync();
            }
        }

        public List<Foto> VerFotos()
        {
            return _ctx.Fotos.ToList();
        }

        public List<Foto> ObtenerFotosPendientesDeAprobacion()
        {
            return _ctx.Fotos.Where(f => !f.EstaAprobada).ToList();
        }

        public Usuario ObtenerUsuarioEnBDD(string nombreUsuario, string password)
        {
            var usuario = _ctx.Usuarios
                .FirstOrDefault(u => u.NombreUsuario == nombreUsuario && u.Password == password);

            return usuario;
        }

        public List<Usuario> ObtenerUsuariosEnBDD()
        {
            return _ctx.Usuarios.ToList();
        }

        public Usuario ObtenerUsuarioPorId(int id)
        {
            return _ctx.Usuarios.FirstOrDefault(u => u.Id == id);
        }

        public Foto ObtenerFotoPorId(int photoId)
        {
            return _ctx.Fotos.FirstOrDefault(f => f.Id == photoId);
        }

        public async Task RechazarFotoAsync(int photoId)
        {
            var foto = await _ctx.Fotos.FindAsync(photoId);
            if (foto != null)
            {
                _ctx.Fotos.Remove(foto);
                await _ctx.SaveChangesAsync();
            }
        }
    }
}
