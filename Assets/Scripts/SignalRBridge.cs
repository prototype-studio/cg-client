#if UNITY_WEBGL
using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using Newtonsoft.Json;

public class SignalRBridge
{
    private CustomHubConnection hubConnection;
    private Dictionary<string, Delegate> handlers = new Dictionary<string, Delegate>();

    public SignalRBridge(CustomHubConnection hubConnection)
    {
        this.hubConnection = hubConnection;
    }

    public void RegisterHandler<T>(string methodName, Action<T> handler) where T : WebSocketMessage
    {
        handlers[methodName] = handler;
    }

    // Called from JS
    public void OnClosed(string error)
    {
        hubConnection?.HandleClosed(error);
    }

    // Called from JS
    public void OnMessage(string json)
    {
        try
        {
            var wrapper = JsonConvert.DeserializeObject<MessageWrapper>(json);
            if (wrapper != null && handlers.TryGetValue(wrapper.method, out var del))
            {
                var type = del.GetType().GenericTypeArguments[0];
                var payload = JsonConvert.DeserializeObject(wrapper.payload.ToString(), type);
                del.DynamicInvoke(payload);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("SignalRBridge OnMessage error: " + ex);
        }
    }

    [Serializable]
    private class MessageWrapper
    {
        public string method;
        public object payload;
    }
}
#endif