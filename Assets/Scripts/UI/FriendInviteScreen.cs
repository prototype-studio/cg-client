using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace UI
{
    public class FriendInviteScreen : UIScreen
    {
        [SerializeField] private TMP_InputField friendUsernameField;
        [SerializeField] private UserSession userSession;
        [SerializeField] private ProgressScreen progressScreen;
        [SerializeField] private MatchScreen matchScreen;

        public void Invite()
        {
            var friendUserName = friendUsernameField.text;
            userSession.StartFriendMatch(friendUserName);
            progressScreen.SetProgressText($"Waiting for {friendUserName} to join...", Color.white);
            _ = progressScreen.Run(WaitForMatchStart, Started, StartFailed, true);
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
        
        private void Started()
        {
            var onlineMatchSession = new OnlineMatchSession(userSession);
            matchScreen.Initialize(onlineMatchSession);
        }
        
        private void StartFailed(string error)
        {
            userSession.CancelMatchRequest();
            Show();
            //Debug.LogError("Match start failed: " + error);
        }
    }
}