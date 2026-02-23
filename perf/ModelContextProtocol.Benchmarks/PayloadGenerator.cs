using System.Text.Json.Nodes;
using ModelContextProtocol.Protocol;

namespace ModelContextProtocol.Benchmarks;

/// <summary>
/// Provides helper methods to generate test payloads for benchmarking.
/// </summary>
public static class PayloadGenerator
{
    /// <summary>
    /// Generates a string of the specified size in bytes (approximately, for ASCII content).
    /// </summary>
    public static string GenerateString(int sizeInBytes)
    {
        return new string('x', sizeInBytes);
    }

    /// <summary>
    /// Creates a JsonRpcRequest with a payload of the specified size.
    /// </summary>
    /// <param name="payloadSizeBytes">The approximate size of the payload in bytes.</param>
    /// <param name="requestId">The request ID to use.</param>
    /// <returns>A JsonRpcRequest with the specified payload size.</returns>
    public static JsonRpcRequest CreateRequest(int payloadSizeBytes, RequestId requestId)
    {
        var content = GenerateString(payloadSizeBytes);
        return new JsonRpcRequest
        {
            Id = requestId,
            Method = "tools/call",
            Params = new JsonObject
            {
                ["name"] = "test_tool",
                ["arguments"] = new JsonObject
                {
                    ["data"] = content
                }
            }
        };
    }

    /// <summary>
    /// Creates a JsonRpcResponse with a payload of the specified size.
    /// </summary>
    /// <param name="payloadSizeBytes">The approximate size of the payload in bytes.</param>
    /// <param name="requestId">The request ID to use.</param>
    /// <returns>A JsonRpcResponse with the specified payload size.</returns>
    public static JsonRpcResponse CreateResponse(int payloadSizeBytes, RequestId requestId)
    {
        var content = GenerateString(payloadSizeBytes);
        return new JsonRpcResponse
        {
            Id = requestId,
            Result = new JsonObject
            {
                ["content"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["type"] = "text",
                        ["text"] = content
                    }
                }
            }
        };
    }

    /// <summary>
    /// Creates a JsonRpcNotification with a payload of the specified size.
    /// </summary>
    /// <param name="payloadSizeBytes">The approximate size of the payload in bytes.</param>
    /// <returns>A JsonRpcNotification with the specified payload size.</returns>
    public static JsonRpcNotification CreateNotification(int payloadSizeBytes)
    {
        var content = GenerateString(payloadSizeBytes);
        return new JsonRpcNotification
        {
            Method = "notifications/progress",
            Params = new JsonObject
            {
                ["progressToken"] = "token-123",
                ["progress"] = 50,
                ["total"] = 100,
                ["data"] = content
            }
        };
    }

    /// <summary>
    /// Predefined payload sizes for benchmarking.
    /// </summary>
    public static class Sizes
    {
        /// <summary>Small payload: 1 KB</summary>
        public const int Small = 1 * 1024;

        /// <summary>Medium payload: 64 KB</summary>
        public const int Medium = 64 * 1024;

        /// <summary>Large payload: 1 MB</summary>
        public const int Large = 1 * 1024 * 1024;

        /// <summary>Extra large payload: 10 MB</summary>
        public const int ExtraLarge = 10 * 1024 * 1024;
    }
}
