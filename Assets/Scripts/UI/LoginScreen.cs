using System.Threading;
using System.Threading.Tasks;
using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LoginScreen : UIScreen
    {
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private ProgressScreen progressScreen;
        [SerializeField] private UserSession userSession;
        [SerializeField] private TextMeshProUGUI infoText;
        
        public void Login()
        {
            progressScreen.SetProgressText("Connecting...", Color.white);
            progressScreen.Run(PerformLogin, OnLoginSuccess, OnLoginError);
        }

        public void SetInfo(string info, Color color)
        {
            infoText.text = info;
            infoText.color = color;
        }

        private async Task<WebSocketConnection> PerformLogin(CancellationToken ct)
        {
            Debug.Log("Logging in...");
            Credentials credentials = new Credentials()
            {
                Username = usernameInput.text,
                Password = passwordInput.text
            };
            string url = $"{Constants.WEB_PROTOCOL}{Constants.HOST}/login";
            var response = await WebClient.Post<Credentials, AuthenticationSuccessResponse>(url, credentials);
            ct.ThrowIfCancellationRequested();
            var token = response.Token;
            return new WebSocketConnection(token);
        }

        private void OnLoginSuccess(WebSocketConnection connection)
        {
            userSession.Init(connection);
        }
        
        private void OnLoginError(string error)
        {
            SetInfo(error, Color.red);
            Show();
        }
    }
}