using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.VisualScripting
{
    [Singleton(Name = "VisualScripting SceneVariables", Automatic = true, Persistent = false)]
    [RequireComponent(typeof(Variables))]
    [DisableAnnotation]
    [AddComponentMenu("")]
    [IncludeInSettings(false)]
    public sealed class SceneVariables : MonoBehaviour, ISingleton
    {
        public static SceneVariables Instance(Scene scene)
        {
            return SceneSingleton<SceneVariables>.InstanceIn(scene);
        }

        public static bool InstantiatedIn(Scene scene)
        {
            return SceneSingleton<SceneVariables>.InstantiatedIn(scene);
        }

        public static VariableDeclarations For(Scene? scene)
        {
            Ensure.That(nameof(scene)).IsNotNull(scene);

            return Instance(scene.Value).variables.declarations;
        }

        private void Awake()
        {
            SceneSingleton<SceneVariables>.Awake(this);
        }

        private void OnDestroy()
        {
            SceneSingleton<SceneVariables>.OnDestroy(this);
        }

        private Variables _variables;

        public Variables variables
        {
            get
            {
                if (_variables == null)
                {
                    _variables = gameObject.GetOrAddComponent<Variables>();
                }

                return _variables;
            }
        }
    }
}
