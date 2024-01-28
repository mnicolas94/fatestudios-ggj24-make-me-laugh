using UnityEngine;

namespace Debugging
{
    public class DebugRectPosition : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;

        private void Update()
        {
            UnityEngine.Debug.Log("_rect.position = " + _rect.anchoredPosition);
        }
    }
}