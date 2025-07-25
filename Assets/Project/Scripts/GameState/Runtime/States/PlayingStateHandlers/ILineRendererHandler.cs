using System.Collections.Generic;
using Project.GridSystem.Runtime;

namespace Project.GameState.Runtime
{
    public interface ILineRendererHandler
    {
        void InitializeLineRenderer();
        void UpdateSelectionLine(List<GridCell> selectedCells);
        void HideSelectionLine();
    }
}