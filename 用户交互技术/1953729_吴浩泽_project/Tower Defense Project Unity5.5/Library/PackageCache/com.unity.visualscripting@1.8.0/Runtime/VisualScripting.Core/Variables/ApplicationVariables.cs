using UnityEngine;

namespace Unity.VisualScripting
{
    public static class ApplicationVariables
    {
        public const string assetPath = "ApplicationVariables";

        private static VariablesAsset _asset;

        public static VariablesAsset asset
        {
            get
            {
                if (_asset == null)
                {
                    Load();
                }

                return _asset;
            }
        }

        public static void Load()
        {
            _asset = Resources.Load<VariablesAsset>(assetPath) ?? ScriptableObject.CreateInstance<VariablesAsset>();
        }

        public static VariableDeclarations runtime { get; private set; }

        public static VariableDeclarations initial => asset.declarations;

        public static VariableDeclarations current => Application.isPlaying ? runtime : initial;

        public static void OnEnterEditMode()
        {
            DestroyRuntimeDeclarations(); // Required because assemblies don't reload on play mode exit
        }

        public static void OnExitEditMode() { }

        internal static void OnEnterPlayMode()
        {
            CreateRuntimeDeclarations();
        }

        internal static void OnExitPlayMode() { }

        private static void CreateRuntimeDeclarations()
        {
            runtime = asset.declarations.CloneViaFakeSerialization();
        }

        private static void DestroyRuntimeDeclarations()
        {
            runtime = null;
        }
    }
}
