using System.Collections.Generic;
using UnityEngine;

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

        public bool IsInStageInRange(Vector3 position) => _stageList[_currentStageIndex].IsStageInRange(position);

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

        public void ChangeStage(Vector3 position)
        {
            for (var index = 0; index < _stageList.Count; index++)
            {
                var stage = _stageList[index];
                bool isInStage = stage.IsStageInRange(position);
                if (isInStage)
                {
                    OnChangedMap(index);
                    break;
                }
            }
        }

        public Vector3 GetWayPosition() => _stageList[_currentStageIndex].GetWaypointPosition();
        
        public void OnChangedMap(int stageIndex)
        {
            _currentStageIndex = stageIndex;

            for (int i = stageIndex - 1; i <= stageIndex + 1; i++)
            {
                if (i < 0 || i >= _stageList.Count)
                {
                    continue;
                }
                
                _stageList[i].SpawnObject();
            }

            for (int i = 0; i < _stageList.Count; i++)
            {
                if (i >= _currentStageIndex - 1 && i <= _currentStageIndex + 1)
                {
                    continue;
                }
                
                _stageList[i].DisableObject();
            }
        }
        
    }
}