using System;
using Project.GridSystem.Runtime;
using UnityEngine;

namespace Project.GameState.Runtime
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