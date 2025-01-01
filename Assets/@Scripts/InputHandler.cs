using System;
using Clicker.Utils;
using UnityEngine;

namespace Scripts
{
    public static class InputHandler
    {
        public static Action<Vector2> onDragAction;
        public static Action<Vector2> onPointerUpAction;
        public static Action<Define.EUIEvent> onChangedUIEvent;
    }
}