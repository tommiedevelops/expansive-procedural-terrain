
using System;
using System.Collections.Generic;
using System.Linq;
using TerrainGenerator.ChunkingSystem;
using TerrainGenerator.NoiseSystem;
using TerrainGenerator.NoiseLayers;
using UnityEngine;
using UnityEngine.Serialization;

namespace TerrainGenerator {
    public class TerrainGeneratorMB : MonoBehaviour
    {
        // Unity Facing Class
        private const int MIN_CHUNK_SIZE = 240;

        private ChunkManager _chunkManager;
        private QuadTree _quadTree;
        private LODManager _lodManager;

        [SerializeField] private Vector3 terrainDimensions;

        [SerializeField] private int rootNodeLengthMultiplier = 10;
        [SerializeField] private Camera viewerCamera;
        [SerializeField] private List<NoiseLayerSO> noiseLayers;
        [SerializeField] private float nodeMultiplier = 3f;
		[SerializeField] private NoiseGenerator noiseGenerator;
        [SerializeField] private Material terrainMaterial; 

        [SerializeField] private float _heightRange = 5.0f;
        
        private float _renderDistance;
        private void Awake()
        {
            _renderDistance = viewerCamera.farClipPlane;
            _chunkManager = new ChunkManager(noiseGenerator, _heightRange, terrainMaterial, transform);
            _quadTree = GenerateQuadTree();
            _lodManager = new LODManager(MIN_CHUNK_SIZE);
            _lodManager.SetNumLODLevels(4);
        }
        
        private void Start()
        {
            // add noise from user input
            foreach(var layer in noiseLayers) noiseGenerator.AddLayer(layer);
            
            _quadTree.UpdateChildren(viewerCamera.transform.position);
            
            var leafNodes = _quadTree.GetAllLeafNodes(_quadTree.GetRootNode());
            var chunksToRender = ConvertQuadNodesToChunkData(leafNodes);
            
            _chunkManager.CreateNewChunksFromChunkData(chunksToRender); 
        }

        private void Update()
        {
            
            var culledNodes = _quadTree.UpdateChildren(viewerCamera.transform.position);
            var culledNodesConverted = ConvertQuadNodesToChunkData(culledNodes);

            _chunkManager.RecycleChunks(culledNodesConverted);

            var currLeafNodes = ConvertQuadNodesToChunkData(
                                    _quadTree.GetAllLeafNodes(_quadTree.GetRootNode()));
            
            var chunksNeeded = IdentifyLeafNodesNotActive(currLeafNodes, _chunkManager.GetActiveChunks().Keys);
            
            _chunkManager.RequestChunks(chunksNeeded);
            
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
        public void SetCamera(Camera cam) { viewerCamera = cam; }
        public ChunkManager GetChunkManager() {return _chunkManager;}
        public LODManager GetLODManager() {return _lodManager;}
        public void SetRootNodeLengthMultiplier(int multiplier) { rootNodeLengthMultiplier = multiplier; }
        public int GetRootNodeLengthMultiplier() { return rootNodeLengthMultiplier;}

        public Vector3 GetTerrainDimensions() { return terrainDimensions; }
    }
}