using UnityEngine;

namespace Unity.VisualScripting
{
    /*
     * It seems that Unity disables all motion and collision code handling usually
     * done by the animator if the game object has a MonoBehaviour with an
     * OnAnimatorMove or OnAnimatorIK movement. This is entirely undocumented,
     * but other people have had similar issues:
     * https://forum.unity.com/threads/root-animation-doesnt-move.457638/
     * https://answers.unity.com/questions/1002771/apply-rootmotion-on-onanimatormove.html
     * https://www.reddit.com/r/Unity3D/comments/2egh14/onanimatormove_overrides_collision/
     * http://hutonggames.com/playmakerforum/index.php?topic=3590.msg16555#msg16555
     * The only solution seems to be to separate the listener as a manual component.
     */
    [AddComponentMenu("Visual Scripting/Listeners/Animator Message Listener")]
    public sealed class AnimatorMessageListener : MonoBehaviour
    {
        private void OnAnimatorMove()
        {
            EventBus.Trigger(EventHooks.OnAnimatorMove, gameObject);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            EventBus.Trigger(EventHooks.OnAnimatorIK, gameObject, layerIndex);
        }
    }
}
