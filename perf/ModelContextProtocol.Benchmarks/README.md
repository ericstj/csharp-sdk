# MCP Transport Benchmarks

Performance benchmarks for measuring throughput and memory usage of MCP transport implementations.

## Running Benchmarks

### List All Benchmarks

```bash
dotnet run -c Release -- --list flat
```

### Run All Benchmarks

```bash
dotnet run -c Release
```

### Run Specific Benchmark Class

```bash
# Serialization benchmarks
dotnet run -c Release -- -f "*Serialization*"

# Read and deserialize benchmarks
dotnet run -c Release -- -f "*ReadAndDeserialize*"

# Stream transport benchmarks
dotnet run -c Release -- -f "*StreamTransport*"

# Throughput benchmarks
dotnet run -c Release -- -f "*Throughput*"

# Round-trip benchmarks
dotnet run -c Release -- -f "*RoundTrip*"

# Client transport benchmarks
dotnet run -c Release -- -f "*ClientStream*"
```

### Run by Payload Size Category

```bash
# Small payloads (1 KB)
dotnet run -c Release -- --anyCategories Small

# Medium payloads (64 KB)
dotnet run -c Release -- --anyCategories Medium

# Large payloads (1 MB)
dotnet run -c Release -- --anyCategories Large
```

## Comparing Two Implementations

To compare the performance of two SDK implementations (e.g., original vs optimized):

### Step 1: Baseline the Original Implementation

```bash
cd C:\src\modelcontextprotocol\csharp-sdk
dotnet run -c Release --project perf\ModelContextProtocol.Benchmarks -- --exporters json --artifacts baseline-results
```

### Step 2: Run Against the Modified Implementation

```bash
cd C:\src\modelcontextprotocol\csharp-sdk2
dotnet run -c Release --project perf\ModelContextProtocol.Benchmarks -- --exporters json --artifacts modified-results
```

### Step 3: Use BenchmarkDotNet Comparison

For automated comparison between two runs, use the `--baseline` option:

```bash
# Run with comparison to baseline
dotnet run -c Release -- --baseline baseline-results/results/*.json
```

### Alternative: Side-by-Side Comparison

For a manual comparison, run both and compare the JSON results:

```bash
# Export results with custom names
dotnet run -c Release -- -j short --exporters json csv --artifacts results-v1

# After switching to the other implementation
dotnet run -c Release -- -j short --exporters json csv --artifacts results-v2
```

## Benchmark Categories

| Benchmark Class | Description | Focus Area |
|-----------------|-------------|------------|
| `SerializationBenchmarks` | JSON serialization/deserialization of MCP messages | Memory allocation, throughput |
| `ReadAndDeserializeBenchmarks` | Deserialization from byte arrays | Read path performance |
| `StreamTransportBenchmarks` | Server-side stream transport send operations | Write path performance |
| `ClientStreamTransportBenchmarks` | Client-side stream transport operations | Client throughput |
| `TransportRoundTripBenchmarks` | Full message round-trip through transports | End-to-end latency |
| `TransportThroughputBenchmarks` | Batch message processing throughput | Sustained throughput |

## Payload Sizes

| Size | Bytes | Use Case |
|------|-------|----------|
| Small | 1 KB | Typical tool calls, short responses |
| Medium | 64 KB | Code snippets, moderate data |
| Large | 1 MB | Large files, substantial content |
| Extra Large | 10 MB | Very large payloads (not commonly used in typical benchmarks) |

## Output Format

Results are output to the console and saved to the `BenchmarkDotNet.Artifacts` directory. Common export formats:

- **Console**: Human-readable table with timing and allocation data
- **JSON**: Machine-readable results for programmatic comparison
- **CSV**: Spreadsheet-friendly format for analysis
- **Markdown**: Documentation-ready format

## Advanced Options

### Memory Profiling

The `[MemoryDiagnoser]` attribute is enabled on all benchmarks, showing:
- `Gen0`, `Gen1`, `Gen2`: Garbage collections per 1000 operations
- `Allocated`: Memory allocated per operation

### Custom Job Configuration

```bash
# Quick iteration for development
dotnet run -c Release -- -j dry

# Full statistical analysis
dotnet run -c Release -- -j long
```

### Filtering by Method Name

```bash
# Only small payload benchmarks
dotnet run -c Release -- -f "*Small*"

# Only serialization (not deserialization)
dotnet run -c Release -- -f "*Serialize*" --filter-not "*Deserialize*"
```

## Interpreting Results

Key metrics to compare:

1. **Mean**: Average execution time per operation
2. **Allocated**: Memory allocated per operation (lower is better)
3. **Gen0/Gen1/Gen2**: GC pressure (lower is better)
4. **Ratio**: Comparison to baseline (when using `[Baseline = true]`)

When comparing implementations, focus on:
- Significant differences in allocation (more than 10% difference)
- Consistent performance across payload sizes
- GC behavior changes (Gen0 vs Gen1 promotions)
