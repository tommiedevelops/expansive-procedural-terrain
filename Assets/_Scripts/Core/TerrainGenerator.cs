using UnityEngine;
using System.Collections.Generic;
using ChunkingSystem;
using static QuadTree;
using static PlaneMeshGenerator;
using System;
using System.Linq;
using _Scripts.ChunkingSystem;
using UnityEngine.Serialization;

namespace Core {
    public class TerrainGenerator : MonoBehaviour
    {
        public const int MIN_CHUNK_SIZE = 120;

        [FormerlySerializedAs("worldLengthMultiplier")] [SerializeField] private int rootNodeLengthMultiplier = 1;
        [SerializeField] private Camera viewerCamera;
        
        private QTViewer _viewer;
        private QuadTree _quadTree;
        private ChunkManager  _chunkManager;
        private float _renderDistance;

        private void Awake()
        {
            _renderDistance = viewerCamera.farClipPlane;
            _viewer = new QTViewer(viewerCamera.transform, viewerCamera.fieldOfView, _renderDistance);
            _chunkManager = new ChunkManager();
            _quadTree = GenerateQuadTree();
        }

        private void Update()
        {
            _quadTree.Update();
        }
        private QuadTree GenerateQuadTree()
        { // Factory method to prevent side effects
            // rootNodeLengthMultiplier set in the Editor
            float rootNodeSideLength = rootNodeLengthMultiplier * MIN_CHUNK_SIZE;
            
            // We want the root node to be centred on (0,0)
            var rootNodeBottomLeftPoint = new Vector2(-rootNodeSideLength /2f, -rootNodeSideLength /2f);
            
            var rootNode = new QuadNode(null, rootNodeBottomLeftPoint, rootNodeSideLength);
            var quadTree = new QuadTree(rootNode);
            quadTree.SetViewer(_viewer);

            return quadTree;
        }
        public QTViewer GetViewer() {return _viewer;}
        public QuadTree GetQuadTree() {return _quadTree;}
    }
}