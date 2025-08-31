using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MatchScreen : UIScreen
    {
        //SERIALIZED
        [SerializeField] private UIScreen offlineEndScreen;
        [SerializeField] private UIScreen onlineEndScreen;
        [SerializeField] private MatchArena matchArena;
        [SerializeField] private ProgressScreen progressScreen;
        [SerializeField] private TextMeshProUGUI whiteUsernameText;
        [SerializeField] private TextMeshProUGUI blackUsernameText;
        
        //PRIVATES
        private IMatchSession matchSession;
        
        public void Initialize(IMatchSession matchSession)
        {
            this.matchSession = matchSession;
            this.matchSession.Ended += Ended;
            progressScreen.SetProgressText("Starting match...", Color.white);
            progressScreen.Run(matchSession.StartMatchSession, Started, StartFailed);
        }
        
        private void Started(MatchSessionData matchSessionData)
        {
            matchArena.gameObject.SetActive(true);
            matchArena.CreateBoard();
            whiteUsernameText.text = matchSessionData.WhiteUsername;
            blackUsernameText.text = matchSessionData.BlackUsername;
            Show();
        }
        
        private void StartFailed(string error)
        {
            Debug.LogError("Match start failed: " + error);
            matchArena?.gameObject?.SetActive(false);
            if(!matchSession.OnlineMatch) offlineEndScreen?.Show();
            else onlineEndScreen?.Show();
        }

        public void EndMatch()
        {
            progressScreen.SetProgressText("Ending match...", Color.white);
            progressScreen.Run(matchSession.EndMatchSession, Ended, EndFailed);
        }

        private void Ended()
        {
            matchSession.Ended -= Ended;
            matchArena.DestroyBoard();
            matchArena.gameObject.SetActive(false);
            if(!matchSession.OnlineMatch) offlineEndScreen.Show();
            else onlineEndScreen.Show();
        }

        private void EndFailed(string error)
        {
            Debug.LogError("Match start failed: " + error);
        }
    }
}