using Grpc.Core;
using Grpc.Shared;

namespace Grpc.Server.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(
        HelloRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation(request.ToString());

        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }

    public override async Task SayHelloServerStream(
        HelloStreamRequest request,
        IServerStreamWriter<HelloReply> responseStream,
        ServerCallContext context)
    {
        for (var i = 0; i < request.NumberOfMessages; i++)
        {
            await responseStream.WriteAsync(new HelloReply { Message = $"{i + 1}: Hello " + request.Name });
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }

    public override async Task SayHelloEcho(
        IAsyncStreamReader<HelloRequest> requestStream,
        IServerStreamWriter<HelloReply> responseStream,
        ServerCallContext context)
    {
        _logger.LogInformation("Starting stream...");

        await foreach (var message in requestStream.ReadAllAsync())
        {
            await responseStream.WriteAsync(new HelloReply { Message = "Hello " + message.Name });
        }

        _logger.LogInformation("Finishing stream...");
    }
}

