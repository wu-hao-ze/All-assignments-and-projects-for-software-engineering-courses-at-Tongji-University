using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class EditorTimeBinding
    {
        public static Func<int> frameBinding;

        public static Func<float> timeBinding;

        public static int frame => (frameBinding != null && UnityThread.allowsAPI) ? frameBinding() : 0;

        public static float time => (timeBinding != null && UnityThread.allowsAPI) ? timeBinding() : 0;

        static EditorTimeBinding()
        {
            frameBinding = () => Time.frameCount;
            timeBinding = () => Time.time;
        }
    }
}
