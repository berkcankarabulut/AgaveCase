using Project.GridSystem.Runtime;
using UnityEngine;

namespace Project.Board.Runtime
{
    public class BoardGridSystem : MonoBehaviour
    {
        [SerializeField] private GridCell _gridPrefab;
        [SerializeField] private Transform _gridContainer;
        [SerializeField] private float _cellSize = 1.0f;
        [SerializeField] private float _screenFillPercentage = 0.8f;

        private GridCell[,] _gridCells;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public GridCell[,] GridCells => _gridCells;

        public void Initialize(int width, int height)
        {
            Width = width;
            Height = height;
            CreateGrid(width, height);
        }
 
        private void CreateGrid(int width, int height)
        {
            _gridCells = new GridCell[width, height];
            
            float adjustedCellSize = CalculateCellSize(width, height);
            Vector3 startPos = CalculateStartPosition(width, height, adjustedCellSize);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 cellPosition = startPos + new Vector3(x * adjustedCellSize, y * adjustedCellSize, 0);
                    
                    GridCell gridCell = Instantiate(_gridPrefab, cellPosition, Quaternion.identity, _gridContainer);
                    gridCell.name = $"Grid_({x},{y})";
                    gridCell.Initialize(new Vector2Int(x, y));
                    
                    AdjustCellSize(gridCell, adjustedCellSize);
                    _gridCells[x, y] = gridCell;
                }
            }
        }

        public GridCell GetCellAt(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                return _gridCells[x, y];
            return null;
        }

        public GridCell GetCellAt(Vector2Int position)
        {
            return GetCellAt(position.x, position.y);
        }

        public bool IsValidPosition(Vector2Int position)
        {
            return position.x >= 0 && position.x < Width && 
                   position.y >= 0 && position.y < Height;
        }
 
        private float CalculateCellSize(int width, int height)
        {
            Camera cam = Camera.main;
            float screenHeight = cam.orthographicSize * 2.0f;
            float screenWidth = screenHeight * cam.aspect;
            
            float maxGridWidth = screenWidth * _screenFillPercentage;
            float maxGridHeight = screenHeight * _screenFillPercentage;
            
            float cellSizeX = maxGridWidth / width;
            float cellSizeY = maxGridHeight / height;
            
            return Mathf.Min(cellSizeX, cellSizeY);
        }

        private Vector3 CalculateStartPosition(int width, int height, float cellSize)
        {
            float totalWidth = width * cellSize;
            float totalHeight = height * cellSize;
            
            Vector3 cameraPosition = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
            
            return cameraPosition - new Vector3(
                totalWidth * 0.5f - (cellSize * 0.5f),
                totalHeight * 0.5f - (cellSize * 0.5f),
                0
            );
        }

        private void AdjustCellSize(GridCell gridCell, float targetCellSize)
        {
            SpriteRenderer spriteRenderer = gridCell.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;
            
            Vector3 originalSize = spriteRenderer.bounds.size;
            float scaleX = targetCellSize / originalSize.x;
            float scaleY = targetCellSize / originalSize.y;
            
            gridCell.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }
    }
}