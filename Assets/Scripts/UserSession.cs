using System;
using System.Threading.Tasks;
using Core;
using UI;
using UnityEngine;

public class UserSession : MonoBehaviour
{
    [SerializeField] private LoginScreen loginScreen;
    [SerializeField] private ProgressScreen progressScreen;
    [SerializeField] private OnlinePlayModeScreen onlinePlayModeScreen;
    
    //EVENTS
    public event Action<MatchSessionData> OnlineMatchStarted;
    public event Action OnlineMatchEnded;
    
    //PROPERTIES
    public bool Connected { get; private set; }
    public bool Authenticated { get; private set; }
    public string Username { get; private set; }
    public MatchSessionData MatchSessionData { get; private set; }

    //PRIVATES
    private WebSocketConnection hubConnection;

    public async Task Init(WebSocketConnection hubConnection)
    {
        this.hubConnection = hubConnection;
        RegisterMessageHandlers();
        this.hubConnection.Connect();
    }
    
    private void Update()
    {
        hubConnection?.PollEvents();
        if (Connected)
        {
            hubConnection?.PollMessages();
        }
    }
    
    #region CONNECTION

    private void OnConnected()
    {
        Connected = true;
        Debug.Log("Connected to server.");
        if (Authenticated)
        {
            // deal with race condition
            StartOnlinePlay();
        }
        else
        {
            progressScreen.SetProgressText("Authenticating...", Color.white);
        }
    }


    private void OnConnectionFailed(Exception ex)
    {
        loginScreen.SetInfo(ex.Message, Color.red);
        OnDisconnected(ex);
    }
    
    
    private void OnAuthenticated(string username)
    {
        Authenticated = true;
        Username = username;
        Debug.Log("Authenticated: " + username);
        if (Connected)
        {
            // deal with race condition
            StartOnlinePlay();
        }
    }

    private void OnAuthenticationFailed(string error)
    {
        loginScreen.SetInfo(error, Color.red);
    }

    private void OnDisconnected(Exception ex)
    {
        if (Connected && Authenticated)
        {
            loginScreen.SetInfo(ex?.Message ?? "Disconnected from server", Color.red);
        }
        Connected = false;
        Authenticated = false;
        Username = null;
        if (MatchSessionData != null)
        {
            OnlineMatchEnded?.Invoke();
            MatchSessionData = null;
        }
        loginScreen.Show();
        Stop();
    }
    
    #endregion

    private void StartOnlinePlay()
    {
        onlinePlayModeScreen.Show();
    }
    
    #region STOP

    private void OnDestroy()
    {
        Stop();
    }

    public void Stop()
    {
        hubConnection?.Stop();
        
    }

    #endregion
    
    #region MESSAGES

    private void RegisterMessageHandlers()
    {
        hubConnection.Connected += OnConnected;
        hubConnection.ConnectionFailed += OnConnectionFailed;
        hubConnection.Disconnected += OnDisconnected;
        hubConnection.AddMessageHandler<AuthenticationResult>(OnAuthenticationResult);
        hubConnection.AddMessageHandler<MatchSessionData>(OnOnlineMatchStarted);
        hubConnection.AddMessageHandler<OnlineMatchEnded>(OnOnlineMatchEnded);
    }

    private void OnAuthenticationResult(WebSocketMessage message)
    {
        var result = message as AuthenticationResult;
        if (string.IsNullOrWhiteSpace(result!.Error)) OnAuthenticated(result.Username);
        else OnAuthenticationFailed(result.Error);
    }

    private void OnOnlineMatchStarted(WebSocketMessage message)
    {
        MatchSessionData = message as MatchSessionData;
        OnlineMatchStarted?.Invoke(MatchSessionData);
    }
    
    private void OnOnlineMatchEnded(WebSocketMessage message)
    {
        MatchSessionData = null;
        OnlineMatchEnded?.Invoke();
    }
    
    #endregion
    
    #region API
    
    public void StartFriendMatch(string friendUsername)
    {
        var request = new OnlineFriendMatchRequest
        {
            FriendUsername = friendUsername
        };
        hubConnection.Send(request);
    }

    public void StartRandomMatch()
    {
        var request = new OnlineRandomMatchRequest();
        hubConnection.Send(request);
    }
    
    public void StartRankedMatch(int stake)
    {
        var request = new OnlineRankedMatchRequest
        {
            Stake = stake
        };
        hubConnection.Send(request);
    }

    public void CancelMatchRequest()
    {
        var request = new CancelMatchRequest();
        hubConnection.Send(request);
    }
    
    public void EndMatch()
    {
        var request = new EndOnlineMatchRequest();
        hubConnection.Send(request);
    }
    
    #endregion
}