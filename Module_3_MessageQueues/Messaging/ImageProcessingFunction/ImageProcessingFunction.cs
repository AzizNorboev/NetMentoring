using System;
using Microsoft.Azure.ServiceBus;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System.Threading;

namespace ImageProcessingFunction
{
    public class ImageProcessingFunction
    {
        [FunctionName("ImageProcessingFunction")]
        public async Task Run([ServiceBusTrigger("image-queue", Connection = "ServiceBusConnectionString")]string message,
            MessageReceiver messageReceiver,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {message}");

            // Convert the message from Base64 to a byte array
            byte[] imageBytes = Convert.FromBase64String(message);

            // Get a reference to the Blob Storage container
            string blobConnectionString = Environment.GetEnvironmentVariable("BlobStorageConnectionString");
            BlobServiceClient blobServiceClient = new BlobServiceClient(blobConnectionString);
            string containerName = Environment.GetEnvironmentVariable("BlobStorageContainerName");
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Generate a unique blob name based on the current date and time
            string blobName = $"{DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss")}.jpg";

            // Upload the image to Blob Storage
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                await blobClient.UploadAsync(stream, true);
            }

            log.LogInformation($"Saved image {blobName} to Blob Storage");
        }
    }
}
