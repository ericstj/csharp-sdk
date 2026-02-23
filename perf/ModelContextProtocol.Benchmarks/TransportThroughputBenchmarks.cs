using BenchmarkDotNet.Attributes;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.IO.Pipelines;
using System.Text.Json;

namespace ModelContextProtocol.Benchmarks;

/// <summary>
/// Benchmarks measuring throughput (messages per time unit) for MCP transports.
/// These benchmarks process multiple messages to measure sustained throughput.
/// </summary>
[MemoryDiagnoser]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class TransportThroughputBenchmarks
{
    private byte[] _smallMessageBatch = null!;
    private byte[] _mediumMessageBatch = null!;
    private byte[] _largeMessageBatch = null!;

    private const int BatchSize = 100;

    [GlobalSetup]
    public void Setup()
    {
        _smallMessageBatch = CreateMessageBatch(PayloadGenerator.Sizes.Small, BatchSize);
        _mediumMessageBatch = CreateMessageBatch(PayloadGenerator.Sizes.Medium, BatchSize);
        _largeMessageBatch = CreateMessageBatch(PayloadGenerator.Sizes.Large, BatchSize);
    }

    private static byte[] CreateMessageBatch(int payloadSize, int count)
    {
        using var ms = new MemoryStream();
        for (int i = 0; i < count; i++)
        {
            var message = PayloadGenerator.CreateRequest(payloadSize, new RequestId(i + 1));
            var bytes = JsonSerializer.SerializeToUtf8Bytes(message, McpJsonUtilities.DefaultOptions);
            ms.Write(bytes);
            ms.WriteByte((byte)'\n');
        }
        return ms.ToArray();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Throughput", "Small")]
    public async Task<int> ThroughputSmallMessages()
    {
        return await ProcessBatchAsync(_smallMessageBatch, BatchSize);
    }

    [Benchmark]
    [BenchmarkCategory("Throughput", "Medium")]
    public async Task<int> ThroughputMediumMessages()
    {
        return await ProcessBatchAsync(_mediumMessageBatch, BatchSize);
    }

    [Benchmark]
    [BenchmarkCategory("Throughput", "Large")]
    public async Task<int> ThroughputLargeMessages()
    {
        return await ProcessBatchAsync(_largeMessageBatch, BatchSize);
    }

    private static async Task<int> ProcessBatchAsync(byte[] batch, int expectedCount)
    {
        var pipe = new Pipe();
        var outputStream = new MemoryStream();

        await using var transport = new StreamServerTransport(
            pipe.Reader.AsStream(),
            outputStream);

        // Write all messages to the pipe
        await pipe.Writer.WriteAsync(batch);
        pipe.Writer.Complete();

        // Read all messages
        int count = 0;
        while (await transport.MessageReader.WaitToReadAsync())
        {
            while (transport.MessageReader.TryRead(out _))
            {
                count++;
                if (count >= expectedCount)
                {
                    return count;
                }
            }
        }

        return count;
    }
}
