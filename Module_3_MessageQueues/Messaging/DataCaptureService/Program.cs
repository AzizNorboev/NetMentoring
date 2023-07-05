// Set the connection string and queue name
using Azure.Messaging.ServiceBus;



using Azure.Storage.Blobs;



// Set the connection string and queue name
string serviceBusConnectionString = "Endpoint=sb://imageprocessingservice.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=bXrVrzmuWDFho2/Qr0xJ9iYSpoehG8Gbk+ASbFrAY5g=";
string queueName = "image-queue";

//await SendVideoMessage(serviceBusConnectionString, queueName);
await SendImageMessage(serviceBusConnectionString, queueName);



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