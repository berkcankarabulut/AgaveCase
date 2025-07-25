using System;
using System.Collections.Generic;
using Project.Elements.Runtime;
using Project.GridSystem.Runtime;
using UnityEngine;

namespace Project.Board.Runtime
{
    public class MatchDetectionHandler
    {
        private readonly BoardManager _boardManager;
        private const int _MIN_MATCH_COUNT = 3;

        public MatchDetectionHandler(BoardManager boardManager)
        {
            _boardManager = boardManager;
        }

        public bool HasPotentialMatches()
        {
            var visited = new HashSet<Vector2Int>();

            for (int x = 0; x < _boardManager.GridWidth; x++)
            {
                for (int y = 0; y < _boardManager.GridHeight; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (visited.Contains(pos)) continue;

                    var cell = _boardManager.GetGridAt(pos);
                    if (cell?.GetElement() == null) continue;

                    // Flood fill ile component size bul
                    var componentSize = GetComponentSize(pos, cell.GetElement().ElementData, visited);
                    if (componentSize >= 3) return true;
                }
            }

            return false;
        }

        private int GetComponentSize(Vector2Int startPos, ElementDataSO targetType, HashSet<Vector2Int> globalVisited)
        { 
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            HashSet<Vector2Int> componentCells = new HashSet<Vector2Int>();
 
            queue.Enqueue(startPos);
            componentCells.Add(startPos);
            globalVisited.Add(startPos);
 
            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(0, 1), 
                new Vector2Int(0, -1),  
                new Vector2Int(-1, 0),  
                new Vector2Int(1, 0)  
            };
 
            while (queue.Count > 0)
            {
                Vector2Int currentPos = queue.Dequeue();
 
                foreach (Vector2Int direction in directions)
                {
                    Vector2Int neighborPos = currentPos + direction;
 
                    if (neighborPos.x < 0 || neighborPos.x >= _boardManager.GridWidth ||
                        neighborPos.y < 0 || neighborPos.y >= _boardManager.GridHeight)
                        continue;
 
                    if (componentCells.Contains(neighborPos))
                        continue;
 
                    GridCell neighborCell = _boardManager.GetGridAt(neighborPos);
                    if (neighborCell?.GetElement() == null)
                        continue;
 
                    if (!IsSameElementType(neighborCell.GetElement().ElementData, targetType))
                        continue;
 
                    componentCells.Add(neighborPos);
                    globalVisited.Add(neighborPos);
                    queue.Enqueue(neighborPos);  
                }
            } 
            return componentCells.Count;
        } 

        private bool IsSameElementType(ElementDataSO element1, ElementDataSO element2)
        {
            if (element1 == null || element2 == null) return false;
            if (element1 == null || element2 == null) return false;

            return element1.Id.Equals(element2.Id);
        }
    }
}