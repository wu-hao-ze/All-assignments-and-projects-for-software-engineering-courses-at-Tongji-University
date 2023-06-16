using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
#if MODULE_PARTICLE_SYSTEM_EXISTS
    /// <summary>
    /// Called when a particle hits a collider.
    /// </summary>
    [UnitCategory("Events/Physics")]
    public sealed class OnParticleCollision : GameObjectEventUnit<GameObject>
    {
        public override Type MessageListenerType => typeof(UnityOnParticleCollisionMessageListener);
        protected override string hookName => EventHooks.OnParticleCollision;

        /// <summary>
        /// A game object with an attached collider struck by the particle system.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput other { get; private set; }

        /// <summary>
        /// The particle collision events.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput collisionEvents { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            other = ValueOutput<GameObject>(nameof(other));

            collisionEvents = ValueOutput<List<ParticleCollisionEvent>>(nameof(collisionEvents));
        }

        protected override void AssignArguments(Flow flow, GameObject other)
        {
            flow.SetValue(this.other, other);

            var collisionEvents = new List<ParticleCollisionEvent>();

            var data = flow.stack.GetElementData<Data>(this);

            data.target.GetComponent<ParticleSystem>().GetCollisionEvents(other, collisionEvents);

            flow.SetValue(this.collisionEvents, collisionEvents);
        }
    }
#endif
}
