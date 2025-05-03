using System.Collections.Generic;
using AgaveCase.Elements.Runtime;
using Grid.Runtime;
using UnityEngine;

namespace AgaveCase.Board.Runtime
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
            if (HasHorizontalAdjacentMatches() || HasVerticalAdjacentMatches())
                return true; 
            return HasPotentialChains();
        }
         
        private bool HasPotentialChains()
        {
            int width = _boardManager.GridWidth;
            int height = _boardManager.GridHeight;
             
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridCell startCell = _boardManager.GetGridAt(x, y);
                    if (startCell == null || startCell.GetElement() == null) continue; 
                    if (CanFormChainOfLength(startCell, _MIN_MATCH_COUNT))
                        return true;
                }
            }
            
            return false;
        }
         
        private bool CanFormChainOfLength(GridCell startCell, int minLength)
        {
            if (startCell == null || startCell.GetElement() == null) return false; 
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
             
            return DepthFirstSearch(startCell, startCell.GetElement().ElementData, visited, minLength);
        }
         
        private bool DepthFirstSearch(GridCell cell, ElementDataSO targetType, HashSet<Vector2Int> visited, int minLength)
        { 
            if (cell == null || cell.GetElement() == null || visited.Contains(cell.Position))
                return false;
                 
            if (cell.GetElement().ElementData.Id != targetType.Id)
                return false;
                 
            visited.Add(cell.Position); 
            if (visited.Count >= minLength)
                return true;
                 
            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(1, 0),  
                new Vector2Int(-1, 0),  
                new Vector2Int(0, 1),   
                new Vector2Int(0, -1)  
            };
            
            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighborPos = cell.Position + dir;
                GridCell neighborCell = _boardManager.GetGridAt(neighborPos);
                 
                if (DepthFirstSearch(neighborCell, targetType, visited, minLength))
                    return true;
            } 
            visited.Remove(cell.Position);
            return false;
        }
          
        private bool HasHorizontalAdjacentMatches()
        {
            int width = _boardManager.GridWidth;
            int height = _boardManager.GridHeight;
            
            for (int y = 0; y < height; y++)
            { 
                ElementBase previousElement = null;
                int adjacentCount = 1;  
                
                for (int x = 0; x < width; x++)
                {
                    GridCell cell = _boardManager.GetGridAt(x, y);
                    if (cell == null) continue;
                    
                    ElementBase currentElement = cell.GetElement();
                    if (currentElement == null) continue;
                     
                    if (previousElement != null && IsSameElementType(previousElement, currentElement))
                    {
                        adjacentCount++; 
                        if (adjacentCount >= _MIN_MATCH_COUNT) return true;
                    }
                    else
                    { 
                        adjacentCount = 1;
                    }
                    
                    previousElement = currentElement;
                }
            }
            
            return false;
        }
          
        private bool HasVerticalAdjacentMatches()
        {
            int width = _boardManager.GridWidth;
            int height = _boardManager.GridHeight;
            
            for (int x = 0; x < width; x++)
            { 
                ElementBase previousElement = null;
                int adjacentCount = 1;
                
                for (int y = 0; y < height; y++)
                {
                    GridCell cell = _boardManager.GetGridAt(x, y);
                    if (cell == null) continue;
                    
                    ElementBase currentElement = cell.GetElement();
                    if (currentElement == null) continue;
                     
                    if (previousElement != null && IsSameElementType(previousElement, currentElement))
                    {
                        adjacentCount++;
                         
                        if (adjacentCount >= _MIN_MATCH_COUNT)
                        {
                            return true;
                        }
                    }
                    else
                    { 
                        adjacentCount = 1;
                    }
                    
                    previousElement = currentElement;
                }
            }
            
            return false;
        }
  
        private bool IsSameElementType(ElementBase element1, ElementBase element2)
        {
            if (element1 == null || element2 == null) return false;
            if (element1.ElementData == null || element2.ElementData == null) return false;
            
            return element1.ElementData.Id.Equals(element2.ElementData.Id);
        }
    }
}