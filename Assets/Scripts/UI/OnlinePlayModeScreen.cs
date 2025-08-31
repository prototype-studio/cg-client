using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public class OnlinePlayModeScreen : UIScreen
    {
        [SerializeField] private MatchScreen matchScreen;
        [SerializeField] private FriendInviteScreen friendInviteScreen;
        [SerializeField] private StakesScreen stakesScreen;
        [SerializeField] private ProgressScreen progressScreen;
        [SerializeField] private UserSession userSession;
        
        public void PlayWithFriend()
        {
            friendInviteScreen.Show();
        }
        
        public void PlayStakes()
        {
            stakesScreen.Show();
        }
        
        #region RANDOM
        
        public void PlayRandom()
        {
            userSession.StartRandomMatch();
            progressScreen.SetProgressText($"Waiting for match...", Color.white);
            _ = progressScreen.Run(WaitForRandomMatchStart, RandomMatchStarted, RandomMatchStartFailed, true);
        }
        
        private async Task WaitForRandomMatchStart(CancellationToken ct)
        {
            while (userSession.MatchSessionData == null)
            {
                if(!userSession.Connected) throw new System.Exception("Disconnected from server.");
                await Task.Yield();
                ct.ThrowIfCancellationRequested();
            }
        }
        
        private void RandomMatchStarted()
        {
            var onlineMatchSession = new OnlineMatchSession(userSession);
            matchScreen.Initialize(onlineMatchSession);
        }
        
        private void RandomMatchStartFailed(string error)
        {
            userSession.CancelMatchRequest();
            Show();
            //Debug.LogError("Match start failed: " + error);
        }
        
        #endregion
    }
}