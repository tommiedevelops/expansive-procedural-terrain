
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
        #region Fields

        private const int MIN_CHUNK_SIZE = 240;
        
        [SerializeField] private int rootNodeLengthMultiplier = 10;
        [SerializeField] private Camera viewerCamera;
        [SerializeField] private List<NoiseLayerSO> noiseLayers;
        [SerializeField] private float globalHeightMultiplier = 1f;
        [SerializeField] private float nodeMultiplier = 3f;
		[SerializeField] private NoiseGenerator noiseGenerator;
        
        private QTViewer _viewer;
        private QuadTree _quadTree;
        private ChunkManager  _chunkManager;
        private LODManager _lodManager;
        private float _renderDistance;
        
        #endregion
        
        #region Unity Functions
        private void Awake()
        {
            _renderDistance = viewerCamera.farClipPlane;
            // keeping QTViewer here in case I want to modify algorithm again
            _viewer = new QTViewer(viewerCamera.transform, viewerCamera.fieldOfView, _renderDistance);
            _chunkManager = new ChunkManager();
            _quadTree = GenerateQuadTree();
            _lodManager = new LODManager(MIN_CHUNK_SIZE);
            _lodManager.SetNumLODLevels(4);
        }
        
        private void Start()
        {
            // add noise from user input
            foreach(var layer in noiseLayers) noiseGenerator.AddLayer(layer);
            ChunkManager.SetNoiseGenerator(noiseGenerator);
            ChunkManager.SetGlobalHeightMultiplier(globalHeightMultiplier);
            noiseGenerator.SetWorldHeightBounds(globalHeightMultiplier, -globalHeightMultiplier);
            
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
        
        #endregion
        
        #region Helpers
        public static List<ChunkManager.ChunkData> IdentifyLeafNodesNotActive(List<ChunkManager.ChunkData> newActiveChunks, Dictionary<ChunkManager.ChunkData, GameObject>.KeyCollection currentActiveChunks)
        {
            var chunksToAdd = newActiveChunks
                .Where(chunk => !currentActiveChunks.Contains<ChunkManager.ChunkData>(chunk))
                .ToList();

            return chunksToAdd;
        }
        private List<ChunkManager.ChunkData> ConvertQuadNodesToChunkData(List<QuadNode> quadNodes)
        {
            var chunks = quadNodes
                .Select(node => new ChunkManager.ChunkData()
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
            if (_viewer == null) throw new Exception("Viewer not initialized");
            
            float rootNodeSideLength = rootNodeLengthMultiplier * MIN_CHUNK_SIZE;
            
            // We want the root node to be centred on (0,0)
            var rootNodeBottomLeftPoint = new Vector2(-rootNodeSideLength /2f, -rootNodeSideLength /2f);
            var rootNode = new QuadNode(null, rootNodeBottomLeftPoint, rootNodeSideLength);
            
            var quadTree = new QuadTree(rootNode, MIN_CHUNK_SIZE, nodeMultiplier);
            quadTree.SetViewer(_viewer);

            return quadTree;
        }
        
        #endregion
        
        #region Getters & Setters
        public QTViewer GetViewer() { return _viewer; }
        public QuadTree GetQuadTree() { return _quadTree; }
        public void SetCamera(Camera cam) { viewerCamera = cam; }
        public ChunkManager GetChunkManager() {return _chunkManager;}
        public LODManager GetLODManager() {return _lodManager;}
        public void SetRootNodeLengthMultiplier(int multiplier) { rootNodeLengthMultiplier = multiplier; }
        public int GetRootNodeLengthMultiplier() { return rootNodeLengthMultiplier;}
        #endregion
    }
}