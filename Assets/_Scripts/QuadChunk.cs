using UnityEngine;
using UnityEngine.AI;
using static PlaneMeshGenerator;
using static QuadTree;

public class QuadChunk {
    Mesh mesh;
    Vector2 botLeftPoint;
    float sideLength;
    int lodLevel;
    Mesh chunkMesh;

    public QuadChunk(QuadNode leafNode) {
        this.botLeftPoint = leafNode.GetBotLeftPoint();
        this.sideLength = leafNode.GetSideLength();

        int leafNodeLevel = leafNode.GetLevel();
        //int chunkLODIndex = quadTree.GetTreeHeight() - leafNodeLevel;
        //int chunkLODIndexOffset = 2;
        //int chunkScaleFactor = RealTerrainManager.FACTORS_OF_MAX_NUM_VERTICES_PER_SIDE[chunkLODIndex + chunkLODIndexOffset];

        //string chunkName = $"BotLeftPoint:{leafNode.GetBotLeftPoint()},chunkLOD:{chunkLODIndex}, chunkScaleFactor:{chunkScaleFactor}, numVerticesPerSide:{numVertsPerSide} ";
        //chunkObject = new GameObject(chunkName, typeof(MeshFilter), typeof(MeshRenderer));
        //chunkObject.GetComponent<MeshFilter>().mesh = newMesh;
        //chunkObject.GetComponent<MeshRenderer>().material = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline.defaultMaterial;
        //chunkObject.transform.position = new Vector3(leafNode.GetBotLeftPoint().x, 0f, leafNode.GetBotLeftPoint().y);

    }


}

public class LODManager {
    // The tree whose chunk LODs we are managing
    QuadTree tree;


}