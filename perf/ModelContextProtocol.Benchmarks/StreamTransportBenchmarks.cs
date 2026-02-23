using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.IO.Pipelines;

namespace ModelContextProtocol.Benchmarks;

/// <summary>
/// Benchmarks for measuring the performance of MCP transport implementations.
/// These benchmarks focus on the read/write path performance including serialization/deserialization.
/// </summary>
[MemoryDiagnoser]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class StreamTransportBenchmarks
{
    private JsonRpcRequest _smallRequest = null!;
    private JsonRpcRequest _mediumRequest = null!;
    private JsonRpcRequest _largeRequest = null!;
    private JsonRpcResponse _smallResponse = null!;
    private JsonRpcResponse _mediumResponse = null!;
    private JsonRpcResponse _largeResponse = null!;

    [GlobalSetup]
    public void Setup()
    {
        _smallRequest = PayloadGenerator.CreateRequest(PayloadGenerator.Sizes.Small, new RequestId(1));
        _mediumRequest = PayloadGenerator.CreateRequest(PayloadGenerator.Sizes.Medium, new RequestId(2));
        _largeRequest = PayloadGenerator.CreateRequest(PayloadGenerator.Sizes.Large, new RequestId(3));
        _smallResponse = PayloadGenerator.CreateResponse(PayloadGenerator.Sizes.Small, new RequestId(1));
        _mediumResponse = PayloadGenerator.CreateResponse(PayloadGenerator.Sizes.Medium, new RequestId(2));
        _largeResponse = PayloadGenerator.CreateResponse(PayloadGenerator.Sizes.Large, new RequestId(3));
    }

    [Benchmark]
    [BenchmarkCategory("Send", "Small")]
    public async Task SendSmallRequest_StreamServerTransport()
    {
        await using var transport = CreateStreamServerTransport(out var outputStream);
        await transport.SendMessageAsync(_smallRequest);
        outputStream.Position = 0;
    }

    [Benchmark]
    [BenchmarkCategory("Send", "Medium")]
    public async Task SendMediumRequest_StreamServerTransport()
    {
        await using var transport = CreateStreamServerTransport(out var outputStream);
        await transport.SendMessageAsync(_mediumRequest);
        outputStream.Position = 0;
    }

    [Benchmark]
    [BenchmarkCategory("Send", "Large")]
    public async Task SendLargeRequest_StreamServerTransport()
    {
        await using var transport = CreateStreamServerTransport(out var outputStream);
        await transport.SendMessageAsync(_largeRequest);
        outputStream.Position = 0;
    }

    [Benchmark]
    [BenchmarkCategory("Send", "Small")]
    public async Task SendSmallResponse_StreamServerTransport()
    {
        await using var transport = CreateStreamServerTransport(out var outputStream);
        await transport.SendMessageAsync(_smallResponse);
        outputStream.Position = 0;
    }

    [Benchmark]
    [BenchmarkCategory("Send", "Medium")]
    public async Task SendMediumResponse_StreamServerTransport()
    {
        await using var transport = CreateStreamServerTransport(out var outputStream);
        await transport.SendMessageAsync(_mediumResponse);
        outputStream.Position = 0;
    }

    [Benchmark]
    [BenchmarkCategory("Send", "Large")]
    public async Task SendLargeResponse_StreamServerTransport()
    {
        await using var transport = CreateStreamServerTransport(out var outputStream);
        await transport.SendMessageAsync(_largeResponse);
        outputStream.Position = 0;
    }

    private static StreamServerTransport CreateStreamServerTransport(out MemoryStream outputStream)
    {
        var inputStream = new MemoryStream();
        outputStream = new MemoryStream();
        return new StreamServerTransport(inputStream, outputStream);
    }
}
