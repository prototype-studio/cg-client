using UnityEngine;

namespace UI
{
    public class UIScreen : MonoBehaviour
    {
        //SERIALIZED
        [SerializeField] private bool useBackground = true;
        
        //PROPERTIES
        public UIMenu Menu
        {
            get
            {
                if(this == null) return null;
                if (!menu)
                {
                    menu = GetComponentInParent<UIMenu>();
                }
                return menu;
            }
        }
        public bool UseBackground => useBackground;
        
        //PRIVATES
        private UIMenu menu;

        public void Show()
        {
            Menu?.ShowScreen(gameObject.name);
        }
    }
}