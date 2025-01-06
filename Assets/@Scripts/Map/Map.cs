using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Clicker.Entity
{
    public class Map : MonoBehaviour
    {
        [SerializeField] private Tilemap _wallTileMap;
        
        private Dictionary<Vector3Int, TileBase> _wallDictionary = new ();
        private List<Vector3> _wallPositionList = new List<Vector3>();

        public bool IsPossibleMoveTo(Vector3 position)
        {
            Vector3Int targetPos = Vector3Int.CeilToInt(position);
            if (_wallDictionary.ContainsKey(targetPos))
            {
                return false;
            }

            return true;
        }
        
        private void Start()
        {
            BoundsInt boundsInt = _wallTileMap.cellBounds;
            for (int y = boundsInt.min.y; y < boundsInt.max.y; y++)
            {
                for (int x = boundsInt.min.x; x < boundsInt.max.x; x++)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, 0);
                    TileBase tile = _wallTileMap.GetTile(cellPos);
                    if (tile != null)
                    {
                        // 셀 좌표를 월드 좌표로 변환하고 저장
                        Vector3 worldPosition = _wallTileMap.CellToWorld(cellPos);
                        Vector3Int position = Vector3Int.CeilToInt(worldPosition);
                        int cellX = 8 / 2;
                        int cellY = 4 / 2;
                        int minX = position.x - cellX;
                        int maxX = position.x + cellX;
                        int minY = position.y - cellY;
                        int maxY = position.y + cellY;

                        _wallPositionList.Add(position);
                        _wallDictionary.Add(position, tile);
                        // for (int _x = minX; _x <= maxX; _x++)
                        // {
                        //     for (int _y = minY; _y <= maxY; _y++)
                        //     {
                        //         Vector3Int pos = new Vector3Int(_x, _y, 0);
                        //         if (!_wallPositionList.Contains(pos))
                        //         {
                        //             _wallPositionList.Add(pos);
                        //         }
                        //
                        //         _wallDictionary.TryAdd(pos, tile);        
                        //     }
                        // }
                        
                        Debug.Log(cellPos);
                    }
                }
            }
            
            Debug.Log(_wallPositionList.Count);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (Vector3 vector3Int in _wallPositionList)
            {
                Gizmos.DrawSphere(vector3Int, 0.1f);
            }
        }
    }
}