using System.Collections;
using System.Collections.Generic;
using Clicker.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Clicker
{
    public class StageTranslation : MonoBehaviour
    {
        public List<Stage> StageList { get { return _stageList; } }
        
        [SerializeField] private List<Stage> _stageList = new List<Stage>();

        public void SetInfo()
        {
            foreach (Stage stage in _stageList)
            {
                stage.SetInfo();
            }
        }
        
    }
}