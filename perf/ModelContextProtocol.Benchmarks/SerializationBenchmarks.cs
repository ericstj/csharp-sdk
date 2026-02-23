#if false
using BenchmarkDotNet.Attributes;
using ModelContextProtocol.Protocol;
using System.Buffers;
using System.Text.Json;

namespace ModelContextProtocol.Benchmarks;

/// <summary>
/// Benchmarks for JSON serialization/deserialization of MCP messages.
/// These benchmarks isolate the serialization layer from transport concerns.
/// </summary>
[MemoryDiagnoser]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class SerializationBenchmarks
{
    private JsonRpcRequest _smallRequest = null!;
    private JsonRpcRequest _mediumRequest = null!;
    private JsonRpcRequest _largeRequest = null!;
    private JsonRpcResponse _smallResponse = null!;
    private JsonRpcResponse _mediumResponse = null!;
    private JsonRpcResponse _largeResponse = null!;

    private byte[] _smallRequestBytes = null!;
    private byte[] _mediumRequestBytes = null!;
    private byte[] _largeRequestBytes = null!;

    [GlobalSetup]
    public void Setup()
    {
        _smallRequest = PayloadGenerator.CreateRequest(PayloadGenerator.Sizes.Small, new RequestId(1));
        _mediumRequest = PayloadGenerator.CreateRequest(PayloadGenerator.Sizes.Medium, new RequestId(2));
        _largeRequest = PayloadGenerator.CreateRequest(PayloadGenerator.Sizes.Large, new RequestId(3));
        _smallResponse = PayloadGenerator.CreateResponse(PayloadGenerator.Sizes.Small, new RequestId(1));
        _mediumResponse = PayloadGenerator.CreateResponse(PayloadGenerator.Sizes.Medium, new RequestId(2));
        _largeResponse = PayloadGenerator.CreateResponse(PayloadGenerator.Sizes.Large, new RequestId(3));

        _smallRequestBytes = JsonSerializer.SerializeToUtf8Bytes(_smallRequest, McpJsonUtilities.DefaultOptions);
        _mediumRequestBytes = JsonSerializer.SerializeToUtf8Bytes(_mediumRequest, McpJsonUtilities.DefaultOptions);
        _largeRequestBytes = JsonSerializer.SerializeToUtf8Bytes(_largeRequest, McpJsonUtilities.DefaultOptions);
    }

    // Serialization benchmarks

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Serialize", "Small")]
    public byte[] SerializeSmallRequest()
    {
        return JsonSerializer.SerializeToUtf8Bytes(_smallRequest, McpJsonUtilities.DefaultOptions);
    }

    [Benchmark]
    [BenchmarkCategory("Serialize", "Medium")]
    public byte[] SerializeMediumRequest()
    {
        return JsonSerializer.SerializeToUtf8Bytes(_mediumRequest, McpJsonUtilities.DefaultOptions);
    }

    [Benchmark]
    [BenchmarkCategory("Serialize", "Large")]
    public byte[] SerializeLargeRequest()
    {
        return JsonSerializer.SerializeToUtf8Bytes(_largeRequest, McpJsonUtilities.DefaultOptions);
    }

    [Benchmark]
    [BenchmarkCategory("Serialize", "Small")]
    public byte[] SerializeSmallResponse()
    {
        return JsonSerializer.SerializeToUtf8Bytes(_smallResponse, McpJsonUtilities.DefaultOptions);
    }

    [Benchmark]
    [BenchmarkCategory("Serialize", "Medium")]
    public byte[] SerializeMediumResponse()
    {
        return JsonSerializer.SerializeToUtf8Bytes(_mediumResponse, McpJsonUtilities.DefaultOptions);
    }

    [Benchmark]
    [BenchmarkCategory("Serialize", "Large")]
    public byte[] SerializeLargeResponse()
    {
        return JsonSerializer.SerializeToUtf8Bytes(_largeResponse, McpJsonUtilities.DefaultOptions);
    }

    // Deserialization benchmarks using Span<byte>

    [Benchmark]
    [BenchmarkCategory("Deserialize", "Small")]
    public JsonRpcMessage? DeserializeSmallRequest_Span()
    {
        return JsonSerializer.Deserialize(_smallRequestBytes.AsSpan(), McpJsonUtilities.DefaultOptions.GetTypeInfo(typeof(JsonRpcMessage))) as JsonRpcMessage;
    }

    [Benchmark]
    [BenchmarkCategory("Deserialize", "Medium")]
    public JsonRpcMessage? DeserializeMediumRequest_Span()
    {
        return JsonSerializer.Deserialize(_mediumRequestBytes.AsSpan(), McpJsonUtilities.DefaultOptions.GetTypeInfo(typeof(JsonRpcMessage))) as JsonRpcMessage;
    }

    [Benchmark]
    [BenchmarkCategory("Deserialize", "Large")]
    public JsonRpcMessage? DeserializeLargeRequest_Span()
    {
        return JsonSerializer.Deserialize(_largeRequestBytes.AsSpan(), McpJsonUtilities.DefaultOptions.GetTypeInfo(typeof(JsonRpcMessage))) as JsonRpcMessage;
    }

    // Deserialization using Utf8JsonReader (as used by transports with multi-segment buffers)

    [Benchmark]
    [BenchmarkCategory("Deserialize", "Small")]
    public JsonRpcMessage? DeserializeSmallRequest_Utf8JsonReader()
    {
        var sequence = new ReadOnlySequence<byte>(_smallRequestBytes);
        var reader = new Utf8JsonReader(sequence, isFinalBlock: true, state: default);
        return JsonSerializer.Deserialize(ref reader, McpJsonUtilities.DefaultOptions.GetTypeInfo(typeof(JsonRpcMessage))) as JsonRpcMessage;
    }

    [Benchmark]
    [BenchmarkCategory("Deserialize", "Medium")]
    public JsonRpcMessage? DeserializeMediumRequest_Utf8JsonReader()
    {
        var sequence = new ReadOnlySequence<byte>(_mediumRequestBytes);
        var reader = new Utf8JsonReader(sequence, isFinalBlock: true, state: default);
        return JsonSerializer.Deserialize(ref reader, McpJsonUtilities.DefaultOptions.GetTypeInfo(typeof(JsonRpcMessage))) as JsonRpcMessage;
    }

    [Benchmark]
    [BenchmarkCategory("Deserialize", "Large")]
    public JsonRpcMessage? DeserializeLargeRequest_Utf8JsonReader()
    {
        var sequence = new ReadOnlySequence<byte>(_largeRequestBytes);
        var reader = new Utf8JsonReader(sequence, isFinalBlock: true, state: default);
        return JsonSerializer.Deserialize(ref reader, McpJsonUtilities.DefaultOptions.GetTypeInfo(typeof(JsonRpcMessage))) as JsonRpcMessage;
    }
}
#endif