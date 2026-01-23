using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class TerrainPreviewer {
    static TerrainPreviewer() {
        SceneView.duringSceneGui += SceneView_duringSceneGui;
    }
    private static void SceneView_duringSceneGui(SceneView obj) {
    }
    private static void DrawWireframe(IPreviewAble wireframe) {
        throw new System.NotImplementedException();
    }
}

public interface IPreviewAble {}
