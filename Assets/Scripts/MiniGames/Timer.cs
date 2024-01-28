using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityAtoms.BaseAtoms;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utils.Tweening;

namespace MiniGames
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private FloatReference _time;

        [SerializeField] private List<Sprite> _sprites;
        [SerializeField] private Image _image;

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
        
        public async void StartTimer()
        {
            await TweeningUtils.TweenTimeAsync(time =>
            {
                int index = (int) (_sprites.Count * time);
                var sprite = _sprites[index];
                _image.sprite = sprite;
            }, _time, Curves.Linear, _cts.Token);
        }

        public async void StopTimer()
        {
            OnDisable();
            OnEnable();
        }

#if UNITY_EDITOR
        [ContextMenu(nameof(LoadSprites))]
        public void LoadSprites()
        {
            var folder = "Assets/Sprites/TiempoSerpiente";
            var guids = AssetDatabase.FindAssets("t:Sprite", new []{ folder });
            var sprites = guids.Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<Sprite>);
            Debug.Log("assets.Count = " + guids.Length);
            
            _sprites.Clear();
            _sprites.AddRange(sprites);
            
            EditorUtility.SetDirty(this);
        }
#endif
    }
}