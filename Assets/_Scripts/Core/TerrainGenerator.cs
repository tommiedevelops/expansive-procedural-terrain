using UnityEngine;
using System.Collections.Generic;
using ChunkingSystem;
using static QuadTree;
using static PlaneMeshGenerator;
using System;

namespace Core {
    public class TerrainGenerator : MonoBehaviour {

        public event Action<IEnumerable<QuadNode>> OnLeafNodesReady;
        public event Action<IEnumerable<QuadNode>> OnCulledLeafNodesReady;

        public const int MAX_NUM_VERTICES_PER_SIDE = 120;
        public static readonly int[] FACTORS_OF_MAX_NUM_VERTICES_PER_SIDE = { 1, 2, 3, 4, 6, 8, 10, 12 };

        public void SetRootNodeLengthMultiplier(int rootNodeLengthMultiplier) {
            this.rootNodeLength = MAX_NUM_VERTICES_PER_SIDE * rootNodeLengthMultiplier;
        }

        [SerializeField] private float renderDistance;
        [SerializeField] private Camera camera;
        [SerializeField] private int rootNodeLengthMultiplier = 1;

        private QTViewer _viewer;
        private QuadTree _quadTree; // Generalise this to a collection in the future
        private readonly Dictionary<uint, GameObject> _quadTreeChunks = new();

        int rootNodeLength;

        void Awake() {
            rootNodeLength = MAX_NUM_VERTICES_PER_SIDE * rootNodeLengthMultiplier;
            _viewer = new QTViewer(camera.transform, camera.fieldOfView, renderDistance);
            
            _quadTree = GenerateQuadTree(_viewer);
        }
        private void Update() {

            // Update the Quad Tree based on the new view triangle
            List<QuadNode> culledLeafNodes = _quadTree.Update();

            // Fire events
            OnCulledLeafNodesReady?.Invoke(culledLeafNodes);
            OnLeafNodesReady?.Invoke(_quadTree.GetRootNode().GetAllLeafNodes());

            //UpdateChunks();
            Debug.Log($"number of chunks: {_quadTree.GetRootNode().GetAllLeafNodes().Count}");

        }
        private void UpdateChunks() {
            List<QuadNode> leafNodes = _quadTree.GetRootNode().GetAllLeafNodes();

            foreach (QuadNode leafNode in leafNodes) {
                uint hash = 0;

                if (_quadTreeChunks.TryGetValue(hash, out GameObject value)) {
                    // Chunk exists
                } else {
                    // Chunk does not exist

                    // Generate new chunk
                    int leafNodeLevel = leafNode.GetLevel();
                    int chunkLODIndex = _quadTree.GetTreeHeight() - leafNodeLevel;
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

                    // Add it to the dictionary
                    _quadTreeChunks[hash] = chunkObject;
                }
            }
        }
        public QuadTree GetQuadTree() { return _quadTree; }

        public QuadTree GenerateQuadTree(QTViewer viewer) {
            // Create root node
            QuadNode rootNode = new QuadNode(null, new Vector2(-0.5f * rootNodeLength, -0.5f * rootNodeLength), rootNodeLength);
            rootNode.SetLevel(0);

            // Create quad tree
            QuadTree tree = new QuadTree(rootNode);

            // Add viewer to the quad tree
            tree.SetViewer(viewer);

            return tree;
        }

        public QTViewer GetQTViewer()
        {
            return _viewer;
        }
    }
}