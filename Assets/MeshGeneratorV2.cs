using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGeneratorV2 : MonoBehaviour {

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    
    
    public void DrawMesh() {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        GetMesh();
        mesh = new() {
            vertices = vertices,
            triangles = triangles
        };
        // let unity calculate normals for us
        mesh.RecalculateNormals();
        meshFilter.sharedMesh = mesh;
    }

    private void GetMesh() {
        vertices = new Vector3[] {
            new Vector3(0,0,0),
            new Vector3(0,0,1),
            new Vector3(1,0,0),
            new Vector3(1,0,1),
        };

        triangles = new int[] {
            0,
            1,
            2,
            1,
            3,
            2
        };
    }
}


[CustomEditor(typeof(MeshGeneratorV2))]
public class MeshGeneratorEditor : Editor {

    MeshGeneratorV2 meshGen;
    public override void OnInspectorGUI() {
        MeshGeneratorV2 meshGen = (MeshGeneratorV2)target;

        if (GUILayout.Button("Generate")) {
            meshGen.DrawMesh();
        }
    }
}