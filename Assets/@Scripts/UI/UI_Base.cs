using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clicker.UI
{
    public class UI_Base : MonoBehaviour
    {
        protected bool _initialized = false;

        public virtual bool Init()
        {
            if (_initialized)
            {
                return false;
            }

            _initialized = true;
            return true;
        }

        protected virtual void Awake()
        {
            Init();
        }
    }
}