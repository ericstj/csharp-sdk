using BenchmarkDotNet.Attributes;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.IO.Pipelines;
using System.Text.Json;

namespace ModelContextProtocol.Benchmarks;

/// <summary>
/// Benchmarks measuring the full round-trip performance of MCP transports.
/// This includes serialization, writing to stream, reading from stream, and deserialization.
/// </summary>
[MemoryDiagnoser]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class TransportRoundTripBenchmarks
{
    private JsonRpcRequest _smallRequest = null!;
    private JsonRpcRequest _mediumRequest = null!;
    private JsonRpcRequest _largeRequest = null!;

    [GlobalSetup]
    public void Setup()
    {
        _smallRequest = PayloadGenerator.CreateRequest(PayloadGenerator.Sizes.Small, new RequestId(1));
        _mediumRequest = PayloadGenerator.CreateRequest(PayloadGenerator.Sizes.Medium, new RequestId(2));
        _largeRequest = PayloadGenerator.CreateRequest(PayloadGenerator.Sizes.Large, new RequestId(3));
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("RoundTrip", "Small")]
    public async Task<JsonRpcMessage?> RoundTripSmall_StreamTransport()
    {
        return await RoundTripAsync(_smallRequest);
    }

    [Benchmark]
    [BenchmarkCategory("RoundTrip", "Medium")]
    public async Task<JsonRpcMessage?> RoundTripMedium_StreamTransport()
    {
        return await RoundTripAsync(_mediumRequest);
    }

    [Benchmark]
    [BenchmarkCategory("RoundTrip", "Large")]
    public async Task<JsonRpcMessage?> RoundTripLarge_StreamTransport()
    {
        return await RoundTripAsync(_largeRequest);
    }

    private static async Task<JsonRpcMessage?> RoundTripAsync(JsonRpcMessage message)
    {
        // Create in-memory pipes for bidirectional communication
        var clientToServer = new Pipe();
        var serverToClient = new Pipe();

        // Create server transport reading from client, writing to client
        await using var serverTransport = new StreamServerTransport(
            clientToServer.Reader.AsStream(),
            serverToClient.Writer.AsStream());

        // Write message to server (simulating client sending)
        var messageBytes = JsonSerializer.SerializeToUtf8Bytes(message, McpJsonUtilities.DefaultOptions);
        await clientToServer.Writer.WriteAsync(messageBytes);
        await clientToServer.Writer.WriteAsync("\n"u8.ToArray());
        await clientToServer.Writer.FlushAsync();
        clientToServer.Writer.Complete();

        // Read the message from the server's message reader
        var result = await serverTransport.MessageReader.ReadAsync();
        
        // Clean up
        serverToClient.Writer.Complete();

        return result;
    }
}
