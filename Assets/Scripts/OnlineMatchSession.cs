using System;
using System.Threading;
using System.Threading.Tasks;
using Core;

public class OnlineMatchSession : IMatchSession
{
    //EVENTS
    public event Action Ended;
    
    //PROPERTIES
    public bool OnlineMatch => true;
    
    //PRIVATES
    private UserSession userSession;

    public OnlineMatchSession(UserSession userSession)
    {
        this.userSession = userSession;
        userSession.OnlineMatchEnded += MatchEnded;
    }
    
    public Task<MatchSessionData> StartMatchSession(CancellationToken ct)
    {
        return Task.FromResult(userSession.MatchSessionData);
    }

    public async Task EndMatchSession(CancellationToken ct)
    {
        userSession.EndMatch();
        while (userSession.MatchSessionData != null)
        {
            await Task.Yield();
        }
    }

    private void MatchEnded()
    {
        userSession.OnlineMatchEnded -= MatchEnded;
        Ended?.Invoke();
    }
}