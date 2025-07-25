using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Project.SceneHandler.Runtime;

namespace Project.SceneHandler.Editor
{
    [CustomPropertyDrawer(typeof(SceneSelectorAttribute))]
    public class SceneSelectorDrawer : PropertyDrawer
    { 
        private static List<SceneData> _sceneList = new List<SceneData>();
        private static string[] _sceneDisplayNames = new string[0];
        private static bool _needsRefresh = true;
 
        private class SceneData
        {
            public string sceneName;
            public string scenePath;
            public int buildIndex;
            public bool inBuildSettings;
            
            public override string ToString()
            {
                if (inBuildSettings)
                {
                    return $"{sceneName} (Index: {buildIndex})";
                }
                return $"{sceneName} (Not in build)";
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_needsRefresh)
            {
                RefreshSceneList();
                _needsRefresh = false;
            }

            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType == SerializedPropertyType.String)
            {
                string currentSceneName = property.stringValue;
                int currentIndex = 0;
 
                for (int i = 0; i < _sceneList.Count; i++)
                {
                    if (_sceneList[i].sceneName == currentSceneName)
                    {
                        currentIndex = i;
                        break;
                    }
                }
 
                Rect dropdownRect = new Rect(position.x, position.y, position.width - 60, position.height);
                int newIndex = EditorGUI.Popup(dropdownRect, label.text, currentIndex, _sceneDisplayNames);
 
                if (newIndex != currentIndex && newIndex >= 0 && newIndex < _sceneList.Count)
                {
                    property.stringValue = _sceneList[newIndex].sceneName;
                }
 
                Rect buttonRect = new Rect(position.x + position.width - 55, position.y, 55, position.height);
                if (GUI.Button(buttonRect, "Refresh"))
                {
                    _needsRefresh = true;
                }
            }
            else
            {
                EditorGUI.LabelField(position, label, new GUIContent("SceneSelector can only be used with string fields"));
            }

            EditorGUI.EndProperty();
        }

        private void RefreshSceneList()
        {
            _sceneList.Clear();
             
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            HashSet<string> scenesInBuild = new HashSet<string>();
            
            for (int i = 0; i < sceneCount; i++)
            {
                string scenePath = EditorBuildSettings.scenes[i].path;
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                
                _sceneList.Add(new SceneData 
                { 
                    sceneName = sceneName, 
                    scenePath = scenePath,
                    buildIndex = i,
                    inBuildSettings = true
                });
                
                scenesInBuild.Add(scenePath);
            }
             
            string[] allSceneGuids = AssetDatabase.FindAssets("t:Scene");
            foreach (string guid in allSceneGuids)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(guid);
                if (!scenesInBuild.Contains(scenePath))
                {
                    string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                    
                    _sceneList.Add(new SceneData 
                    { 
                        sceneName = sceneName, 
                        scenePath = scenePath,
                        buildIndex = -1,
                        inBuildSettings = false
                    });
                }
            }
             
            _sceneDisplayNames = new string[_sceneList.Count];
            for (int i = 0; i < _sceneList.Count; i++)
            {
                _sceneDisplayNames[i] = _sceneList[i].ToString();
            }
        }
    }
}