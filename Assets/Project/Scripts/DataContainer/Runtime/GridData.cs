using UnityEngine;

namespace AgaveCase.Data.Runtime
{
    [System.Serializable]
    public class GridData 
    { 
        [Header("Grid Dimensions")]
        [Range(4, 10)]
        [SerializeField] private int _width = 8; 
        [Range(4, 10)]
        [SerializeField] private int _height = 8; 
        [Header("Grid Visuals")]
        [SerializeField] private float _cellSize = 1.0f;
        [Tooltip("How much of the screen should the grid fill (0-1)")]
        [Range(0.5f, 0.95f)]
        [SerializeField] private float _screenFillPercentage = 0.8f; 
        public int Width => _width; 
        public int Height => _height; 
        public float CellSize => _cellSize;

        public float ScreenFillPercentage => _screenFillPercentage;

        public void SetDimensions(int width, int height)
        {
            _width = width;
            _height = height;
        }
    }
}