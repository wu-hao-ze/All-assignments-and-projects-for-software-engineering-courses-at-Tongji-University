namespace UnityEditor.Timeline.Actions
{
    /// <summary>
    /// Indicates the validity of an action for a given data set.
    /// </summary>
    public enum ActionValidity
    {
        /// <summary>
        /// Action is valid in the provided context.
        /// If the action is linked to a menu item, the menu item will be visible.
        /// </summary>
        Valid,
        /// <summary>
        /// Action is not applicable in the current context.
        /// If the action is linked to a menu item, the menu item will not be shown.
        /// </summary>
        NotApplicable,
        /// <summary>
        /// Action is not valid in the current context.
        /// If the action is linked to a menu item, the menu item will be shown but grayed out.
        /// </summary>
        Invalid
    }

    struct MenuActionItem
    {
        public string category;
        public string entryName;
        public string shortCut;
        public int priority;
        public bool isActiveInMode;
        public ActionValidity state;
        public bool isChecked;
        public GenericMenu.MenuFunction callback;
    }
}
