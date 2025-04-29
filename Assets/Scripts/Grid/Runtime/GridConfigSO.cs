using UnityEngine;

namespace Grid.Runtime
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "Agave Case/Grid System/Grid Config")]
    public class GridConfigSO : ScriptableObject
    {
        [SerializeField] private int _width = 8;
        [SerializeField] private int _height = 8;

        public int Width => _width;
        public int Height => _height;

        public void SetDimensions(int width, int height)
        {
            _width = width;
            _height = height;
        }
    }
}