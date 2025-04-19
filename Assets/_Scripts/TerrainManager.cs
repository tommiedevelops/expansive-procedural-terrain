using UnityEngine;
using System.Collections.Generic;
using static QuadTree;
using static PlaneMeshGenerator;

public class TerrainManager : MonoBehaviour {

    public const int MAX_NUM_VERTICES_PER_SIDE = 120;
    public static readonly int[] FACTORS_OF_MAX_NUM_VERTICES_PER_SIDE = { 1, 2, 3, 4, 6, 8, 10, 12 };
   
    #region Debugging
    List<Bounds> boundsToDraw = new(); // For Debugging
    List<Bounds> culledBounds = new();
    private void OnDrawGizmos() {
        //if (null == viewer) return;
        GizmosDrawViewTriangleAndTriBounds();
        GizmosDrawNodeSquares();
        GizmosDrawCulledNodes();
    }

    void GizmosDrawCulledNodes() {
        Gizmos.color = Color.red;
        foreach (Bounds bounds in culledBounds) {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }

    private void GizmosDrawNodeSquares() {
        Gizmos.color = Color.green;
        foreach (Bounds bounds in boundsToDraw) {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
    private void GizmosDrawViewTriangleAndTriBounds() {
        Vector3[] viewTriangle = viewer.GetViewTriangle();
        if (viewTriangle == null || viewTriangle.Length == 0) return;
        Gizmos.DrawLine(viewTriangle[0], viewTriangle[1]);
        Gizmos.DrawLine(viewTriangle[1], viewTriangle[2]);
        Gizmos.DrawLine(viewTriangle[0], viewTriangle[2]);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(viewer.GetTriBounds().center, viewer.GetTriBounds().size);
    }
    #endregion

    #region Serialize Fields
    [SerializeField] QTViewer viewer;
    [SerializeField] int rootNodeLengthMultiplier = 1;
    [SerializeField] GameObject quadChunkParent;
    [SerializeField] GameObject bin;
    #endregion

    #region Regular Fields
    QuadTree quadTree; // Generalise this to a collection in the future
    readonly Dictionary<uint, GameObject> quadTreeChunks = new();
    
    int rootNodeLength;
    Vector3 storedViewerPosition;
    #endregion

    #region Unity Functions
    void Awake() {
        rootNodeLength = MAX_NUM_VERTICES_PER_SIDE * rootNodeLengthMultiplier;
        storedViewerPosition = viewer.GetPosition();

        // Create root node
        QuadNode rootNode = new QuadNode(null, new Vector2(-0.5f * rootNodeLength, -0.5f * rootNodeLength), rootNodeLength);
        rootNode.SetLevel(0);

        // Create quad tree
        quadTree = new QuadTree(rootNode, viewer);
    }
    private void Update() {
        
        List<uint> culledLeafNodeHashes = quadTree.Update(viewer.GetViewTriangle(), viewer.GetTriBounds());
        quadTree.DrawTreeForDebugging(ref boundsToDraw);
        //quadTree.DrawCulledNodesForDebugging(ref culledBounds);

        //DealWithCulledNodes(culledLeafNodeHashes); // Still WIP

        List<QuadNode> leafNodes = quadTree.GetRootNode().GetAllLeafNodes();

        foreach (QuadNode leafNode in leafNodes) {
            uint hash = leafNode.ComputeHash();

            if (quadTreeChunks.TryGetValue(hash, out GameObject value)) {
                // Chunk exists
            } else {
                // Chunk does not exist

                // Generate new chunk
                int leafNodeLevel = leafNode.GetLevel();
                int chunkLODIndex = quadTree.GetTreeHeight() - leafNodeLevel;
                //int chunkLODIndexOffset = 2;
                int chunkScaleFactor = FACTORS_OF_MAX_NUM_VERTICES_PER_SIDE[chunkLODIndex];

                float requiredMeshLength = leafNode.GetSideLength();

                int numVertsPerSide = MAX_NUM_VERTICES_PER_SIDE / chunkScaleFactor;

                MeshData newMeshData = new MeshData(numVertsPerSide, numVertsPerSide, requiredMeshLength);
                Mesh newMesh = GeneratePlaneMesh(newMeshData);

                GameObject chunkObject;
                string chunkName = $"BotLeftPoint:{leafNode.GetBotLeftPoint()},chunkLOD:{chunkLODIndex}, chunkScaleFactor:{chunkScaleFactor}, numVerticesPerSide:{numVertsPerSide} ";
                chunkObject = new GameObject(chunkName, typeof(MeshFilter), typeof(MeshRenderer));
                chunkObject.GetComponent<MeshFilter>().mesh = newMesh;
                chunkObject.GetComponent<MeshRenderer>().material = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline.defaultMaterial;
                chunkObject.transform.position = new Vector3(leafNode.GetBotLeftPoint().x, 0f, leafNode.GetBotLeftPoint().y);
                chunkObject.transform.SetParent(quadChunkParent.transform);

                // Add it to the dictionary
                quadTreeChunks[hash] = chunkObject;
            }
        }

        // Debugging
    }
    private void DealWithCulledNodes(List<uint> culledLeafNodeHashes) {
        foreach (uint hash in culledLeafNodeHashes) {

            if (quadTreeChunks.TryGetValue(hash, out GameObject culledChunk)) {
                culledChunk.transform.SetParent(bin.transform);
                culledChunk.SetActive(false);
                quadTreeChunks.Remove(hash);
            }
        }
    }
    private void OnValidate() {
        // I should really be configuring the noise separately then 
        // running it on here.
        ValidateNoiseSettings();
        //UpdateNoise();
    }
    #endregion

    #region Helper Functions
    // HELPERS

    private void ValidateNoiseSettings() {
        //noiseSettings.width = MAX_NUM_VERTICES_PER_SIDE;
        //noiseSettings.length = MAX_NUM_VERTICES_PER_SIDE;
        //if (noiseSettings.persistance > 1) noiseSettings.persistance = 0.99f;
        //if (noiseSettings.persistance < 0) noiseSettings.persistance = 0.01f;
        //if (noiseSettings.octaves < 0) noiseSettings.octaves = 0;
        //if (noiseSettings.octaves > 6) noiseSettings.octaves = 6;
        //if (noiseSettings.lacunarity < 0) noiseSettings.lacunarity = 0.01f;
    }
    #endregion


}
