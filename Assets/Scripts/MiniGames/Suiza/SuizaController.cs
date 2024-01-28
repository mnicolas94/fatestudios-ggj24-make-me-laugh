using System.Threading;
using System.Threading.Tasks;
using Jnk.TinyContainer;
using SerializableCallback;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniGames.Suiza
{
    public class SuizaController : MonoBehaviour
    {
        [Header("Animations")]
        [SerializeField] private SerializableCallback<CancellationToken, Task> _jumpAnimation;
        [SerializeField] private SerializableCallback<CancellationToken, Task> _loseAnimation;

        [Header("Lose condition")]
        [SerializeField] private FloatReference _timeToWin;

        [Header("Input")]
        [SerializeField] private InputActionReference _leftInputAction;
        [SerializeField] private InputActionReference _rightInputAction;

        private bool _jumping;
        private bool _completed;

        private CancellationTokenSource _cts;
        
        private void Start()
        {
            _leftInputAction.action.Enable();
            _rightInputAction.action.Enable();
        }

        private void OnEnable()
        {
            _completed = false;
            _jumping = false;
            _leftInputAction.action.performed += Jump;
            _rightInputAction.action.performed += Jump;
            
            _cts = new CancellationTokenSource();

            WaitTimeAndWind();
        }

        private void OnDisable()
        {
            _leftInputAction.action.performed -= Jump;
            _rightInputAction.action.performed -= Jump;
            
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }

            _cts.Dispose();
            _cts = null;
        }
        
        
        private async void WaitTimeAndWind()
        {
            await AsyncUtils.Utils.Delay(_timeToWin, _cts.Token);

            if (!_completed)
            {
                _completed = true;
                // win
                var controller = TinyContainer.Global.Get<MiniGamesController>();
                var miniGame = TinyContainer.For(this).Get<MiniGame>();
                controller.NotifyWin(miniGame);
            }
        }

        private async void Jump(InputAction.CallbackContext obj)
        {
            if (_completed)
            {
                return;
            }
            
            _jumping = true;
            
            // execute animation
            await _jumpAnimation.Invoke(_cts.Token);

            _jumping = false;
        }
        
        public async void CloseJumpWindow()
        {
            if (_completed)
            {
                return;
            }
            
            if (!_jumping)
            {
                _completed = true;
                // lose
                await _loseAnimation.Invoke(_cts.Token);
                
                var controller = TinyContainer.Global.Get<MiniGamesController>();
                var miniGame = TinyContainer.For(this).Get<MiniGame>();
                controller.NotifyLose(miniGame);
            }
        }
    }
}