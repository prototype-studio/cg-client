using System;
using System.Threading.Tasks;
using Core;

public interface IHubConnection
{
    //EVENTS
    event Action<Exception> Closed;
    Task Connect();
    void On<T>(string methodName, Action<T> handler) where T : WebSocketMessage;
    Task Send<T>(T message) where T : WebSocketMessage;
    void Stop();
}