using System;
using System.Collections.Generic;
using AgaveCase.Elements.Runtime;
using Grid.Runtime;
using UnityEngine;

namespace AgaveCase.Board.Runtime
{  
    public class ShuffleHandler
    {
        private readonly BoardManager _boardManager;
        private readonly MatchDetectionHandler _matchDetectionHandler;
        private int _shuffleAttemptCount = 0;
        private const int _MAX_SHUFFLE_ATTEMPTS = 10;

        public ShuffleHandler(BoardManager boardManager, MatchDetectionHandler matchDetectionHandler)
        {
            _boardManager = boardManager;
            _matchDetectionHandler = matchDetectionHandler;
        }
 
        public void StartShuffleAnimation(GridCell[,] gridCells, float duration, Action onCompleted)
        {
            Vector3 centerPosition = Vector3.zero;
            List<ElementBase> allElements = new List<ElementBase>();
             
            int width = gridCells.GetLength(0);
            int height = gridCells.GetLength(1);
             
            if (width > 0 && height > 0 && gridCells[0, 0] != null)
            {
                Vector3 bottomLeft = gridCells[0, 0].transform.position;
                Vector3 topRight = gridCells[width - 1, height - 1].transform.position;
                centerPosition = (bottomLeft + topRight) * 0.5f;
            }
             
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridCell cell = gridCells[x, y];
                    if (cell == null) continue;
                    
                    ElementBase element = cell.GetElement();
                    if (element == null) continue;
                    
                    allElements.Add(element);
                }
            }
             
            if (allElements.Count == 0)
            {
                onCompleted?.Invoke();
                return;
            }
             
            AnimateElementsToCells(duration, onCompleted, allElements, centerPosition);
        }

        private static void AnimateElementsToCells(float duration, Action onCompleted, List<ElementBase> allElements, Vector3 centerPosition)
        {
            int completedCount = 0;
            foreach (ElementBase element in allElements)
            {
                if (element is DefaultElement defaultElement)
                {
                    defaultElement.PlayFallAnimation(
                        centerPosition,
                        duration / 2f,
                        DG.Tweening.Ease.InBack,
                        0f,
                        () => {
                            completedCount++;
                            if (completedCount >= allElements.Count)
                            { 
                                onCompleted?.Invoke();
                            }
                        }
                    );
                }
                else
                { 
                    element.transform.position = centerPosition;
                    completedCount++;
                    if (completedCount >= allElements.Count)
                    {
                        onCompleted?.Invoke();
                    }
                }
            }
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
             
            bool hasPotentialMatches = _matchDetectionHandler.HasPotentialMatches(); 
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