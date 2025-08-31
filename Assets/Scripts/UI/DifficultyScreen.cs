using UnityEngine;

namespace UI
{
    public class DifficultyScreen : UIScreen
    {
        //SERIALIZED
        [SerializeField] private MatchScreen matchScreen;
        
        public void StartMatch(int difficulty)
        {
            var matchSession = new OfflineMatchSession(difficulty);
            matchScreen.Initialize(matchSession);
        }
    }
}