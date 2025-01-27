using System.Collections.Generic;
using System.IO;
using Clicker.Entity;
using Clicker.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Clicker.Manager
{
    public class MapManager
    {
        public StageTranslation StageTranslation => _stageTranslation;
        public Dictionary<Vector3Int, BaseObject> _cellDict = new();
        
        private Dictionary<Vector3Int, TileBase> _heroCampMoveableTileDict = new();
        private Define.CollisionType[,] _collisionArray;
        private Grid _grid;
        private int _minX;
        private int _minY;
        private int _maxX;
        private int _maxY;
        private GameObject _mapObject;
        private StageTranslation _stageTranslation;
        
        private readonly Vector3Int[] _dirArray =
        {
            new Vector3Int(1, 0),
            new Vector3Int(0, 1),
            new Vector3Int(-1, 0),
            new Vector3Int(0, -1),
            new Vector3Int(1, 1),
            new Vector3Int(1, -1),
            new Vector3Int(-1, 1),
            new Vector3Int(-1, -1),
        };

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
            ParseCollision(mapName); //1번으로 처리
            GameObject mapPrefab = Managers.Resource.Instantiate(mapName);
            _mapObject = mapPrefab;
            _grid = _mapObject.GetComponent<Grid>();
            _stageTranslation = _mapObject.GetComponent<StageTranslation>();
            _stageTranslation.SetInfo();
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

            if (tileObject.activeSelf)
            {
                tileObject.SetActive(false);
            }
        }

        public bool MoveToCell(Vector3Int cellPos, Vector3Int previousCellPos, BaseObject baseObject)
        {
            if (CanGo(cellPos.x, cellPos.y, baseObject) == false)
                return false;

            // 기존 좌표에 있던 오브젝트를 밀어준다.
            // (단, 처음 신청했으면 해당 CellPos의 오브젝트가 본인이 아닐 수도 있음)
            RemoveObject(baseObject);

            // 새 좌표에 오브젝트를 등록한다.
            AddObject(baseObject, cellPos);

            // 셀 좌표 이동
            baseObject.SetCellPosition(cellPos, WorldToCell(cellPos));
            //Debug.Log($"Move To {cellPos}");

            return true;
        }
        
        public void RemoveObject(BaseObject obj)
        {
            // 기존의 좌표 제거
            int extraCells = 0;
            if (obj != null)
                extraCells = obj.ExtraSize;

            Vector3Int cellPos = obj.CellPosition;

            for (int dx = -extraCells; dx <= extraCells; dx++)
            {
                for (int dy = -extraCells; dy <= extraCells; dy++)
                {
                    Vector3Int newCellPos = new Vector3Int(cellPos.x + dx, cellPos.y + dy);
                    BaseObject prev = GetBaseObject(newCellPos);

                    if (prev == obj)
                        _cellDict.Remove(newCellPos);
                }
            }
        }

        void AddObject(BaseObject obj, Vector3Int cellPos)
        {
            int extraCells = 0;
            if (obj != null)
                extraCells = obj.ExtraSize;

            for (int dx = -extraCells; dx <= extraCells; dx++)
            {
                for (int dy = -extraCells; dy <= extraCells; dy++)
                {
                    Vector3Int newCellPos = new Vector3Int(cellPos.x + dx, cellPos.y + dy);

                    BaseObject prev = GetBaseObject(newCellPos);
                    if (prev != null && prev != obj)
                        Debug.LogWarning($"AddObject 수상함");

                    _cellDict[newCellPos] = obj;
                }
            }
        }
        
        public void RemoveCellPosition(Vector3Int cellPos, BaseObject baseObject)
        {
            _cellDict.Remove(cellPos);
            // _cellDict.TryAdd(cellPos, baseObject);
        }

        public void SetCellPosition(Vector3Int cellPos, BaseObject baseObject)
        {
            _cellDict.TryAdd(cellPos, baseObject);
        }
        
        public List<Vector3Int> PathFinding(Vector3Int startPosition, Vector3Int destPosition, BaseObject targetObject, int maxDepth = 10)
        {
            // 상하좌우 + 대각선 (8 방향)
            //int[] cost = { 10, 10, 10, 10, 14, 14, 14, 14 }; // 대각선 이동은 비용 14

            Dictionary<Vector3Int, int> bestDict = new Dictionary<Vector3Int, int>();
            HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
            PriorityQueue<Node> queue = new PriorityQueue<Node>();
            Dictionary<Vector3Int, Vector3Int> pathDict = new Dictionary<Vector3Int, Vector3Int>();

            //목적지에 도착하지 못할 경우에 그나마 가장 가까운 위치로 보낸다.
            int closedH = int.MaxValue;
            Vector3Int closePos = Vector3Int.zero;
            int depth = 0;

            {
                int h = Mathf.Abs(destPosition.x - startPosition.x) + Mathf.Abs(destPosition.y - startPosition.y);
                Node startNode = new Node(h, depth, startPosition);
                queue.Push(startNode);
                bestDict[startPosition] = h;
                pathDict[startPosition] = startPosition;

                closedH = h;
                closePos = startPosition;
            }

            while (!queue.IsEmpty())
            {
                Node node = queue.Top();
                queue.Pop();

                Vector3Int nodePos = new Vector3Int(node.x, node.y, 0);
                if (nodePos == destPosition)
                {
                    // closePos = nodePos;
                    break;
                }

                if (visited.Contains(nodePos))
                {
                    continue;
                }
                
                visited.Add(nodePos);
                depth = node.depth;

                // Debug.Log($"{depth}");
                if (depth == maxDepth)
                {
                    break;
                }
                
                for (int i = 0; i < _dirArray.Length; i++)
                {
                    int nextY = node.y + _dirArray[i].y;
                    int nextX = node.x + _dirArray[i].x;
                    if (!CanGo(nextX, nextY, targetObject))
                    {
                        continue;
                    }

                    // Debug.Log("Can");
                    Vector3Int nextPos = new Vector3Int(nextX, nextY, 0);
                    //int g = cost[i] + node.g;
                    int h = Mathf.Abs(destPosition.x - nextPos.x) + Mathf.Abs(destPosition.y - nextPos.y);
                    if (bestDict.ContainsKey(nextPos) && bestDict[nextPos] <= h)
                    {
                        continue;
                    }

                    bestDict[nextPos] = h;
                    queue.Push(new Node(h, depth + 1, nextPos));
                    pathDict[nextPos] = nodePos;
                    
                    if (closedH > h)
                    {
                        closedH = h;
                        closePos = nextPos;
                    }
                }
            }
            
            List<Vector3Int> list = new List<Vector3Int>();
            Vector3Int now = pathDict.ContainsKey(destPosition) ? destPosition : closePos;
            if (!pathDict.ContainsKey(now))
            {
                return new List<Vector3Int>();
            }
            
            while (pathDict[now] != now)
            {
                if(!list.Contains(now))
                        list.Add(now);
                now = pathDict[now];
            }

            list.Add(now);
            list.Reverse();
            // Debug.Log($"now {depth} {now} / {destPosition} / {closePos} / {startPosition} / {list.Count}");
            return list;
        }

        public BaseObject GetBaseObject(Vector3Int cellPos)
        {
            if (_cellDict.TryGetValue(cellPos, out BaseObject baseObject))
            {
                return baseObject;
            }

            return null;
        }

        public bool CanGo(int x, int y, BaseObject targetObject, bool ignoreObject = false)
        {
            if (y < _minY || y > _maxY || x < _minX || x > _maxX)
            {
                return false;
            }

            if (!ignoreObject)
            {
                BaseObject baseObject = GetBaseObject(new Vector3Int(x, y, 0));
                if (baseObject != null && baseObject != targetObject)
                {
                    return false;
                }
            }
            
            // if (_cellDict.ContainsKey(new Vector3Int(x, y)))
            // {
            //     return false;
            // }

            int targetX = x - _minX;
            int targetY = _maxY - y;
            Define.CollisionType type = _collisionArray[targetX, targetY];
            if (type == Define.CollisionType.None)
            {
                return true;
            }

            return false;
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