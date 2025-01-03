using Clicker.Controllers;
using Clicker.GameComponent;
using UnityEngine;

namespace Clicker.Manager
{
    public class CameraManager
    {
        private CameraGroupComponent _cameraGroupComponent;
        private CameraController _cameraController;

        public void SetTarget(Transform target)
        {
            if (!_cameraController)
            {
                _cameraController = GameObject.FindObjectOfType<CameraController>();
            }
            
            _cameraController.SetTarget(target);
        }
        
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