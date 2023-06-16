using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    enum CurveChangeType
    {
        None,
        CurveModified,
        CurveAddedOrRemoved
    }

    abstract class CurveDataSource
    {
        public static CurveDataSource Create(IRowGUI trackGUI)
        {
            if (trackGUI.asset is AnimationTrack)
                return new InfiniteClipCurveDataSource(trackGUI);

            return new TrackParametersCurveDataSource(trackGUI);
        }

        public static CurveDataSource Create(TimelineClipGUI clipGUI)
        {
            if (clipGUI.clip.animationClip != null)
                return new ClipAnimationCurveDataSource(clipGUI);

            return new ClipParametersCurveDataSource(clipGUI);
        }

        int? m_ID = null;
        public int id
        {
            get
            {
                if (!m_ID.HasValue)
                    m_ID = CreateHashCode();

                return m_ID.Value;
            }
        }

        readonly IRowGUI m_TrackGUI;
        protected CurveDataSource(IRowGUI trackGUI)
        {
            m_TrackGUI = trackGUI;
        }

        public abstract AnimationClip animationClip { get; }

        public abstract float start { get; }
        public abstract float timeScale { get; }
        public abstract string groupingName { get; }

        // Applies changes from the visual curve in the curve wrapper back to the animation clips
        public virtual void ApplyCurveChanges(IEnumerable<CurveWrapper> updatedCurves)
        {
            Undo.RegisterCompleteObjectUndo(animationClip, "Edit Clip Curve");
            foreach (CurveWrapper c in updatedCurves)
            {
                if (c.curve.length > 0)
                    AnimationUtility.SetEditorCurve(animationClip, c.binding, c.curve);
                else
                    RemoveCurves(new[] { c.binding });
                c.changed = false;
            }
        }

        /// <summary>The clip version is a value that will change when a curve gets updated.
        /// it's used to detect when an animation clip has been changed externally </summary>
        /// <returns>A versioning value indicating the state of the curve. If the curve is updated externally this value will change. </returns>
        public virtual UInt64 GetClipVersion()
        {
            return animationClip.ClipVersion();
        }

        /// <summary>Call this method to check if the underlying clip has changed</summary>
        /// <param name="curveVersion">A versioning value. This will be updated to the latest version</param>
        /// <returns>A value indicating how the clip has changed</returns>
        public virtual CurveChangeType UpdateExternalChanges(ref UInt64 curveVersion)
        {
            return animationClip.GetChangeType(ref curveVersion);
        }

        public virtual string ModifyPropertyDisplayName(string path, string propertyName) => propertyName;

        public virtual void RemoveCurves(IEnumerable<EditorCurveBinding> bindings)
        {
            Undo.RegisterCompleteObjectUndo(animationClip, "Remove Curve(s)");
            foreach (var binding in bindings)
            {
                if (binding.isPPtrCurve)
                    AnimationUtility.SetObjectReferenceCurve(animationClip, binding, null);
                else
                    AnimationUtility.SetEditorCurve(animationClip, binding, null);
            }
        }

        public Rect GetBackgroundRect(WindowState state)
        {
            var trackRect = m_TrackGUI.boundingRect;
            return new Rect(
                state.timeAreaTranslation.x + trackRect.xMin,
                trackRect.y,
                (float)state.editSequence.asset.duration * state.timeAreaScale.x,
                trackRect.height
            );
        }

        public List<CurveWrapper> GenerateWrappers(IEnumerable<EditorCurveBinding> bindings)
        {
            var wrappers = new List<CurveWrapper>(bindings.Count());
            int curveWrapperId = 0;

            foreach (EditorCurveBinding b in bindings)
            {
                // General configuration
                var wrapper = new CurveWrapper
                {
                    id = curveWrapperId++,
                    binding = b,
                    groupId = -1,
                    hidden = false,
                    readOnly = false,
                    getAxisUiScalarsCallback = () => new Vector2(1, 1)
                };

                // Specific configuration
                ConfigureCurveWrapper(wrapper);

                wrappers.Add(wrapper);
            }

            return wrappers;
        }

        protected virtual void ConfigureCurveWrapper(CurveWrapper wrapper)
        {
            wrapper.color = CurveUtility.GetPropertyColor(wrapper.binding.propertyName);
            wrapper.renderer = new NormalCurveRenderer(AnimationUtility.GetEditorCurve(animationClip, wrapper.binding));
            wrapper.renderer.SetCustomRange(0.0f, animationClip.length);
        }

        protected virtual int CreateHashCode()
        {
            return m_TrackGUI.asset.GetHashCode();
        }
    }

    class ClipAnimationCurveDataSource : CurveDataSource
    {
        static readonly string k_GroupingName = L10n.Tr("Animated Values");

        readonly TimelineClipGUI m_ClipGUI;

        public ClipAnimationCurveDataSource(TimelineClipGUI clipGUI) : base(clipGUI.parent)
        {
            m_ClipGUI = clipGUI;
        }

        public override AnimationClip animationClip
        {
            get { return m_ClipGUI.clip.animationClip; }
        }

        public override float start
        {
            get { return (float)m_ClipGUI.clip.FromLocalTimeUnbound(0.0); }
        }

        public override float timeScale
        {
            get { return (float)m_ClipGUI.clip.timeScale; }
        }

        public override string groupingName
        {
            get { return k_GroupingName; }
        }

        protected override int CreateHashCode()
        {
            return base.CreateHashCode().CombineHash(m_ClipGUI.clip.GetHashCode());
        }

        public override string ModifyPropertyDisplayName(string path, string propertyName)
        {
            if (!AnimatedPropertyUtility.IsMaterialProperty(propertyName))
                return propertyName;

            var track = m_ClipGUI.clip.GetParentTrack();
            if (track == null)
                return propertyName;

            var gameObjectBinding = TimelineUtility.GetSceneGameObject(TimelineEditor.inspectedDirector, track);
            if (gameObjectBinding == null)
                return propertyName;

            if (!string.IsNullOrEmpty(path))
            {
                var transform = gameObjectBinding.transform.Find(path);
                if (transform == null)
                    return propertyName;
                gameObjectBinding = transform.gameObject;
            }

            return AnimatedPropertyUtility.RemapMaterialName(gameObjectBinding, propertyName);
        }
    }

    class ClipParametersCurveDataSource : CurveDataSource
    {
        static readonly string k_GroupingName = L10n.Tr("Clip Properties");

        readonly TimelineClipGUI m_ClipGUI;
        readonly CurvesProxy m_CurvesProxy;

        private int m_ClipDirtyVersion;

        public ClipParametersCurveDataSource(TimelineClipGUI clipGUI) : base(clipGUI.parent)
        {
            m_ClipGUI = clipGUI;
            m_CurvesProxy = new CurvesProxy(clipGUI.clip);
        }

        public override AnimationClip animationClip
        {
            get { return m_CurvesProxy.curves; }
        }

        public override UInt64 GetClipVersion()
        {
            return sourceAnimationClip.ClipVersion();
        }

        public override CurveChangeType UpdateExternalChanges(ref ulong curveVersion)
        {
            if (m_ClipGUI == null || m_ClipGUI.clip == null)
                return CurveChangeType.None;

            var changeType = sourceAnimationClip.GetChangeType(ref curveVersion);
            if (changeType != CurveChangeType.None)
            {
                m_CurvesProxy.ApplyExternalChangesToProxy();
            }
            else if (m_ClipDirtyVersion != m_ClipGUI.clip.DirtyIndex)
            {
                m_CurvesProxy.UpdateProxyCurves();
                if (changeType == CurveChangeType.None)
                    changeType = CurveChangeType.CurveModified;
            }
            m_ClipDirtyVersion = m_ClipGUI.clip.DirtyIndex;
            return changeType;
        }

        public override float start
        {
            get { return (float)m_ClipGUI.clip.FromLocalTimeUnbound(0.0); }
        }

        public override float timeScale
        {
            get { return (float)m_ClipGUI.clip.timeScale; }
        }

        public override string groupingName
        {
            get { return k_GroupingName; }
        }

        public override void RemoveCurves(IEnumerable<EditorCurveBinding> bindings)
        {
            m_CurvesProxy.RemoveCurves(bindings);
        }

        public override void ApplyCurveChanges(IEnumerable<CurveWrapper> updatedCurves)
        {
            m_CurvesProxy.UpdateCurves(updatedCurves);
        }

        protected override void ConfigureCurveWrapper(CurveWrapper wrapper)
        {
            m_CurvesProxy.ConfigureCurveWrapper(wrapper);
        }

        protected override int CreateHashCode()
        {
            return base.CreateHashCode().CombineHash(m_ClipGUI.clip.GetHashCode());
        }

        private AnimationClip sourceAnimationClip
        {
            get
            {
                if (m_ClipGUI == null || m_ClipGUI.clip == null || m_ClipGUI.clip.curves == null)
                    return null;
                return m_ClipGUI.clip.curves;
            }
        }
    }

    class InfiniteClipCurveDataSource : CurveDataSource
    {
        static readonly string k_GroupingName = L10n.Tr("Animated Values");

        readonly AnimationTrack m_AnimationTrack;

        public InfiniteClipCurveDataSource(IRowGUI trackGui) : base(trackGui)
        {
            m_AnimationTrack = trackGui.asset as AnimationTrack;
        }

        public override AnimationClip animationClip
        {
            get { return m_AnimationTrack.infiniteClip; }
        }

        public override float start
        {
            get { return 0.0f; }
        }

        public override float timeScale
        {
            get { return 1.0f; }
        }

        public override string groupingName
        {
            get { return k_GroupingName; }
        }

        public override string ModifyPropertyDisplayName(string path, string propertyName)
        {
            if (m_AnimationTrack == null || !AnimatedPropertyUtility.IsMaterialProperty(propertyName))
                return propertyName;

            var binding = m_AnimationTrack.GetBinding(TimelineEditor.inspectedDirector);
            if (binding == null)
                return propertyName;

            var target = binding.transform;
            if (!string.IsNullOrEmpty(path))
                target = target.Find(path);

            if (target == null)
                return propertyName;

            return AnimatedPropertyUtility.RemapMaterialName(target.gameObject, propertyName);
        }
    }

    class TrackParametersCurveDataSource : CurveDataSource
    {
        static readonly string k_GroupingName = L10n.Tr("Track Properties");

        readonly CurvesProxy m_CurvesProxy;
        private int m_TrackDirtyVersion;

        public TrackParametersCurveDataSource(IRowGUI trackGui) : base(trackGui)
        {
            m_CurvesProxy = new CurvesProxy(trackGui.asset);
        }

        public override AnimationClip animationClip
        {
            get { return m_CurvesProxy.curves; }
        }

        public override UInt64 GetClipVersion()
        {
            return sourceAnimationClip.ClipVersion();
        }

        public override CurveChangeType UpdateExternalChanges(ref ulong curveVersion)
        {
            if (m_CurvesProxy.targetTrack == null)
                return CurveChangeType.None;

            var changeType = sourceAnimationClip.GetChangeType(ref curveVersion);
            if (changeType != CurveChangeType.None)
            {
                m_CurvesProxy.ApplyExternalChangesToProxy();
            }
            // track property has changed externally, update the curve proxies
            else if (m_TrackDirtyVersion != m_CurvesProxy.targetTrack.DirtyIndex)
            {
                if (changeType == CurveChangeType.None)
                    changeType = CurveChangeType.CurveModified;
                m_CurvesProxy.UpdateProxyCurves();
            }
            m_TrackDirtyVersion = m_CurvesProxy.targetTrack.DirtyIndex;
            return changeType;
        }

        public override float start
        {
            get { return 0.0f; }
        }

        public override float timeScale
        {
            get { return 1.0f; }
        }

        public override string groupingName
        {
            get { return k_GroupingName; }
        }

        public override void RemoveCurves(IEnumerable<EditorCurveBinding> bindings)
        {
            m_CurvesProxy.RemoveCurves(bindings);
        }

        public override void ApplyCurveChanges(IEnumerable<CurveWrapper> updatedCurves)
        {
            m_CurvesProxy.UpdateCurves(updatedCurves);
        }

        protected override void ConfigureCurveWrapper(CurveWrapper wrapper)
        {
            m_CurvesProxy.ConfigureCurveWrapper(wrapper);
        }

        private AnimationClip sourceAnimationClip
        {
            get
            {
                if (m_CurvesProxy.targetTrack == null || m_CurvesProxy.targetTrack.curves == null)
                    return null;
                return m_CurvesProxy.targetTrack.curves;
            }
        }
    }
}
