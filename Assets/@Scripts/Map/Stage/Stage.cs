using System;
using System.Collections;
using System.Collections.Generic;
using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Manager;
using Clicker.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class Stage : MonoBehaviour
{
    [SerializeField] private Tilemap _terrainTileMap;
    [SerializeField] private Tilemap _objectTileMap;

    public void SetInfo()
    {
        _terrainTileMap = Util.FindChild<Tilemap>(gameObject, "Terrain_01");
        SpawnObject();
    }

    public void SpawnObject()
    {
        SaveSpawnInfos();    
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

                Debug.Log($"{tile.ObjectType} / {tile.DataId} / {worldPos}");
                switch (tile.ObjectType)
                {
                    // case Define.EObjectType.Monster:
                    //     var monster = Managers.Object.CreateObject<Monster>(Define.EObjectType.Monster, tile.DataId);
                    //     monster.Spawn(worldPos);
                    //     break;
                    case Define.EObjectType.Npc:
                        var npc = Managers.Object.CreateObject<Npc>(Define.EObjectType.Npc, tile.DataId);
                        npc.Spawn(worldPos);
                        break;
                }
                
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
