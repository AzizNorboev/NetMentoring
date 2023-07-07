// Set the connection string and queue name
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;



// Set the connection string and queue name
string serviceBusConnectionString = "Endpoint=sb://imageprocessingservice.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=bXrVrzmuWDFho2/Qr0xJ9iYSpoehG8Gbk+ASbFrAY5g=";
string queueName = "image-queue";
string filePath = @"D:\test1.mp4";

//await SendVideoMessage(serviceBusConnectionString, queueName);
//await SendImageMessage(serviceBusConnectionString, queueName);
await SendMessageAsync(serviceBusConnectionString, queueName, filePath);



static async Task SendVideoMessage(string connectionString, string queueName)
{
    // Create a ServiceBusClient object
    ServiceBusClient serviceBusClient = new ServiceBusClient(connectionString);

    // Create a ServiceBusSender object
    ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(queueName);

    // Set the connection string and container name for Azure Blob Storage
    string blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=imagestorage2023;AccountKey=CjT6BOoFY236N5Gx/kjYU79g2EKrFk2+sHPEmdcRZ//aUzElN+GMQP8z7k9PK3Wtg5ltJYuGU9cK+AStSSDSrg==;EndpointSuffix=core.windows.net";
    string containerName = "videos";

    // Create a BlobServiceClient object
    BlobServiceClient blobServiceClient = new BlobServiceClient(blobStorageConnectionString);

    // Get a reference to the Blob Storage container
    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

    // Get the path to the files
    string mp4FilePath = @"D:\Desktop\EBSCO\Recordings\Create a Repo.mp4";

    // Generate a unique blob name based on the current date and time
    string blobName = $"{DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss")}.mp4";

    // Upload the video to Blob Storage
    BlobClient blobClient = containerClient.GetBlobClient(blobName);
    await using (FileStream stream = File.OpenRead(mp4FilePath))
    {
        await blobClient.UploadAsync(stream, true);
    }

    // Get the URL of the uploaded video in Blob Storage
    string videoBlobUrl = blobClient.Uri.ToString();

    // Create a ServiceBusMessage object with the video blob URL as the message body
    ServiceBusMessage message = new ServiceBusMessage(videoBlobUrl);

    // Set the content type of the message to indicate that it contains a video
    message.ContentType = "video/mp4";

    // Send the message using the ServiceBusSender object
    await serviceBusSender.SendMessageAsync(message);

    Console.WriteLine("Video sent successfully.");


    // Close the ServiceBusClient object
    await serviceBusClient.DisposeAsync();
}

static async Task SendImageMessage(string connectionString, string queueName)
{
    // Create a ServiceBusClient object
    ServiceBusClient client = new ServiceBusClient(connectionString);

    // Create a ServiceBusSender object
    ServiceBusSender sender = client.CreateSender(queueName);

    // Get the path to the image file
    string imagePath = @"D:\test.png";

    // Read the image file into a byte array
    byte[] imageBytes = File.ReadAllBytes(imagePath);

    // Convert the byte array into a Base64 string
    string base64String = Convert.ToBase64String(imageBytes);

    // Create a ServiceBusMessage object with the Base64 string as the message body
    ServiceBusMessage message = new ServiceBusMessage(base64String);

    // Set the content type of the message to indicate that it contains an image
    message.ContentType = "image/jpeg";

    // Send the message using the ServiceBusSender object
    await sender.SendMessageAsync(message);

    Console.WriteLine("Image sent successfully.");

    // Close the ServiceBusClient object
    await client.DisposeAsync();
}

static async Task SendMessageAsync(string connectionString, string queueName, string filePath)
{
    // Create a ServiceBusClient object
    await using (ServiceBusClient client = new ServiceBusClient(connectionString))
    {
        // Create a ServiceBusSender object
        ServiceBusSender sender = client.CreateSender(queueName);

        // Get the file name and extension
        string fileName = Path.GetFileName(filePath);
        string fileExtension = Path.GetExtension(filePath);

        // Set the content type based on the file extension
        string contentType = GetContentType(fileExtension);

        // Read the file
        byte[] fileBytes = File.ReadAllBytes(filePath);

        // Determine the number of chunks based on the allowed size
        const int chunkSize = 262000; // Allowed size in bytes
        int totalChunks = (int)Math.Ceiling((double)fileBytes.Length / chunkSize);

        // Send each chunk of the file as a separate message
        for (int chunkNumber = 0; chunkNumber < totalChunks; chunkNumber++)
        {
            // Calculate the start and end index of the current chunk
            int startIndex = chunkNumber * chunkSize;
            int endIndex = Math.Min(startIndex + chunkSize, fileBytes.Length);

            // Extract the chunk of the file
            byte[] chunkData = new byte[endIndex - startIndex];
            Buffer.BlockCopy(fileBytes, startIndex, chunkData, 0, chunkData.Length);

            // Create a ServiceBusMessage with the chunk of the file
            ServiceBusMessage message = new ServiceBusMessage(chunkData);
            message.ContentType = contentType;
            message.ApplicationProperties.Add("FileName", fileName);
            message.ApplicationProperties.Add("TotalChunks", totalChunks);
            message.ApplicationProperties.Add("ChunkNumber", chunkNumber + 1);

            // Send the message using the ServiceBusSender object
            await sender.SendMessageAsync(message);

            // Log the progress
            Console.WriteLine($"Sent chunk {chunkNumber + 1}/{totalChunks}");
        }

        Console.WriteLine("File sent successfully.");
    }
}

static string GetContentType(string fileExtension)
{
    // Map file extensions to content types
    switch (fileExtension.ToLower())
    {
        case ".jpg":
        case ".jpeg":
        case ".png":
        case ".gif":
            return "image/jpeg";

        case ".mp4":
        case ".avi":
        case ".mov":
            return "video/mp4";

        // Add more mappings for other file types as needed

        default:
            return "application/octet-stream";
    }
}



