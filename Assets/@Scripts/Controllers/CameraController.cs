using System;
using System.Collections;
using System.Collections.Generic;
using Clicker.Manager;
using UnityEngine;

namespace Clicker.Controllers
{
    public class CameraController : MonoBehaviour
    {
        private Transform _target;
        private Vector3 _prevPos;

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void Update()
        {
            _prevPos = transform.position;
        }

        private void LateUpdate()
        {
            if(!_target)
            {
                return;
            }
            
            Vector3 myPos = transform.position;
            Vector3 lerp = Vector3.Lerp(myPos, _target.position, Time.deltaTime * 10f);

            lerp.z = -10;
            transform.position = lerp;
        }
    }
}