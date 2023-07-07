using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System.Threading.Tasks;

namespace FileProcessingFunction
{
    public class Function1
    {
        private const string BlobStorageConnectionStringKey = "BlobStorageConnectionString";
        private const string BlobFileStorageContainerNameKey = "BlobFileStorageContainerName";

        [FunctionName("CombineChunksAndUpload")]
        public static async Task Run(
            [ServiceBusTrigger("image-queue", Connection = "ConnectionStringToImageQueue")] byte[] message,
            ILogger log)
        {
            try
            {
                // Retrieve the message properties
                string fileName = $"{DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-fff")}.mp4";
                int totalChunks = 1; // Update with the total number of chunks
                int currentChunk = 1; // Update with the current chunk number

                // Combine the chunks into a single file. But not working...
                string combinedFilePath = await CombineChunks(fileName, totalChunks, currentChunk, message);

                await UploadFileToBlobStorage(combinedFilePath, fileName, log);

                File.Delete(combinedFilePath);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing message");
            }
        }

        private static async Task<string> CombineChunks(string fileName, int totalChunks, int currentChunk, byte[] chunkData)
        {
            string tempDirectory = Path.GetTempPath();
            string combinedFileName = Path.GetFileNameWithoutExtension(fileName) + "_combined" + Path.GetExtension(fileName);
            string combinedFilePath = Path.Combine(tempDirectory, combinedFileName);

            // Append the chunk to the combined file
            using (FileStream combinedStream = new FileStream(combinedFilePath, FileMode.Append))
            {
                await combinedStream.WriteAsync(chunkData, 0, chunkData.Length);
            }

            // Check if all chunks have been received
            if (currentChunk == totalChunks)
            {
                return combinedFilePath; // Return the combined file path when all chunks are received
            }

            return null; // Return null to indicate that the file is not yet complete
        }

        private static async Task UploadFileToBlobStorage(string filePath, string fileName, ILogger log)
        {
            string blobConnectionString = Environment.GetEnvironmentVariable(BlobStorageConnectionStringKey);
            string containerName = Environment.GetEnvironmentVariable(BlobFileStorageContainerNameKey);

            BlobServiceClient blobServiceClient = new BlobServiceClient(blobConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            using (FileStream fileStream = File.OpenRead(filePath))
            {
                await blobClient.UploadAsync(fileStream, overwrite: true);
            }

            log.LogInformation($"Uploaded file {fileName} to Blob Storage.");
        }
    }
}

