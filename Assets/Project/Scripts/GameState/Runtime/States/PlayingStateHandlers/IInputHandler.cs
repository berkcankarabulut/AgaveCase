using System;
using Grid.Runtime;
using UnityEngine;

namespace AgaveCase.GameState.Runtime
{
    public interface IInputHandler
    { 
        event Action OnSelectionStarted;
        event Action<Vector3> OnSelectionContinued;
        event Action OnSelectionEnded;
         
        void ProcessInput();
        void EnableInput(bool enabled);
        GridCell GetCellUnderCursor();
    }
}