using System;
using UnityEngine;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_2D_EXISTS
    /// <summary>
    /// Called when a joint attached to the same game object broke.
    /// </summary>
    [UnitCategory("Events/Physics 2D")]
    public sealed class OnJointBreak2D : GameObjectEventUnit<Joint2D>
    {
        public override Type MessageListenerType => typeof(UnityOnJointBreak2DMessageListener);

        protected override string hookName => EventHooks.OnJointBreak2D;

        /// <summary>
        /// The force that needs to be applied for the joint that broke to break.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput breakForce { get; private set; }

        /// <summary>
        /// The torque that needs to be applied for the joint that broke to break.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput breakTorque { get; private set; }

        /// <summary>
        /// The 2D rigidbody to which the other end of the joint is attached (ie, the object without the joint component).
        /// </summary>
        [DoNotSerialize]
        public ValueOutput connectedBody { get; private set; }

        /// <summary>
        /// The reaction force of the joint that broke.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput reactionForce { get; private set; }

        /// <summary>
        /// The reaction torque of the joint that broke.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput reactionTorque { get; private set; }

        /// <summary>
        /// The joint that broke.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput joint { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            breakForce = ValueOutput<float>(nameof(breakForce));
            breakTorque = ValueOutput<float>(nameof(breakTorque));
            connectedBody = ValueOutput<Rigidbody2D>(nameof(connectedBody));
            reactionForce = ValueOutput<Vector2>(nameof(reactionForce));
            reactionTorque = ValueOutput<float>(nameof(reactionTorque));
            joint = ValueOutput<Joint2D>(nameof(joint));
        }

        protected override void AssignArguments(Flow flow, Joint2D joint)
        {
            flow.SetValue(breakForce, joint.breakForce);
            flow.SetValue(breakTorque, joint.breakTorque);
            flow.SetValue(connectedBody, joint.connectedBody);
            flow.SetValue(reactionForce, joint.reactionForce);
            flow.SetValue(reactionTorque, joint.reactionTorque);
            flow.SetValue(this.joint, joint);
        }
    }
#endif
}
