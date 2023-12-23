#nullable enable
using System.Text.Json.Serialization;

namespace Chrono.Shared.Api;

/// <summary>
///     Builds API responses following the JSend (see https://github.com/omniti-labs/jsend) specification.
/// </summary>
public static class JSendResponseBuilder
{
    private static JSendResponseBuilder<T> CreateResponse<T>(string status, T? data, string? message = null)
    {
        return new JSendResponseBuilder<T>(status) { Data = data, Message = message };
    }

    public static JSendResponseBuilder<T> Success<T>(T data, string? message = null)
    {
        return CreateResponse("success", data, message);
    }

    public static JSendResponseBuilder<T> Fail<T>(T data, string? message = null)
    {
        return CreateResponse("fail", data, message);
    }

    public static JSendResponseBuilder<T> Error<T>(string message, T? data = default)
    {
        return CreateResponse("fail", data, message);
    }
}

public class JSendResponseBuilder<T>(string status)
{
    public string Status { get; set; } = status;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T? Data { get; set; }
}
