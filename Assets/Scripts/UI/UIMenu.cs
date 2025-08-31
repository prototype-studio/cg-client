using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMenu : MonoBehaviour
    {
        //SERIALIZED
        [SerializeField] private Image backgroundImage;
        [SerializeField] private string initialScreen;
        
        //PRIVATES
        private readonly Dictionary<string, UIScreen> screens = new();
        
        private void Awake()
        {
            var screenComponents = GetComponentsInChildren<UIScreen>(true);
            foreach (var screen in screenComponents)
            {
                screens.Add(screen.name, screen);
            }
            ShowScreen(initialScreen);
        }
        
        public void ShowScreen(string screenName)
        {
            foreach (var screen in screens.Values)
            {
                screen.gameObject.SetActive(screen.name == screenName);
            }

            if (screens.TryGetValue(screenName, out var selectedScreen))
            {
                ShowBackground(selectedScreen.UseBackground);
            }
            else
            {
                ShowBackground(false);
            }
        }

        public void ShowBackground(bool show)
        {
            if (backgroundImage)
            {
                backgroundImage.enabled = show;
            }
        }
    }
}
