using System;
using System.Collections.Generic;
using Clicker.Controllers;
using Clicker.Manager;
using Clicker.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[Serializable]
public struct SpawnData
{
    public int dataId;
    public Vector3 spawnWorldPosition;
    public bool isStartPos;
    public bool isWayPoint;
    public Define.EObjectType objectType;

    public SpawnData(int dataId, Define.EObjectType objectType, Vector3 spawnWorldPosition, bool isStartPos, bool isWayPoint)
    {
        this.dataId = dataId;
        this.objectType = objectType;
        this.spawnWorldPosition = spawnWorldPosition;
        this.isStartPos = isStartPos;
        this.isWayPoint = isWayPoint;
    }
}

public class Stage : MonoBehaviour
{
    [SerializeField] private Tilemap _terrainTileMap; // 오브젝트 셋팅용
    [SerializeField] private Tilemap _objectTileMap;

    private List<SpawnData> _objectSpawnList = new();

    public void SetInfo()
    {
        _terrainTileMap = Util.FindChild<Tilemap>(gameObject, "Terrain_01");
        SaveSpawnInfos();
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }

    public Vector3 GetWaypointPosition()
    {
        foreach (SpawnData spawnData in _objectSpawnList)
        {
            if (spawnData.isWayPoint)
            {
                return spawnData.spawnWorldPosition;
            }
        }

        Debug.Log("failed");
        return Vector3.zero;
    }

    public void SpawnObject()
    {
        gameObject.SetActive(true);
        foreach (SpawnData spawnData in _objectSpawnList)
        {
            Define.EObjectType objectType = spawnData.objectType;
            switch (objectType)
            {
                // case Define.EObjectType.Monster:
                //     var monster = Managers.Object.CreateObject<Monster>(Define.EObjectType.Monster, tile.DataId);
                //     monster.Spawn(worldPos);
                //     break;
                case Define.EObjectType.Npc:
                    var npc = Managers.Object.CreateObject<Npc>(Define.EObjectType.Npc, spawnData.dataId);
                    npc.Spawn(spawnData.spawnWorldPosition);
                    break;
            }
        }
    }
    
    private void SaveSpawnInfos()
    {
        if (_objectTileMap != null)
            _objectTileMap.gameObject.SetActive(false);

        for (int y = _objectTileMap.cellBounds.yMax; y >= _objectTileMap.cellBounds.yMin; y--)
        {
            for (int x = _objectTileMap.cellBounds.xMin; x <= _objectTileMap.cellBounds.xMax; x++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                CustomTile tile = _objectTileMap.GetTile(new Vector3Int(x, y, 0)) as CustomTile;

                if (tile == null)
                    continue;

                Vector3 worldPos = Managers.Map.CellToWorld(cellPos);

                SpawnData spawnData = new SpawnData(tile.DataId, tile.ObjectType, worldPos, tile.isStartPos,
                    tile.isWayPoint);
                _objectSpawnList.Add(spawnData);
                
                // ObjectSpawnInfo info = new ObjectSpawnInfo(tile.Name, tile.DataId, x, y, worldPos, tile.ObjectType);
                
                // if (tile.isStartPos)
                // {
                //     StartSpawnInfo = info;
                //     continue;
                // }
                //
                // Debug.Log($"{tile.name} , {tile.isWayPoint}, {tile.ObjectType}");
                // if (tile.isWayPoint)
                // {
                //     WaypointSpawnInfo = info;
                // }
                //
                // _spawnInfos.Add(info);
            }
        }
    }
}
