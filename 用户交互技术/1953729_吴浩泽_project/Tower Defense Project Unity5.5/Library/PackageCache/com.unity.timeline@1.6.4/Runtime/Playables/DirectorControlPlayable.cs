using System;
using UnityEngine;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
    /// <summary>
    /// Playable Behaviour used to control a PlayableDirector.
    /// </summary>
    /// <remarks>
    /// This playable is used to control other PlayableDirector components from a Timeline sequence.
    /// </remarks>
    public class DirectorControlPlayable : PlayableBehaviour
    {
        /// <summary>
        /// The PlayableDirector being controlled by this PlayableBehaviour
        /// </summary>
        public PlayableDirector director;

        private bool m_SyncTime = false;

        private double m_AssetDuration = double.MaxValue;

        /// <summary>
        /// Creates a Playable with a DirectorControlPlayable attached
        /// </summary>
        /// <param name="graph">The graph to inject the playable into</param>
        /// <param name="director">The director to control</param>
        /// <returns>Returns a Playable with a DirectorControlPlayable attached</returns>
        public static ScriptPlayable<DirectorControlPlayable> Create(PlayableGraph graph, PlayableDirector director)
        {
            if (director == null)
                return ScriptPlayable<DirectorControlPlayable>.Null;

            var handle = ScriptPlayable<DirectorControlPlayable>.Create(graph);
            handle.GetBehaviour().director = director;

#if UNITY_EDITOR
            if (!Application.isPlaying && UnityEditor.PrefabUtility.IsPartOfPrefabInstance(director))
                UnityEditor.PrefabUtility.prefabInstanceUpdated += handle.GetBehaviour().OnPrefabUpdated;
#endif

            return handle;
        }

        /// <summary>
        /// This function is called when this PlayableBehaviour is destroyed.
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        public override void OnPlayableDestroy(Playable playable)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.PrefabUtility.prefabInstanceUpdated -= OnPrefabUpdated;
#endif
            if (director != null && director.playableAsset != null)
                director.Stop();
        }

        /// <summary>
        /// This function is called during the PrepareFrame phase of the PlayableGraph.
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            if (director == null || !director.isActiveAndEnabled || director.playableAsset == null)
                return;

            // resync the time on an evaluate or a time jump (caused by loops, or some setTime calls)
            m_SyncTime |= (info.evaluationType == FrameData.EvaluationType.Evaluate) ||
                DetectDiscontinuity(playable, info);

            SyncSpeed(info.effectiveSpeed);
            SyncStart(playable.GetGraph(), playable.GetTime());
#if !UNITY_2021_2_OR_NEWER
            SyncStop(playable.GetGraph(), playable.GetTime());
#endif
        }

        /// <summary>
        /// This function is called when the Playable play state is changed to Playables.PlayState.Playing.
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            m_SyncTime = true;

            if (director != null && director.playableAsset != null)
                m_AssetDuration = director.playableAsset.duration;
        }

        /// <summary>
        /// This function is called when the Playable play state is changed to PlayState.Paused.
        /// </summary>
        /// <param name="playable">The playable this behaviour is attached to.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (director != null && director.playableAsset != null)
            {
                if (info.effectivePlayState == PlayState.Playing) // graph was paused
                    director.Pause();
                else
                    director.Stop();
            }
        }

        /// <summary>
        /// This function is called during the ProcessFrame phase of the PlayableGraph.
        /// </summary>
        /// <param name="playable">The playable this behaviour is attached to.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        /// <param name="playerData">unused</param>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (director == null || !director.isActiveAndEnabled || director.playableAsset == null)
                return;

            if (m_SyncTime || DetectOutOfSync(playable))
            {
                UpdateTime(playable);
                if (director.playableGraph.IsValid())
                {
                    director.playableGraph.Evaluate();
#if TIMELINE_FRAMEACCURATE
                    director.playableGraph.SynchronizeEvaluation(playable.GetGraph());
#endif
                }
                else
                {
                    director.Evaluate();
                }
            }

            m_SyncTime = false;
#if UNITY_2021_2_OR_NEWER
            SyncStop(playable.GetGraph(), playable.GetTime());
#endif
        }

#if UNITY_EDITOR
        void OnPrefabUpdated(GameObject go)
        {
            // When the prefab asset is updated, we rebuild the graph to reflect the changes in editor
            if (UnityEditor.PrefabUtility.GetRootGameObject(director) == go)
                director.RebuildGraph();
        }

#endif

        void SyncSpeed(double speed)
        {
            if (director.playableGraph.IsValid())
            {
                int roots = director.playableGraph.GetRootPlayableCount();
                for (int i = 0; i < roots; i++)
                {
                    var rootPlayable = director.playableGraph.GetRootPlayable(i);
                    if (rootPlayable.IsValid())
                    {
                        rootPlayable.SetSpeed(speed);
                    }
                }
            }
        }

        void SyncStart(PlayableGraph graph, double time)
        {
            if (director.state == PlayState.Playing
                || !graph.IsPlaying()
                || (director.extrapolationMode == DirectorWrapMode.None && time > m_AssetDuration))
                return;
#if TIMELINE_FRAMEACCURATE
            if (graph.IsMatchFrameRateEnabled())
                director.Play(graph.GetFrameRate());
            else
                director.Play();
#else
            director.Play();
#endif
        }

        void SyncStop(PlayableGraph graph, double time)
        {
            if (director.state == PlayState.Paused)
                return;

            bool expectedFinished = director.extrapolationMode == DirectorWrapMode.None && time > m_AssetDuration;
            if (expectedFinished || !graph.IsPlaying())
                director.Pause();
        }

        bool DetectDiscontinuity(Playable playable, FrameData info)
        {
            return Math.Abs(playable.GetTime() - playable.GetPreviousTime() - info.m_DeltaTime * info.m_EffectiveSpeed) > DiscreteTime.tickValue;
        }

        bool DetectOutOfSync(Playable playable)
        {
            double expectedTime = playable.GetTime();
            if (playable.GetTime() >= m_AssetDuration)
            {
                switch (director.extrapolationMode)
                {
                    case DirectorWrapMode.None:
                        expectedTime = m_AssetDuration;
                        break;
                    case DirectorWrapMode.Hold:
                        expectedTime = m_AssetDuration;
                        break;
                    case DirectorWrapMode.Loop:
                        expectedTime %= m_AssetDuration;
                        break;
                }
            }

            if (!Mathf.Approximately((float)expectedTime, (float)director.time))
            {
#if UNITY_EDITOR
                double lastDelta = playable.GetTime() - playable.GetPreviousTime();
                if (UnityEditor.Unsupported.IsDeveloperBuild())
                    Debug.LogWarningFormat("Internal Warning - Control track desync detected on {2} ({0:F10} vs {1:F10} with delta {3:F10}). Time will be resynchronized. Known to happen with nested control tracks", playable.GetTime(), director.time, director.name, lastDelta);
#endif
                return true;
            }
            return false;
        }

        // We need to handle loop modes explicitly since we are setting the time directly
        void UpdateTime(Playable playable)
        {
            double duration = Math.Max(0.1, director.playableAsset.duration);
            switch (director.extrapolationMode)
            {
                case DirectorWrapMode.Hold:
                    director.time = Math.Min(duration, Math.Max(0, playable.GetTime()));
                    break;
                case DirectorWrapMode.Loop:
                    director.time = Math.Max(0, playable.GetTime() % duration);
                    break;
                case DirectorWrapMode.None:
                    director.time = Math.Min(duration, Math.Max(0, playable.GetTime()));
                    break;
            }
        }
    }
}
