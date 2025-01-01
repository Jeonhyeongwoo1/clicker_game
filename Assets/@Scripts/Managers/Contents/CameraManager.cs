using Clicker.GameComponent;
using UnityEngine;

namespace Clicker.Manager
{
    public class CameraManager
    {
        private CameraGroupComponent _cameraGroupComponent;

        public void AddViewTarget(Transform target)
        {
            if (_cameraGroupComponent == null)
            {
                _cameraGroupComponent = GameObject.FindObjectOfType<CameraGroupComponent>();
            }
            
            _cameraGroupComponent.AddViewTarget(target);
        }
    }
}