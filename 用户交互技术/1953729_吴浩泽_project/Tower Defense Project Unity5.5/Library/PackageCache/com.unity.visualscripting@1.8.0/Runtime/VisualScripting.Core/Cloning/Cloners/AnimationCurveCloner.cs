using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    public sealed class AnimationCurveCloner : Cloner<AnimationCurve>
    {
        public override bool Handles(Type type)
        {
            return type == typeof(AnimationCurve);
        }

        public override AnimationCurve ConstructClone(Type type, AnimationCurve original)
        {
            return new AnimationCurve();
        }

        public override void FillClone(Type type, ref AnimationCurve clone, AnimationCurve original, CloningContext context)
        {
            for (int i = 0; i < clone.length; i++)
            {
                clone.RemoveKey(i);
            }

            foreach (var key in original.keys)
            {
                clone.AddKey(key);
            }
        }
    }
}
