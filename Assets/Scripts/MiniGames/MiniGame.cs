using Jnk.TinyContainer;
using UnityEngine;

namespace MiniGames
{
    public class MiniGame : MonoBehaviour
    {
        public void WinGame()
        {
            var controller = TinyContainer.Global.Get<MiniGamesController>();
            controller.NotifyWin(this);
        }
        
        public void LoseGame()
        {
            var controller = TinyContainer.Global.Get<MiniGamesController>();
            controller.NotifyLose(this);
        }
    }
}