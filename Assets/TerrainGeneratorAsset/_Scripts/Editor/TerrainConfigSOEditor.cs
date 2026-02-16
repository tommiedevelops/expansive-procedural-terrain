
using TerrainGeneratorAsset;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainConfigSO))]
public class TerrainConfigSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var it = serializedObject.GetIterator();
        bool enterChildren = true;
        while (it.NextVisible(enterChildren))
        {
            if (it.propertyPath == "m_Script") continue; // don't draw the script reference
            EditorGUILayout.PropertyField(it, includeChildren: true);
            enterChildren = false;

        }
        
        serializedObject.ApplyModifiedProperties();
    }
}