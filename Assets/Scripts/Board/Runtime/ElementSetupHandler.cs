using AgaveCase.Elements.Runtime;
using Grid.Runtime;
using Pooler;
using UnityEngine;

namespace AgaveCase.Board.Runtime
{
   public class ElementSetupHandler
    {
        private readonly ObjectPooler _objectPooler;
        private readonly ElementDataSO[] _elementTypes;
        private readonly ElementBase _element;
        private readonly float _elementScaleFactor = 0.8f;  
        
        public ElementSetupHandler(ElementDataSO[] elementTypes, ElementBase element, ObjectPooler objectPooler)
        { 
            _objectPooler = objectPooler;
            _element = element;
            _elementTypes = elementTypes;
        }

        public void SetupElements(GridCell[,] gridCells)
        {
            if (gridCells == null || _elementTypes == null || _elementTypes.Length == 0) return;
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

        private ElementDataSO GetRandomElementType()
        {
            if (_elementTypes.Length == 0) return null;

            int randomIndex = Random.Range(0, _elementTypes.Length);
            return _elementTypes[randomIndex];
        }

        private void SpawnElementAtCell(GridCell cell, ElementDataSO elementData)
        { 
            if (_element == null)
            {
                Debug.LogWarning("Element prefab not found for element type: " + elementData.ElementName);
                return;
            } 
            
            GameObject elementObj = _objectPooler.Spawn(_element.gameObject, cell.transform.position, Quaternion.identity);
            if (elementObj == null) return;
 
            ElementBase element = elementObj.GetComponent<ElementBase>();
            if (element != null)
            {
                element.Initialize(elementData);
                 
                AdjustElementSizeToCell(element, cell); 
                element.transform.SetParent(cell.transform); 
                cell.SetCandy(element);  
            }
            else
            {
                Debug.LogError("Spawned object does not have an ElementBase component");
            }
        }
        
        private void AdjustElementSizeToCell(ElementBase element, GridCell cell)
        {
            if (element == null || cell == null) return;
             
            SpriteRenderer cellRenderer = cell.GetComponent<SpriteRenderer>();
            if (cellRenderer == null) return;
             
            SpriteRenderer elementRenderer = element.GetComponent<SpriteRenderer>();
            if (elementRenderer == null) return;
            
            Vector2 cellSize = cellRenderer.bounds.size;
            Vector2 originalElementSize = elementRenderer.bounds.size;
             
            float scaleX = (cellSize.x * _elementScaleFactor) / originalElementSize.x;
            float scaleY = (cellSize.y * _elementScaleFactor) / originalElementSize.y;
             
            float scale = Mathf.Min(scaleX, scaleY);
             
            element.transform.localScale = new Vector3(scale, scale, 1f);
        }

        public void ClearElements(GridCell[,] gridCells)
        {
            if (gridCells == null) return;

            int width = gridCells.GetLength(0);
            int height = gridCells.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridCell cell = gridCells[x, y];
                    if (cell == null) continue;

                    ElementBase element = cell.GetCandy();
                    if (element == null) continue;
                    _objectPooler.Release(element.GameObject, element);
                    cell.SetCandy(null);
                }
            }
        }
    }
}