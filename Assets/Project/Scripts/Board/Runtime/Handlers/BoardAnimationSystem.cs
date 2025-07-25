using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Project.GridSystem.Runtime;
using Project.Elements.Runtime;
using UnityEngine;
using DG.Tweening;

namespace Project.Board.Runtime
{
    public class BoardAnimationSystem  
    {
        private readonly BoardFacade _board;
        private int _animationCount = 0;
        
        // Settings
        private readonly float _matchDelay;
        private readonly float _fallDuration;
        private readonly float _fallDelay;

        public BoardAnimationSystem(BoardFacade board, float matchDelay = 0.5f, 
                                   float fallDuration = 0.3f, float fallDelay = 0.05f)
        {
            _board = board;
            _matchDelay = matchDelay;
            _fallDuration = fallDuration;
            _fallDelay = fallDelay;
        }

        // ✅ Main methods
        public async UniTask ProcessMatchAsync(List<Vector2Int> positions)
        {
            if (positions.Count == 0) return;

            RemoveElements(positions);
            await UniTask.Delay((int)(_matchDelay * 1000));
            await ApplyGravityAsync();
            await FillEmptySpacesAsync();
        }

        public async UniTask ShuffleAsync()
        {
            var elements = GetAllElements();
            var center = CalculateCenter(elements);
            
            await AnimateToCenter(elements, center);
        }

        // ✅ ANIMATION CONTROL METHODS - EKLENEN KISIM
        public void PauseAllAnimations()
        {
            DOTween.PauseAll();
            Debug.Log("⏸️ All animations paused");
        }

        public void ResumeAllAnimations()
        {
            DOTween.PlayAll();
            Debug.Log("▶️ All animations resumed");
        }

        public bool IsAnimating()
        {
            return _animationCount > 0 || DOTween.TotalPlayingTweens() > 0;
        }

        public void KillAllAnimations()
        {
            DOTween.KillAll();
            _animationCount = 0;
            Debug.Log("🛑 All animations stopped");
        }

        public int GetActiveAnimationCount()
        {
            return _animationCount;
        }

        // ✅ Private methods - same as before
        private void RemoveElements(List<Vector2Int> positions)
        {
            foreach (var pos in positions)
            {
                var cell = _board.GetCellAt(pos);
                var element = cell?.GetElement() as DefaultElement;
                
                if (element != null)
                {
                    cell.SetElement(null);
                    element.OnMatched();
                }
            }
        }

        private async UniTask ApplyGravityAsync()
        {
            _animationCount = 0;
            bool anyMoved = false;

            for (int x = 0; x < _board.GridWidth; x++)
            {
                anyMoved |= ProcessColumn(x);
            }

            if (anyMoved)
            {
                await WaitForAnimations();
            }
        }

        private bool ProcessColumn(int x)
        {
            bool moved = false;
            int fallCount = 0;

            for (int y = 0; y < _board.GridHeight; y++)
            {
                var currentCell = _board.GetCellAt(x, y);
                if (currentCell?.GetElement() != null) continue;

                var elementAbove = FindElementAbove(x, y);
                if (elementAbove?.GetElement() is DefaultElement element)
                {
                    MoveElementDown(element, elementAbove, currentCell, fallCount);
                    fallCount++;
                    moved = true;
                    y--; // Check same position again
                }
            }

            return moved;
        }

        private GridCell FindElementAbove(int x, int startY)
        {
            for (int y = startY + 1; y < _board.GridHeight; y++)
            {
                var cell = _board.GetCellAt(x, y);
                if (cell?.GetElement() != null) return cell;
            }
            return null;
        }

        private void MoveElementDown(DefaultElement element, GridCell from, GridCell to, int order)
        {
            to.SetElement(element);
            from.SetElement(null);
            element.transform.SetParent(to.transform);

            _animationCount++;
            float delay = order * _fallDelay;

            element.PlayFallAnimation(to.transform.position, _fallDuration, 
                DG.Tweening.Ease.OutBounce, delay, () => _animationCount--);
        }

        private async UniTask FillEmptySpacesAsync()
        {
            _animationCount = 0;

            for (int x = 0; x < _board.GridWidth; x++)
            {
                int spawnOrder = 0;
                for (int y = 0; y < _board.GridHeight; y++)
                {
                    var cell = _board.GetCellAt(x, y);
                    if (cell?.GetElement() == null)
                    {
                        SpawnNewElement(cell, spawnOrder);
                        spawnOrder++;
                    }
                }
            }

            if (_animationCount > 0)
            {
                await WaitForAnimations();
            }
        }

        private void SpawnNewElement(GridCell cell, int order)
        {
            _animationCount++;
            _board.SpawnElementAtCellWithAnimation(cell, order, null, () => _animationCount--);
        }

        private async UniTask WaitForAnimations()
        {
            while (_animationCount > 0)
            {
                await UniTask.Yield();
            }
            await UniTask.Delay(100); // Small polish delay
        }

        // Shuffle helpers
        private List<Transform> GetAllElements()
        {
            var elements = new List<Transform>();
            for (int x = 0; x < _board.GridWidth; x++)
            {
                for (int y = 0; y < _board.GridHeight; y++)
                {
                    var element = _board.GetCellAt(x, y)?.GetElement();
                    if (element != null) elements.Add(element.transform);
                }
            }
            return elements;
        }

        private Vector3 CalculateCenter(List<Transform> elements)
        {
            if (elements.Count == 0) return Vector3.zero;
            
            Vector3 center = Vector3.zero;
            elements.ForEach(e => center += e.position);
            return center / elements.Count;
        }

        private async UniTask AnimateToCenter(List<Transform> elements, Vector3 center)
        {
            if (elements.Count == 0) return;

            var completion = new UniTaskCompletionSource();
            var sequence = DOTween.Sequence();

            elements.ForEach(e => sequence.Join(e.DOMove(center, 0.5f)));
            sequence.OnComplete(() => completion.TrySetResult());

            await completion.Task;
        }
    }
}