using System;
using System.Threading;
using System.Threading.Tasks;
using Core;

public class OfflineMatchSession : IMatchSession
{
    //EVENTS
    public event Action Ended;
    
    //PROPERTIES
    public bool OnlineMatch => false;
    
    private readonly int difficulty;
    private readonly string[] difficulties = { "Easy", "Medium", "Hard" };
    public OfflineMatchSession(int difficulty)
    {
        this.difficulty = difficulty;
    }
    
    public Task<MatchSessionData> StartMatchSession(CancellationToken ct)
    {
        // here we initialize ai for specified difficulty
        return Task.FromResult(new MatchSessionData()
        {
            WhiteUsername = "Myself",
            BlackUsername = $"AI ({difficulties[difficulty]})",
        });
    }
    
    public Task EndMatchSession(CancellationToken ct)
    {
        // here we can clean up ai resources if needed
        return Task.CompletedTask;
    }
}