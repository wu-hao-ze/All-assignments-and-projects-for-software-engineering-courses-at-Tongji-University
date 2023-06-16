namespace Unity.PlasticSCM.Editor.UI.StatusBar
{
    internal class IncomingChangesNotification
    {
        internal string InfoText;
        internal string ActionText;
        internal string TooltipText;
        internal bool HasUpdateAction;
        internal PlasticNotification.Status Status;

        internal void Clear()
        {
            InfoText = string.Empty;
            ActionText = string.Empty;
            TooltipText = string.Empty;
            HasUpdateAction = false;
            Status = PlasticNotification.Status.None;
        }
    }
}