using System;
using UnityEngine;

public class MapDisplay : MonoBehaviour {
    [Header("References")]
    [SerializeField] Renderer textureRenderer;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;
    public void DrawTexture(Texture2D texture) {
        textureRenderer.sharedMaterial.mainTexture = texture; // So we can see it w/o having to run game
        textureRenderer.transform.localScale = new Vector3(texture.width, 1f, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture) {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
 