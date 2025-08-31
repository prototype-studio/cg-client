using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using UnityEngine;

public class WebSocketConnection
{
    //EVENTS
    public event Action Connected;
    public event Action<Exception> ConnectionFailed;
    public event Action<Exception> Disconnected;
    
    //PRIVATES
    private readonly string token;
    
    #region .NET
    
    public WebSocketConnection(string token)
    {
        this.token = token;
        InitConnection();
    }
    
    #region CONNECTION
    
    private IHubConnection hubConnection;

    private void InitConnection()
    {
        hubConnection = new CustomHubConnection(token);
        hubConnection.Closed += OnClosed;
    }

    public async Task Connect()
    {
        try
        {
            await hubConnection.Connect();
            Connected?.Invoke();
        }
        catch (Exception ex)
        {
            ConnectionFailed?.Invoke(ex);
        }
    }
    
    private void OnDisconnected(Exception ex)
    {
        Disconnected?.Invoke(ex);
    }
    
    #endregion

    #region INCOMING MESSAGES
    
    private readonly ConcurrentQueue<WebSocketMessage> messageQueue = new ConcurrentQueue<WebSocketMessage>();
    private readonly Dictionary<string, Action<WebSocketMessage>> messageHandlers = new Dictionary<string, Action<WebSocketMessage>>();
    
    public void AddMessageHandler<T>(Action<WebSocketMessage> handler) where T : WebSocketMessage
    {
        var typeName = typeof(T).Name;
        if (messageHandlers.TryAdd(typeName, handler))
        {
            hubConnection.On<T>(typeName, messageQueue.Enqueue);
        }
    }

    public void PollMessages()
    {
        while (messageQueue.TryDequeue(out var message))
        {
            if (messageHandlers.TryGetValue(message.GetType().Name, out var handler))
            {
                handler?.Invoke(message);
            }
        }
    }
    
    #endregion
    
    #region EVENTS
    
    private readonly ConcurrentQueue<Action> eventQueue = new ConcurrentQueue<Action>();
    
    private void QueueEvent(Action action) => eventQueue.Enqueue(action);
    
    public void PollEvents()
    {
        while (eventQueue.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }

    private void OnClosed(Exception reason)
    {
        QueueEvent(() => OnDisconnected(reason));
    }
    
    #endregion

    public void Stop()
    {
        hubConnection?.Stop();
    }
    
    #endregion
    
    #region SEND
    
    public void Send(WebSocketMessage message)
    {
        try
        {
            var task = hubConnection.Send(message);
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                    Debug.LogError("Send error: " + t.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
        catch (Exception ex)
        {
            Debug.LogError("Send error: " + ex);
        }
    }
    
    #endregion
}