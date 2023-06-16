using System;
using UnityEngine;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_EXISTS
    /// <summary>
    /// Called when the controller hits a collider while performing a move.
    /// </summary>
    [UnitCategory("Events/Physics")]
    [TypeIcon(typeof(CharacterController))]
    public sealed class OnControllerColliderHit : GameObjectEventUnit<ControllerColliderHit>
    {
        public override Type MessageListenerType => typeof(UnityOnControllerColliderHitMessageListener);
        protected override string hookName => EventHooks.OnControllerColliderHit;

        /// <summary>
        /// The collider that was hit by the controller.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput collider { get; private set; }

        /// <summary>
        /// The controller that hit the collider.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput controller { get; private set; }

        /// <summary>
        /// The direction the CharacterController was moving in when the collision occured.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput moveDirection { get; private set; }

        /// <summary>
        /// How far the character has travelled until it hit the collider.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput moveLength { get; private set; }

        /// <summary>
        /// The normal of the surface we collided with in world space.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput normal { get; private set; }

        /// <summary>
        /// The impact point in world space.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput point { get; private set; }

        /// <summary>
        /// The impact point in world space.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput data { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            collider = ValueOutput<Collider>(nameof(collider));
            controller = ValueOutput<CharacterController>(nameof(controller));
            moveDirection = ValueOutput<Vector3>(nameof(moveDirection));
            moveLength = ValueOutput<float>(nameof(moveLength));
            normal = ValueOutput<Vector3>(nameof(normal));
            point = ValueOutput<Vector3>(nameof(point));
            data = ValueOutput<ControllerColliderHit>(nameof(data));
        }

        protected override void AssignArguments(Flow flow, ControllerColliderHit hitData)
        {
            flow.SetValue(collider, hitData.collider);
            flow.SetValue(controller, hitData.controller);
            flow.SetValue(moveDirection, hitData.moveDirection);
            flow.SetValue(moveLength, hitData.moveLength);
            flow.SetValue(normal, hitData.normal);
            flow.SetValue(point, hitData.point);
            flow.SetValue(data, hitData);
        }
    }
#endif
}
