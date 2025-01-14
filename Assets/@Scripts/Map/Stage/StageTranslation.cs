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
        private int _currentStageIndex;
        
        public void SetInfo()
        {
            foreach (Stage stage in _stageList)
            {
                stage.SetInfo();
            }

            _currentStageIndex = 1;
            OnChangedMap(_currentStageIndex);
        }

        public bool TryChangeStage(string name)
        {
            for (var i = 0; i < _stageList.Count; i++)
            {
                if (_stageList[i].name == name && _currentStageIndex != i)
                {
                    OnChangedMap(i);
                    return true;
                }
            }

            return false;
        }

        public Vector3 GetWayPosition() => _stageList[_currentStageIndex].GetWaypointPosition();
        
        public void OnChangedMap(int stageIndex)
        {
            //기존에 있는건 모두 끈다
            foreach (Stage stage in _stageList)
            {
                stage.DisableObject();
            }

            //이전 맵과 다음 맵을 제외한 나머지는 Disable
            for (int i = stageIndex - 1; i <= stageIndex + 1; i++)
            {
                if (i < 0 || i >= _stageList.Count)
                {
                    continue;
                }
                
                _stageList[i].SpawnObject();
            }

            _currentStageIndex = stageIndex;
        }
        
    }
}