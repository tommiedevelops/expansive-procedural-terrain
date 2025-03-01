using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class MeshGeneratorV2 : MonoBehaviour {

    MeshData meshData;
    Mesh mesh;
    [SerializeField] int planeWidth = 20;
    [SerializeField] int planeLength = 10;

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

    public void DrawPlaneMesh() {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshData planeMeshData = CreatePlaneMeshData(planeWidth, planeLength);
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

    public void ClearMesh() {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        mesh = new() {
            vertices = null,
            triangles = null
        };
        meshFilter.sharedMesh = mesh;
    }
    public MeshData CreatePlaneMeshData(int width, int length) {
        /* 
         * Width and Length are measured in the number of vertices
         * The distance between vertices is given by the unit distance defined
         * by the game engine (Unity).
         */

        int numVertices = width * length;
        int numTriangleVertices = 6 * numVertices;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numTriangleVertices];

        // Vertices constructed like so:
        // (0,0), (0,1), (0,2) ... (0,width), (1,width), ... (length, width)
        for(int i = 0, x = 0; x < width; x++) {
            for(int y=0; y < length; y++) {
                vertices[i] = new Vector3(x, 0f, y);
                i++;
            }
        }

        // Create triangles
        // suppose we have vertices
        // a b e f
        // c d g h
        // a is responsible for triangles (a,d,c) and (a,b,d)
        // e is responsible for triangles (e,h,g) and (e,f,h)
        // triangles = {}
        for (int i = 0, j = 0; j < (vertices.Length - width - 1) ; i+=6, j++) {
            // first triangle
            triangles[i] = j;
            triangles[i + 1] = j + width + 1;
            triangles[i + 2] = j + width;
            // second triangle
            triangles[i + 3] = j;
            triangles[i + 4] = j + 1;
            triangles[i + 5] = j + width + 1;
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
            meshGen.DrawPlaneMesh();
        }

        if(GUILayout.Button("Clear Mesh")) {
            meshGen.ClearMesh();
        }

    }


}