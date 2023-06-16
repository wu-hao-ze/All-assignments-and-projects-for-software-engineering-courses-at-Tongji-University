using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    /// <remarks>
    /// Does not support objects hidden with hide flags.
    /// </remarks>
    public static class Singleton<T> where T : MonoBehaviour, ISingleton
    {
        static Singleton()
        {
            awoken = new HashSet<T>();

            attribute = typeof(T).GetAttribute<SingletonAttribute>();

            if (attribute == null)
            {
                throw new InvalidImplementationException($"Missing singleton attribute for '{typeof(T)}'.");
            }
        }

        private static readonly SingletonAttribute attribute;

        private static bool persistent => attribute.Persistent;

        private static bool automatic => attribute.Automatic;

        private static string name => attribute.Name;

        private static HideFlags hideFlags => attribute.HideFlags;

        private static readonly object _lock = new object();

        private static readonly HashSet<T> awoken;

        private static T _instance;

        public static bool instantiated
        {
            get
            {
                lock (_lock)
                {
                    if (Application.isPlaying)
                    {
                        return _instance != null;
                    }
                    else
                    {
                        return FindInstances().Length == 1;
                    }
                }
            }
        }

        public static T instance
        {
            get
            {
                lock (_lock)
                {
                    if (Application.isPlaying)
                    {
                        if (_instance == null)
                        {
                            Instantiate();
                        }

                        return _instance;
                    }
                    else
                    {
                        return Instantiate();
                    }
                }
            }
        }

        private static T[] FindInstances()
        {
            // Fails here on hidden hide flags
            return UnityObject.FindObjectsOfType<T>();
        }

        public static T Instantiate()
        {
            lock (_lock)
            {
                var instances = FindInstances();

                if (instances.Length == 1)
                {
                    _instance = instances[0];
                }
                else if (instances.Length == 0)
                {
                    if (automatic)
                    {
                        // Create the parent game object with the proper hide flags
                        var singleton = new GameObject(name ?? typeof(T).Name);
                        singleton.hideFlags = hideFlags;

                        // Instantiate the component, letting Awake assign the real instance variable
                        var _instance = singleton.AddComponent<T>();
                        _instance.hideFlags = hideFlags;

                        // Sometimes in the editor, for example when creating a new scene,
                        // AddComponent seems to call Awake add a later frame, making this call
                        // fail for exactly one frame. We'll force-awake it if need be.
                        Awake(_instance);

                        // Make the singleton persistent if need be
                        if (persistent && Application.isPlaying)
                        {
                            UnityObject.DontDestroyOnLoad(singleton);
                        }
                    }
                    else
                    {
                        throw new UnityException($"Missing '{typeof(T)}' singleton in the scene.");
                    }
                }
                else if (instances.Length > 1)
                {
                    throw new UnityException($"More than one '{typeof(T)}' singleton in the scene.");
                }

                return _instance;
            }
        }

        public static void Awake(T instance)
        {
            Ensure.That(nameof(instance)).IsNotNull(instance);

            if (awoken.Contains(instance))
            {
                return;
            }

            if (_instance != null)
            {
                throw new UnityException($"More than one '{typeof(T)}' singleton in the scene.");
            }

            _instance = instance;

            awoken.Add(instance);
        }

        public static void OnDestroy(T instance)
        {
            Ensure.That(nameof(instance)).IsNotNull(instance);

            if (_instance == instance)
            {
                _instance = null;
            }
            else
            {
                throw new UnityException($"Trying to destroy invalid instance of '{typeof(T)}' singleton.");
            }
        }
    }
}
