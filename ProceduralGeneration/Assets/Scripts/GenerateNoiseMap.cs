using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateNoiseMap : MonoBehaviour
{
    public Terrain terrain;
    public AnimationCurve heightCurve;

    public float scale;

    [Header("Terrain Parameters")]
    public int width;
    public int length;
    public int height;

    [Header("Noise Layering")]
    public int octaves;
    public float frequencyMultiplyer;
    public float amplitudeMultiplyer;
    public float xOffset;
    public float zOffset;

    public bool falloffBool;

    public TerrainData EditTerrain(TerrainData terrainData)
    {
        terrainData.size = new Vector3(width, height, length);
        terrainData.SetHeights(0, 0, GenerateHeights());

        return terrainData;
    }

    float[,] GenerateHeights ()
    {
        float[,] heights = new float[width + 1, length + 1];
        float[,] falloffMap = new float[width + 1, length + 1];

        for (int x = 0; x < width + 1; x++)
        {
            for (int z = 0; z < length + 1; z++)
            {
                float frequency = 1;
                float amplitude = 1;
                float noiseHeights;
                for (int i = 0; i < octaves; i++)
                {
                    float xSample = (float)x / (width + 1) / scale * frequency + xOffset; 
                    float zSample = (float)z / (width + 1) / scale * frequency + zOffset;

                    noiseHeights = heightCurve.Evaluate(Mathf.PerlinNoise(xSample, zSample));
                    heights[x, z] += noiseHeights * amplitude;

                    frequency *= frequencyMultiplyer;
                    amplitude *= amplitudeMultiplyer;
                }

                //FalloffMap
                float xFalloff = Mathf.Abs((float)x / (width + 1) * 2 - 1);
                float zFalloff = Mathf.Abs((float)z / (width + 1) * 2 - 1);

                float value = Mathf.Max(Mathf.Abs(xFalloff), Mathf.Max(zFalloff));
                falloffMap[x, z] = Evaluate(value);

                if (falloffBool)
                {
                    heights[x, z] = Mathf.Clamp01(heights[x, z] - falloffMap[x, z]);
                }
            }
        }

        return heights;
    }

    static float Evaluate (float value)
    {
        float a = 3;
        float b = 3;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
