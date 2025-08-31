using System;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Match;

public struct TurnStartData
{
    public int TimeLimit;
    public Team Team;
}

public interface IMatchSession
{
    //EVENTS
    event Action Ended;
    
    //PROPERTIES
    bool OnlineMatch { get; }
    
    Task<MatchSessionData> StartMatchSession(CancellationToken ct);
    Task EndMatchSession(CancellationToken ct);
}