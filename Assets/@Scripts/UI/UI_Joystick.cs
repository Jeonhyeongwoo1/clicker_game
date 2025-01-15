using System.Collections;
using System.Collections.Generic;
using Clicker.UI.Popup;
using Clicker.Utils;
using Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Clicker.UI.Panel
{
    public class UI_Joystick : UI_Base, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private RectTransform _joystickBg;
        [SerializeField] private RectTransform _joystick;
        [SerializeField] private CanvasGroup _canvasGroup;

        private Define.EUIEvent _euiEvent;
        private Vector2 _inputVector;
        
        public override bool Init()
        {
            return base.Init();
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos;
            // 배경 안에서 핸들의 위치 계산
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _joystickBg,
                eventData.position,
                eventData.pressEventCamera,
                out pos
            );

            pos.x = (pos.x / _joystickBg.sizeDelta.x);
            pos.y = (pos.y / _joystickBg.sizeDelta.y);

            // _handlerRectTransform.anchoredPosition = pos;
            _inputVector = new Vector2(pos.x * 2, pos.y * 2);
            _inputVector = (_inputVector.magnitude > 1.0f) ? _inputVector.normalized : _inputVector;

            // Debug.Log(_inputVector);
            _joystick.anchoredPosition = _joystickBg.anchoredPosition + new Vector2(
                _inputVector.x * (_joystickBg.sizeDelta.x / 2),
                _inputVector.y * (_joystickBg.sizeDelta.y / 2)
            );
            
            SetEuiEvent(Define.EUIEvent.Drag);
            InputHandler.onDragAction?.Invoke(_inputVector);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SetEuiEvent(Define.EUIEvent.PointerDown);
            Vector2 beginPosition = eventData.pressEventCamera.ScreenToWorldPoint(eventData.position);
            _inputVector = beginPosition;
            _joystickBg.position = beginPosition;
            _joystick.position = beginPosition;
            _canvasGroup.alpha = 1;
        }

        private void SetEuiEvent(Define.EUIEvent euiEvent)
        {
            // if (_euiEvent == euiEvent)
            // {
            //     return;
            // }

            _euiEvent = euiEvent;
            InputHandler.onChangedUIEvent.Invoke(_euiEvent);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            InputHandler.onPointerUpAction?.Invoke(Vector2.zero);
            _canvasGroup.alpha = 0f;
            SetEuiEvent(Define.EUIEvent.PointerUp);
        }
    }
}