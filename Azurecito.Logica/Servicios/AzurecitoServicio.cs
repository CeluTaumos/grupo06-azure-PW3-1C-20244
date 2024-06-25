using Azure.Storage.Blobs;
using Azurecito.Data.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azurecito.Logica.Servicios
{
    public interface IFotoService
    {
        Task<string> SubirFotoTemporalAsync(IFormFile file, int userId, string path);
        Task AprobarFotoAsync(int photoId, string pathRoot);
        Task<List<string>> ObtenerTodasLasFotosDeBlobAsync();
        List<Foto> VerFotos(int id);
        List<Foto> ObtenerFotosPendientesDeAprobacion();
        List<Usuario> ObtenerUsuariosEnBDD();
        Usuario ObtenerUsuarioEnBDD(string nombreUsuario, string password);
        Usuario ObtenerUsuarioPorId(int? id);
        Foto ObtenerFotoPorId(int photoId);
        Task RechazarFotoAsync(int photoId);
    }

    public class AzurecitoServicio : IFotoService
    {
        private readonly AzurecitoFotosContext _ctx;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _tempUploadPath;

        public AzurecitoServicio(AzurecitoFotosContext context, IConfiguration confi)
        {
            var connectionString = confi.GetConnectionString("AzureBlobStorage");
            _blobServiceClient = new BlobServiceClient(connectionString);
            _ctx = context;
            _tempUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "TempUploads");
            Directory.CreateDirectory(_tempUploadPath);

        }

        public async Task<string> SubirFotoTemporalAsync(IFormFile file, int userId, string root)
        {
            try
            {
                Directory.CreateDirectory(root);
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(root, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(stream);
                }
                var relativeFilePath = Path.Combine("/TempUploadRoot", fileName);
                var foto = new Foto
                {
                    UserId = userId,
                    FotoUrl = relativeFilePath,
                    EstaAprobada = false
                };
                _ctx.Fotos.Add(foto);
                await _ctx.SaveChangesAsync();

                return relativeFilePath;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        public async Task AprobarFotoAsync(int photoId, string pathRoot)
        {
            try
            {
                Directory.CreateDirectory(pathRoot);
                var photo = await _ctx.Fotos.FindAsync(photoId);
                if (photo != null && !photo.EstaAprobada)
                {
                    

                    var fileName = Path.GetFileName(photo.FotoUrl);
                    var filePath = Path.Combine(pathRoot, fileName); //ACA ESTAMOS HACIENDOLA RUTA DEL ARCHIVO

                    var containerClient = _blobServiceClient.GetBlobContainerClient("fotitos");
                    await containerClient.CreateIfNotExistsAsync();
                    var blobClient = containerClient.GetBlobClient(fileName);


                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        await blobClient.UploadAsync(stream, true);
                    }


                    photo.FotoUrl = blobClient.Uri.ToString();
                    photo.EstaAprobada = true;
                    await _ctx.SaveChangesAsync();

                    //UNA VEZ APROBADA,SE ELIMINA DE LOS ARCHIVOS TEMPORALES
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {

                string m =  ex.Message;
            }
        }

        public List<Foto> VerFotos(int id)
        {
            return _ctx.Fotos.Where(f => f.UserId == id).ToList();
        }

        public List<Foto> ObtenerFotosPendientesDeAprobacion()
        {
            return _ctx.Fotos.Include(f => f.User).Where(f => !f.EstaAprobada).ToList();
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

        public Usuario ObtenerUsuarioPorId(int? id)
        {
            return _ctx.Usuarios.FirstOrDefault(u => u.Id == id);
        }

        public Foto ObtenerFotoPorId(int photoId)
        {
            return _ctx.Fotos.Include(f => f.User).FirstOrDefault(f => f.Id == photoId);
        }
        public async Task<List<string>> ObtenerTodasLasFotosDeBlobAsync()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("fotitos");
            var fotosUrls = new List<string>();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                fotosUrls.Add(blobClient.Uri.ToString());
            }

            return fotosUrls;
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
