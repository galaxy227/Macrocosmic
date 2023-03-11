using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TextureGenerator : MonoBehaviour
{
    public enum AlgorithmType
    {
        Perlin,
        Random,
        PerlinRandom,
    }

    public enum TextureType
    {
        Nebula, // Perlin
        Star, // Random
        NebulaStar, // PerlinRandom
    }

    [Header("General")]
    public AlgorithmType algorithmType;
    public TextureType textureType;
    public int size;
    public int seed;
    public bool usePreset;
    public bool randomSeed;
    public bool isFalloff;
    public bool isWhite;
    public bool isTransparent;

    [Header("Perlin")]
    public float scale;
    public int octaves;
    [Range(0f, 1f)] public float persistence;
    [Range(1f, 10f)] public float lacunarity;
    public Vector2 offset;

    [Header("Random")]
    public float power;

    // Texture
    public Texture2D GenerateTexture(TextureType typeOfTexture)
    {
        if (usePreset)
        {
            SetPresetValues(typeOfTexture);
        }

        if (randomSeed)
        {
            System.Random rand = new System.Random();
            seed = rand.Next(0, 9999999);
        }

        float[,] noiseMap = GenerateNoise();
        Color[] colorMap = GetColorMap(noiseMap);

        return TextureHelper.TextureFromColorMap(colorMap, size, false);
    }
    private float[,] GenerateNoise()
    {
        float[,] noiseMap = null;

        if (algorithmType == AlgorithmType.Perlin)
        {
            noiseMap = Noise.GeneratePerlinNoiseMap(size, seed, scale, octaves, persistence, lacunarity, offset);
        }
        else if (algorithmType == AlgorithmType.Random)
        {
            noiseMap = Noise.GenerateRandomNoiseMap(size, seed, power);
        }
        else if (algorithmType == AlgorithmType.PerlinRandom)
        {
            noiseMap = GeneratePerlinRandomNoiseMap();
        }

        if (isFalloff)
        {
            float[,] falloffMap = Noise.GenerateCircleFalloffMap(size);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
            }
        }

        return noiseMap;
    }
    private float[,] GeneratePerlinRandomNoiseMap()
    {
        float[,] noiseMap = new float[size, size];

        float[,] perlin = Noise.GeneratePerlinNoiseMap(size, seed, scale, octaves, persistence, lacunarity, offset);
        float[,] random = Noise.GenerateRandomNoiseMap(size, seed, power);

        // PERLIN, make brightness of textures more consistent
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float multiplier = Mathf.Lerp(0, 0.5f, perlin[x, y]);
                float subtractAmount = perlin[x, y] * multiplier;

                perlin[x, y] = perlin[x, y] - subtractAmount;
            }
        }

        // RANDOM, Calculate Random falloff once before again calculating the entire texture's falloff to reduce Noise near edges of texture
        if (isFalloff)
        {
            float[,] falloffMap = Noise.GenerateCircleFalloffMap(size);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    random[x, y] = Mathf.Clamp01(random[x, y] - falloffMap[x, y]);
                }
            }
        }

        // Add perlin and random to noiseMap
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                noiseMap[x, y] = perlin[x, y] + random[x, y];
            }
        }

        // Get max value from noiseMap
        float maxValue = Noise.GetMaxValueFromNoiseMap(noiseMap);

        // InverseLerp noiseMap values between 0 and maxValue
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Mathf.InverseLerp(0, maxValue, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    private Color[] GetColorMap(float[,] noiseMap)
    {
        Color[] colorMap = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Color color = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);

                if (isWhite)
                {
                    color = Color.white;
                }

                if (isTransparent)
                {
                    float maskValue = 0.4f;
                    float alpha = Mathf.InverseLerp(maskValue, 1f, noiseMap[x, y]);

                    color = new Color(color.r, color.g, color.b, alpha);
                }

                colorMap[y * size + x] = color;
            }
        }

        return colorMap;
    }

    // Utility
    public void SetPresetValues(TextureType typeOfTexture)
    {
        if (typeOfTexture == TextureType.Nebula)
        {
            algorithmType = AlgorithmType.Perlin;

            size = 256;
            scale = 60;
            octaves = 4;
            persistence = 0.6f; // 0.5f
            lacunarity = 1.8f; // 2f
            offset = new Vector2(0, 0);

            randomSeed = false;
            isFalloff = true;
            isWhite = false;
            isTransparent = true;
            usePreset = false;
        }
        else if (typeOfTexture == TextureType.Star)
        {
            algorithmType = AlgorithmType.Random;

            size = 256;
            power = 100;

            randomSeed = false;
            isFalloff = true;
            isWhite = false;
            isTransparent = true;
            usePreset = false;
        }
        else if (typeOfTexture == TextureType.NebulaStar)
        {
            algorithmType = AlgorithmType.PerlinRandom;

            size = 768;
            // Perlin
            scale = 112.5f;
            octaves = 4;
            persistence = 0.6f; // 0.5f
            lacunarity = 1.8f; // 2f
            offset = new Vector2(0, 0);
            // Random
            power = 200;

            randomSeed = false;
            isFalloff = true;
            isWhite = false;
            isTransparent = true;
            usePreset = false;
        }
    }
}

// HELPER CLASS
public static class TextureHelper
{
    public static Texture2D TextureFromColorMap(Color[] colorMap, int size, bool blockyPixels)
    {
        // Texture
        Texture2D texture = new Texture2D(size, size);

        if (blockyPixels)
        {
            texture.filterMode = FilterMode.Point;
        }
        
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }

    public static TextureAtlas CreateTextureAtlas(List<Texture2D> textureList, bool blockyPixels)
    {
        // COUNT of textureList MUST BE "PERFECT SQUARE" (1, 4, 9, etc) to avoid empty textures in atlas
        // resolution of ALL subTextures must be the same, and resolution must be square

        // Tiling
        int widthOfAtlas = 0;

        do
        {
            widthOfAtlas++;

        } while (textureList.Count > Mathf.Pow(widthOfAtlas, 2));

        //int tiling = 1 / widthOfAtlas;
        //mat.mainTextureScale = new Vector2(tiling, tiling);

        int subTextureWidth = textureList[0].width;
        int widthOfAtlasInPixels = widthOfAtlas * subTextureWidth;
        Color[] colorMap = new Color[widthOfAtlasInPixels * widthOfAtlasInPixels];

        int textureListIndex = -1;

        // Texture2D Atlas
        for (int yAtlas = 0; yAtlas < widthOfAtlas; yAtlas++)
        {
            for (int xAtlas = 0; xAtlas < widthOfAtlas; xAtlas++)
            {
                textureListIndex++;

                if (textureListIndex < textureList.Count) // if index exists
                {
                    int yStart = subTextureWidth * yAtlas;
                    int yEnd = subTextureWidth + yStart;

                    int xStart = subTextureWidth * xAtlas;
                    int xEnd = subTextureWidth + xStart;

                    for (int y = yStart; y < yEnd; y++)
                    {
                        for (int x = xStart; x < xEnd; x++)
                        {
                            int ySubTexture = y - yStart;
                            int xSubTexture = x - xStart;

                            Color color = textureList[textureListIndex].GetPixel(xSubTexture, ySubTexture);

                            colorMap[y * widthOfAtlasInPixels + x] = color;
                        }
                    }
                }
            }
        }

        Texture2D texture = TextureFromColorMap(colorMap, widthOfAtlasInPixels, blockyPixels);
        TextureAtlas textureAtlas = new TextureAtlas(texture, widthOfAtlas);

        return textureAtlas;
    }
}

// ATLAS STRUCT
public struct TextureAtlas
{
    public Texture2D textureAtlas;
    public int widthOfAtlas;

    public TextureAtlas(Texture2D atlas, int widthOfAtlas)
    {
        this.textureAtlas = atlas;
        this.widthOfAtlas = widthOfAtlas;
    }
}
