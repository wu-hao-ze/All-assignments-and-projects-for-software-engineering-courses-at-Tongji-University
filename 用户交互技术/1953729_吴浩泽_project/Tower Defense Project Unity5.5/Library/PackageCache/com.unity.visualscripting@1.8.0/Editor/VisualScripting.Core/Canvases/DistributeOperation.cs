namespace Unity.VisualScripting
{
    public enum DistributeOperation
    {
        /// <summary>
        /// Distribute the selected elements so that the left  edges
        /// are at equal distance of one another.
        /// </summary>
        DistributeLeftEdges,

        /// <summary>
        /// Distribute the selected elements so that the horizontal centers
        /// are at equal distance of one another.
        /// </summary>
        DistributeCenters,

        /// <summary>
        /// Distribute the selected elements so that the right edges
        /// are at equal distance of one another.
        /// </summary>
        DistributeRightEdges,

        /// <summary>
        /// Distribute the selected elements so that the horizontal gaps
        /// are all of equal size.
        /// </summary>
        DistributeHorizontalGaps,

        /// <summary>
        /// Distribute the selected elements so that the top edges
        /// are at equal distance of one another.
        /// </summary>
        DistributeTopEdges,

        /// <summary>
        /// Distribute the selected elements so that the vertical middles
        /// are at equal distance of one another.
        /// </summary>
        DistributeMiddles,

        /// <summary>
        /// Distribute the selected elements so that the bottom edges
        /// are at equal distance of one another.
        /// </summary>
        DistributeBottomEdges,

        /// <summary>
        /// Distribute the selected elements so that the vertical gaps
        /// are all of equal size.
        /// </summary>
        DistributeVerticalGaps
    }
}
