using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Timeline;
using UnityObject = UnityEngine.Object;

namespace UnityEditor.Timeline
{
    class CurvesProxy : ICurvesOwner
    {
        public AnimationClip curves
        {
            get { return proxyCurves != null ? proxyCurves : m_OriginalOwner.curves; }
        }

        public bool hasCurves
        {
            get { return m_IsAnimatable || m_OriginalOwner.hasCurves; }
        }

        public double duration
        {
            get { return m_OriginalOwner.duration; }
        }

        public string defaultCurvesName
        {
            get { return m_OriginalOwner.defaultCurvesName; }
        }

        public UnityObject asset
        {
            get { return m_OriginalOwner.asset; }
        }

        public UnityObject assetOwner
        {
            get { return m_OriginalOwner.assetOwner; }
        }

        public TrackAsset targetTrack
        {
            get { return m_OriginalOwner.targetTrack; }
        }

        readonly ICurvesOwner m_OriginalOwner;
        readonly bool m_IsAnimatable;
        readonly Dictionary<EditorCurveBinding, SerializedProperty> m_PropertiesMap = new Dictionary<EditorCurveBinding, SerializedProperty>();
        int m_ProxyIsRebuilding = 0;

        AnimationClip m_ProxyCurves;
        AnimationClip proxyCurves
        {
            get
            {
                if (!m_IsAnimatable) return null;

                if (m_ProxyCurves == null)
                    RebuildProxyCurves();

                return m_ProxyCurves;
            }
        }

        public CurvesProxy([NotNull] ICurvesOwner originalOwner)
        {
            m_OriginalOwner = originalOwner;
            m_IsAnimatable = originalOwner.HasAnyAnimatableParameters();

            RebuildProxyCurves();
        }

        public void CreateCurves(string curvesClipName)
        {
            m_OriginalOwner.CreateCurves(curvesClipName);
            TimelineEditor.window.state.rebuildGraph = true;
        }

        public void ConfigureCurveWrapper(CurveWrapper wrapper)
        {
            var color = CurveUtility.GetPropertyColor(wrapper.binding.propertyName);
            wrapper.color = color;

            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            wrapper.wrapColorMultiplier = Color.HSVToRGB(h, s * 0.33f, v * 1.15f);

            var curve = AnimationUtility.GetEditorCurve(proxyCurves, wrapper.binding);

            wrapper.renderer = new NormalCurveRenderer(curve);

            // Use curve length instead of animation clip length
            wrapper.renderer.SetCustomRange(0.0f, curve.keys.Last().time);
        }

        public void RebuildCurves()
        {
            RebuildProxyCurves();
        }

        public void RemoveCurves(IEnumerable<EditorCurveBinding> bindings)
        {
            if (m_ProxyIsRebuilding > 0 || !m_OriginalOwner.hasCurves)
                return;

            Undo.RegisterCompleteObjectUndo(m_OriginalOwner.curves, L10n.Tr("Remove Clip Curve"));
            foreach (var binding in bindings)
                AnimationUtility.SetEditorCurve(m_OriginalOwner.curves, binding, null);
            m_OriginalOwner.SanitizeCurvesData();
            RebuildProxyCurves();
        }

        public void UpdateCurves(IEnumerable<CurveWrapper> updatedCurves)
        {
            if (m_ProxyIsRebuilding > 0)
                return;

            Undo.RegisterCompleteObjectUndo(m_OriginalOwner.asset, L10n.Tr("Edit Clip Curve"));
            if (m_OriginalOwner.curves != null)
                Undo.RegisterCompleteObjectUndo(m_OriginalOwner.curves, L10n.Tr("Edit Clip Curve"));

            var requireRebuild = false;
            foreach (var curve in updatedCurves)
            {
                requireRebuild |= curve.curve.length == 0;
                UpdateCurve(curve.binding, curve.curve);
            }

            if (requireRebuild)
                m_OriginalOwner.SanitizeCurvesData();

            AnimatedParameterUtility.UpdateSerializedPlayableAsset(m_OriginalOwner.asset);
        }

        public void ApplyExternalChangesToProxy()
        {
            using (new RebuildGuard(this))
            {
                if (m_OriginalOwner.curves == null)
                    return;

                var curveInfo = AnimationClipCurveCache.Instance.GetCurveInfo(m_OriginalOwner.curves);
                for (int i = 0; i < curveInfo.bindings.Length; i++)
                {
                    if (curveInfo.curves[i] != null && curveInfo.curves.Length != 0)
                    {
                        if (m_PropertiesMap.TryGetValue(curveInfo.bindings[i], out var prop) && AnimatedParameterUtility.IsParameterAnimatable(prop))
                            AnimationUtility.SetEditorCurve(m_ProxyCurves, curveInfo.bindings[i], curveInfo.curves[i]);
                    }
                }
            }
        }

        void UpdateCurve(EditorCurveBinding binding, AnimationCurve curve)
        {
            ApplyConstraints(binding, curve);

            if (curve.length == 0)
            {
                HandleAllKeysDeleted(binding);
                return;
            }

            // there is no curve in the animation clip, this is a proxy curve
            if (IsConstantCurve(binding, curve))
                HandleConstantCurveValueChanged(binding, curve);
            else
                HandleCurveUpdated(binding, curve);
        }

        bool IsConstantCurve(EditorCurveBinding binding, AnimationCurve curve)
        {
            if (curve.length != 1)
                return false;
            return m_OriginalOwner.curves == null || AnimationUtility.GetEditorCurve(m_OriginalOwner.curves, binding) == null;
        }

        void ApplyConstraints(EditorCurveBinding binding, AnimationCurve curve)
        {
            if (curve.length == 0)
                return;

            var curveUpdated = false;

            var property = m_PropertiesMap[binding];
            if (property.propertyType == SerializedPropertyType.Boolean)
            {
                TimelineAnimationUtilities.ConstrainCurveToBooleanValues(curve);
                curveUpdated = true;
            }
            else
            {
                var range = AnimatedParameterUtility.GetAttributeForProperty<RangeAttribute>(property);
                if (range != null)
                {
                    TimelineAnimationUtilities.ConstrainCurveToRange(curve, range.min, range.max);
                    curveUpdated = true;
                }
            }

            if (!curveUpdated)
                return;

            using (new RebuildGuard(this))
            {
                AnimationUtility.SetEditorCurve(m_ProxyCurves, binding, curve);
            }
        }

        void HandleCurveUpdated(EditorCurveBinding binding, AnimationCurve updatedCurve)
        {
            if (!m_OriginalOwner.hasCurves)
                CreateCurves(String.Empty);

            AnimationUtility.SetEditorCurve(m_OriginalOwner.curves, binding, updatedCurve);
            AnimationUtility.SetEditorCurve(m_ProxyCurves, binding, updatedCurve);
        }

        void HandleConstantCurveValueChanged(EditorCurveBinding binding, AnimationCurve updatedCurve)
        {
            var prop = m_PropertiesMap[binding];
            if (prop == null)
                return;

            Undo.RegisterCompleteObjectUndo(prop.serializedObject.targetObject, L10n.Tr("Edit Clip Curve"));
            prop.serializedObject.UpdateIfRequiredOrScript();
            CurveEditUtility.SetFromKeyValue(prop, updatedCurve.keys[0].value);
            prop.serializedObject.ApplyModifiedProperties();

            AnimationUtility.SetEditorCurve(m_ProxyCurves, binding, updatedCurve);
        }

        void HandleAllKeysDeleted(EditorCurveBinding binding)
        {
            if (m_OriginalOwner.hasCurves)
            {
                // Remove curve from original asset
                AnimationUtility.SetEditorCurve(m_OriginalOwner.curves, binding, null);
                SetProxyCurve(m_PropertiesMap[binding], binding);
            }
        }

        void RebuildProxyCurves()
        {
            if (!m_IsAnimatable)
                return;

            using (new RebuildGuard(this))
            {
                if (m_ProxyCurves == null)
                {
                    m_ProxyCurves = new AnimationClip
                    {
                        legacy = true,
                        name = "Constant Curves",
                        hideFlags = HideFlags.HideAndDontSave,
                        frameRate = m_OriginalOwner.targetTrack.timelineAsset == null
                            ? (float)TimelineAsset.EditorSettings.kDefaultFrameRate
                            : (float)m_OriginalOwner.targetTrack.timelineAsset.editorSettings.frameRate
                    };
                }
                else
                {
                    m_ProxyCurves.ClearCurves();
                }

                m_OriginalOwner.SanitizeCurvesData();
                AnimatedParameterUtility.UpdateSerializedPlayableAsset(m_OriginalOwner.asset);
                var parameters = m_OriginalOwner.GetAllAnimatableParameters().ToArray();
                foreach (var param in parameters)
                    CreateProxyCurve(param, m_ProxyCurves, m_OriginalOwner.asset, param.propertyPath);

                AnimationClipCurveCache.Instance.GetCurveInfo(m_ProxyCurves).dirty = true;
            }
        }

        // updates the just the proxied values. This can be called when the asset changes, so the proxy values are properly updated
        public void UpdateProxyCurves()
        {
            if (!m_IsAnimatable || m_ProxyCurves == null || m_ProxyCurves.empty)
                return;

            AnimatedParameterUtility.UpdateSerializedPlayableAsset(m_OriginalOwner.asset);
            var parameters = m_OriginalOwner.GetAllAnimatableParameters().ToArray();
            using (new RebuildGuard(this))
            {
                if (m_OriginalOwner.hasCurves)
                {
                    var bindingInfo = AnimationClipCurveCache.Instance.GetCurveInfo(m_OriginalOwner.curves);
                    foreach (var param in parameters)
                    {
                        var binding = AnimatedParameterUtility.GetCurveBinding(m_OriginalOwner.asset, param.propertyPath);
                        if (!bindingInfo.bindings.Contains(binding, AnimationPreviewUtilities.EditorCurveBindingComparer.Instance))
                            SetProxyCurve(param, AnimatedParameterUtility.GetCurveBinding(m_OriginalOwner.asset, param.propertyPath));
                    }
                }
                else
                {
                    foreach (var param in parameters)
                        SetProxyCurve(param, AnimatedParameterUtility.GetCurveBinding(m_OriginalOwner.asset, param.propertyPath));
                }
            }

            AnimationClipCurveCache.Instance.GetCurveInfo(m_ProxyCurves).dirty = true;
        }

        void CreateProxyCurve(SerializedProperty prop, AnimationClip clip, UnityObject owner, string propertyName)
        {
            var binding = AnimatedParameterUtility.GetCurveBinding(owner, propertyName);

            var originalCurve = m_OriginalOwner.hasCurves
                ? AnimationUtility.GetEditorCurve(m_OriginalOwner.curves, binding)
                : null;

            if (originalCurve != null)
            {
                AnimationUtility.SetEditorCurve(clip, binding, originalCurve);
            }
            else
            {
                SetProxyCurve(prop, binding);
            }

            m_PropertiesMap[binding] = prop;
        }

        void SetProxyCurve(SerializedProperty prop, EditorCurveBinding binding)
        {
            var curve = new AnimationCurve();
            CurveEditUtility.AddKeyFrameToCurve(
                curve, 0.0f, m_ProxyCurves.frameRate, CurveEditUtility.GetKeyValue(prop),
                prop.propertyType == SerializedPropertyType.Boolean);
            AnimationUtility.SetEditorCurve(m_ProxyCurves, binding, curve);
        }

        struct RebuildGuard : IDisposable
        {
            CurvesProxy m_Owner;
            AnimationUtility.OnCurveWasModified m_Callback;

            public RebuildGuard(CurvesProxy owner)
            {
                m_Callback = AnimationUtility.onCurveWasModified;
                AnimationUtility.onCurveWasModified = null;
                m_Owner = owner;
                m_Owner.m_ProxyIsRebuilding++;
            }

            public void Dispose()
            {
                AnimationUtility.onCurveWasModified = m_Callback;
                m_Owner.m_ProxyIsRebuilding--;
                m_Owner = null;
            }
        }
    }
}
