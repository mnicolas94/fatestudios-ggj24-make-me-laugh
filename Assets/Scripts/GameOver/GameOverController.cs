using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace GameOver
{
    public class GameOverController : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private List<Sprite> _sprites;

        private void OnEnable()
        {
            _image.sprite = _sprites.GetRandom();
        }
    }
}