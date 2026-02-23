using BenchmarkDotNet.Attributes;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace ModelContextProtocol.Benchmarks;

/// <summary>
/// Benchmarks for the client-side stream transport (StreamClientSessionTransport).
/// Focuses on send operations since round-trip benchmarks require complex async coordination.
/// </summary>
[MemoryDiagnoser]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class ClientStreamTransportBenchmarks
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
    [BenchmarkCategory("ClientSend", "Small")]
    public async Task SendSmallRequest_StreamClientTransport()
    {
        await using var transport = await CreateStreamClientTransportAsync();
        await transport.SendMessageAsync(_smallRequest);
    }

    [Benchmark]
    [BenchmarkCategory("ClientSend", "Medium")]
    public async Task SendMediumRequest_StreamClientTransport()
    {
        await using var transport = await CreateStreamClientTransportAsync();
        await transport.SendMessageAsync(_mediumRequest);
    }

    [Benchmark]
    [BenchmarkCategory("ClientSend", "Large")]
    public async Task SendLargeRequest_StreamClientTransport()
    {
        await using var transport = await CreateStreamClientTransportAsync();
        await transport.SendMessageAsync(_largeRequest);
    }

    private static async Task<ITransport> CreateStreamClientTransportAsync()
    {
        var serverInput = new MemoryStream();
        var serverOutput = new MemoryStream();
        var clientTransport = new StreamClientTransport(serverInput, serverOutput);
        return await clientTransport.ConnectAsync();
    }
}

