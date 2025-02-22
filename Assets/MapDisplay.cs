using UnityEngine;

public class MapDisplay : MonoBehaviour {
    [Header("References")]
    [SerializeField] Renderer textureRenderer;

    public void DrawNoiseMap(float[,] noiseMap) {
        int width = noiseMap.GetLength(0); 
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D (width, height);
        Color[] colorMap = new Color[width * height]; //1D array
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                // convert 2D array into 1D representation (think it through)
                // what is this line of code doing? 
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }

        texture.SetPixels(colorMap); // faster than SetPixel()
        texture.Apply();

        //textureRenderer.material is only instantiated at runtime
        textureRenderer.sharedMaterial.mainTexture = texture; // So we can see it w/o having to run game
        textureRenderer.transform.localScale = new Vector3(width, 1f, height);
    }

}
