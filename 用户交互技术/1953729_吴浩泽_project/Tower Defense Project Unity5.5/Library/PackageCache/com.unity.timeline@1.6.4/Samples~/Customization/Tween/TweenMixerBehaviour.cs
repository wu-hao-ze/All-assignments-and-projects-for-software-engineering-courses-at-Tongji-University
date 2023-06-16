using UnityEngine;
using UnityEngine.Playables;

namespace Timeline.Samples
{
    // The runtime instance of a Tween track. It is responsible for blending and setting
    // the final data on the transform binding.
    public class TweenMixerBehaviour : PlayableBehaviour
    {
        static AnimationCurve s_DefaultCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

        bool m_ShouldInitializeTransform = true;
        Vector3 m_InitialPosition;
        Quaternion m_InitialRotation;

        // Performs blend of position and rotation of all clips connected to a track mixer
        // The result is applied to the track binding's (playerData) transform.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            Transform trackBinding = playerData as Transform;

            if (trackBinding == null)
                return;

            // Get the initial position and rotation of the track binding, only when ProcessFrame is first called
            InitializeIfNecessary(trackBinding);

            Vector3 accumPosition = Vector3.zero;
            Quaternion accumRotation = QuaternionUtils.zero;

            float totalPositionWeight = 0.0f;
            float totalRotationWeight = 0.0f;

            // Iterate on all mixer's inputs (ie each clip on the track)
            int inputCount = playable.GetInputCount();
            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                if (inputWeight <= 0)
                    continue;

                Playable input = playable.GetInput(i);
                float normalizedInputTime = (float)(input.GetTime() / input.GetDuration());

                // get the clip's behaviour and evaluate the progression along the curve
                TweenBehaviour tweenInput = GetTweenBehaviour(input);
                float tweenProgress = GetCurve(tweenInput).Evaluate(normalizedInputTime);

                // calculate the position's progression along the curve according to the input's (clip) weight
                if (tweenInput.shouldTweenPosition)
                {
                    totalPositionWeight += inputWeight;
                    accumPosition += TweenPosition(tweenInput, tweenProgress, inputWeight);
                }

                // calculate the rotation's progression along the curve according to the input's (clip) weight
                if (tweenInput.shouldTweenRotation)
                {
                    totalRotationWeight += inputWeight;
                    accumRotation = TweenRotation(tweenInput, accumRotation, tweenProgress, inputWeight);
                }
            }

            // Apply the final position and rotation values in the track binding
            trackBinding.position = accumPosition + m_InitialPosition * (1.0f - totalPositionWeight);
            trackBinding.rotation = accumRotation.Blend(m_InitialRotation, 1.0f - totalRotationWeight);
            trackBinding.rotation.Normalize();
        }

        void InitializeIfNecessary(Transform transform)
        {
            if (m_ShouldInitializeTransform)
            {
                m_InitialPosition = transform.position;
                m_InitialRotation = transform.rotation;
                m_ShouldInitializeTransform = false;
            }
        }

        Vector3 TweenPosition(TweenBehaviour tweenInput, float progress, float weight)
        {
            Vector3 startPosition = m_InitialPosition;
            if (tweenInput.startLocation != null)
            {
                startPosition = tweenInput.startLocation.position;
            }

            Vector3 endPosition = m_InitialPosition;
            if (tweenInput.endLocation != null)
            {
                endPosition = tweenInput.endLocation.position;
            }

            return Vector3.Lerp(startPosition, endPosition, progress) * weight;
        }

        Quaternion TweenRotation(TweenBehaviour tweenInput, Quaternion accumRotation, float progress, float weight)
        {
            Quaternion startRotation = m_InitialRotation;
            if (tweenInput.startLocation != null)
            {
                startRotation = tweenInput.startLocation.rotation;
            }

            Quaternion endRotation = m_InitialRotation;
            if (tweenInput.endLocation != null)
            {
                endRotation = tweenInput.endLocation.rotation;
            }

            Quaternion desiredRotation = Quaternion.Lerp(startRotation, endRotation, progress);
            return accumRotation.Blend(desiredRotation.NormalizeSafe(), weight);
        }

        static TweenBehaviour GetTweenBehaviour(Playable playable)
        {
            ScriptPlayable<TweenBehaviour> tweenInput = (ScriptPlayable<TweenBehaviour>)playable;
            return tweenInput.GetBehaviour();
        }

        static AnimationCurve GetCurve(TweenBehaviour tween)
        {
            if (tween == null || tween.curve == null)
                return s_DefaultCurve;
            return tween.curve;
        }
    }
}
