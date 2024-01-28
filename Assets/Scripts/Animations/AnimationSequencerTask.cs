using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnimatorSequencerExtensions.Extensions;
using BrunoMikoski.AnimationSequencer;
using UnityEngine;

namespace Animations
{
    public class AnimationSequencerTask : MonoBehaviour
    {
        [SerializeField] private AnimationSequencerController _animation;

        public async Task Run(CancellationToken ct)
        {
            _animation.Play();
            await _animation.PlayingSequence.AsyncWaitForCompletion(ct);
        }
    }
}