using System; 
using Project.GridSystem.Runtime;
using Project.Elements.Runtime;
using Project.Pooler.Runtime;
using UnityEngine;

namespace Project.Board.Runtime
{ 
    public class ElementHandler
    {
        private readonly ElementDataSO[] _availableElements;
        private readonly ElementBase _elementPrefab;
        private readonly ObjectPooler _objectPooler;
        private readonly Vector3 _elementScale = new Vector3(0.6f,0.6f,1);
        public ElementHandler(ElementDataSO[] availableElements, ElementBase elementPrefab, ObjectPooler objectPooler)
        {
            _availableElements = availableElements;
            _elementPrefab = elementPrefab;
            _objectPooler = objectPooler;
        }
        
        public void SetupElements(GridCell[,] gridCells)
        {
            if (gridCells == null || _availableElements == null || _availableElements.Length == 0) return;
            
            int width = gridCells.GetLength(0);
            int height = gridCells.GetLength(1);
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridCell cell = gridCells[x, y];
                    if (cell == null) continue;
                    
                    ElementDataSO elementData = GetRandomElementType();
                    if (elementData == null) continue;
                    
                    SpawnElementAtCell(cell, elementData);
                }
            }
        }
         
        public void SpawnElementAtCellWithAnimation(
            GridCell cell, 
            int spawnOrder, 
            float duration,
            DG.Tweening.Ease easeType,
            float delayBetweenFalls,
            Action onSpawned = null, 
            Action onCompleted = null)
        {
            if (cell == null || _elementPrefab == null) return; 
            ElementDataSO elementData = GetRandomElementType();
            if (elementData == null) return;
             
            Vector3 spawnPosition = cell.transform.position + new Vector3(0, 1.5f, 0);
            GameObject elementObj = _objectPooler.Spawn(elementData.ElementPoolTypeSo, spawnPosition, Quaternion.identity);
            if (elementObj == null) return;

            DefaultElement element = elementObj.GetComponent<DefaultElement>();
            if (element == null) return; 
             
            SetupForElement(cell, elementData, element);
            onSpawned?.Invoke();
             
            float delay = spawnOrder * delayBetweenFalls;
            element.PlaySpawnAnimation(
                spawnPosition,
                cell.transform.position,
                duration,
                easeType,
                onCompleted
            );
        }
         
        public void RemoveElementAt(Vector2Int position, BoardManager boardManager)
        {
            GridCell cell = boardManager.GetGridAt(position);
            if (cell == null) return;

            ElementBase element = cell.GetElement();
            if (element == null) return;
            
            _objectPooler.Release(element);
            cell.SetElement(null);
        }

        private ElementDataSO GetRandomElementType()
        {
            if (_availableElements == null || _availableElements.Length == 0) return null;

            int randomIndex = UnityEngine.Random.Range(0, _availableElements.Length);
            return _availableElements[randomIndex];
        }
         
        private void SpawnElementAtCell(GridCell cell, ElementDataSO elementData)
        {
            if (_elementPrefab == null) return;
            
            GameObject elementObj = _objectPooler.Spawn(elementData.ElementPoolTypeSo, cell.transform.position, Quaternion.identity);
            if (elementObj == null) return; 
            
            DefaultElement element = elementObj.GetComponent<DefaultElement>();
            if (element == null) return;
            
            SetupForElement(cell, elementData, element);
        }

        private void SetupForElement(GridCell cell, ElementDataSO elementData, DefaultElement element)
        {
            if (element == null) return;
            element.transform.SetParent(cell.transform); 
            element.transform.localScale = _elementScale;
         
            element.Initialize(elementData);
            cell.SetElement(element); 
        }
    }
}