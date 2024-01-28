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
using Object = UnityEngine.Object;

namespace MiniGames
{
    public class MiniGamesController : MonoBehaviour
    {
        [SerializeField] private List<MiniGame> _miniGames;
        [SerializeField] private AnimationSequencerController _fadeInTransition;
        [SerializeField] private AnimationSequencerController _fadeOutTransition;
        [SerializeField] private Timer _timer;

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
        
        private async void Start()
        {
            await StartMiniGame();
        }

        private MiniGame GetRandomMiniGamePrefab()
        {
            var miniGames = _miniGames.Where(game => game != _currentMiniGamePrefab).ToList();
            return miniGames.GetRandom();
        }
        
        private void InstantiateMiniGame()
        {
            var miniGamePrefab = GetRandomMiniGamePrefab();
            var instance = _miniGamesInstances.GetOrCreateInstance<MiniGame>(miniGamePrefab, OnCreateMiniGame);
            instance.gameObject.SetActive(true);
            _currentMiniGame = instance;
            _currentMiniGamePrefab = miniGamePrefab;
        }

        private void OnCreateMiniGame(Object miniGame)
        {
            ((MiniGame) miniGame).transform.SetParent(transform);
        }

        private void DisposeMiniGame()
        {
            _currentMiniGame.gameObject.SetActive(false);
        }

        private async void ChangeMiniGame(MiniGame miniGame)
        {
            // stop timer
            _timer.StopTimer();
            
            await PlayAnimation(_fadeInTransition);
            
            if (_currentMiniGame != null && _currentMiniGame == miniGame)
            {
                DisposeMiniGame();
            }
            
            // wait time
            await AsyncUtils.Utils.Delay(0.1f, _cts.Token);
            
            await StartMiniGame();
        }

        private async Task StartMiniGame()
        {
            InstantiateMiniGame();
            _timer.StartTimer();

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