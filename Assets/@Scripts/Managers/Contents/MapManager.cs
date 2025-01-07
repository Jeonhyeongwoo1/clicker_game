using System.Collections.Generic;
using System.IO;
using Clicker.Controllers;
using Clicker.Utils;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Clicker.Manager
{
    public class MapManager
    {
        private Dictionary<Vector3Int, TileBase> _heroCampMoveableTileDict = new();
        private Define.CollisionType[,] _collisionArray;
        private Grid _grid;
        private int _minX;
        private int _minY;
        private int _maxX;
        private int _maxY;
        private GameObject _mapObject;
        
        public bool IsPossibleHeroCampMove(Vector3 position)
        {
            Vector3Int targetPos = Vector3Int.CeilToInt(position);
            if (!_heroCampMoveableTileDict.ContainsKey(targetPos))
            {
                return false;
            }

            return true;
        }
        
        public Vector3Int WorldToCell(Vector3 position)
        {
            return _grid.WorldToCell(position);
        }

        public Vector3 CellToWorld(Vector3Int cellPosition)
        {
            return _grid.CellToWorld(cellPosition);
        }
        
        public void CreateMap(string mapName)
        {
            GameObject mapPrefab = Managers.Resource.Instantiate(mapName);
            _mapObject = mapPrefab;
            Debug.Log(_mapObject.name);
            _grid = _mapObject.GetComponent<Grid>();
            ParseCollision(mapName);
            SetHeroCampMoveableTile();
        }
        
        private void SetHeroCampMoveableTile()
        {
            var tileObject = Util.FindChild(_mapObject, "Tilemap_Collision", true);
            Tilemap wallTilemap = tileObject.GetComponent<Tilemap>();
            BoundsInt boundsInt = wallTilemap.cellBounds;
            for (int y = boundsInt.min.y; y <= boundsInt.max.y; y++)
            {
                for (int x = boundsInt.min.x; x <= boundsInt.max.x; x++)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, 0);
                    TileBase tile = wallTilemap.GetTile(cellPos);
                    if (tile != null)
                    {
                        Vector3 worldPosition = wallTilemap.CellToWorld(cellPos);
                        Vector3Int position = Vector3Int.CeilToInt(worldPosition);
                        _heroCampMoveableTileDict.Add(position, tile);
                    }
                }
            }   
        }
        
        public void CreateBaseObjects()
        {
            Tilemap tm = Util.FindChild<Tilemap>(_mapObject, "Tilemap_Object", true);

            if (tm != null)
                tm.gameObject.SetActive(false);

            for (int y = tm.cellBounds.yMax; y >= tm.cellBounds.yMin; y--)
            {
                for (int x = tm.cellBounds.xMin; x <= tm.cellBounds.xMax; x++)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, 0);
                    CustomTile tile = tm.GetTile(cellPos) as CustomTile;
                    if (tile == null)
                        continue;

                    Debug.Log($"{tile} / {tile.ObjectType} / {tile.DataTemplateID}");
                    if (tile.ObjectType == Define.EObjectType.Env)
                    {
                        if (tile.DataTemplateID == 202001)
                        {
                            continue;
                        }
                        var env = Managers.Object.CreateObject<Env>(tile.ObjectType, tile.DataTemplateID);
                        Vector3 worldPosition = CellToWorld(cellPos);
                        env.Spawn(worldPosition);
                    }
                    else if(tile.ObjectType == Define.EObjectType.Monster)
                    {
                        var monster = Managers.Object.CreateObject<Monster>(tile.ObjectType, tile.DataTemplateID);
                        Vector3 worldPosition = CellToWorld(cellPos);
                        monster.Spawn(worldPosition);
                    }
                    
                }
            }
        }
        
        public List<Vector3Int> PathFinding(Vector3Int currentPosition, Vector3Int destPosition)
        {
            // 상하좌우 + 대각선 (8 방향)
            int[,] dir = {
                { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 }, // 상하좌우
                { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } // 대각선
            };
            int[] cost = { 10, 10, 10, 10, 14, 14, 14, 14 }; // 대각선 이동은 비용 14

            Dictionary<Vector3Int, int> bestDict = new Dictionary<Vector3Int, int>();
            HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
            PriorityQueue<Node> queue = new PriorityQueue<Node>();
            Dictionary<Vector3Int, Vector3Int> pathDict = new Dictionary<Vector3Int, Vector3Int>();

            int closedF = int.MaxValue;
            Vector3Int closePos = Vector3Int.zero;

            {
                int g = 0;
                int h = Mathf.Abs(destPosition.x - currentPosition.x) + Mathf.Abs(destPosition.y - currentPosition.y);
                int f = g + h;
                Node startNode = new Node(h, g, f, currentPosition);
                queue.Push(startNode);
                bestDict[currentPosition] = g;
                pathDict[currentPosition] = currentPosition;

                closedF = f;
                closePos = currentPosition;
            }

            while (!queue.IsEmpty())
            {
                Node node = queue.Top();
                queue.Pop();

                Vector3Int nodePos = new Vector3Int(node.x, node.y, 0);
                if (nodePos == destPosition)
                {
                    closePos = nodePos;
                    break;
                }

                if (visited.Contains(nodePos))
                {
                    continue;
                }

                visited.Add(nodePos);

                for (int i = 0; i < 8; i++)
                {
                    int nextY = node.y + dir[i, 0];
                    int nextX = node.x + dir[i, 1];
                    if (!CanGo(nextX, nextY))
                    {
                        continue;
                    }

                    Vector3Int nextPos = new Vector3Int(nextX, nextY, 0);
                    int g = cost[i] + node.g;
                    int h = Mathf.Abs(destPosition.x - nextPos.x) + Mathf.Abs(destPosition.y - nextPos.y);
                    int f = g + h;

                    if (bestDict.ContainsKey(nextPos) && bestDict[nextPos] <= g)
                    {
                        continue;
                    }

                    bestDict[nextPos] = g;
                    queue.Push(new Node(h, g, f, nextPos));
                    pathDict[nextPos] = nodePos;

                    if (closedF > f)
                    {
                        closedF = f;
                        closePos = nextPos;
                    }
                }
            }
            
            List<Vector3Int> list = new List<Vector3Int>();
            Vector3Int now = pathDict.ContainsKey(destPosition) ? closePos : destPosition;
            if (!pathDict.ContainsKey(now))
            {
                return new List<Vector3Int>();
            }
            
            while (pathDict[now] != now)
            {
                list.Add(now);
                now = pathDict[now];
            }

            list.Add(now);
            list.Reverse();
            return list;
        }

        private bool CanGo(int x, int y)
        {
            if (y < _minY || y > _maxY || x < _minX || x > _maxX)
            {
                return false;
            }

            int targetX = x - _minX;
            int targetY = _maxY - y;
            Define.CollisionType type = _collisionArray[targetX, targetY];
            return type == Define.CollisionType.None;
        }
        
        private void ParseCollision(string mapName)
        {
            // Collision 관련 파일 BaseMapCollision
            TextAsset txt = Managers.Resource.Load<TextAsset>($"{mapName}Collision");
            StringReader reader = new StringReader(txt.text);
            
            _minX = int.Parse(reader.ReadLine());
            _maxX = int.Parse(reader.ReadLine());
            _minY = int.Parse(reader.ReadLine());
            _maxY = int.Parse(reader.ReadLine());

            int maxX = _maxX - _minX + 1;
            int maxY = _maxY - _minY + 1;
            _collisionArray = new Define.CollisionType[maxX, maxY];
            for (int y = 0; y < maxY; y++)
            {
                string line = reader.ReadLine();
                for (int x = 0; x < maxX; x++)
                {
                    Define.CollisionType collisionType = Define.CollisionType.None;
                    if (line[x] == '0')
                    {
                        collisionType = Define.CollisionType.Wall;
                    }
                    else if (line[x] == '1')
                    {
                        collisionType = Define.CollisionType.None;
                    }
                    else if (line[x] == '2')
                    {
                        collisionType = Define.CollisionType.SemiWall;
                    }
                    _collisionArray[x, y] = collisionType;
                }
            } 
        }
        
        // private GameObject _pathFindingTest;
        // public void Test(Vector3Int startPos, Vector3Int endPos)
        // {
        //     List<Vector3Int> path = new List<Vector3Int>();
        //     Vector3Int start = WorldToCell(startPos);
        //     Vector3Int end = WorldToCell(endPos);
        //     path = PathFinding(start, end);
        //
        //     List<Vector3> s = new ();
        //     foreach (Vector3Int vector3Int in path)
        //     {
        //         s.Add(CellToWorld(vector3Int));
        //     }
        //     
        //     if (_pathFindingTest == null)
        //     {
        //         GameObject go = new GameObject();
        //         go.AddComponent<PathFindingTest>();
        //         _pathFindingTest = go;
        //     }
        //     
        //     _pathFindingTest.GetComponent<PathFindingTest>().SetPathList(s, CellToWorld(start), CellToWorld(endPos));
        // }
    }
}