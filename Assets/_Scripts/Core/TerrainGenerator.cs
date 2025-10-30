
using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.ChunkingSystem;
using _Scripts.QuadTreeSystem;
using _Scripts.NoiseSystem;
using _Scripts.NoiseSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Core {
    public class TerrainGenerator : MonoBehaviour
    {
        // Unity Facing Class

        private const int MIN_CHUNK_SIZE = 240;

        private ChunkManager _chunkManager;
        private NoiseGenerator _noiseGenerator;
        private QTViewer _terrainViewer;
        private QuadTree _quadTree;
        private LODManager _lodManager;
        
        [SerializeField] private int rootNodeLengthMultiplier = 10;
        [SerializeField] private Camera viewerCamera;
        [SerializeField] private List<NoiseLayerSO> noiseLayers;
        [SerializeField] private float globalHeightMultiplier = 5.0f;
        [SerializeField] private float nodeMultiplier = 3f;
		[SerializeField] private NoiseGenerator noiseGenerator;
        
        private float _renderDistance;
        private void Awake()
        {
            _renderDistance = viewerCamera.farClipPlane;
            _terrainViewer = new QTViewer(viewerCamera.transform, viewerCamera.fieldOfView, _renderDistance);
            _chunkManager = new ChunkManager(noiseGenerator);
            _quadTree = GenerateQuadTree();
            _lodManager = new LODManager(MIN_CHUNK_SIZE);
            _lodManager.SetNumLODLevels(4);
        }
        
        private void Start()
        {
            // add noise from user input
            foreach(var layer in noiseLayers) noiseGenerator.AddLayer(layer);
            ChunkManager.SetGlobalHeightMultiplier(globalHeightMultiplier);
            
            _quadTree.Update();
            
            var leafNodes = _quadTree.GetRootNode().GetAllLeafNodes();
            var chunksToRender = ConvertQuadNodesToChunkData(leafNodes);
            
            _chunkManager.CreateNewChunksFromChunkData(chunksToRender); 
        }

        private void Update()
        {
            
            var culledNodes = _quadTree.Update();
            var culledNodesConverted = ConvertQuadNodesToChunkData(culledNodes);

            _chunkManager.RecycleChunks(culledNodesConverted);
            
            var currLeafNodes = ConvertQuadNodesToChunkData(
                                    _quadTree.GetRootNode().GetAllLeafNodes());
            
            var chunksNeeded = IdentifyLeafNodesNotActive(currLeafNodes, _chunkManager.GetActiveChunks().Keys);
            
            _chunkManager.RequestChunks(chunksNeeded);
            
        }
        
        #region Helpers
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
            if (_terrainViewer == null) throw new Exception("Viewer not initialized");
            
            float rootNodeSideLength = rootNodeLengthMultiplier * MIN_CHUNK_SIZE;
            
            // We want the root node to be centred on (0,0)
            var rootNodeBottomLeftPoint = new Vector2(-rootNodeSideLength /2f, -rootNodeSideLength /2f);
            var rootNode = new QuadNode(null, rootNodeBottomLeftPoint, rootNodeSideLength);
            
            var quadTree = new QuadTree(rootNode, MIN_CHUNK_SIZE, nodeMultiplier);
            quadTree.SetViewer(_terrainViewer);

            return quadTree;
        }
        
        #endregion
        
        #region Getters & Setters
        public QTViewer GetViewer() { return _terrainViewer; }
        public QuadTree GetQuadTree() { return _quadTree; }
        public void SetCamera(Camera cam) { viewerCamera = cam; }
        public ChunkManager GetChunkManager() {return _chunkManager;}
        public LODManager GetLODManager() {return _lodManager;}
        public void SetRootNodeLengthMultiplier(int multiplier) { rootNodeLengthMultiplier = multiplier; }
        public int GetRootNodeLengthMultiplier() { return rootNodeLengthMultiplier;}
        #endregion
    }
}