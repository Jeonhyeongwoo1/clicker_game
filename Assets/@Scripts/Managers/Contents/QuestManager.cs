using System;
using System.Collections.Generic;
using Clicker.ContentData;
using Clicker.Utils;

namespace Clicker.Manager
{
    public class QuestTask
    {
        public Define.EQuestStateType QuestStateTypeType => _questTaskSaveData.stateType;
        
        private QuestTaskSaveData _questTaskSaveData;
        private QuestTaskData _questTask;
        
        public QuestTask(QuestTaskData questTaskData, QuestTaskSaveData questTaskSaveData)
        {
            _questTask = questTaskData;
            _questTaskSaveData = questTaskSaveData;
        }

        public void AddValue(Define.EBroadcastEventType eventType, int value)
        {
            Define.EQuestObjectiveType type = (Define.EQuestObjectiveType)_questTask.ObjectiveDataId;
            switch (eventType)
            {
                case Define.EBroadcastEventType.ChangeMeat:
                    if (Define.EQuestObjectiveType.EarnMeat == type
                        || Define.EQuestObjectiveType.SpendMeat == type)
                    {
                        _questTaskSaveData.currentValue += value;
                    }

                    break;
                case Define.EBroadcastEventType.ChangeWood:
                    if (Define.EQuestObjectiveType.EarnWood == type
                        || Define.EQuestObjectiveType.SpendWood == type)
                    {
                        _questTaskSaveData.currentValue += value;
                    }
                    break;
                case Define.EBroadcastEventType.ChangeMineral:
                    if (Define.EQuestObjectiveType.EarnMineral == type
                        || Define.EQuestObjectiveType.SpendMineral == type)
                    {
                        _questTaskSaveData.currentValue += value;
                    }
                    break;
                case Define.EBroadcastEventType.ChangeGold:
                    if (Define.EQuestObjectiveType.EarnGold == type
                        || Define.EQuestObjectiveType.SpendGold == type)
                    {
                        _questTaskSaveData.currentValue += value;
                    }
                    break;
                case Define.EBroadcastEventType.KillMonster:
                    if (Define.EQuestObjectiveType.KillMonster == type)
                    {
                        _questTaskSaveData.currentValue += value;
                    }
                    break;
                case Define.EBroadcastEventType.DungeonClear:
                    if (Define.EQuestObjectiveType.ClearDungeon == type)
                    {
                        _questTaskSaveData.currentValue += value;
                    }

                    break;
            }
        }

        public void ChangeState(Define.EQuestStateType stateType)
        {
            _questTaskSaveData.stateType = stateType;
        }

        public bool IsCompleted()
        {
            return _questTaskSaveData.currentValue >= _questTask.ObjectiveCount;
        }
    }
    
    public class Quest
    {
        public int DataId => _questData.DataId;
        public QuestData QuestData => _questData;
        public Define.EQuestStateType QuestStateType => _questSaveData.stateType;
        
        private List<QuestTask> _task;
        private QuestData _questData;
        private QuestSaveData _questSaveData;
        
        public Quest(QuestData questData, QuestSaveData questSaveData)
        {
            _questData = questData;
            _questSaveData = questSaveData;
            _task = new List<QuestTask>(questData.QuestTasks.Count);
            foreach (QuestTaskData taskData in questData.QuestTasks)
            {
                QuestTaskSaveData saveData =
                    questSaveData.taskDataList.Find(v => v.objectiveDataId == taskData.ObjectiveDataId) ??
                    new QuestTaskSaveData();
                QuestTask task = new QuestTask(taskData, saveData);
                _task.Add(task);
            }
        }

        public void ProcessTask(Define.EBroadcastEventType questObjectiveType, int value)
        {
            if (QuestStateType != Define.EQuestStateType.OnGoing)
            {
                return;
            }

            QuestTask questTask = _task[_questSaveData.currentTaskIndex];
            questTask.AddValue(questObjectiveType, value);
            if (questTask.IsCompleted())
            {
                questTask.ChangeState(Define.EQuestStateType.Completed);
                _questSaveData.currentTaskIndex++;
            }
        }

        public void UpdateState(Define.EQuestStateType stateType)
        {
            _questSaveData.stateType = stateType;
        }

        public bool IsAllCompleted()
        {
            return _task.TrueForAll(x=> x.IsCompleted());
        }
    }
    
    public class QuestManager
    {
        private List<Quest> _questList = new ();
        
        public void Initialize()
        {
            Managers.Game.OnHandleBroadcastEventAction -= OnHandleBroadcastEvent;
            Managers.Game.OnHandleBroadcastEventAction += OnHandleBroadcastEvent;
            List<QuestSaveData> questSaveDataList = Managers.Game.GameSaveData.Quests ?? new List<QuestSaveData>();
            if (questSaveDataList.Count == 0)
            {
                foreach (var (key, questData) in Managers.Data.QuestDataDict)
                {
                    QuestSaveData saveData = new QuestSaveData
                    {
                        dataId = questData.DataId,
                        currentTaskIndex = 0,
                        stateType = Define.EQuestStateType.Waiting,
                        taskDataList = new List<QuestTaskSaveData>(questData.QuestTasks.Count)
                    };

                    foreach (QuestTaskData taskData in questData.QuestTasks)
                    {
                        QuestTaskSaveData questTaskSaveData = new QuestTaskSaveData
                        {
                            objectiveDataId = taskData.ObjectiveDataId,
                            stateType = Define.EQuestStateType.Waiting,
                            currentValue = 0
                        };
                        
                        saveData.taskDataList.Add(questTaskSaveData);
                    }
                
                    Quest quest = new Quest(questData, saveData);
                    _questList.Add(quest);
                    questSaveDataList.Add(saveData);
                }
            
                Managers.Game.GameSaveData.Quests.AddRange(questSaveDataList);
                return;
            }
            
            foreach (QuestSaveData saveData in questSaveDataList)
            {
                QuestData questData = Managers.Data.QuestDataDict[saveData.dataId];
                Quest quest = new Quest(questData, saveData);
                _questList.Add(quest);
            }
        }
        
        private void OnHandleBroadcastEvent(Define.EBroadcastEventType eventType, int value)
        {
            foreach (Quest quest in _questList)
            {
                quest.ProcessTask(eventType, value);                
                if (quest.IsAllCompleted())
                {
                    quest.UpdateState(Define.EQuestStateType.Completed);
                }
            }
        }

        public void AssignQuest(QuestData questData)
        {
            Quest quest = _questList.Find(v=> v.DataId == questData.DataId);
            if (quest == null)
            {
                return;
            }
            
            quest.UpdateState(Define.EQuestStateType.OnGoing);
        }

        public void GetReward(int questId)
        {
            Quest quest = _questList.Find(v => v.DataId == questId);
            if (quest == null)
            {
                LogUtils.LogError("Failed get completed quest :" + questId);
                return;
            }
            
            foreach (QuestRewardData rewardData in quest.QuestData.Rewards)
            {
                GameManager game = Managers.Game;
                Define.EQuestRewardType rewardType = rewardData.RewardType;
                switch (rewardType)
                {
                    case Define.EQuestRewardType.Hero:
                        break;
                    case Define.EQuestRewardType.Gold:
                        game.Gold += rewardData.RewardCount;
                        break;
                    case Define.EQuestRewardType.Mineral:
                        game.Mineral += rewardData.RewardCount;
                        break;
                    case Define.EQuestRewardType.Meat:
                        game.Meat += rewardData.RewardCount;
                        break;
                    case Define.EQuestRewardType.Wood:
                        game.Wood += rewardData.RewardCount;
                        break;
                    // case Define.EQuestRewardType.Item:
                    //     game.Item += rewardData.RewardCount;
                    //     break;
                }
            }

            quest.UpdateState(Define.EQuestStateType.Rewarded);
        }
    }
}