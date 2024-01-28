using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnimatorSequencerExtensions.Extensions;
using BrunoMikoski.AnimationSequencer;
using UnityEngine;
using Utils;
using Utils.Extensions;

namespace MiniGames
{
    public class MiniGamesController : MonoBehaviour
    {
        [SerializeField] private List<MiniGame> _miniGames;
        [SerializeField] private AnimationSequencerController _fadeInTransition;
        [SerializeField] private AnimationSequencerController _fadeOutTransition;

        private readonly PrefabsToInstanceMap _miniGamesInstances = new();
        private MiniGame _currentMiniGame;
        private MiniGame _currentMiniGamePrefab;

        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            _cts = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }

            _cts.Dispose();
            _cts = null;
        }
        
        private void Start()
        {
            ChangeMiniGame(null);
        }

        private MiniGame GetRandomMiniGamePrefab()
        {
            var miniGames = _miniGames.Where(game => game != _currentMiniGamePrefab).ToList();
            return miniGames.GetRandom();
        }
        
        private void InstantiateMiniGame()
        {
            var miniGamePrefab = GetRandomMiniGamePrefab();
            var instance = _miniGamesInstances.GetOrCreateInstance<MiniGame>(miniGamePrefab);
            instance.gameObject.SetActive(true);
            _currentMiniGame = instance;
            _currentMiniGamePrefab = miniGamePrefab;
        }

        private void DisposeMiniGame()
        {
            _currentMiniGame.gameObject.SetActive(false);
        }

        private async void ChangeMiniGame(MiniGame miniGame)
        {
            await PlayAnimation(_fadeInTransition);
            
            if (_currentMiniGame != null && _currentMiniGame == miniGame)
            {
                DisposeMiniGame();
            }

            await AsyncUtils.Utils.Delay(0.1f, _cts.Token);
            
            InstantiateMiniGame();
            
            await PlayAnimation(_fadeOutTransition);
        }

        private async Task PlayAnimation(AnimationSequencerController animation)
        {
            animation.Play();
            await animation.PlayingSequence.AsyncWaitForCompletion(_cts.Token);
        }

        public void NotifyMiniGameEnd(MiniGame miniGame, bool didWin)
        {
            if (didWin)
            {
                NotifyWin(miniGame);
            }
            else
            {
                NotifyLose(miniGame);
            }
        }
        
        public void NotifyWin(MiniGame miniGame)
        {
            ChangeMiniGame(miniGame);
            Debug.Log("Win");
        }

        public void NotifyLose(MiniGame miniGame)
        {
           ChangeMiniGame(miniGame);
           Debug.Log("Lose");
        }
    }
}