using System;
using System.Threading;
using System.Threading.Tasks;
using Jnk.TinyContainer;
using SerializableCallback;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Attributes;

namespace MiniGames.Guajiro
{
    public class PrecisionMiniGame : MonoBehaviour
    {
        [SerializeField, AutoProperty] private MovingBar _movingBar;
        [SerializeField] private RectTransform _winBounds;
        [SerializeField] private InputActionReference _leftInputActionReference;
        [SerializeField] private InputActionReference _rightInputActionReference;
        [SerializeField] private FloatReference _endTime;
        
        [SerializeField] private SerializableCallback<CancellationToken, Task> _winAnimation;
        [SerializeField] private SerializableCallback<CancellationToken, Task> _loseAnimation;

        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            _cts = new CancellationTokenSource();
            
            _movingBar.enabled = true;

            StartMiniGameLoop();
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

        private async void StartMiniGameLoop()
        {
            var ct = _cts.Token;

            await AsyncUtils.Utils.WaitFirstToFinish(ct, new (Func<CancellationToken, Task>, Action<Task>)[]
            {
                (linkedCt => AsyncUtils.Utils.WaitForInputAction(_leftInputActionReference.action.Clone(), linkedCt),
                    null),
                (linkedCt => AsyncUtils.Utils.WaitForInputAction(_rightInputActionReference.action.Clone(), linkedCt),
                    null),
                (linkedCt => AsyncUtils.Utils.Delay(_endTime, linkedCt), null),
            });
            
            _movingBar.enabled = false;

            if (!ct.IsCancellationRequested)
            {
                var didWin = IsPointInsideRectTransform(_movingBar.CurrentPosition, _winBounds);
                var controller = TinyContainer.Global.Get<MiniGamesController>();
                var miniGame = TinyContainer.For(this).Get<MiniGame>();
                
                var animation = didWin ? _winAnimation : _loseAnimation;
                if (animation.target != null)
                {
                    await animation.Invoke(ct);
                }
                
                controller.NotifyMiniGameEnd(miniGame, didWin);
            }
        }
        
        private bool IsPointInsideRectTransform(Vector2 point, RectTransform rt)
        {
            // Get the rectangular bounding box of your UI element
            Rect rect = rt.rect;

            // Check to see if the point is in the calculated bounds
            if (point.x >= rect.xMin + rt.position.x &&
                point.x <= rect.xMax + rt.position.x &&
                point.y >= rect.yMin + rt.position.y &&
                point.y <= rect.yMax + rt.position.y)
            {
                return true;
            }
            
            return false;
        }
    }
}