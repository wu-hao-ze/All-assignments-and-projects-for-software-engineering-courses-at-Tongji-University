using System;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    interface ISequenceState : IDisposable
    {
        TimelineAsset asset { get; }
        PlayableDirector director { get; }
        TimelineClip hostClip { get; }
        List<UnityEngine.Object> cachedChildAssets { get; }
        double start { get; }
        double timeScale { get; }
        double duration { get; }
        bool isReadOnly { get; }
        bool isAssetOnly { get; set; }
        TimelineAssetViewModel viewModel { get; }
        double time { get; set; }
        int frame { get; set; }
        double frameRate { get; set; }

        Range GetEvaluableRange();
        double ToGlobalTime(double t);
        double ToLocalTime(double t);
        void ResetIsReadOnly();
        void InvalidateChildAssetCache();
    }

    /**
     * This class is used to hold default values for an implementation of ISequenceState.
     * It could be removed in a phase 2, but it is currently used to limit the scope of
     * this refactoring: it allows client code to access sequence info without having to
     * worry about `currentSequence` being null.
     * In the future this should be removed and we should pass around the correct data
     * structure (i.e. SequenceState OR WindowState) based on the situation.
     */
    class NullSequenceState : ISequenceState
    {
        public TimelineAsset asset { get { return null; } }
        public PlayableDirector director { get { return null; } }
        public TimelineClip hostClip { get { return null; } }

        List<UnityEngine.Object> ISequenceState.cachedChildAssets { get { return null; } }

        public double start { get { return 0.0; } }
        public double timeScale { get { return 1.0; } }
        public double duration { get { return 0.0; } }
        public bool isReadOnly { get { return false; } }
        public bool isAssetOnly { get { return false; } set { } }

        TimelineAssetViewModel m_ViewModel;

        public TimelineAssetViewModel viewModel
        {
            get
            {
                if (m_ViewModel == null)
                    m_ViewModel = TimelineWindowViewPrefs.CreateUnassociatedViewModel();
                return m_ViewModel;
            }
        }

        public double time
        {
            get { return 0.0; }
            set { /* NO-OP*/ }
        }

        public int frame
        {
            get { return 0; }
            set { /* NO-OP*/ }
        }

        public double frameRate
        {
            get { return TimelineAsset.EditorSettings.kDefaultFrameRate; }
            set { /* NO-OP*/ }
        }

        public Range GetEvaluableRange()
        {
            return new Range();
        }

        public double ToGlobalTime(double t)
        {
            return t;
        }

        public double ToLocalTime(double t)
        {
            return t;
        }

        public void ResetIsReadOnly()
        {
            // NO-OP
        }

        public void InvalidateChildAssetCache()
        {
            // NO-OP
        }

        public void Dispose()
        {
            // NO-OP
        }
    }
}
