#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using GuidSystem.Runtime;
using AgaveCase.Elements.Runtime;

namespace AgaveCase.Elements.Editor
{
    [CustomEditor(typeof(ElementDataSO))]
    public class ElementDataSOEditor : UnityEditor.Editor
    {
        private SerializedProperty _elementNameProp;
        private SerializedProperty _iconProp;
        private SerializedProperty _guidProp;
        private SerializedProperty _baseSpeedProp; 
        private SerializedProperty _baseValueProp;
        private SerializedProperty _spawnEffectPrefabProp;
        private SerializedProperty _destroyEffectPrefabProp;
        private SerializedProperty _spawnSoundProp;
        private SerializedProperty _destroySoundProp;
        
        private ElementDataSO _elementData;
        private Texture2D _previewBackground;

        private void OnEnable()
        {
            _elementNameProp = serializedObject.FindProperty("_elementName");
            _iconProp = serializedObject.FindProperty("_icon");
            _guidProp = serializedObject.FindProperty("_id"); 
            _baseSpeedProp = serializedObject.FindProperty("_baseSpeed"); 
            _baseValueProp = serializedObject.FindProperty("_baseValue");
            _spawnEffectPrefabProp = serializedObject.FindProperty("_spawnEffectPrefab");
            _destroyEffectPrefabProp = serializedObject.FindProperty("_destroyEffectPrefab");
            _spawnSoundProp = serializedObject.FindProperty("_spawnSound");
            _destroySoundProp = serializedObject.FindProperty("_destroySound");
            
            _elementData = (ElementDataSO)target;
             
            EnsureGuidExists(); 
            _previewBackground = CreateCheckerboardTexture(16, 16, Color.gray, new Color(0.3f, 0.3f, 0.3f, 1.0f));
        }
        
        private Texture2D CreateCheckerboardTexture(int width, int height, Color col1, Color col2)
        {
            Texture2D tex = new Texture2D(width, height);
            Color[] colors = new Color[width * height];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colors[y * width + x] = ((x + y) % 2 == 0) ? col1 : col2;
                }
            }
            
            tex.SetPixels(colors);
            tex.Apply();
            return tex;
        }
        
        private void OnDisable()
        { 
            if (_previewBackground != null)
            {
                DestroyImmediate(_previewBackground);
                _previewBackground = null;
            }
        }
        
        private void EnsureGuidExists()
        {
            SerializedProperty part1Prop = _guidProp.FindPropertyRelative("Part1");
            SerializedProperty part2Prop = _guidProp.FindPropertyRelative("Part2");
            SerializedProperty part3Prop = _guidProp.FindPropertyRelative("Part3");
            SerializedProperty part4Prop = _guidProp.FindPropertyRelative("Part4");
             
            if (part1Prop.longValue == 0 && part2Prop.longValue == 0 && 
                part3Prop.longValue == 0 && part4Prop.longValue == 0)
            { 
                SerializableGuid newGuid = SerializableGuid.NewGuid();
                 
                part1Prop.longValue = newGuid.Part1;
                part2Prop.longValue = newGuid.Part2;
                part3Prop.longValue = newGuid.Part3;
                part4Prop.longValue = newGuid.Part4;
                
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
             
            Sprite icon = _elementData.Icon;
            if (icon != null)
            {
                EditorGUILayout.Space(5);
                DrawIconPreview(icon, 128);
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }
            
            EditorGUILayout.PropertyField(_elementNameProp, new GUIContent("Element Name"));
            EditorGUILayout.PropertyField(_iconProp, new GUIContent("Icon"));
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Element Properties", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_baseSpeedProp, new GUIContent("Base Speed")); 
            EditorGUILayout.PropertyField(_baseValueProp, new GUIContent("Base Value"));
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_spawnEffectPrefabProp, new GUIContent("Spawn Effect"));
            EditorGUILayout.PropertyField(_destroyEffectPrefabProp, new GUIContent("Destroy Effect"));
            EditorGUILayout.PropertyField(_spawnSoundProp, new GUIContent("Spawn Sound"));
            EditorGUILayout.PropertyField(_destroySoundProp, new GUIContent("Destroy Sound"));
            
            EditorGUILayout.Space(10);
            EditorGUI.BeginDisabledGroup(true);
             
            SerializedProperty part1Prop = _guidProp.FindPropertyRelative("Part1");
            SerializedProperty part2Prop = _guidProp.FindPropertyRelative("Part2");
            SerializedProperty part3Prop = _guidProp.FindPropertyRelative("Part3");
            SerializedProperty part4Prop = _guidProp.FindPropertyRelative("Part4");
             
            SerializableGuid currentGuid = new SerializableGuid(
                (uint)part1Prop.longValue,
                (uint)part2Prop.longValue,
                (uint)part3Prop.longValue,
                (uint)part4Prop.longValue
            );
            
            EditorGUILayout.TextField("GUID", currentGuid.ToHexString());
            EditorGUI.EndDisabledGroup();
             
            if (GUILayout.Button("Generate New GUID"))
            {
                Undo.RecordObject(_elementData, "Generate New GUID");
                 
                SerializableGuid newGuid = SerializableGuid.NewGuid();
                 
                part1Prop.longValue = newGuid.Part1;
                part2Prop.longValue = newGuid.Part2;
                part3Prop.longValue = newGuid.Part3;
                part4Prop.longValue = newGuid.Part4;
                
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawIconPreview(Sprite icon, float maxSize)
        {
            if (icon == null || icon.texture == null) return;
             
            float ratio = icon.rect.width / icon.rect.height;
            float width, height;
            
            if (ratio > 1)  
            {
                width = maxSize;
                height = maxSize / ratio;
            }
            else 
            {
                width = maxSize * ratio;
                height = maxSize;
            }
             
            Rect previewRect = GUILayoutUtility.GetRect(width, height);
            float availableWidth = EditorGUIUtility.currentViewWidth - 30; 
            previewRect.x = (availableWidth - width) * 0.5f;
            previewRect.width = width;
             
            if (_previewBackground != null)
            {
                GUI.DrawTextureWithTexCoords(previewRect, _previewBackground, 
                    new Rect(0, 0, previewRect.width / 16, previewRect.height / 16));
            }
             
            Rect texCoords = new Rect(
                icon.rect.x / icon.texture.width,
                icon.rect.y / icon.texture.height,
                icon.rect.width / icon.texture.width,
                icon.rect.height / icon.texture.height);
             
            GUI.DrawTextureWithTexCoords(previewRect, icon.texture, texCoords);
             
            EditorGUI.DrawRect(new Rect(previewRect.x - 1, previewRect.y - 1, previewRect.width + 2, 1), Color.gray); 
            EditorGUI.DrawRect(new Rect(previewRect.x - 1, previewRect.y, 1, previewRect.height), Color.gray); 
            EditorGUI.DrawRect(new Rect(previewRect.x + previewRect.width, previewRect.y, 1, previewRect.height), Color.gray); 
            EditorGUI.DrawRect(new Rect(previewRect.x - 1, previewRect.y + previewRect.height, previewRect.width + 2, 1), Color.gray);  
        }
    }
}
#endif