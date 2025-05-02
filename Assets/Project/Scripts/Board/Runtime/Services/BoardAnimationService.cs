using System;
using System.Collections.Generic;
using UnityEngine;
using Grid.Runtime;
using AgaveCase.Elements.Runtime;
using DG.Tweening;

namespace AgaveCase.Board.Runtime
{
    public class BoardAnimationService
    {
        private int _activeAnimationsCount = 0;
        private List<Action> _allAnimationCompletedCallbacks = new List<Action>();
        private readonly BoardManager _boardManager;

        public BoardAnimationService(BoardManager boardManager)
        {
            _boardManager = boardManager;
        }

        public void ProcessMatchedElements(List<Vector2Int> positions, float matchAnimationDelay)
        {
            if (positions == null || positions.Count == 0) return;
            _activeAnimationsCount = 0;
            RemoveElements(positions);
            DOVirtual.DelayedCall(matchAnimationDelay, () => { ApplyGravity(); });
        }

        public void ProcessMatchedElementsWithCallback(List<Vector2Int> positions, float matchAnimationDelay,
            Action onCompleted)
        {
            if (onCompleted != null)
            {
                AddAnimationCompletedCallback(onCompleted);
            }

            ProcessMatchedElements(positions, matchAnimationDelay);
        }

        private void RemoveElements(List<Vector2Int> positions)
        {
            if (positions == null) return;

            foreach (Vector2Int pos in positions)
            {
                GridCell cell = _boardManager.GetGridAt(pos);
                if (cell == null) continue;
        
                ElementBase element = cell.GetElement(); 
                if (element is DefaultElement defaultElement) 
                { 
                    var tempElement = defaultElement;  
                    cell.SetElement(null);  
                    tempElement.OnMatched();  
                }
            }
        }

        private void ApplyGravity()
        {
            bool anyElementMoved = false;
            int gridWidth = _boardManager.GridWidth;
            int gridHeight = _boardManager.GridHeight;

            for (int x = 0; x < gridWidth; x++)
            {
                int fallCount = 0;

                for (int y = 0; y < gridHeight; y++)
                {
                    GridCell currentCell = _boardManager.GetGridAt(x, y);
                    if (currentCell == null || currentCell.GetElement() != null) continue;

                    GridCell upperCellWithCandy = FindUpperCellWithCandy(x, y);
                    if (upperCellWithCandy == null) continue;

                    MoveElementToEmptyCell(upperCellWithCandy, currentCell, fallCount,
                        _boardManager.FallDuration, _boardManager.FallingEase, _boardManager.DelayBetweenFalls);
                    fallCount++;
                    anyElementMoved = true;

                    y--;
                }
            }

            if (!anyElementMoved)
            {
                DOVirtual.DelayedCall(0.2f, FillEmptyCells);
            }
        }

        private GridCell FindUpperCellWithCandy(int x, int startY)
        {
            int gridHeight = _boardManager.GridHeight;

            for (int y = startY + 1; y < gridHeight; y++)
            {
                GridCell cell = _boardManager.GetGridAt(x, y);
                if (cell != null && cell.GetElement() != null)
                {
                    return cell;
                }
            }

            return null;
        }

        private void MoveElementToEmptyCell(GridCell sourceCell, GridCell targetCell, int fallOrder, float duration,
            Ease easeType, float delayBetweenFalls)
        {
            ElementBase element = sourceCell.GetElement();

            if (!(element is DefaultElement defaultElement)) return;

            targetCell.SetElement(element);
            sourceCell.SetElement(null);
            element.transform.SetParent(targetCell.transform);
            _activeAnimationsCount++;
            float delay = fallOrder * delayBetweenFalls;

            defaultElement.PlayFallAnimation(targetCell.transform.position, duration, easeType, delay, () =>
                {
                    _activeAnimationsCount--;
                    if (_activeAnimationsCount == 0)
                    {
                        DOVirtual.DelayedCall(0.2f, FillEmptyCells);
                    }
                }
            );
        }

        private void FillEmptyCells()
        { 
            int gridWidth = _boardManager.GridWidth;
            int gridHeight = _boardManager.GridHeight;
            bool anyNewElementSpawned = false;

            for (int x = 0; x < gridWidth; x++)
            {
                int newElementCount = 0;
                for (int y = 0; y < gridHeight; y++)
                {
                    GridCell cell = _boardManager.GetGridAt(x, y);
                    if (cell == null) continue;
 
                    if (cell.GetElement() == null)
                    { 
                        _boardManager.SpawnElementAtCellWithAnimation(cell, newElementCount, OnNewElementSpawned,
                            OnNewElementAnimationCompleted);
                        newElementCount++;
                        anyNewElementSpawned = true;
                    }
                }
            }

            if (!anyNewElementSpawned && _allAnimationCompletedCallbacks.Count > 0)
            { 
                OnAllAnimationsCompleted();
            }
            else if (!anyNewElementSpawned)
            {
                Debug.Log("[Board] No new elements spawned and no callbacks pending.");
            }
            else if (_activeAnimationsCount == 0)
            { 
                OnAllAnimationsCompleted();
            }
            else
            { 
            }
        }

        private void OnNewElementSpawned()
        {
            _activeAnimationsCount++; 
        }

        private void OnNewElementAnimationCompleted()
        {
            _activeAnimationsCount--;

            if (_activeAnimationsCount != 0) return;
            OnAllAnimationsCompleted();
        }

        public void AddAnimationCompletedCallback(Action callback)
        {
            if (callback != null)
            {
                _allAnimationCompletedCallbacks.Add(callback);
            }
        }

        private void OnAllAnimationsCompleted()
        {
            foreach (var callback in _allAnimationCompletedCallbacks)
            {
                callback?.Invoke();
            }

            _allAnimationCompletedCallbacks.Clear();
        }
    }
}