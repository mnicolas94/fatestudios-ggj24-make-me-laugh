using UnityEngine;

namespace MiniGames.Guajiro
{
    public class MovingBar : MonoBehaviour
    {
        // [SerializeField] private GameObject _bar;
        [SerializeField] private RectTransform _barTransform;
        [SerializeField] private float _speed;
        [SerializeField] private Transform _startPosition;
        [SerializeField] private Transform _endPosition;

        private Transform _currentTarget;
        
        public Vector3 CurrentPosition => _barTransform.position;

        private void OnEnable()
        {
            _currentTarget = _startPosition;
        }
        
        private void Update()
        {
            var reached = MoveStep(_currentTarget.position);
            if (reached)
            {
                _currentTarget = _currentTarget == _startPosition ? _endPosition : _startPosition;
            }
        }

        private bool MoveStep(Vector3 targetPosition)
        {
            var position = _barTransform.position;
            var moveDistance = _speed * Time.deltaTime;
            var currentDistance = Vector3.Distance(position, targetPosition);

            var didReach = moveDistance >= currentDistance;
            position = Vector3.MoveTowards(position, targetPosition, moveDistance);
            _barTransform.position = position;
            
            return didReach;
        }
    }
}