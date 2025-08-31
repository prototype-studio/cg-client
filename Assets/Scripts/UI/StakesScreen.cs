using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public class StakesScreen : UIScreen
    {
        [SerializeField] private MatchScreen matchScreen;
        [SerializeField] private ProgressScreen progressScreen;
        [SerializeField] private UserSession userSession;
        
        public void Play(int stake)
        {
            userSession.StartRankedMatch(stake);
            progressScreen.SetProgressText($"Waiting for match...", Color.white);
            _ = progressScreen.Run(WaitForMatchStart, MatchStarted, MatchStartFailed, true);
        }
        
        private async Task WaitForMatchStart(CancellationToken ct)
        {
            while (userSession.MatchSessionData == null)
            {
                if(!userSession.Connected) throw new System.Exception("Disconnected from server.");
                await Task.Yield();
                ct.ThrowIfCancellationRequested();
            }
        }
        
        private void MatchStarted()
        {
            var onlineMatchSession = new OnlineMatchSession(userSession);
            matchScreen.Initialize(onlineMatchSession);
        }
        
        private void MatchStartFailed(string error)
        {
            userSession.CancelMatchRequest();
            Show();
        }
    }
}