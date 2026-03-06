using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testFalloff : MonoBehaviour
{
    public int width;
    public int height;

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.sharedMaterial.mainTexture = GenerateTexture();
    }

    Texture2D GenerateTexture ()
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xFalloff = Mathf.Abs((float)x / (width) * 2 - 1);
                float yFalloff = Mathf.Abs((float)y / (width) * 2 - 1);

                float value = Mathf.Max(Mathf.Abs(xFalloff), Mathf.Max(yFalloff));

                value = Evaluate(value);

                Color colour = new Color(value, value, value);
                texture.SetPixel(x, y, colour);
            }
        }

        texture.Apply();
        return texture;
    }

    static float Evaluate(float value)
    {
        float a = 3;
        float b = 3f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
