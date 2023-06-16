using UnityEngine;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_2D_EXISTS
    [UnitCategory("Events/Physics 2D")]
    public abstract class TriggerEvent2DUnit : GameObjectEventUnit<Collider2D>
    {
        /// <summary>
        /// The other collider involved in the collision.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput collider { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            collider = ValueOutput<Collider2D>(nameof(collider));
        }

        protected override void AssignArguments(Flow flow, Collider2D other)
        {
            flow.SetValue(collider, other);
        }
    }
#endif
}
