using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    /// <remarks>
    /// Does not support objects hidden with hide flags.
    /// </remarks>
    public static class SceneSingleton<T> where T : MonoBehaviour, ISingleton
    {
        static SceneSingleton()
        {
            instances = new Dictionary<Scene, T>();

            attribute = typeof(T).GetAttribute<SingletonAttribute>();

            if (attribute == null)
            {
                throw new InvalidImplementationException($"Missing singleton attribute for '{typeof(T)}'.");
            }
        }

        private static Dictionary<Scene, T> instances;

        private static readonly SingletonAttribute attribute;
        private static bool persistent => attribute.Persistent;
        private static bool automatic => attribute.Automatic;
        private static string name => attribute.Name;
        private static HideFlags hideFlags => attribute.HideFlags;

        private static void EnsureSceneValid(Scene scene)
        {
            if (!scene.IsValid())
            {
                throw new InvalidOperationException($"Scene '{scene.name}' is invalid and cannot be used in singleton operations.");
            }
        }

        public static bool InstantiatedIn(Scene scene)
        {
            EnsureSceneValid(scene);

            if (Application.isPlaying)
            {
                return instances.ContainsKey(scene);
            }
            else
            {
                return FindInstances(scene).Length == 1;
            }
        }

        public static T InstanceIn(Scene scene)
        {
            EnsureSceneValid(scene);

            if (Application.isPlaying)
            {
                if (instances.ContainsKey(scene))
                {
                    return instances[scene];
                }
                else
                {
                    return FindOrCreateInstance(scene);
                }
            }
            else
            {
                return FindOrCreateInstance(scene);
            }
        }

        private static T[] FindInstances(Scene scene)
        {
            EnsureSceneValid(scene);

            // Fails here on hidden hide flags
            return UnityObject.FindObjectsOfType<T>().Where(o => o.gameObject.scene == scene).ToArray();
        }

        private static T FindOrCreateInstance(Scene scene)
        {
            EnsureSceneValid(scene);

            var instances = FindInstances(scene);

            if (instances.Length == 1)
            {
                return instances[0];
            }
            else if (instances.Length == 0)
            {
                if (automatic)
                {
                    // Because DontDestroyOnLoad moves objects to another scene internally,
                    // the scene singleton system does not support persistence (which wouldn't
                    // make sense logically either!)
                    if (persistent)
                    {
                        throw new UnityException("Scene singletons cannot be persistent.");
                    }

                    // Create the parent game object with the proper hide flags
                    var singleton = new GameObject(name ?? typeof(T).Name);
                    singleton.hideFlags = hideFlags;

                    // Immediately move it to the proper scene so that Register can determine dictionary key
                    SceneManager.MoveGameObjectToScene(singleton, scene);

                    // Instantiate the component, letting Awake register the instance if we're in play mode
                    var instance = singleton.AddComponent<T>();
                    instance.hideFlags = hideFlags;

                    return instance;
                }
                else
                {
                    throw new UnityException($"Missing '{typeof(T)}' singleton in scene '{scene.name}'.");
                }
            }
            else // if (instances.Length > 1)
            {
                throw new UnityException($"More than one '{typeof(T)}' singleton in scene '{scene.name}'.");
            }
        }

        public static void Awake(T instance)
        {
            Ensure.That(nameof(instance)).IsNotNull(instance);

            var scene = instance.gameObject.scene;

            EnsureSceneValid(scene);

            if (instances.ContainsKey(scene))
            {
                throw new UnityException($"More than one '{typeof(T)}' singleton in scene '{scene.name}'.");
            }

            instances.Add(scene, instance);
        }

        public static void OnDestroy(T instance)
        {
            Ensure.That(nameof(instance)).IsNotNull(instance);

            var scene = instance.gameObject.scene;

            // If the scene is invalid, it has probably been unloaded already
            // (for example the DontDestroyOnLoad pseudo-scene), so we can't
            // access it. However, we'll need to check in the dictionary by
            // value to remove the entry anyway.
            if (!scene.IsValid())
            {
                foreach (var kvp in instances)
                {
                    if (kvp.Value == instance)
                    {
                        instances.Remove(kvp.Key);

                        break;
                    }
                }

                return;
            }

            if (instances.ContainsKey(scene))
            {
                var sceneInstance = instances[scene];

                if (sceneInstance == instance)
                {
                    instances.Remove(scene);
                }
                else
                {
                    throw new UnityException($"Trying to destroy invalid instance of '{typeof(T)}' singleton in scene '{scene.name}'.");
                }
            }
            else
            {
                throw new UnityException($"Trying to destroy invalid instance of '{typeof(T)}' singleton in scene '{scene.name}'.");
            }
        }
    }
}
