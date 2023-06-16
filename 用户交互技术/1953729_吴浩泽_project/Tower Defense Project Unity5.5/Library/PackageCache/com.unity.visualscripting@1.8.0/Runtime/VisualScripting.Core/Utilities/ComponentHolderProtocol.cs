using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public static class ComponentHolderProtocol
    {
        public static bool IsComponentHolderType(Type type)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            return typeof(GameObject).IsAssignableFrom(type) || typeof(Component).IsAssignableFrom(type);
        }

        public static bool IsComponentHolder(this UnityObject uo)
        {
            return uo is GameObject || uo is Component;
        }

        public static GameObject GameObject(this UnityObject uo)
        {
            if (uo is GameObject)
            {
                return (GameObject)uo;
            }
            else if (uo is Component)
            {
                return ((Component)uo).gameObject;
            }
            else
            {
                return null;
            }
        }

        public static T AddComponent<T>(this UnityObject uo) where T : Component
        {
            if (uo is GameObject)
            {
                return ((GameObject)uo).AddComponent<T>();
            }
            else if (uo is Component)
            {
                return ((Component)uo).gameObject.AddComponent<T>();
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static T GetOrAddComponent<T>(this UnityObject uo) where T : Component
        {
            return uo.GetComponent<T>() ?? uo.AddComponent<T>();
        }

        public static T GetComponent<T>(this UnityObject uo)
        {
            if (uo is GameObject)
            {
                return ((GameObject)uo).GetComponent<T>();
            }
            else if (uo is Component)
            {
                return ((Component)uo).GetComponent<T>();
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static T GetComponentInChildren<T>(this UnityObject uo)
        {
            if (uo is GameObject)
            {
                return ((GameObject)uo).GetComponentInChildren<T>();
            }
            else if (uo is Component)
            {
                return ((Component)uo).GetComponentInChildren<T>();
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static T GetComponentInParent<T>(this UnityObject uo)
        {
            if (uo is GameObject)
            {
                return ((GameObject)uo).GetComponentInParent<T>();
            }
            else if (uo is Component)
            {
                return ((Component)uo).GetComponentInParent<T>();
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static T[] GetComponents<T>(this UnityObject uo)
        {
            if (uo is GameObject)
            {
                return ((GameObject)uo).GetComponents<T>();
            }
            else if (uo is Component)
            {
                return ((Component)uo).GetComponents<T>();
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static T[] GetComponentsInChildren<T>(this UnityObject uo)
        {
            if (uo is GameObject)
            {
                return ((GameObject)uo).GetComponentsInChildren<T>();
            }
            else if (uo is Component)
            {
                return ((Component)uo).GetComponentsInChildren<T>();
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static T[] GetComponentsInParent<T>(this UnityObject uo)
        {
            if (uo is GameObject)
            {
                return ((GameObject)uo).GetComponentsInParent<T>();
            }
            else if (uo is Component)
            {
                return ((Component)uo).GetComponentsInParent<T>();
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static Component GetComponent(this UnityObject uo, Type type)
        {
            if (uo is GameObject)
            {
                return ((GameObject)uo).GetComponent(type);
            }
            else if (uo is Component)
            {
                return ((Component)uo).GetComponent(type);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static Component GetComponentInChildren(this UnityObject uo, Type type)
        {
            if (uo is GameObject)
            {
                return ((GameObject)uo).GetComponentInChildren(type);
            }
            else if (uo is Component)
            {
                return ((Component)uo).GetComponentInChildren(type);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static Component GetComponentInParent(this UnityObject uo, Type type)
        {
            if (uo is GameObject)
            {
                return ((GameObject)uo).GetComponentInParent(type);
            }
            else if (uo is Component)
            {
                return ((Component)uo).GetComponentInParent(type);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static Component[] GetComponents(this UnityObject uo, Type type)
        {
            if (uo is GameObject)
            {
                return ((GameObject)uo).GetComponents(type);
            }
            else if (uo is Component)
            {
                return ((Component)uo).GetComponents(type);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static Component[] GetComponentsInChildren(this UnityObject uo, Type type)
        {
            if (uo is GameObject)
            {
                return ((GameObject)uo).GetComponentsInChildren(type);
            }
            else if (uo is Component)
            {
                return ((Component)uo).GetComponentsInChildren(type);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static Component[] GetComponentsInParent(this UnityObject uo, Type type)
        {
            if (uo is GameObject)
            {
                return ((GameObject)uo).GetComponentsInParent(type);
            }
            else if (uo is Component)
            {
                return ((Component)uo).GetComponentsInParent(type);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
