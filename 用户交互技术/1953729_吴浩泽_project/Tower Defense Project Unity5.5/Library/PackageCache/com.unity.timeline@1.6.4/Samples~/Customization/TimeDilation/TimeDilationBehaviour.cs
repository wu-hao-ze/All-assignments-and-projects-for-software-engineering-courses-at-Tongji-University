using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.Samples
{
    // Runtime representation of a time dilation clip.
    // The Serializable attribute is required to be animated by timeline, and used as a template.
    [Serializable]
    public class TimeDilationBehaviour : PlayableBehaviour
    {
        [Tooltip("Time.timeScale replacement value.")]
        public float timeScale = 1f;
    }
}
