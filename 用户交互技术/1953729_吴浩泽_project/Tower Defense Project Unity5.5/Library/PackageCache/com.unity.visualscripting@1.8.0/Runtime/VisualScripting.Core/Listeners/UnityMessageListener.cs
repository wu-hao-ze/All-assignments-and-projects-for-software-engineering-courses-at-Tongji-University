using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.VisualScripting
{
    [AddComponentMenu("")]
    [Obsolete("UnityMessageListener is deprecated and has been replaced by separate message listeners for each event, eg. UnityOnCollisionEnterMessageListener or UnityOnButtonClickMessageListener.")]
    public sealed class UnityMessageListener : MessageListener,
                                               IPointerEnterHandler,
                                               IPointerExitHandler,
                                               IPointerDownHandler,
                                               IPointerUpHandler,
                                               IPointerClickHandler,

                                               IBeginDragHandler,
                                               IDragHandler,
                                               IEndDragHandler,
                                               IDropHandler,
                                               IScrollHandler,

                                               ISelectHandler,
                                               IDeselectHandler,

                                               ISubmitHandler,
                                               ICancelHandler,

                                               IMoveHandler
    {
        private void Start()
        {
            AddGUIListeners();
        }

        #region GUI

        public void AddGUIListeners()
        {
            GetComponent<Button>()?.onClick?.AddListener(() => EventBus.Trigger(EventHooks.OnButtonClick, gameObject));
            GetComponent<Toggle>()?.onValueChanged?.AddListener((value) => EventBus.Trigger(EventHooks.OnToggleValueChanged, gameObject, value));
            GetComponent<Slider>()?.onValueChanged?.AddListener((value) => EventBus.Trigger(EventHooks.OnSliderValueChanged, gameObject, value));
            GetComponent<Scrollbar>()?.onValueChanged?.AddListener((value) => EventBus.Trigger(EventHooks.OnScrollbarValueChanged, gameObject, value));
            GetComponent<Dropdown>()?.onValueChanged?.AddListener((value) => EventBus.Trigger(EventHooks.OnDropdownValueChanged, gameObject, value));
            GetComponent<InputField>()?.onValueChanged?.AddListener((value) => EventBus.Trigger(EventHooks.OnInputFieldValueChanged, gameObject, value));
            GetComponent<InputField>()?.onEndEdit?.AddListener((value) => EventBus.Trigger(EventHooks.OnInputFieldEndEdit, gameObject, value));
            GetComponent<ScrollRect>()?.onValueChanged?.AddListener((value) => EventBus.Trigger(EventHooks.OnScrollRectValueChanged, gameObject, value));
        }

        // TODO: Profile performance on these. If they add significant overhead,
        // we should detect whether there is a GUI component on the object before
        // adding a separate UnityEventSystemListener component with these interfaces

        public void OnPointerEnter(PointerEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnPointerEnter, gameObject, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnPointerExit, gameObject, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnPointerDown, gameObject, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnPointerUp, gameObject, eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnPointerClick, gameObject, eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnBeginDrag, gameObject, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnDrag, gameObject, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnEndDrag, gameObject, eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnDrop, gameObject, eventData);
        }

        public void OnScroll(PointerEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnScroll, gameObject, eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnSelect, gameObject, eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnDeselect, gameObject, eventData);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnSubmit, gameObject, eventData);
        }

        public void OnCancel(BaseEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnCancel, gameObject, eventData);
        }

        public void OnMove(AxisEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnMove, gameObject, eventData);
        }

        #endregion

        private void OnBecameInvisible()
        {
            EventBus.Trigger(EventHooks.OnBecameInvisible, gameObject);
        }

        private void OnBecameVisible()
        {
            EventBus.Trigger(EventHooks.OnBecameVisible, gameObject);
        }

#if MODULE_PHYSICS_EXISTS
        private void OnCollisionEnter(Collision collision)
        {
            EventBus.Trigger(EventHooks.OnCollisionEnter, gameObject, collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            EventBus.Trigger(EventHooks.OnCollisionExit, gameObject, collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            EventBus.Trigger(EventHooks.OnCollisionStay, gameObject, collision); ;
        }
#endif

#if MODULE_PHYSICS_2D_EXISTS
        private void OnCollisionEnter2D(Collision2D collision)
        {
            EventBus.Trigger(EventHooks.OnCollisionEnter2D, gameObject, collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            EventBus.Trigger(EventHooks.OnCollisionExit2D, gameObject, collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            EventBus.Trigger(EventHooks.OnCollisionStay2D, gameObject, collision);
        }
#endif

#if MODULE_PHYSICS_EXISTS
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            EventBus.Trigger(EventHooks.OnControllerColliderHit, gameObject, hit);
        }
#endif

        private void OnJointBreak(float breakForce)
        {
            EventBus.Trigger(EventHooks.OnJointBreak, gameObject, breakForce);
        }

#if MODULE_PHYSICS_2D_EXISTS
        private void OnJointBreak2D(Joint2D brokenJoint)
        {
            EventBus.Trigger(EventHooks.OnJointBreak2D, gameObject, brokenJoint);
        }
#endif

        private void OnMouseDown()
        {
            EventBus.Trigger(EventHooks.OnMouseDown, gameObject);
        }

        private void OnMouseDrag()
        {
            EventBus.Trigger(EventHooks.OnMouseDrag, gameObject);
        }

        private void OnMouseEnter()
        {
            EventBus.Trigger(EventHooks.OnMouseEnter, gameObject);
        }

        private void OnMouseExit()
        {
            EventBus.Trigger(EventHooks.OnMouseExit, gameObject);
        }

        private void OnMouseOver()
        {
            EventBus.Trigger(EventHooks.OnMouseOver, gameObject);
        }

        private void OnMouseUp()
        {
            EventBus.Trigger(EventHooks.OnMouseUp, gameObject);
        }

        private void OnMouseUpAsButton()
        {
            EventBus.Trigger(EventHooks.OnMouseUpAsButton, gameObject);
        }

        private void OnParticleCollision(GameObject other)
        {
            EventBus.Trigger(EventHooks.OnParticleCollision, gameObject, other);
        }

        private void OnTransformChildrenChanged()
        {
            EventBus.Trigger(EventHooks.OnTransformChildrenChanged, gameObject);
        }

        private void OnTransformParentChanged()
        {
            EventBus.Trigger(EventHooks.OnTransformParentChanged, gameObject);
        }

#if MODULE_PHYSICS_EXISTS
        private void OnTriggerEnter(Collider other)
        {
            EventBus.Trigger(EventHooks.OnTriggerEnter, gameObject, other);
        }

        private void OnTriggerExit(Collider other)
        {
            EventBus.Trigger(EventHooks.OnTriggerExit, gameObject, other);
        }

        private void OnTriggerStay(Collider other)
        {
            EventBus.Trigger(EventHooks.OnTriggerStay, gameObject, other);
        }
#endif

#if MODULE_PHYSICS_2D_EXISTS
        private void OnTriggerEnter2D(Collider2D other)
        {
            EventBus.Trigger(EventHooks.OnTriggerEnter2D, gameObject, other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            EventBus.Trigger(EventHooks.OnTriggerExit2D, gameObject, other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            EventBus.Trigger(EventHooks.OnTriggerStay2D, gameObject, other);
        }
#endif
    }
}
