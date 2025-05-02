using System.Collections.Generic;
using AgaveCase.Elements.Runtime;
using Grid.Runtime;
using UnityEngine;

namespace AgaveCase.Board.Runtime
{  
    public class BoardShuffleService
    {
        private readonly BoardManager _boardManager;
        private readonly BoardMatchDetectionService _matchDetectionService;
        private int _shuffleAttemptCount = 0;
        private const int _MAX_SHUFFLE_ATTEMPTS = 10;

        public BoardShuffleService(BoardManager boardManager, BoardMatchDetectionService matchDetectionService)
        {
            _boardManager = boardManager;
            _matchDetectionService = matchDetectionService;
        }

        public void ShuffleBoard()
        {  
            _shuffleAttemptCount++;
            if (_shuffleAttemptCount > _MAX_SHUFFLE_ATTEMPTS)
            { 
                _shuffleAttemptCount = 0; 
                return;
            }
             
            List<ElementDataSO> allElements = GetAllElements(); 
            ShuffleElementsOnBoard(allElements);
             
            bool hasPotentialMatches = _matchDetectionService.HasPotentialMatches(); 
            if (!hasPotentialMatches && _shuffleAttemptCount < _MAX_SHUFFLE_ATTEMPTS) 
                ShuffleBoard();  
            else 
                _shuffleAttemptCount = 0;  
        }
          
        private List<ElementDataSO> GetAllElements()
        {
            int width = _boardManager.GridWidth;
            int height = _boardManager.GridHeight;
            
            Dictionary<string, ElementDataSO> uniqueElements = new Dictionary<string, ElementDataSO>(); 
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridCell cell = _boardManager.GetGridAt(x, y);
                    if (cell == null) continue;
                    
                    ElementBase element = cell.GetElement();
                    if (element == null || element.ElementData == null) continue;
                    
                    string elementId = element.ElementData.Id.ToHexString();
                    if (!uniqueElements.ContainsKey(elementId))
                    {
                        uniqueElements.Add(elementId, element.ElementData);
                    }
                }
            }
             
            return new List<ElementDataSO>(uniqueElements.Values);
        }
          
        private void ShuffleElementsOnBoard(List<ElementDataSO> elementTypes)
        {
            int width = _boardManager.GridWidth;
            int height = _boardManager.GridHeight; 
            List<Vector2Int> allPositions = new List<Vector2Int>();
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridCell cell = _boardManager.GetGridAt(x, y);
                    if (cell != null && cell.GetElement() != null)
                    {
                        allPositions.Add(new Vector2Int(x, y));
                    }
                }
            }
             
            ShuffleList(allPositions);
             
            List<ElementBase> allElements = new List<ElementBase>();
            
            foreach (Vector2Int pos in allPositions)
            {
                GridCell cell = _boardManager.GetGridAt(pos);
                if (cell != null && cell.GetElement() != null)
                {
                    allElements.Add(cell.GetElement()); 
                    cell.SetElement(null);
                }
            }
             
            ShuffleList(allElements);
             
            for (int i = 0; i < allElements.Count && i < allPositions.Count; i++)
            {
                Vector2Int pos = allPositions[i];
                ElementBase element = allElements[i];
                
                GridCell cell = _boardManager.GetGridAt(pos);
                if (cell != null && element != null)
                {
                    element.transform.position = cell.transform.position;
                    element.transform.SetParent(cell.transform);
                    cell.SetElement(element);
                }
            }
        } 
         
        private void ShuffleList<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        } 
    }
}