using System;
using System.Collections.Generic;

namespace Clicker.Manager
{
    [Serializable]
    public class QuestData
    {
        public int questId;
        public int questType;
        public int completedValued;
        public int rewardItemId;
        public string description;
        public int nextQuestId;
    }
    
    public class QuestTask
    {
        private int _completedValue;
        private int _currentValue;

        public QuestTask(int completedValue)
        {
            _completedValue = completedValue;
        }

        public void AddValue(int value)
        {
            _currentValue += value;
        }

        public bool IsCompleted()
        {
            return _currentValue >= _completedValue;
        }

    }
    
    public class Quest
    {
        private QuestSaveData _saveData;
        
        private Queue<QuestTask> _task;
        private QuestData _questData;
        
        public Quest(QuestData questData)
        {
            _questData = questData;
            AddTask();
        }

        public void AddTask()
        {
            while (true)
            {
                QuestTask task = new QuestTask(_questData.questId);
                _task.Enqueue(task);

                if (_questData.nextQuestId == 0)
                {
                    break;
                }
            }
        }

        public void ProcessTask()
        {
            

        }

        public bool IsAllCompleted()
        {
            return false;
        }
    }
    
    public class QuestManager
    {
        
        
        
    }
}