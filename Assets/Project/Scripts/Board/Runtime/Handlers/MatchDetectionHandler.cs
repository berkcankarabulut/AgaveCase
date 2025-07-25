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
            // BFS (Breadth-First Search) kullanarak component'i tara
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            HashSet<Vector2Int> componentCells = new HashSet<Vector2Int>();

            // Başlangıç cell'i ekle
            queue.Enqueue(startPos);
            componentCells.Add(startPos);
            globalVisited.Add(startPos);

            // 4 yön: yukarı, aşağı, sol, sağ
            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(0, 1), // Yukarı
                new Vector2Int(0, -1), // Aşağı  
                new Vector2Int(-1, 0), // Sol
                new Vector2Int(1, 0) // Sağ
            };

            // BFS loop - tüm bağlı cell'leri bul
            while (queue.Count > 0)
            {
                Vector2Int currentPos = queue.Dequeue();

                // 4 komşuyu kontrol et
                foreach (Vector2Int direction in directions)
                {
                    Vector2Int neighborPos = currentPos + direction;

                    // Grid sınırları içinde mi?
                    if (neighborPos.x < 0 || neighborPos.x >= _boardManager.GridWidth ||
                        neighborPos.y < 0 || neighborPos.y >= _boardManager.GridHeight)
                        continue;

                    // Bu cell'i daha önce ziyaret ettik mi?
                    if (componentCells.Contains(neighborPos))
                        continue;

                    // Bu pozisyonda element var mı?
                    GridCell neighborCell = _boardManager.GetGridAt(neighborPos);
                    if (neighborCell?.GetElement() == null)
                        continue;

                    // Aynı renk/tip mi?
                    if (!IsSameElementType(neighborCell.GetElement().ElementData, targetType))
                        continue;

                    // Evet! Bu cell'i component'e ekle
                    componentCells.Add(neighborPos);
                    globalVisited.Add(neighborPos);
                    queue.Enqueue(neighborPos); // Bunun da komşularını kontrol et
                }
            }

            // Toplam kaç cell bulduğumuzu döndür
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