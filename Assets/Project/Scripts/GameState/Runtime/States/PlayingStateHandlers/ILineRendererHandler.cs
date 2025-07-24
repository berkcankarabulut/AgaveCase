using System.Collections.Generic;
using Grid.Runtime;

namespace AgaveCase.GameState.Runtime
{
    public interface ILineRendererHandler
    {
        void InitializeLineRenderer();
        void UpdateSelectionLine(List<GridCell> selectedCells);
        void HideSelectionLine();
    }
}