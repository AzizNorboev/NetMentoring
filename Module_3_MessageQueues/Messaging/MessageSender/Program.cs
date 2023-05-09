// Set the connection string and queue name
using Azure.Messaging.ServiceBus;

string connectionString = "Endpoint=sb://imageprocessingservice.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=bXrVrzmuWDFho2/Qr0xJ9iYSpoehG8Gbk+ASbFrAY5g=";
string queueName = "image-queue";

// Create a ServiceBusClient object
ServiceBusClient client = new ServiceBusClient(connectionString);

// Create a ServiceBusSender object
ServiceBusSender sender = client.CreateSender(queueName);

// Get the path to the image file
string imagePath = @"D:\Desktop\Screenshots\New folder\Screenshot_141.png";

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