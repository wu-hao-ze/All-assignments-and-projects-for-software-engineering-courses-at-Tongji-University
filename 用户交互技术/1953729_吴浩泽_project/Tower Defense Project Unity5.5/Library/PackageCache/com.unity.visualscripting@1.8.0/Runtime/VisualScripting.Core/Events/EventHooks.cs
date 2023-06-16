namespace Unity.VisualScripting
{
    public static class EventHooks
    {
        // Bolt
        public const string Custom = nameof(Custom);

        // Global
        public const string OnGUI = nameof(OnGUI);
        public const string OnApplicationFocus = nameof(OnApplicationFocus);
        public const string OnApplicationLostFocus = nameof(OnApplicationLostFocus);
        public const string OnApplicationPause = nameof(OnApplicationPause);
        public const string OnApplicationResume = nameof(OnApplicationResume);
        public const string OnApplicationQuit = nameof(OnApplicationQuit);

        // Lifecycle
        public const string OnEnable = nameof(OnEnable);
        public const string Start = nameof(Start);
        public const string Update = nameof(Update);
        public const string FixedUpdate = nameof(FixedUpdate);
        public const string LateUpdate = nameof(LateUpdate);
        public const string OnDisable = nameof(OnDisable);
        public const string OnDestroy = nameof(OnDestroy);

        // External
        public const string AnimationEvent = nameof(AnimationEvent);
        public const string UnityEvent = nameof(UnityEvent);

        // Editor
        public const string OnDrawGizmos = nameof(OnDrawGizmos);
        public const string OnDrawGizmosSelected = nameof(OnDrawGizmosSelected);

        // Game Object
        public const string OnPointerEnter = nameof(OnPointerEnter);
        public const string OnPointerExit = nameof(OnPointerExit);
        public const string OnPointerDown = nameof(OnPointerDown);
        public const string OnPointerUp = nameof(OnPointerUp);
        public const string OnPointerClick = nameof(OnPointerClick);
        public const string OnBeginDrag = nameof(OnBeginDrag);
        public const string OnDrag = nameof(OnDrag);
        public const string OnEndDrag = nameof(OnEndDrag);
        public const string OnDrop = nameof(OnDrop);
        public const string OnScroll = nameof(OnScroll);
        public const string OnSelect = nameof(OnSelect);
        public const string OnDeselect = nameof(OnDeselect);
        public const string OnSubmit = nameof(OnSubmit);
        public const string OnCancel = nameof(OnCancel);
        public const string OnMove = nameof(OnMove);
        public const string OnBecameInvisible = nameof(OnBecameInvisible);
        public const string OnBecameVisible = nameof(OnBecameVisible);
        public const string OnCollisionEnter = nameof(OnCollisionEnter);
        public const string OnCollisionExit = nameof(OnCollisionExit);
        public const string OnCollisionStay = nameof(OnCollisionStay);
        public const string OnCollisionEnter2D = nameof(OnCollisionEnter2D);
        public const string OnCollisionExit2D = nameof(OnCollisionExit2D);
        public const string OnCollisionStay2D = nameof(OnCollisionStay2D);
        public const string OnControllerColliderHit = nameof(OnControllerColliderHit);
        public const string OnJointBreak = nameof(OnJointBreak);
        public const string OnJointBreak2D = nameof(OnJointBreak2D);
        public const string OnMouseDown = nameof(OnMouseDown);
        public const string OnMouseDrag = nameof(OnMouseDrag);
        public const string OnMouseEnter = nameof(OnMouseEnter);
        public const string OnMouseExit = nameof(OnMouseExit);
        public const string OnMouseOver = nameof(OnMouseOver);
        public const string OnMouseUp = nameof(OnMouseUp);
        public const string OnMouseUpAsButton = nameof(OnMouseUpAsButton);
        public const string OnParticleCollision = nameof(OnParticleCollision);
        public const string OnTransformChildrenChanged = nameof(OnTransformChildrenChanged);
        public const string OnTransformParentChanged = nameof(OnTransformParentChanged);
        public const string OnTriggerEnter = nameof(OnTriggerEnter);
        public const string OnTriggerExit = nameof(OnTriggerExit);
        public const string OnTriggerStay = nameof(OnTriggerStay);
        public const string OnTriggerEnter2D = nameof(OnTriggerEnter2D);
        public const string OnTriggerExit2D = nameof(OnTriggerExit2D);
        public const string OnTriggerStay2D = nameof(OnTriggerStay2D);
        public const string OnAnimatorMove = nameof(OnAnimatorMove);
        public const string OnAnimatorIK = nameof(OnAnimatorIK);

        // GUI
        public const string OnButtonClick = nameof(OnButtonClick);
        public const string OnToggleValueChanged = nameof(OnToggleValueChanged);
        public const string OnSliderValueChanged = nameof(OnSliderValueChanged);
        public const string OnScrollbarValueChanged = nameof(OnScrollbarValueChanged);
        public const string OnDropdownValueChanged = nameof(OnDropdownValueChanged);
        public const string OnInputFieldValueChanged = nameof(OnInputFieldValueChanged);
        public const string OnInputFieldEndEdit = nameof(OnInputFieldEndEdit);
        public const string OnScrollRectValueChanged = nameof(OnScrollRectValueChanged);
    }
}
