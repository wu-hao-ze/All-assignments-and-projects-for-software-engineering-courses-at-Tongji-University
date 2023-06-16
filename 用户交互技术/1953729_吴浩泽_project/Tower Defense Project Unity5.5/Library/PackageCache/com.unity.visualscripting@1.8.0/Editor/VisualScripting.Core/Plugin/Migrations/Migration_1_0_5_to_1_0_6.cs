using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class Migration_1_0_5_to_1_0_6 : BoltCoreMigration
    {
        public Migration_1_0_5_to_1_0_6(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.0.5";
        public override SemanticVersion to => "1.0.6";

        public List<LooseAssemblyName> defaultAssemblyOptions { get; private set; } = new List<LooseAssemblyName>()
        {
            // .NET
            "mscorlib",

            // User
            "Assembly-CSharp-firstpass",
            "Assembly-CSharp",

            // Core
            "UnityEngine",
            "UnityEngine.CoreModule",

            // Input
            "UnityEngine.InputModule",
            "UnityEngine.ClusterInputModule",

            // Physics
            "UnityEngine.PhysicsModule",
            "UnityEngine.Physics2DModule",
            "UnityEngine.TerrainPhysicsModule",
            "UnityEngine.VehiclesModule",

            // Audio
            "UnityEngine.AudioModule",

            // Animation
            "UnityEngine.AnimationModule",
            "UnityEngine.VideoModule",
            "UnityEngine.DirectorModule",
            "UnityEngine.Timeline",

            // Effects
            "UnityEngine.ParticleSystemModule",
            "UnityEngine.ParticlesLegacyModule",
            "UnityEngine.WindModule",
            "UnityEngine.ClothModule",

            // 2D
            "UnityEngine.TilemapModule",
            "UnityEngine.SpriteMaskModule",

            // Rendering
            "UnityEngine.TerrainModule",
            "UnityEngine.ImageConversionModule",
            "UnityEngine.TextRenderingModule",
            "UnityEngine.ClusterRendererModule",
            "UnityEngine.ScreenCaptureModule",

            // AI
            "UnityEngine.AIModule",

            // UI
            "UnityEngine.UI",
            "UnityEngine.UIModule",
            "UnityEngine.IMGUIModule",
            "UnityEngine.UIElementsModule",
            "UnityEngine.StyleSheetsModule",

            // XR
            "UnityEngine.VR",
            "UnityEngine.VRModule",
            "UnityEngine.ARModule",
            "UnityEngine.HoloLens",
            "UnityEngine.SpatialTracking",
            "UnityEngine.GoogleAudioSpatializer",

            // Networking
            "UnityEngine.Networking",

            // Services
            "UnityEngine.Analytics",
            "UnityEngine.Advertisements",
            "UnityEngine.Purchasing",
            "UnityEngine.UnityConnectModule",
            "UnityEngine.UnityAnalyticsModule",
            "UnityEngine.GameCenterModule",
            "UnityEngine.AccessibilityModule",
        };

        public List<Type> defaultTypeOptions { get; private set; } = new List<Type>()
        {
            typeof(object),
            typeof(bool),
            typeof(int),
            typeof(float),
            typeof(string),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Quaternion),
            typeof(Matrix4x4),
            typeof(Rect),
            typeof(Bounds),
            typeof(Color),
            typeof(AnimationCurve),
            typeof(LayerMask),
            typeof(Ray),
            typeof(Ray2D),
            typeof(RaycastHit),
            typeof(RaycastHit2D),
            typeof(ContactPoint),
            typeof(ContactPoint2D),
            typeof(Scene),
            typeof(Application),
            typeof(Mathf),
            typeof(Debug),
            typeof(Input),
            typeof(Time),
            typeof(UnityEngine.Random),
            typeof(Physics),
            typeof(Physics2D),
            typeof(SceneManager),
            typeof(GUI),
            typeof(GUILayout),
            typeof(GUIUtility),
            typeof(AudioMixerGroup),
            typeof(NavMesh),
            typeof(Gizmos),
            typeof(AnimatorStateInfo),
            typeof(IList),
            typeof(IDictionary),
        };

        public override void Run()
        {
            RequireAction("In the process of fixing a deserialization issue, some of your object references may have been lost. " +
                "You may need to reassign references to scene objects or macros in your graphs, especially in prefabs. " +
                "We deeply apologize for the inconvenience and are taking precautions to ensure this doesn't happen again.");

            foreach (var defaultAssemblyOption in defaultAssemblyOptions)
            {
                if (!BoltCore.Configuration.assemblyOptions.Contains(defaultAssemblyOption))
                {
                    BoltCore.Configuration.assemblyOptions.Add(defaultAssemblyOption);
                }
            }

            foreach (var defaultTypeOption in defaultTypeOptions)
            {
                if (!BoltCore.Configuration.typeOptions.Contains(defaultTypeOption))
                {
                    BoltCore.Configuration.typeOptions.Add(defaultTypeOption);
                }
            }

            BoltCore.Configuration.Save();
        }
    }
}
