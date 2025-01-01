using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Clicker.GameComponent
{
    public class CameraGroupComponent : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField] private CinemachineTargetGroup _targetGroup;

        private void Awake()
        {
            _camera.Follow = _targetGroup.transform;
            _camera.LookAt = _targetGroup.transform;
            var groupComposer = _camera.GetCinemachineComponent<CinemachineGroupComposer>();
            if (groupComposer == null)
            {
                Debug.LogError("CinemachineGroupComposer is not set on the virtual camera.");
                return;
            }

            groupComposer.m_MinimumOrthoSize = 15;
        }

        public void AddViewTarget(Transform target)
        {
            _targetGroup.AddMember(target, 1, 1);
        }
    }
}
