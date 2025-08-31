#if !UNITY_WEBGL
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

public class CustomHubConnection : IHubConnection
{
    //EVENTS
    public event Action<Exception> Closed;
    
    //PRIVATES
    private HubConnection hubConnection;
    
    public CustomHubConnection(string token)
    {
        hubConnection = new HubConnectionBuilder()
        .WithUrl($"{Constants.WEB_PROTOCOL}{Constants.HOST}/session", options =>
        {
            options.AccessTokenProvider = () => Task.FromResult(token);
            options.HttpMessageHandlerFactory = _ => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
        })
        .AddJsonProtocol(cfg =>
        {
            cfg.PayloadSerializerOptions = Constants.JsonOptions;
        })
        .Build();
        hubConnection.Closed += (ex) =>
        {
            Closed?.Invoke(ex);
            return Task.CompletedTask;
        };
    }

    public async Task Connect()
    {
        await hubConnection.StartAsync();
    }
    
    public void On<T>(string methodName, Action<T> handler) where T : WebSocketMessage
    {
        hubConnection.On(methodName, handler);
    }
    
    public async Task Send<T>(T message) where T : WebSocketMessage
    {
        await hubConnection.SendAsync(message.GetType().Name, message);
    }

    public void Stop()
    {
        try
        {
            hubConnection?.StopAsync().Wait();
            hubConnection?.DisposeAsync().AsTask().Wait();
        }
        catch
        {
            /*ignore*/
        }
    }
}
#endif