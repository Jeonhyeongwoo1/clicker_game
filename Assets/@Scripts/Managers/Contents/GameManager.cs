using Clicker.Controllers;
using Clicker.Entity;
using UnityEngine;

namespace Clicker.Manager
{
    public class GameManager
    {

        private Vector3Int GetNearbyPosition(Creature creature, Vector3 position, float range = 5)
        {
            const int count = 100;
            for (int i = 0; i < count; i++)
            {
                float x = Random.Range(-range, range);
                float y = Random.Range(-range, range);
                Vector3 targetPos = position + new Vector3(x, y);
                Vector3Int cellPos = Managers.Map.WorldToCell(targetPos);
                if (!Managers.Map.CanGo(cellPos.x, cellPos.y, null))
                {
                    continue;
                }

                return cellPos;
            }

            Debug.LogWarning($"SSSSS {position} / {Managers.Map.WorldToCell(position)}");
            return Managers.Map.WorldToCell(position);
        }

        public void TeleportHeros(string name)
        {
            StageTranslation stageTranslation = Managers.Map.StageTranslation;
            bool isSuccess = stageTranslation.TryChangeStage(name);
            if (isSuccess)
            {
                Vector3 startPos = stageTranslation.GetWayPosition();
                foreach (Creature creature in Managers.Object.HeroSet)
                {
                    Vector3Int position = GetNearbyPosition(creature, startPos);
                    Managers.Map.MoveToCell(position, creature.CellPosition, creature);
                }
                
                Managers.Object.HeroCamp.MoveToWaypointPosition(startPos);
            }
        }

    }
}