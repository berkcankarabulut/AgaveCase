using Grid.Runtime;
using UnityEngine;

namespace AgaveCase.Board.Runtime
{ 
    public class BoardGridService
    {
        private readonly GridCell _gridCellPrefab;
        private readonly Transform _gridContainer;
        private readonly float _cellSize;
        private readonly float _screenFillPercentage;
        private readonly Camera _camera; 
        public BoardGridService(GridCell gridCellPrefab,  Transform gridContainer, float cellSize, float screenFillPercentage, Camera camera)
        {
            _gridCellPrefab = gridCellPrefab;
            _gridContainer = gridContainer;
            _cellSize = cellSize;
            _screenFillPercentage = screenFillPercentage;
            _camera = camera;
        } 
        
        public GridCell[,] CreateGrid(int width, int height)
        {
            GridCell[,] gridCells = new GridCell[width, height];
             
            float adjustedCellSize = CalculateCellSizeBasedOnCamera(width, height);
             
            Vector3 startPos = CalculateStartPosition(width, height);
             
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 cellPosition = startPos + new Vector3(x * adjustedCellSize, y * adjustedCellSize, 0);
                    
                    GridCell gridCell = Object.Instantiate(_gridCellPrefab, cellPosition, Quaternion.identity, _gridContainer);
                    gridCell.name = $"Grid_({x},{y})";
                    
                    AdjustGridCellSize(gridCell, adjustedCellSize);
                    gridCell.Initialize(new Vector2Int(x, y));
                    gridCells[x, y] = gridCell;
                }
            }
             
            _gridContainer.forward = _camera.transform.forward;
            
            return gridCells;
        }
         
        public GridCell GetCellAt(int x, int y, GridCell[,] gridCells)
        {
            int width = gridCells.GetLength(0);
            int height = gridCells.GetLength(1);
            
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                return gridCells[x, y];
            }
            
            return null;
        } 
         
        private float CalculateCellSizeBasedOnCamera(int width, int height)
        {
            float screenHeight = _camera.orthographicSize * 2.0f;
            float screenWidth = screenHeight * _camera.aspect;
            
            float maxGridWidth = screenWidth * _screenFillPercentage;
            float maxGridHeight = screenHeight * _screenFillPercentage;
            
            float cellSizeX = maxGridWidth / width;
            float cellSizeY = maxGridHeight / height;
            
            return Mathf.Min(cellSizeX, cellSizeY);
        }
         
        private Vector3 CalculateStartPosition(int width, int height)
        {
            float adjustedCellSize = CalculateCellSizeBasedOnCamera(width, height);
    
            float totalWidth = width * adjustedCellSize;
            float totalHeight = height * adjustedCellSize;
    
            Vector3 cameraPosition = new Vector3(_camera.transform.position.x, _camera.transform.position.y, 0);
     
            float yOffset = 0;
            if (height > width) { 
                yOffset = totalHeight * 0.1f;  
            }
    
            Vector3 startPos = cameraPosition - new Vector3(
                totalWidth * 0.5f - (adjustedCellSize * 0.5f),
                totalHeight * 0.5f - (adjustedCellSize * 0.5f) + yOffset,
                0
            );
    
            return startPos;
        }
         
        private void AdjustGridCellSize(GridCell gridCell, float targetCellSize)
        {
            SpriteRenderer spriteRenderer = gridCell.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;
            
            Vector3 originalSize = spriteRenderer.bounds.size;
            
            float scaleX = targetCellSize / originalSize.x * gridCell.transform.localScale.x;
            float scaleY = targetCellSize / originalSize.y * gridCell.transform.localScale.y;
            
            gridCell.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }
    }
}