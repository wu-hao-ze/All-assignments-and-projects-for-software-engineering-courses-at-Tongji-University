using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
    /// <summary>
    /// Playable that synchronizes a particle system simulation.
    /// </summary>
    public class ParticleControlPlayable : PlayableBehaviour
    {
        const float kUnsetTime = float.MaxValue;
        float m_LastPlayableTime = kUnsetTime;
        float m_LastParticleTime = kUnsetTime;
        uint m_RandomSeed = 1;


        /// <summary>
        /// Creates a Playable with a ParticleControlPlayable behaviour attached
        /// </summary>
        /// <param name="graph">The PlayableGraph to inject the Playable into.</param>
        /// <param name="component">The particle systtem to control</param>
        /// <param name="randomSeed">A random seed to use for particle simulation</param>
        /// <returns>Returns the created Playable.</returns>
        public static ScriptPlayable<ParticleControlPlayable> Create(PlayableGraph graph, ParticleSystem component, uint randomSeed)
        {
            if (component == null)
                return ScriptPlayable<ParticleControlPlayable>.Null;

            var handle = ScriptPlayable<ParticleControlPlayable>.Create(graph);
            handle.GetBehaviour().Initialize(component, randomSeed);
            return handle;
        }

        /// <summary>
        /// The particle system to control
        /// </summary>
        public ParticleSystem particleSystem { get; private set; }

        /// <summary>
        /// Initializes the behaviour with a particle system and random seed.
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="randomSeed"></param>
        public void Initialize(ParticleSystem ps, uint randomSeed)
        {
            m_RandomSeed = Math.Max(1, randomSeed);
            particleSystem = ps;
            SetRandomSeed(particleSystem, m_RandomSeed);

#if UNITY_EDITOR
            if (!Application.isPlaying && UnityEditor.PrefabUtility.IsPartOfPrefabInstance(ps))
                UnityEditor.PrefabUtility.prefabInstanceUpdated += OnPrefabUpdated;
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// This function is called when the Playable that owns the PlayableBehaviour is destroyed.
        /// </summary>
        /// <param name="playable">The playable this behaviour is attached to.</param>
        public override void OnPlayableDestroy(Playable playable)
        {
            if (!Application.isPlaying)
                UnityEditor.PrefabUtility.prefabInstanceUpdated -= OnPrefabUpdated;
        }

        void OnPrefabUpdated(GameObject go)
        {
            // When the instance is updated from, this will cause the next evaluate to resimulate.
            if (UnityEditor.PrefabUtility.GetRootGameObject(particleSystem) == go)
                m_LastPlayableTime = kUnsetTime;
        }

#endif

        static void SetRandomSeed(ParticleSystem particleSystem, uint randomSeed)
        {
            if (particleSystem == null)
                return;

            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (particleSystem.useAutoRandomSeed)
            {
                particleSystem.useAutoRandomSeed = false;
                particleSystem.randomSeed = randomSeed;
            }

            for (int i = 0; i < particleSystem.subEmitters.subEmittersCount; i++)
            {
                SetRandomSeed(particleSystem.subEmitters.GetSubEmitterSystem(i), ++randomSeed);
            }
        }

        /// <summary>
        /// This function is called during the PrepareFrame phase of the PlayableGraph.
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="data">A FrameData structure that contains information about the current frame context.</param>
        public override void PrepareFrame(Playable playable, FrameData data)
        {
            if (particleSystem == null || !particleSystem.gameObject.activeInHierarchy)
            {
                // case 1212943
                m_LastPlayableTime = kUnsetTime;
                return;
            }

            var time = (float)playable.GetTime();
            var particleTime = particleSystem.time;

            // if particle system time has changed externally, a re-sync is needed
            if (m_LastPlayableTime > time || !Mathf.Approximately(particleTime, m_LastParticleTime))
                Simulate(time, true);
            else if (m_LastPlayableTime < time)
                Simulate(time - m_LastPlayableTime, false);

            m_LastPlayableTime = time;
            m_LastParticleTime = particleSystem.time;
        }

        /// <summary>
        /// This function is called when the Playable play state is changed to Playables.PlayState.Playing.
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            m_LastPlayableTime = kUnsetTime;
        }

        /// <summary>
        /// This function is called when the Playable play state is changed to PlayState.Paused.
        /// </summary>
        /// <param name="playable">The playable this behaviour is attached to.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            m_LastPlayableTime = kUnsetTime;
        }

        private void Simulate(float time, bool restart)
        {
            const bool withChildren = false;
            const bool fixedTimeStep = false;
            float maxTime = Time.maximumDeltaTime;

            if (restart)
                particleSystem.Simulate(0, withChildren, true, fixedTimeStep);

            // simulating by too large a time-step causes sub-emitters not to work, and loops not to
            // simulate correctly
            while (time > maxTime)
            {
                particleSystem.Simulate(maxTime, withChildren, false, fixedTimeStep);
                time -= maxTime;
            }

            if (time > 0)
                particleSystem.Simulate(time, withChildren, false, fixedTimeStep);
        }
    }
}
