using System.Text.Json;

public static class Constants
{
    public const string WEB_PROTOCOL = "http://";
    public const string WEBSOCKET_PROTOCOL = "ws://";
    public const string HOST = "localhost";
    public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}