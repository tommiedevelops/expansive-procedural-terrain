using UnityEngine;
using Unity.Mathematics;
public class QuadNode {

    // CONSTRUCTOR
    public QuadNode(Vector2 botLeftPoint, float sideLength) {
        this.botLeftPoint = botLeftPoint;
        this.sideLength = sideLength;
        levelSet = false;

        Vector3 boundsCenter = new(botLeftPoint.x + sideLength / 2f, 0f, botLeftPoint.y + sideLength / 2f);
        Vector3 boundsDimensions = new(sideLength, 0f, sideLength);

        this.bounds = new Bounds(boundsCenter, boundsDimensions);
    }

    #region Fields
    Vector2 botLeftPoint;
    float sideLength; // in metres
    public QuadNode[] children = null;
    Bounds bounds;
    int level;
    bool levelSet;
    #endregion

    #region Helper Functions
    // HELPERS
    public uint ComputeHash() {
        float3 data = new float3(botLeftPoint.x, botLeftPoint.y, sideLength);
        return math.hash(data);
    }
    public bool HasChildren() {
        bool hasChildren = false;
        foreach(QuadNode child in children) {
            if (child != null) hasChildren = true;
        }
        return hasChildren;
    }
    public bool IsLevelSet() { return levelSet; }
    public void PrintNode() { Debug.Log($"BotLeftPoint:{GetBotLeftPoint()} SideLength:{GetSideLength()}"); }
    #endregion

    #region Getters & Setters
    public void SetLevel(int level) { this.level = level; levelSet = true; } // JUST BROKE THIS BTW
    public int GetLevel() { return this.level; }
    public Bounds GetBounds() { return bounds; }
    public float GetSideLength() { return sideLength; }
    public Vector2 GetBotLeftPoint() { return botLeftPoint; }
    public QuadNode[] GetChildren() {
        return children;
    }
    internal void SetBotLeftChild(QuadNode botLeft) {
        children[0] = botLeft;
    }
    internal void SetTopLeftChild(QuadNode topLeft) {
        children[1] = topLeft;
    }
    internal void SetTopRightChild(QuadNode topRight) {
        children[2] = topRight;
    }
    internal void SetBotRightChild(QuadNode botRight) {
        children[3] = botRight;
    }
    #endregion
}

