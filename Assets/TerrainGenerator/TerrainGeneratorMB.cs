using System.Collections.Generic;
using System.Linq;
using TerrainGenerator.ChunkingSystem;
using TerrainGenerator.NoiseSystem;
using TerrainGenerator.NoiseLayers;
using UnityEngine;
using System.Data;


namespace TerrainGenerator {
        
    public class TerrainGeneratorMB : MonoBehaviour
    {
        private const int MIN_CHUNK_SIZE = 240;

        private ChunkManager _chunkManager;
        private QuadTree     _quadTree;
        private LODManager   _lodManager;
        private NoiseGeneratorSO _noiseGen;

        [Header("Terrain Config")]
        [SerializeField] TerrainConfigSO _terrainConfigSO;

        [Header("Noise Editor")]
        [SerializeField] NoiseGeneratorSO _noiseGeneratorSO;
        [SerializeField] private List<NoiseLayerSO> noiseLayers;

        // Noise Generator Config
        [SerializeField] private int rootNodeLengthMultiplier = 10;
        [SerializeField] private float nodeMultiplier = 3f;
        [SerializeField] private float _heightRange = 5.0f;

        [Header("Optimization Settings")]
        [SerializeField] Transform viewer;
        [SerializeField] Transform terrainParent;

        private void Awake()
        {
            _chunkManager = new ChunkManager(_noiseGen, _heightRange, _terrainConfigSO.terrainMaterial, transform);
            _quadTree = GenerateQuadTree();
            _lodManager = new LODManager(MIN_CHUNK_SIZE);
            _lodManager.SetNumLODLevels(4);
        }
        private void Start()
        {
            // add noise from user input
            foreach(var layer in noiseLayers) _noiseGen.AddLayer(layer);
            _quadTree.UpdateChildren(viewer.position);
            var leafNodes = _quadTree.GetAllLeafNodes(_quadTree.GetRootNode());
            var chunksToRender = ConvertQuadNodesToChunkData(leafNodes);
            _chunkManager.CreateNewChunksFromChunkData(chunksToRender); 
        }
        private void Update()
        {
            
            var culledNodes = _quadTree.UpdateChildren(viewer.position);
            var culledNodesConverted = ConvertQuadNodesToChunkData(culledNodes);

            _chunkManager.RecycleChunks(culledNodesConverted);

            var currLeafNodes = ConvertQuadNodesToChunkData(
                                    _quadTree.GetAllLeafNodes(_quadTree.GetRootNode()));
            
            var chunksNeeded = IdentifyLeafNodesNotActive(currLeafNodes, _chunkManager.GetActiveChunks().Keys);
            
            _chunkManager.RequestChunks(chunksNeeded);
            
        }
        public float GetHeight(float x, float z) {
            return 0.0f;
        }
        internal static void SampleNoise(Vector3 worldPos) {
            // TODO
        }
        public static List<ChunkData> IdentifyLeafNodesNotActive(List<ChunkData> newActiveChunks, Dictionary<ChunkData, GameObject>.KeyCollection currentActiveChunks)
        {
            var chunksToAdd = newActiveChunks
                .Where(chunk => !currentActiveChunks.Contains<ChunkData>(chunk))
                .ToList();

            return chunksToAdd;
        }
        private List<ChunkData> ConvertQuadNodesToChunkData(List<QuadNode> quadNodes)
        {
            var chunks = quadNodes
                .Select(node => new ChunkData()
                {
                    SideLength = node.GetSideLength(),
                    BotLeftPoint = node.GetBotLeftPoint(),
                    NumVertices = _lodManager.ComputeLOD(node.GetLevel(), _quadTree.GetTreeHeight()-1)
                })
                .ToList();
            return chunks;
        }
        private QuadTree GenerateQuadTree()
        { // Factory method to prevent side effects
            
            float rootNodeSideLength = rootNodeLengthMultiplier * MIN_CHUNK_SIZE;
            
            // We want the root node to be centred on (0,0)
            var rootNodeBottomLeftPoint = new Vector2(-rootNodeSideLength /2f, -rootNodeSideLength /2f);
            var rootNode = new QuadNode(null, rootNodeBottomLeftPoint, rootNodeSideLength);
            
            var quadTree = new QuadTree(rootNode, MIN_CHUNK_SIZE, nodeMultiplier);

            return quadTree;
        }
        public QuadTree GetQuadTree() { return _quadTree; }
        public ChunkManager GetChunkManager() {return _chunkManager;}
        public LODManager GetLODManager() {return _lodManager;}
        public void SetRootNodeLengthMultiplier(int multiplier) { rootNodeLengthMultiplier = multiplier; }
        public int GetRootNodeLengthMultiplier() { return rootNodeLengthMultiplier;}
        public Vector3 GetTerrainDimensions() {
            return _terrainConfigSO.terrainDimensions;
        }
        public Vector2Int GetTerrainResolution() {
            return new Vector2Int(_terrainConfigSO.resolutionX, _terrainConfigSO.resolutionY);
        }
        public NoiseGeneratorSO GetNoiseGenerator() {
            return _noiseGeneratorSO;
        }
    }
}