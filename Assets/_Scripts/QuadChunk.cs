using UnityEngine;
using UnityEngine.AI;
using static PlaneMeshGenerator;

public class QuadChunk {
    Mesh mesh;
    Vector2 botLeftPoint;
    float sideLength;
    int lodLevel;

    public QuadChunk(Vector3 botLeftPoint, float lodConstant, float sideLength) {
        this.botLeftPoint = botLeftPoint;
        this.sideLength = sideLength;
        //this.lodConstant = lodConstant;

        MeshData meshData = new((int)sideLength, (int)sideLength, lodConstant);
        mesh = GeneratePlaneMesh(meshData);
    }

    public GameObject RenderChunk() {
        // Render the chunk
        GameObject chunkObject;
        chunkObject = new GameObject("Chunk", typeof(MeshFilter), typeof(MeshRenderer));
        chunkObject.GetComponent<MeshFilter>().mesh = mesh;
        chunkObject.GetComponent<MeshRenderer>().material = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline.defaultMaterial;
        chunkObject.transform.position = new Vector3(botLeftPoint.x, 0f, botLeftPoint.y);
        return chunkObject;
    }
}

public class LODManager {
    // The tree whose chunk LODs we are managing
    QuadTree tree;


}