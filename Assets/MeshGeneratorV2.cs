using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class MeshGeneratorV2 : MonoBehaviour {

    MeshData meshData;
    Mesh mesh;
    private int planeWidth = 10;
    private int planeHeight = 10;

    public void ClearMesh() {

    public struct MeshData {
        public readonly Vector3[] vertices;
        public readonly int[] triangles;

        public MeshData(Vector3[] vertices, int[] triangles) {
            this.vertices = vertices;
            this.triangles = triangles;
        }
    }

    private void OnDrawGizmos() {
        // also draw the vertices for debugging
        if(meshData.vertices == null) return;
        for (int i = 0; i < meshData.vertices.Length; i++)
            Gizmos.DrawSphere(meshData.vertices[i], 0.1f);
    }

    public void DrawMesh() {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshData planeMeshData = CreatePlaneMeshData(planeWidth, planeHeight);
        meshData = planeMeshData;

        mesh = new() {
            vertices = planeMeshData.vertices,
            triangles = planeMeshData.triangles
        };
        // let unity calculate normals for us
        mesh.RecalculateNormals();
        meshFilter.sharedMesh = mesh;

        // Draw gizmos for debugging
        OnDrawGizmos();
    }

    public MeshData CreatePlaneMeshData(int width, int length) {
        /* 
         * Width and Length are measured in the number of vertices
         * The distance between vertices is given by the unit distance defined
         * by the game engine (Unity).
         */

        int numVertices = width * length;
        int numTriangles = 3*4 * numVertices / 2;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numTriangles];

        for(int i = 0, y = 0; y < length; y++) {
            for(int x=0; x < width; x++) {
                vertices[i] = new Vector3(x, 0f, y);
                i++;
            }
        }

        return new MeshData(vertices, triangles);

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

        if (GUILayout.Button("Clear") {
            meshGen.
        }
    }


}