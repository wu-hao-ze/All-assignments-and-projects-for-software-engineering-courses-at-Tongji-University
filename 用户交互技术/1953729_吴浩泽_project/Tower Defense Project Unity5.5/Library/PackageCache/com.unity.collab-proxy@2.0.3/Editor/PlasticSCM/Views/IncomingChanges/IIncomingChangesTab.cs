namespace Unity.PlasticSCM.Editor.Views.IncomingChanges
{
    internal interface IIncomingChangesTab
    {
        bool IsVisible
        {
            get; set;
        }

        void OnDisable();
        void Update();
        void OnGUI();
        void AutoRefresh();
    }
}
