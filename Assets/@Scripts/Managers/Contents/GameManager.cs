using Clicker.Controllers;
using Clicker.Entity;
using UnityEngine;

namespace Clicker.Manager
{
    public class GameManager
    {
        private HeroCamp _heroCamp;
        public HeroCamp HeroCamp => _heroCamp;
        
        private CameraController _cameraController;
        
        public HeroCamp CreateHeroCamp()
        {
            GameObject prefab = Managers.Resource.Instantiate("HeroCamp");
            _heroCamp = prefab.GetComponent<HeroCamp>();
            
            if (_cameraController == null)
            {
                _cameraController = GameObject.FindObjectOfType<CameraController>();
            }

            _cameraController.SetTarget(_heroCamp.transform);
            return _heroCamp;
        }
    }
}