using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Project.Elements.Runtime;
using Project.GridSystem.Runtime;
using Project.Pooler.Runtime;
using UnityEngine;

namespace Project.Board.Runtime
{
    public class BoardElementSystem : MonoBehaviour
    {
        [SerializeField] private ElementBase _elementPrefab;
        [SerializeField] private ElementDataSO[] _availableElements;
        [SerializeField] private ObjectPooler _objectPooler;
        [SerializeField] private Vector3 _elementScale = new Vector3(0.6f, 0.6f, 1f);

        private BoardFacade _board;

        public void Initialize(BoardFacade board)
        {
            _board = board;
        }

        // ✅ ElementHandler.SetupElements logic'i burada
        public async UniTask PopulateGridAsync()
        {
            if (_availableElements == null || _availableElements.Length == 0) return;

            for (int x = 0; x < _board.GridWidth; x++)
            {
                for (int y = 0; y < _board.GridHeight; y++)
                {
                    var cell = _board.GetCellAt(new Vector2Int(x, y));
                    if (cell == null) continue;

                    var elementData = GetRandomElementType();
                    if (elementData == null) continue;

                    SpawnElementAtCell(cell, elementData);
                }
            }

            await UniTask.Yield();
        }

        // ✅ ElementHandler.SpawnElementAtCellWithAnimation logic'i burada
        public async UniTask SpawnElementAtCellWithAnimationAsync(GridCell cell, int spawnOrder, 
            float duration, DG.Tweening.Ease easeType, float delayBetweenFalls)
        {
            if (cell == null || _elementPrefab == null) return;

            var elementData = GetRandomElementType();
            if (elementData == null) return;

            Vector3 spawnPosition = cell.transform.position + new Vector3(0, 1.5f, 0);
            GameObject elementObj = _objectPooler.Spawn(elementData.ElementPoolTypeSo, spawnPosition, Quaternion.identity);
            
            if (elementObj?.GetComponent<DefaultElement>() is DefaultElement element)
            {
                SetupElement(cell, elementData, element);

                var completion = new UniTaskCompletionSource();
                float delay = spawnOrder * delayBetweenFalls;

                element.PlaySpawnAnimation(spawnPosition, cell.transform.position, duration, easeType, 
                    () => completion.TrySetResult());

                await completion.Task;
            }
        }

        public async UniTask RemoveElementsAsync(List<Vector2Int> positions)
        {
            var tasks = new List<UniTask>();

            foreach (var pos in positions)
            {
                var cell = _board.GetCellAt(pos);
                var element = cell?.GetElement() as DefaultElement;

                if (element != null)
                {
                    var completion = new UniTaskCompletionSource();
                    element.AnimationHandler.PlayMatchAnimation(() => completion.TrySetResult());
                    cell.SetElement(null);
                    tasks.Add(completion.Task);
                }
            }

            await UniTask.WhenAll(tasks);
        }

        public async UniTask ShuffleElementsAsync()
        {
            var elements = new List<ElementBase>();
            var positions = new List<Vector2Int>();

            // Collect all elements
            for (int x = 0; x < _board.GridWidth; x++)
            {
                for (int y = 0; y < _board.GridHeight; y++)
                {
                    var pos = new Vector2Int(x, y);
                    var element = GetElementAt(pos);
                    if (element != null)
                    {
                        elements.Add(element);
                        positions.Add(pos);
                        SetElementAt(pos, null);
                    }
                }
            }

            // Shuffle and replace
            ShuffleList(elements);
            for (int i = 0; i < elements.Count && i < positions.Count; i++)
            {
                SetElementAt(positions[i], elements[i]);
                elements[i].transform.position = _board.GetCellAt(positions[i]).transform.position;
            }

            await UniTask.Yield();
        }

        // ✅ ElementHandler.RemoveElementAt logic'i burada
        public void RemoveElementAt(Vector2Int position)
        {
            var cell = _board.GetCellAt(position);
            var element = cell?.GetElement();
            
            if (element != null)
            {
                _objectPooler.Release(element);
                cell.SetElement(null);
            }
        }

        // ✅ Private helper methods - ElementHandler logic'i burada
        private ElementDataSO GetRandomElementType()
        {
            if (_availableElements == null || _availableElements.Length == 0) return null;
            int randomIndex = Random.Range(0, _availableElements.Length);
            return _availableElements[randomIndex];
        }

        private void SpawnElementAtCell(GridCell cell, ElementDataSO elementData)
        {
            if (_elementPrefab == null) return;

            GameObject elementObj = _objectPooler.Spawn(elementData.ElementPoolTypeSo, 
                cell.transform.position, Quaternion.identity);

            if (elementObj?.GetComponent<DefaultElement>() is DefaultElement element)
            {
                SetupElement(cell, elementData, element);
            }
        }

        private void SetupElement(GridCell cell, ElementDataSO elementData, DefaultElement element)
        {
            if (element == null) return;

            element.transform.SetParent(cell.transform);
            element.transform.localScale = _elementScale;
            element.Initialize(elementData);
            cell.SetElement(element);
        }

        private ElementBase GetElementAt(Vector2Int position)
        {
            return _board.GetCellAt(position)?.GetElement();
        }

        private void SetElementAt(Vector2Int position, ElementBase element)
        {
            _board.GetCellAt(position)?.SetElement(element);
        }

        private void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = Random.Range(i, list.Count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
    }
}