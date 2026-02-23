#if false
using BenchmarkDotNet.Attributes;
using ModelContextProtocol.Protocol;
using System.Text.Json;

namespace ModelContextProtocol.Benchmarks;

/// <summary>
/// Benchmarks focusing specifically on the read and deserialization path of MCP transports.
/// This measures the performance impact of deserializing JSON-RPC messages from byte streams.
/// </summary>
[MemoryDiagnoser]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class ReadAndDeserializeBenchmarks
{
    private byte[] _smallRequestBytes = null!;
    private byte[] _mediumRequestBytes = null!;
    private byte[] _largeRequestBytes = null!;
    private byte[] _smallResponseBytes = null!;
    private byte[] _mediumResponseBytes = null!;
    private byte[] _largeResponseBytes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var smallRequest = PayloadGenerator.CreateRequest(PayloadGenerator.Sizes.Small, new RequestId(1));
        var mediumRequest = PayloadGenerator.CreateRequest(PayloadGenerator.Sizes.Medium, new RequestId(2));
        var largeRequest = PayloadGenerator.CreateRequest(PayloadGenerator.Sizes.Large, new RequestId(3));
        var smallResponse = PayloadGenerator.CreateResponse(PayloadGenerator.Sizes.Small, new RequestId(1));
        var mediumResponse = PayloadGenerator.CreateResponse(PayloadGenerator.Sizes.Medium, new RequestId(2));
        var largeResponse = PayloadGenerator.CreateResponse(PayloadGenerator.Sizes.Large, new RequestId(3));

        _smallRequestBytes = JsonSerializer.SerializeToUtf8Bytes(smallRequest, McpJsonUtilities.DefaultOptions);
        _mediumRequestBytes = JsonSerializer.SerializeToUtf8Bytes(mediumRequest, McpJsonUtilities.DefaultOptions);
        _largeRequestBytes = JsonSerializer.SerializeToUtf8Bytes(largeRequest, McpJsonUtilities.DefaultOptions);
        _smallResponseBytes = JsonSerializer.SerializeToUtf8Bytes(smallResponse, McpJsonUtilities.DefaultOptions);
        _mediumResponseBytes = JsonSerializer.SerializeToUtf8Bytes(mediumResponse, McpJsonUtilities.DefaultOptions);
        _largeResponseBytes = JsonSerializer.SerializeToUtf8Bytes(largeResponse, McpJsonUtilities.DefaultOptions);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Request", "Small")]
    public JsonRpcMessage? DeserializeSmallRequest()
    {
        return JsonSerializer.Deserialize(_smallRequestBytes.AsSpan(), McpJsonUtilities.DefaultOptions.GetTypeInfo(typeof(JsonRpcMessage))) as JsonRpcMessage;
    }

    [Benchmark]
    [BenchmarkCategory("Request", "Medium")]
    public JsonRpcMessage? DeserializeMediumRequest()
    {
        return JsonSerializer.Deserialize(_mediumRequestBytes.AsSpan(), McpJsonUtilities.DefaultOptions.GetTypeInfo(typeof(JsonRpcMessage))) as JsonRpcMessage;
    }

    [Benchmark]
    [BenchmarkCategory("Request", "Large")]
    public JsonRpcMessage? DeserializeLargeRequest()
    {
        return JsonSerializer.Deserialize(_largeRequestBytes.AsSpan(), McpJsonUtilities.DefaultOptions.GetTypeInfo(typeof(JsonRpcMessage))) as JsonRpcMessage;
    }

    [Benchmark]
    [BenchmarkCategory("Response", "Small")]
    public JsonRpcMessage? DeserializeSmallResponse()
    {
        return JsonSerializer.Deserialize(_smallResponseBytes.AsSpan(), McpJsonUtilities.DefaultOptions.GetTypeInfo(typeof(JsonRpcMessage))) as JsonRpcMessage;
    }

    [Benchmark]
    [BenchmarkCategory("Response", "Medium")]
    public JsonRpcMessage? DeserializeMediumResponse()
    {
        return JsonSerializer.Deserialize(_mediumResponseBytes.AsSpan(), McpJsonUtilities.DefaultOptions.GetTypeInfo(typeof(JsonRpcMessage))) as JsonRpcMessage;
    }

    [Benchmark]
    [BenchmarkCategory("Response", "Large")]
    public JsonRpcMessage? DeserializeLargeResponse()
    {
        return JsonSerializer.Deserialize(_largeResponseBytes.AsSpan(), McpJsonUtilities.DefaultOptions.GetTypeInfo(typeof(JsonRpcMessage))) as JsonRpcMessage;
    }
}
#endif