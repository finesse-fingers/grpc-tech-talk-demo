// See https://aka.ms/new-console-template for more information
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Shared;

using var channel = GrpcChannel.ForAddress("http://localhost:5186");
var client = new Greeter.GreeterClient(channel);

//Console.WriteLine("starting client-server request-response...");
//var reply = await client.SayHelloAsync(new HelloRequest
//{
//    Name = "mck build friends"
//});
//Console.WriteLine(reply.ToString());

//Console.WriteLine("starting server-side stream...");
//using var serverStream = client.SayHelloServerStream(new HelloStreamRequest { Name = "mck build friends", NumberOfMessages = 7 });
//await foreach (var response in serverStream.ResponseStream.ReadAllAsync())
//{
//    Console.WriteLine(response.ToString());
//}

Console.WriteLine("Starting background task to receive messages");
using var call = client.SayHelloEcho();
var readTask = Task.Run(async () =>
{
    await foreach (var response in call.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine("From server: " + response.Message);
        // Echo messages sent to the service
    }
});
Console.WriteLine("Starting to send messages");
Console.WriteLine("Type a message to echo then press enter.");
while (true)
{
    var result = Console.ReadLine();
    if (string.IsNullOrEmpty(result))
    {
        break;
    }

    await call.RequestStream.WriteAsync(new HelloRequest { Name = result });
}
Console.WriteLine("Disconnecting");
await call.RequestStream.CompleteAsync();
await readTask;

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

