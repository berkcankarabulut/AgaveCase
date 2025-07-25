using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Project.Elements.Runtime;
using UnityEngine; 

namespace Project.Board.Runtime
{
    public class BoardMatchDetectionSystem 
    {
        private readonly BoardFacade _board; 
        private readonly HashSet<Vector2Int> _globalVisited = new HashSet<Vector2Int>();
        private readonly Queue<Vector2Int> _bfsQueue = new Queue<Vector2Int>();
        private readonly HashSet<Vector2Int> _componentBuffer = new HashSet<Vector2Int>();

        public BoardMatchDetectionSystem(BoardFacade board)
        {
            _board = board;
        }
        
        private static readonly Vector2Int[] Directions = new Vector2Int[]
        {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1)
        };
 
        public bool HasPotentialMatches()
        {
            _globalVisited.Clear();

            for (int x = 0; x < _board.GridWidth; x++)
            {
                for (int y = 0; y < _board.GridHeight; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (_globalVisited.Contains(pos)) continue;

                    var cell = _board.GetCellAt(pos);
                    var element = cell?.GetElement();
                    if (element?.ElementData == null) continue;

                    var componentSize = GetComponentSize(pos, element.ElementData);
                    if (componentSize >= 3) return true;
                }
            }
            return false;
        }
 
        private int GetComponentSize(Vector2Int startPos, ElementDataSO targetType)
        {
            _componentBuffer.Clear();
            _bfsQueue.Clear();

            _bfsQueue.Enqueue(startPos);
            _componentBuffer.Add(startPos);
            _globalVisited.Add(startPos);

            while (_bfsQueue.Count > 0)
            {
                Vector2Int currentPos = _bfsQueue.Dequeue();

                foreach (Vector2Int direction in Directions)
                {
                    Vector2Int neighborPos = currentPos + direction;

                    if (neighborPos.x < 0 || neighborPos.x >= _board.GridWidth ||
                        neighborPos.y < 0 || neighborPos.y >= _board.GridHeight)
                        continue;

                    if (_componentBuffer.Contains(neighborPos)) continue;

                    var neighborCell = _board.GetCellAt(neighborPos);
                    var neighborElement = neighborCell?.GetElement();
                    if (neighborElement?.ElementData == null) continue;

                    if (!IsSameElementType(neighborElement.ElementData, targetType))
                        continue;

                    _componentBuffer.Add(neighborPos);
                    _globalVisited.Add(neighborPos);
                    _bfsQueue.Enqueue(neighborPos);
                }
            }

            return _componentBuffer.Count;
        }

        public bool IsValidMatch(List<Vector2Int> positions)
        {
            if (positions == null || positions.Count < 3) return false;
            
            // Check adjacency
            for (int i = 1; i < positions.Count; i++)
            {
                if (!AreAdjacent(positions[i-1], positions[i])) return false;
            }

            // Check same element type
            var firstElement = _board.GetElementAt(positions[0]);
            if (firstElement == null) return false;

            foreach (var pos in positions)
            {
                var element = _board.GetElementAt(pos);
                if (element == null || !IsSameElementType(element.ElementData, firstElement.ElementData))
                    return false;
            }

            return true;
        }

        public async UniTask<bool> HasPotentialMatchesAsync()
        {
            // Background thread'de çalıştır
            return await UniTask.RunOnThreadPool(() => HasPotentialMatches());
        }

        // ✅ Helper methods
        private bool IsSameElementType(ElementDataSO type1, ElementDataSO type2)
        {
            if (type1 == null || type2 == null) return false;
            return type1.Id.Equals(type2.Id);
        }

        private bool AreAdjacent(Vector2Int pos1, Vector2Int pos2)
        {
            var diff = pos1 - pos2;
            return Mathf.Abs(diff.x) + Mathf.Abs(diff.y) == 1;
        }
    }
}