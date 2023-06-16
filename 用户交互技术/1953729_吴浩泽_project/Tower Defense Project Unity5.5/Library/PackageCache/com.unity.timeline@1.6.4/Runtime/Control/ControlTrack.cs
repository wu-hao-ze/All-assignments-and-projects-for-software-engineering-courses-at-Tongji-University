using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
    /// <summary>
    /// A Track whose clips control time-related elements on a GameObject.
    /// </summary>
    [TrackClipType(typeof(ControlPlayableAsset), false)]
    [ExcludeFromPreset]
    [TimelineHelpURL(typeof(ControlTrack))]
    public class ControlTrack : TrackAsset
    {
#if UNITY_EDITOR
        private static readonly HashSet<PlayableDirector> s_ProcessedDirectors = new HashSet<PlayableDirector>();

        /// <inheritdoc/>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            if (director == null)
                return;

            // avoid recursion
            if (s_ProcessedDirectors.Contains(director))
                return;

            s_ProcessedDirectors.Add(director);

            var particlesToPreview = new HashSet<ParticleSystem>();
            var activationToPreview = new HashSet<GameObject>();
            var timeControlToPreview = new HashSet<MonoBehaviour>();
            var subDirectorsToPreview = new HashSet<PlayableDirector>();

            foreach (var clip in GetClips())
            {
                var controlPlayableAsset = clip.asset as ControlPlayableAsset;
                if (controlPlayableAsset == null)
                    continue;

                var gameObject = controlPlayableAsset.sourceGameObject.Resolve(director);
                if (gameObject == null)
                    continue;

                if (controlPlayableAsset.updateParticle)
                    particlesToPreview.UnionWith(gameObject.GetComponentsInChildren<ParticleSystem>(true));
                if (controlPlayableAsset.active)
                    activationToPreview.Add(gameObject);
                if (controlPlayableAsset.updateITimeControl)
                    timeControlToPreview.UnionWith(ControlPlayableAsset.GetControlableScripts(gameObject));
                if (controlPlayableAsset.updateDirector)
                    subDirectorsToPreview.UnionWith(controlPlayableAsset.GetComponent<PlayableDirector>(gameObject));
            }

            ControlPlayableAsset.PreviewParticles(driver, particlesToPreview);
            ControlPlayableAsset.PreviewActivation(driver, activationToPreview);
            ControlPlayableAsset.PreviewTimeControl(driver, director, timeControlToPreview);
            ControlPlayableAsset.PreviewDirectors(driver, subDirectorsToPreview);

            s_ProcessedDirectors.Remove(director);

            particlesToPreview.Clear();
            activationToPreview.Clear();
            timeControlToPreview.Clear();
            subDirectorsToPreview.Clear();
        }

#endif
    }
}
