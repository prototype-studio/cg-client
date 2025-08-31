#if UNITY_WEBGL
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Core;

public class CustomHubConnection : IHubConnection
{
    // EVENTS
    public event Action<Exception> Closed;

    // PRIVATES
    private string connectionId;
    private SignalRBridge bridge;

    [DllImport("__Internal")] private static extern string CustomHubConnection_Create(string url, string token);
    [DllImport("__Internal")] private static extern void CustomHubConnection_Connect(string id);
    [DllImport("__Internal")] private static extern void CustomHubConnection_On(string id, string methodName);
    [DllImport("__Internal")] private static extern void CustomHubConnection_Stop(string id);

    public CustomHubConnection(string token)
    {
        // Build URL the same way
        var url = $"{Constants.WEB_PROTOCOL}{Constants.HOST}/session";
        connectionId = CustomHubConnection_Create(url, token);
        bridge = new SignalRBridge(this);
    }

    public async System.Threading.Tasks.Task Connect()
    {
        CustomHubConnection_Connect(connectionId);
        await System.Threading.Tasks.Task.CompletedTask;
    }

    public void On<T>(string methodName, Action<T> handler) where T : WebSocketMessage
    {
        // Register in JS
        CustomHubConnection_On(connectionId, methodName);

        // Register in bridge to dispatch payloads
        bridge.RegisterHandler<T>(methodName, handler);
    }
    
    public Task Send<T>(T message) where T : WebSocketMessage
    {
        return Task.CompletedTask;
    }

    public void Stop()
    {
        CustomHubConnection_Stop(connectionId);
    }

    // Called by JS -> Unity
    internal void HandleClosed(string error)
    {
        Closed?.Invoke(string.IsNullOrEmpty(error) ? null : new Exception(error));
    }
}
#endif