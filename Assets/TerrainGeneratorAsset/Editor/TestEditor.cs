using UnityEngine;
using UnityEditor;

// Create an editor window which can display a chosen GameObject.
// Use OnInteractivePreviewGUI to display the GameObject and
// allow it to be interactive.

public class ExampleClass: EditorWindow
{
    GameObject gameObject;
    Editor gameObjectEditor;

    [MenuItem("Example/GameObject Editor")]
    static void ShowWindow()
    {
        GetWindowWithRect<ExampleClass>(new Rect(0, 0, 256, 256));
    }

    void OnGUI()
    {
        gameObject = (GameObject) EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

        GUIStyle bgColor = new GUIStyle();
        bgColor.normal.background = EditorGUIUtility.whiteTexture;

        if (gameObject != null)
        {
            if (gameObjectEditor == null)
                gameObjectEditor = Editor.CreateEditor(gameObject);

            gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 256), bgColor);
        }
    }
}