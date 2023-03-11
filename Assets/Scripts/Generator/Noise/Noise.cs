using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Noise
{
    // Noise
    public static float[,] GeneratePerlinNoiseMap(int size, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[size, size];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        float halfSize = size / 2f;

        // SET INTEGER VALUES FOR EACH COORDINATE
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfSize) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfSize) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }

                if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
    public static float[,] GenerateRandomNoiseMap(int size, int seed, float power)
    {
        float[,] noiseMap = new float[size, size];
        System.Random rand = new System.Random(seed);

        int min = 0;
        int max = 999999999;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int valueSeed = rand.Next(min, max);
                noiseMap[x, y] = Mathf.Pow(valueSeed / (float)max, power);
            }
        }

        return noiseMap;
    }

    // Falloff
    public static float[,] GenerateCircleFalloffMap(int size)
    {
        float[,] map = new float[size, size];

        int halfSize = size / 2;
        Vector2 center = new Vector2(halfSize, halfSize);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distanceFromCenter = Vector2.Distance(center, new Vector2(x, y));

                map[x, y] = Mathf.Clamp01(Mathf.InverseLerp(0, halfSize, distanceFromCenter));
            }
        }

        return map;
    }
    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = EvaluateFalloff(value);
            }
        }

        return map;
    }
    static float EvaluateFalloff(float value)
    {
        float a = 3f; // 3f Lague default
        float b = 2.2f; // 2.2f Lague default

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }

    // Utility
    public static float GetMaxValueFromNoiseMap(float[,] noiseMap)
    {
        int size = noiseMap.GetLength(0);
        float[] array = new float[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                array[y * size + x] = noiseMap[x, y];
            }
        }

        return array.Max();
    }
}
