using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
    /// <summary>
    /// Playable Asset that generates playables for controlling time-related elements on a GameObject.
    /// </summary>
    [Serializable]
    [NotKeyable]
    public class ControlPlayableAsset : PlayableAsset, IPropertyPreview, ITimelineClipAsset
    {
        const int k_MaxRandInt = 10000;
        static readonly List<PlayableDirector> k_EmptyDirectorsList = new List<PlayableDirector>(0);
        static readonly List<ParticleSystem> k_EmptyParticlesList = new List<ParticleSystem>(0);
        static readonly HashSet<ParticleSystem> s_SubEmitterCollector = new HashSet<ParticleSystem>();

        /// <summary>
        /// GameObject in the scene to control, or the parent of the instantiated prefab.
        /// </summary>
        [SerializeField] public ExposedReference<GameObject> sourceGameObject;

        /// <summary>
        /// Prefab object that will be instantiated.
        /// </summary>
        [SerializeField] public GameObject prefabGameObject;

        /// <summary>
        /// Indicates whether Particle Systems will be controlled.
        /// </summary>
        [SerializeField] public bool updateParticle = true;

        /// <summary>
        /// Random seed to supply particle systems that are set to use autoRandomSeed
        /// </summary>
        /// <remarks>
        /// This is used to maintain determinism when playing back in timeline. Sub emitters will be assigned incrementing random seeds to maintain determinism and distinction.
        /// </remarks>
        [SerializeField] public uint particleRandomSeed;

        /// <summary>
        /// Indicates whether playableDirectors are controlled.
        /// </summary>
        [SerializeField] public bool updateDirector = true;

        /// <summary>
        /// Indicates whether Monobehaviours implementing ITimeControl will be controlled.
        /// </summary>
        [SerializeField] public bool updateITimeControl = true;

        /// <summary>
        /// Indicates whether to search the entire hierarchy for controllable components.
        /// </summary>
        [SerializeField] public bool searchHierarchy = false;

        /// <summary>
        /// Indicate whether GameObject activation is controlled
        /// </summary>
        [SerializeField] public bool active = true;

        /// <summary>
        /// Indicates the active state of the GameObject when Timeline is stopped.
        /// </summary>
        [SerializeField] public ActivationControlPlayable.PostPlaybackState postPlayback = ActivationControlPlayable.PostPlaybackState.Revert;

        PlayableAsset m_ControlDirectorAsset;
        double m_Duration = PlayableBinding.DefaultDuration;
        bool m_SupportLoop;

        private static HashSet<PlayableDirector> s_ProcessedDirectors = new HashSet<PlayableDirector>();
        private static HashSet<GameObject> s_CreatedPrefabs = new HashSet<GameObject>();

        // does the last instance created control directors and/or particles
        internal bool controllingDirectors { get; private set; }
        internal bool controllingParticles { get; private set; }

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        public void OnEnable()
        {
            // can't be set in a constructor
            if (particleRandomSeed == 0)
                particleRandomSeed = (uint)Random.Range(1, k_MaxRandInt);
        }

        /// <summary>
        /// Returns the duration in seconds needed to play the underlying director or particle system exactly once.
        /// </summary>
        public override double duration { get { return m_Duration; } }

        /// <summary>
        /// Returns the capabilities of TimelineClips that contain a ControlPlayableAsset
        /// </summary>
        public ClipCaps clipCaps
        {
            get { return ClipCaps.ClipIn | ClipCaps.SpeedMultiplier | (m_SupportLoop ? ClipCaps.Looping : ClipCaps.None); }
        }

        /// <summary>
        /// Creates the root of a Playable subgraph to control the contents of the game object.
        /// </summary>
        /// <param name="graph">PlayableGraph that will own the playable</param>
        /// <param name="go">The GameObject that triggered the graph build</param>
        /// <returns>The root playable of the subgraph</returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            // case 989856
            if (prefabGameObject != null)
            {
                if (s_CreatedPrefabs.Contains(prefabGameObject))
                {
                    Debug.LogWarningFormat("Control Track Clip ({0}) is causing a prefab to instantiate itself recursively. Aborting further instances.", name);
                    return Playable.Create(graph);
                }
                s_CreatedPrefabs.Add(prefabGameObject);
            }

            Playable root = Playable.Null;
            var playables = new List<Playable>();

            GameObject sourceObject = sourceGameObject.Resolve(graph.GetResolver());
            if (prefabGameObject != null)
            {
                Transform parenTransform = sourceObject != null ? sourceObject.transform : null;
                var controlPlayable = PrefabControlPlayable.Create(graph, prefabGameObject, parenTransform);

                sourceObject = controlPlayable.GetBehaviour().prefabInstance;
                playables.Add(controlPlayable);
            }

            m_Duration = PlayableBinding.DefaultDuration;
            m_SupportLoop = false;

            controllingParticles = false;
            controllingDirectors = false;

            if (sourceObject != null)
            {
                var directors = updateDirector ? GetComponent<PlayableDirector>(sourceObject) : k_EmptyDirectorsList;
                var particleSystems = updateParticle ? GetControllableParticleSystems(sourceObject) : k_EmptyParticlesList;

                // update the duration and loop values (used for UI purposes) here
                // so they are tied to the latest gameObject bound
                UpdateDurationAndLoopFlag(directors, particleSystems);

                var director = go.GetComponent<PlayableDirector>();
                if (director != null)
                    m_ControlDirectorAsset = director.playableAsset;

                if (go == sourceObject && prefabGameObject == null)
                {
                    Debug.LogWarningFormat("Control Playable ({0}) is referencing the same PlayableDirector component than the one in which it is playing.", name);
                    active = false;
                    if (!searchHierarchy)
                        updateDirector = false;
                }

                if (active)
                    CreateActivationPlayable(sourceObject, graph, playables);

                if (updateDirector)
                    SearchHierarchyAndConnectDirector(directors, graph, playables, prefabGameObject != null);

                if (updateParticle)
                    SearchHierarchyAndConnectParticleSystem(particleSystems, graph, playables);

                if (updateITimeControl)
                    SearchHierarchyAndConnectControlableScripts(GetControlableScripts(sourceObject), graph, playables);

                // Connect Playables to Generic to Mixer
                root = ConnectPlayablesToMixer(graph, playables);
            }

            if (prefabGameObject != null)
                s_CreatedPrefabs.Remove(prefabGameObject);

            if (!root.IsValid())
                root = Playable.Create(graph);

            return root;
        }

        static Playable ConnectPlayablesToMixer(PlayableGraph graph, List<Playable> playables)
        {
            var mixer = Playable.Create(graph, playables.Count);

            for (int i = 0; i != playables.Count; ++i)
            {
                ConnectMixerAndPlayable(graph, mixer, playables[i], i);
            }

            mixer.SetPropagateSetTime(true);

            return mixer;
        }

        void CreateActivationPlayable(GameObject root, PlayableGraph graph,
            List<Playable> outplayables)
        {
            var activation = ActivationControlPlayable.Create(graph, root, postPlayback);
            if (activation.IsValid())
                outplayables.Add(activation);
        }

        void SearchHierarchyAndConnectParticleSystem(IEnumerable<ParticleSystem> particleSystems, PlayableGraph graph,
            List<Playable> outplayables)
        {
            foreach (var particleSystem in particleSystems)
            {
                if (particleSystem != null)
                {
                    controllingParticles = true;
                    outplayables.Add(ParticleControlPlayable.Create(graph, particleSystem, particleRandomSeed));
                }
            }
        }

        void SearchHierarchyAndConnectDirector(IEnumerable<PlayableDirector> directors, PlayableGraph graph,
            List<Playable> outplayables, bool disableSelfReferences)
        {
            foreach (var director in directors)
            {
                if (director != null)
                {
                    if (director.playableAsset != m_ControlDirectorAsset)
                    {
                        outplayables.Add(DirectorControlPlayable.Create(graph, director));
                        controllingDirectors = true;
                    }
                    // if this self references, disable the director.
                    else if (disableSelfReferences)
                    {
                        director.enabled = false;
                    }
                }
            }
        }

        static void SearchHierarchyAndConnectControlableScripts(IEnumerable<MonoBehaviour> controlableScripts, PlayableGraph graph, List<Playable> outplayables)
        {
            foreach (var script in controlableScripts)
            {
                outplayables.Add(TimeControlPlayable.Create(graph, (ITimeControl)script));
            }
        }

        static void ConnectMixerAndPlayable(PlayableGraph graph, Playable mixer, Playable playable,
            int portIndex)
        {
            graph.Connect(playable, 0, mixer, portIndex);
            mixer.SetInputWeight(playable, 1.0f);
        }

        internal IList<T> GetComponent<T>(GameObject gameObject)
        {
            var components = new List<T>();
            if (gameObject != null)
            {
                if (searchHierarchy)
                {
                    gameObject.GetComponentsInChildren<T>(true, components);
                }
                else
                {
                    gameObject.GetComponents<T>(components);
                }
            }
            return components;
        }

        internal static IEnumerable<MonoBehaviour> GetControlableScripts(GameObject root)
        {
            if (root == null)
                yield break;

            foreach (var script in root.GetComponentsInChildren<MonoBehaviour>())
            {
                if (script is ITimeControl)
                    yield return script;
            }
        }

        internal void UpdateDurationAndLoopFlag(IList<PlayableDirector> directors, IList<ParticleSystem> particleSystems)
        {
            if (directors.Count == 0 && particleSystems.Count == 0)
                return;

            const double invalidDuration = double.NegativeInfinity;

            var maxDuration = invalidDuration;
            var supportsLoop = false;

            foreach (var director in directors)
            {
                if (director.playableAsset != null)
                {
                    var assetDuration = director.playableAsset.duration;

                    if (director.playableAsset is TimelineAsset && assetDuration > 0.0)
                        // Timeline assets report being one tick shorter than they actually are, unless they are empty
                        assetDuration = (double)((DiscreteTime)assetDuration).OneTickAfter();

                    maxDuration = Math.Max(maxDuration, assetDuration);
                    supportsLoop = supportsLoop || director.extrapolationMode == DirectorWrapMode.Loop;
                }
            }

            foreach (var particleSystem in particleSystems)
            {
                maxDuration = Math.Max(maxDuration, particleSystem.main.duration);
                supportsLoop = supportsLoop || particleSystem.main.loop;
            }

            m_Duration = double.IsNegativeInfinity(maxDuration) ? PlayableBinding.DefaultDuration : maxDuration;
            m_SupportLoop = supportsLoop;
        }

        IList<ParticleSystem> GetControllableParticleSystems(GameObject go)
        {
            var roots = new List<ParticleSystem>();

            // searchHierarchy will look for particle systems on child objects.
            // once a particle system is found, all child particle systems are controlled with playables
            // unless they are subemitters

            if (searchHierarchy || go.GetComponent<ParticleSystem>() != null)
            {
                GetControllableParticleSystems(go.transform, roots, s_SubEmitterCollector);
                s_SubEmitterCollector.Clear();
            }

            return roots;
        }

        static void GetControllableParticleSystems(Transform t, ICollection<ParticleSystem> roots, HashSet<ParticleSystem> subEmitters)
        {
            var ps = t.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                if (!subEmitters.Contains(ps))
                {
                    roots.Add(ps);
                    CacheSubEmitters(ps, subEmitters);
                }
            }

            for (int i = 0; i < t.childCount; ++i)
            {
                GetControllableParticleSystems(t.GetChild(i), roots, subEmitters);
            }
        }

        static void CacheSubEmitters(ParticleSystem ps, HashSet<ParticleSystem> subEmitters)
        {
            if (ps == null)
                return;

            for (int i = 0; i < ps.subEmitters.subEmittersCount; i++)
            {
                subEmitters.Add(ps.subEmitters.GetSubEmitterSystem(i));
                // don't call this recursively. subEmitters are only simulated one level deep.
            }
        }

        /// <inheritdoc/>
        public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            // This method is no longer called by Control Tracks.
            if (director == null)
                return;

            // prevent infinite recursion
            if (s_ProcessedDirectors.Contains(director))
                return;
            s_ProcessedDirectors.Add(director);

            var gameObject = sourceGameObject.Resolve(director);
            if (gameObject != null)
            {
                if (updateParticle)// case 1076850 -- drive all emitters, not just roots.
                    PreviewParticles(driver, gameObject.GetComponentsInChildren<ParticleSystem>(true));

                if (active)
                    PreviewActivation(driver, new[] { gameObject });

                if (updateITimeControl)
                    PreviewTimeControl(driver, director, GetControlableScripts(gameObject));

                if (updateDirector)
                    PreviewDirectors(driver, GetComponent<PlayableDirector>(gameObject));
            }
            s_ProcessedDirectors.Remove(director);
        }

        internal static void PreviewParticles(IPropertyCollector driver, IEnumerable<ParticleSystem> particles)
        {
            foreach (var ps in particles)
            {
                driver.AddFromName<ParticleSystem>(ps.gameObject, "randomSeed");
                driver.AddFromName<ParticleSystem>(ps.gameObject, "autoRandomSeed");
            }
        }

        internal static void PreviewActivation(IPropertyCollector driver, IEnumerable<GameObject> objects)
        {
            foreach (var gameObject in objects)
                driver.AddFromName(gameObject, "m_IsActive");
        }

        internal static void PreviewTimeControl(IPropertyCollector driver, PlayableDirector director, IEnumerable<MonoBehaviour> scripts)
        {
            foreach (var script in scripts)
            {
                var propertyPreview = script as IPropertyPreview;
                if (propertyPreview != null)
                    propertyPreview.GatherProperties(director, driver);
                else
                    driver.AddFromComponent(script.gameObject, script);
            }
        }

        internal static void PreviewDirectors(IPropertyCollector driver, IEnumerable<PlayableDirector> directors)
        {
            foreach (var childDirector in directors)
            {
                if (childDirector == null)
                    continue;

                var timeline = childDirector.playableAsset as TimelineAsset;
                if (timeline == null)
                    continue;

                timeline.GatherProperties(childDirector, driver);
            }
        }
    }
}
