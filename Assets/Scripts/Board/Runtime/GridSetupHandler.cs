using Grid.Runtime;
using UnityEngine;

namespace AgaveCase.Board.Runtime
{ 
    public class GridSetupHandler
    {
        private readonly GridCell _gridCellPrefab;
        private readonly Transform _gridContainer;
        private float _cellSize;
        private float _screenFillPercentage;
        private bool _autoAdjustToCamera;
        private Camera _mainCamera;

        public GridSetupHandler(GridCell gridCellPrefab, Transform gridContainer, float cellSize = 1.0f, bool autoAdjustToCamera = true, float screenFillPercentage = 0.8f)
        {
            _gridCellPrefab = gridCellPrefab;
            _gridContainer = gridContainer;
            _cellSize = cellSize;
            _autoAdjustToCamera = autoAdjustToCamera;
            _screenFillPercentage = screenFillPercentage;
            _mainCamera = Camera.main;
        }
 
        public GridCell[,] CreateGrid(int width, int height, Vector3 centerPosition)
        {
            GridCell[,] gridCells = new GridCell[width, height]; 
            
            if (_autoAdjustToCamera)
            {
                CalculateCellSizeBasedOnCamera(width, height);
            }
              
            Vector3 startPos = CalculateStartPosition(width, height, centerPosition);
             
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {  
                    Vector3 cellPosition = startPos + new Vector3(x * _cellSize, y * _cellSize, 0);
                      
                    GridCell gridCellGridCell = Object.Instantiate(_gridCellPrefab, cellPosition, Quaternion.identity, _gridContainer);
                    gridCellGridCell.name = $"Grid_({x},{y})";
                      
                    AdjustGridCellSize(gridCellGridCell); 
                    gridCellGridCell.Initialize(new Vector2Int(x, y)); 
                    gridCells[x, y] = gridCellGridCell;
                }
            } 
            return gridCells;
        }
        
        private void CalculateCellSizeBasedOnCamera(int width, int height)
        {  
            // For orthographic camera game
            float screenHeight = _mainCamera.orthographicSize * 2.0f;
            float screenWidth = screenHeight * _mainCamera.aspect;
             
            float maxGridWidth = screenWidth * _screenFillPercentage;
            float maxGridHeight = screenHeight * _screenFillPercentage;
             
            float cellSizeX = maxGridWidth / width;
            float cellSizeY = maxGridHeight / height;
             
            _cellSize = Mathf.Min(cellSizeX, cellSizeY); 
        } 
        
        private Vector3 CalculateStartPosition(int width, int height, Vector3 centerPosition)
        { 
            float totalWidth = width * _cellSize;
            float totalHeight = height * _cellSize;
             
            Vector3 startPos = centerPosition - new Vector3(totalWidth * 0.5f - (_cellSize * 0.5f), totalHeight * 0.5f - (_cellSize * 0.5f), 0);
            return startPos;
        }
        
        private void AdjustGridCellSize(GridCell gridCellGridCell)
        { 
            SpriteRenderer spriteRenderer = gridCellGridCell.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            { 
                Vector3 originalSize = spriteRenderer.bounds.size;
                 
                float scaleX = _cellSize / originalSize.x * gridCellGridCell.transform.localScale.x;
                float scaleY = _cellSize / originalSize.y * gridCellGridCell.transform.localScale.y;
                
                gridCellGridCell.transform.localScale = new Vector3(scaleX, scaleY, 1f);
            }
              
        }
 
        public void CleanupGrid(GridCell[,] gridCells)
        {
            if (gridCells == null) return;
            
            for (int x = 0; x < gridCells.GetLength(0); x++)
            {
                for (int y = 0; y < gridCells.GetLength(1); y++)
                {
                    if (gridCells[x, y] != null)
                    {
                        Object.Destroy(gridCells[x, y].gameObject);
                    }
                }
            }
             
            if (_gridContainer != null)
            {
                foreach (Transform child in _gridContainer)
                {
                    Object.Destroy(child.gameObject);
                }
            }
        }
    }
}