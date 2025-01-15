using System;
using System.Collections.Generic;
using Clicker.Controllers;
using Clicker.Entity;
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
    [SerializeField] private Tilemap _terrainTileMap; 
    [SerializeField] private Tilemap _objectTileMap;// 오브젝트 셋팅용

    private bool _isActive = false;
    private List<SpawnData> _objectSpawnDataList = new(); 
    private List<BaseObject> _spawnedObjectList = new();

    public void SetInfo()
    {
        _isActive = true; // 처음에는 모두 켜져있으니 꺼준다
        _terrainTileMap = Util.FindChild<Tilemap>(gameObject, "Terrain_01");
        SaveSpawnInfos();
    }

    public void DisableObject()
    {
        if (!_isActive)
        {
            return;
        }

        _isActive = false;
        foreach (BaseObject baseObject in _spawnedObjectList)
        {
            Managers.Object.Despawn(baseObject);
        }
        
        _spawnedObjectList.Clear();
        gameObject.SetActive(false);
    }

    public bool IsStageInRange(Vector3 position)
    {
        Vector3Int cellPos = _terrainTileMap.layoutGrid.WorldToCell(position);
        if (_terrainTileMap.GetTile(cellPos))
        {
            return true;
        }

        return false;
    }
    
    public Vector3 GetWaypointPosition()
    {
        foreach (SpawnData spawnData in _objectSpawnDataList)
        {
            if (spawnData.isWayPoint)
            {
                return spawnData.spawnWorldPosition;
            }
        }

        return Vector3.zero;
    }

    public void SpawnObject()
    {
        // if (_isActive)
        // {
        //     return;
        // }
        
        _isActive = true;
        gameObject.SetActive(true);
        foreach (SpawnData spawnData in _objectSpawnDataList)
        {
            Vector3Int cellPos = Managers.Map.WorldToCell(spawnData.spawnWorldPosition);
            if (!Managers.Map.CanGo(cellPos.x, cellPos.y, null, true))
            {
                continue;
            }
            
            Define.EObjectType objectType = spawnData.objectType;
            switch (objectType)
            {
                case Define.EObjectType.Monster:
                    if (spawnData.dataId == 202004)
                    {
                        continue;
                    }
                    
                    var monster = Managers.Object.CreateObject<Monster>(Define.EObjectType.Monster, spawnData.dataId);
                    monster.Spawn(spawnData.spawnWorldPosition);
                    _spawnedObjectList.Add(monster);
                    break;
                case Define.EObjectType.Npc:
                    var npc = Managers.Object.CreateObject<Npc>(Define.EObjectType.Npc, spawnData.dataId);
                    npc.Spawn(spawnData.spawnWorldPosition);
                    _spawnedObjectList.Add(npc);
                    break;
            }
        }
    }
    
    private void SaveSpawnInfos()
    {
        if (_objectTileMap != null)
        {
            _objectTileMap.gameObject.SetActive(false);
        }

        for (int y = _objectTileMap.cellBounds.yMax; y >= _objectTileMap.cellBounds.yMin; y--)
        {
            for (int x = _objectTileMap.cellBounds.xMin; x <= _objectTileMap.cellBounds.xMax; x++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                CustomTile tile = _objectTileMap.GetTile(new Vector3Int(x, y, 0)) as CustomTile;

                if (tile == null)
                {
                    continue;
                }
                    
                Vector3 worldPos = Managers.Map.CellToWorld(cellPos);
                SpawnData spawnData = new SpawnData(tile.DataId, tile.ObjectType, worldPos, tile.isStartPos,
                    tile.isWayPoint);
                _objectSpawnDataList.Add(spawnData);
            }
        }
    }
}
