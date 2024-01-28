using System.Threading;
using System.Threading.Tasks;
using Jnk.TinyContainer;
using SerializableCallback;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniGames.Serrucho
{
    public class SerruchoMiniGame : MonoBehaviour
    {
        [Header("Animations")]
        [SerializeField] private SerializableCallback<CancellationToken, Task> _leftAnimation;
        [SerializeField] private SerializableCallback<CancellationToken, Task> _rightAnimation;
        [SerializeField] private float _inputDelay;
        [SerializeField] private SerializableCallback<CancellationToken, Task> _winAnimation;
        [SerializeField] private SerializableCallback<CancellationToken, Task> _loseAnimation;

        [Header("Lose condition")]
        [SerializeField] private FloatReference _timeToLose;

        [Header("Progress")]
        [SerializeField] private Transform _progressBar;
        [SerializeField] private Transform _progressMinPosition;
        [SerializeField] private Transform _progressMaxPosition;
        [SerializeField] private float _progressGoal;
        [SerializeField] private float _progressStepPerPress;
        [SerializeField] private float _progressDecreasePerSecond;

        [Header("Input")]
        [SerializeField] private InputActionReference _leftInputAction;
        [SerializeField] private InputActionReference _rightInputAction;

        private SerializableCallback<CancellationToken, Task> _currentAnimation;
        private float _currentProgress;
        private InputActionReference _lastInputPressed;
        private float _lastTimeInputPressed;
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
            _currentProgress = 0;
            _lastInputPressed = null;
            _currentAnimation = null;
            
            _leftInputAction.action.performed += OnLeftPressed;
            _rightInputAction.action.performed += OnRightPressed;
            
            _cts = new CancellationTokenSource();

            WaitTimeAndLose();
        }

        private void OnDisable()
        {
            _leftInputAction.action.performed -= OnLeftPressed;
            _rightInputAction.action.performed -= OnRightPressed;
            
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }

            _cts.Dispose();
            _cts = null;
        }

        private async void WaitTimeAndLose()
        {
            var ct = _cts.Token;
            await AsyncUtils.Utils.Delay(_timeToLose, ct);

            if (!_completed && !_cts.IsCancellationRequested)
            {
                _completed = true;
                
                // execute win animation
                if (_loseAnimation.target != null)
                {
                    await _loseAnimation.Invoke(ct);
                }
                
                // lose
                var controller = TinyContainer.Global.Get<MiniGamesController>();
                var miniGame = TinyContainer.For(this).Get<MiniGame>();
                controller.NotifyLose(miniGame);
            }
        }
        
        private async void OnInputPressed(InputActionReference input)
        {
            if (_completed)
            {
                return;
            }
            
            // start animation
            if (_currentAnimation == null)
            {
                var animation = input == _leftInputAction ? _leftAnimation : _rightAnimation;
                RunAnimation(animation);
            }
            
            // if is different, increase progress
            if (_lastInputPressed != input)
            {
                _currentProgress += _progressStepPerPress;
            }
            
            // if progress reached goal win
            if (_currentProgress >= _progressGoal)
            {
                _completed = true;
                
                // execute win animation
                if (_winAnimation.target != null)
                {
                    await _winAnimation.Invoke(_cts.Token);
                }
                
                // win
                var controller = TinyContainer.Global.Get<MiniGamesController>();
                var miniGame = TinyContainer.For(this).Get<MiniGame>();
                controller.NotifyWin(miniGame);
            }

            _currentProgress = Mathf.Clamp(_currentProgress, 0, _progressGoal);

            // track last input pressed
            _lastInputPressed = input;
            _lastTimeInputPressed = Time.time;
        }

        private async void RunAnimation(SerializableCallback<CancellationToken, Task> animation)
        {
            _currentAnimation = animation;
            await animation.Invoke(_cts.Token);
            var timeSinceInput = Time.time - _lastTimeInputPressed;
            if (timeSinceInput <= _inputDelay)
            {
                var nextAnimation = _currentAnimation == _leftAnimation ? _rightAnimation : _leftAnimation;
                RunAnimation(nextAnimation);
            }
            else
            {
                _currentAnimation = null;
            }
        }

        private void OnLeftPressed(InputAction.CallbackContext ctx)
        {
            OnInputPressed(_leftInputAction);
        }
        
        private void OnRightPressed(InputAction.CallbackContext ctx)
        {
            OnInputPressed(_rightInputAction);
        }

        private void Update()
        {
            // decrease progress
            _currentProgress -= _progressDecreasePerSecond * Time.deltaTime;

            // set progress bar position
            var normalizedProgress = _currentProgress / _progressGoal;
            _progressBar.position = Vector3.Lerp(_progressMinPosition.position, _progressMaxPosition.position,
                normalizedProgress);
        }
    }
}