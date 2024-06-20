using Azurecito.Data.Entidades;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunctions
{
    public class AprobarFotoFunction
    {
        private readonly AzurecitoFotosContext _context;

        public AprobarFotoFunction(AzurecitoFotosContext context)
        {
            _context = context;
        }

        [FunctionName("AprobarFotoFunction")]
        public async Task Run([QueueTrigger("photo-approval-requests", Connection = "AzureWebJobsStorage")] int photoId, ILogger log)
        {
            var photo = await _context.Fotos.FindAsync(photoId);
            if (photo != null)
            {
                photo.EstaAprobada = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
