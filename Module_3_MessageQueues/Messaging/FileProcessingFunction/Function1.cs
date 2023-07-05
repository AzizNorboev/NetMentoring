using System;
using Microsoft.Azure.ServiceBus;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Linq;
using System.Security.Policy;

namespace FileProcessingFunction
{
    public class Function1
    {
        [FunctionName("Function1")]
        public async Task Run([ServiceBusTrigger("image-queue", Connection = "ConnectionStringToImageQueue")] string message, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {message}");

            try
            {
                // Determine if it's an image or video based on the file extension
                bool isImage = IsImageFile(message);

                // Get a reference to the Blob Storage container
                string blobConnectionString = Environment.GetEnvironmentVariable("BlobStorageConnectionString");
                BlobServiceClient blobServiceClient = new BlobServiceClient(blobConnectionString);
                string containerName = Environment.GetEnvironmentVariable("BlobImageStorageContainerName");
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Generate a unique blob name based on the current date and time
                string fileExtension = isImage ? ".mp4": ".png";
                string blobName = $"{DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss")}{fileExtension}";

                // Upload the file to Blob Storage
                byte[] fileBytes = await GetFileBytesAsync(message); 
                BlobClient blobClient = containerClient.GetBlobClient(blobName);
                using (MemoryStream stream = new MemoryStream(fileBytes))
                {
                    await blobClient.UploadAsync(stream, true);
                }

                log.LogInformation($"Saved {blobName} to Blob Storage");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing message");
            }
        }

        private async Task<byte[]> GetFileBytesAsync(string message)
        {
            if(IsImageFile(message))
            {
                int lastSlashIndex = message.LastIndexOf('/');

                // Extract the blob name from the URI
                string blobName = message.Substring(lastSlashIndex + 1);

                string storageConnectionString = Environment.GetEnvironmentVariable("BlobStorageConnectionString");
                BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);
                string containerName = Environment.GetEnvironmentVariable("BlobVideosStorageContainerName");
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Download the blob content
                BlobClient blobClient = containerClient.GetBlobClient(blobName);
                using (MemoryStream stream = new MemoryStream())
                {
                    await blobClient.DownloadToAsync(stream);
                    return stream.ToArray();
                }
            }
            else
            {
                return Convert.FromBase64String(message);
            }
            
        }

        private bool IsImageFile(string message)
        {
            string fileExtension = message;

            string[] imageExtensions = { ".mp4" };

            return imageExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }
    }
}
