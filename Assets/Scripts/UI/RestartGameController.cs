using Jnk.TinyContainer;
using MiniGames;
using UnityEngine;

namespace UI
{
    public class RestartGameController : MonoBehaviour
    {
        public void RestartGame()
        {
            var controller = TinyContainer.Global.Get<MiniGamesController>();
            controller.RestartGames();
        }
    }
}