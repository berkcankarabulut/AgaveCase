using UnityEngine;
using UnityEngine.Serialization;

namespace AgaveCase.Data.Runtime
{
    [CreateAssetMenu(fileName = "GameDataContainer", menuName = "Agave Case/Game Data Container")]
    public class DataContainer : ScriptableObject
    { 
        [Header("Grid Configuration")]
        public GridData gridData = new GridData();
        
        [Header("Score Configuration")]
        public GameData gameData = new GameData(); 
         
    }
}