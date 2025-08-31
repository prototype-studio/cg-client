using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ProgressScreen : UIScreen
    {
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private GameObject cancelButton;
        
        //PRIVATES
        private CancellationTokenSource cts;

        public void SetProgressText(string newText, Color newTextColor)
        {
            progressText.text = newText;
            progressText.color = newTextColor;
        }

        public void Stop()
        {
            SetProgressText("Stopping...", Color.white);
            cts?.Cancel();
            cts = null;
        }

        public async Task Run(Func<CancellationToken, Task> task, Action onSuccess = null, Action<string> onFail = null, bool cancellable = false)
        {
            cancelButton.SetActive(cancellable);
            cts = new CancellationTokenSource();
            Show();
            try
            {
                await task(cts.Token);
                if (this == null) return;
                onSuccess?.Invoke();
            }
            catch (Exception e)
            {
                if (this == null) return;
                SetProgressText(e.Message, Color.red);
                onFail?.Invoke(e.Message);
            }
        }
        
        public async Task Run<T>(Func<CancellationToken, Task<T>> task, Action<T> onSuccess = null, Action<string> onFail = null, bool cancellable = false)
        {
            cancelButton.SetActive(cancellable);
            cts = new CancellationTokenSource();
            try
            {
                Show();
                Debug.Log("Task()");
                var result = await task(cts.Token);
                if (this == null) return;
                onSuccess?.Invoke(result);
            }
            catch (Exception e)
            {
                if (this == null) return;
                SetProgressText(e.Message, Color.red);
                onFail?.Invoke(e.Message);
            }
        }
    }
}