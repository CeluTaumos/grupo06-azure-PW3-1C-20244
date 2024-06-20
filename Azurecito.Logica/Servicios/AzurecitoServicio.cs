using Azure.Storage.Blobs;
using Azurecito.Data.Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Azurecito.Logica.Servicios
{
    public interface IFotoService
    {
        Task<string> SubirFotoAsync(Stream photoStream, string fileName, int userId);
        List<Foto> VerFotos();
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

            //GUARDAR LA FOTO EN LA BDD
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

        public List<Foto> VerFotos()
        {
            return _ctx.Fotos.ToList();
        }
    }
}
