using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.VisualScripting
{
    [DisableAnnotation]
    [AddComponentMenu("")]
    [IncludeInSettings(false)]
    public abstract class MessageListener : MonoBehaviour
    {
        private static Type[] _listenerTypes;

        [Obsolete("listenerTypes is deprecated", false)]
        public static Type[] listenerTypes
        {
            get
            {
                if (_listenerTypes == null)
                {
                    _listenerTypes = RuntimeCodebase.types
                        .Where(t => typeof(MessageListener).IsAssignableFrom(t) && t.IsConcrete() && !Attribute.IsDefined(t, typeof(ObsoleteAttribute)))
                        .ToArray();
                }

                return _listenerTypes;
            }
        }

        [Obsolete("Use the overload with a messageListenerType parameter instead", false)]
        public static void AddTo(GameObject gameObject)
        {
            foreach (var listenerType in listenerTypes)
            {
                if (gameObject.GetComponent(listenerType) == null)
                {
                    gameObject.AddComponent(listenerType);
                }
            }
        }

        public static void AddTo(Type messageListenerType, GameObject gameObject)
        {
            Assert.IsNotNull(messageListenerType, "The messageListenerType must not be null");
#if UNITY_2019_4_OR_NEWER
            if (!gameObject.TryGetComponent(messageListenerType, out _))
#else
            if (gameObject.GetComponent(messageListenerType) != null)
#endif
                gameObject.AddComponent(messageListenerType);
        }
    }
}
