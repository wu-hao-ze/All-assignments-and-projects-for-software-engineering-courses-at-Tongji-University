using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [InitializeAfterPlugins]
    public static class EditorTypeUtility
    {
        // Used to call the static constructor on the main thread before threaded work might query things from here
        // EditorSettings.defaultBehaviourMode errors when queried from anything but the main thread.
        internal static void Initialize() { }

        private static EditorBehaviorMode lastBehaviorMode = EditorSettings.defaultBehaviorMode;

        private static EditorBehaviorMode behaviorMode
        {
            get
            {
                if (UnityThread.allowsAPI)
                {
                    lastBehaviorMode = EditorSettings.defaultBehaviorMode;
                }

                return lastBehaviorMode;
            }
        }

        public static IEnumerable<Type> commonTypes
        {
            get
            {
                yield return typeof(float);
                yield return typeof(int);
                yield return typeof(string);
                yield return typeof(bool);
                yield return new BehaviourTypeAssociation(typeof(Vector3), typeof(Vector2));
                yield return typeof(GameObject);
            }
        }

        private struct BehaviourTypeAssociation
        {
            public readonly Type typeFor3D;
            public readonly Type typeFor2D;

            public BehaviourTypeAssociation(Type typeFor3D, Type typeFor2D)
            {
                this.typeFor3D = typeFor3D;
                this.typeFor2D = typeFor2D;
            }

            public Type For(EditorBehaviorMode mode)
            {
                switch (mode)
                {
                    case EditorBehaviorMode.Mode3D:
                        return typeFor3D;
                    case EditorBehaviorMode.Mode2D:
                        return typeFor2D;
                    default:
                        throw new UnexpectedEnumValueException<EditorBehaviorMode>(mode);
                }
            }

            public static implicit operator Type(BehaviourTypeAssociation bta)
            {
                return bta.For(behaviorMode);
            }
        }
    }
}
