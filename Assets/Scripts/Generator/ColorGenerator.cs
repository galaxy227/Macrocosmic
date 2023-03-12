using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class ColorGenerator
{
    public static Color Primary;
    public static Color Secondary;
    public static Color Mixed; // Mixture of Primary and Secondary
    public static Color Contrast; // Opposite of Primary

    public static void GenerateGalaxyColors()
    {
        System.Random rand = new System.Random(GalaxyGenerator.Instance.Seed);

        float bigValue = 1f;

        float mediumSeed = rand.Next(0, 100) / 100f;
        float mediumValue = Mathf.Lerp(0.625f, 0.8f, mediumSeed);

        float smallSeed = rand.Next(0, 100) / 100f;
        float smallValue = Mathf.Lerp(0.425f, 0.6f, smallSeed);

        // Primary & Secondary
        int colorSeed = rand.Next(0, 4);

        if (GalaxyGenerator.Instance.colorType == GalaxyGenerator.ColorType.Red)
        {
            // Primary
            if (colorSeed == 0) // green-secondary
            {
                Primary = new Color(bigValue, mediumValue, smallValue);
            }
            else if (colorSeed == 1) // blue-secondary
            {
                Primary = new Color(bigValue, smallValue, mediumValue);
            }
            else if (colorSeed == 2) // red flat
            {
                Primary = new Color(bigValue, mediumValue, mediumValue);
            }
            else if (colorSeed == 3) // red saturated
            {
                Primary = new Color(bigValue, smallValue, smallValue);
            }

            // Secondary
            float difference = Primary.r * 0.25f;

            if (colorSeed <= 1) // green-shifted
            {
                Secondary = new Color(Primary.r - difference, Mathf.Clamp01(Primary.g + difference), Primary.b);
            }
            else if (colorSeed <= 3) // blue-shifted
            {
                Secondary = new Color(Primary.r - difference, Primary.g, Mathf.Clamp01(Primary.b + difference));
            }
        }
        else if (GalaxyGenerator.Instance.colorType == GalaxyGenerator.ColorType.Green)
        {
            if (colorSeed == 0) // red-secondary
            {
                Primary = new Color(mediumValue, bigValue, smallValue);
            }
            else if (colorSeed == 1) // blue-secondary
            {
                Primary = new Color(smallValue, bigValue, mediumValue);
            }
            else if (colorSeed == 2) // green flat
            {
                Primary = new Color(mediumValue, bigValue, mediumValue);
            }
            else if (colorSeed == 3) // green saturated
            {
                Primary = new Color(smallValue, bigValue, smallValue);
            }

            // Secondary
            float difference = Primary.g * 0.25f;

            if (colorSeed <= 1) // red-shifted
            {
                Secondary = new Color(Mathf.Clamp01(Primary.r + difference), Primary.g - difference, Primary.b);
            }
            else if (colorSeed <= 3) // blue-shifted
            {
                Secondary = new Color(Primary.r, Primary.g - difference, Mathf.Clamp01(Primary.b + difference));
            }
        }
        else if (GalaxyGenerator.Instance.colorType == GalaxyGenerator.ColorType.Blue)
        {
            if (colorSeed == 0) // red-secondary
            {
                Primary = new Color(mediumValue, smallValue, bigValue);
            }
            else if (colorSeed == 1) // green-secondary
            {
                Primary = new Color(smallValue, mediumValue, bigValue);
            }
            else if (colorSeed == 2) // blue flat
            {
                Primary = new Color(mediumValue, mediumValue, bigValue);
            }
            else if (colorSeed == 3) // blue saturated
            {
                Primary = new Color(smallValue, smallValue, bigValue);
            }

            // Secondary
            float difference = Primary.b * 0.25f;

            if (colorSeed <= 1) // red-shifted
            {
                Secondary = new Color(Mathf.Clamp01(Primary.r + difference), Primary.g, Primary.b - difference);
            }
            else if (colorSeed <= 3) // green-shifted
            {
                Secondary = new Color(Primary.r, Mathf.Clamp01(Primary.g + difference), Primary.b - difference);
            }
        }

        // Mixed
        Mixed = (Primary + Secondary) / 2f;

        // Contrast
        Contrast = Color.white - Primary;
    }
}

public static class ColorHelper
{
    // Color
    public static Color GetSolarSystemColor(System.Random rand, Vector3 position)
    {
        // DECLARE
        Color solarSystemColor = new Color(1, 1, 1, 1);

        // COLOR
        int colorSeed = rand.Next(0, 10);

        if (colorSeed <= 3) // 40%
        {
            solarSystemColor = ColorGenerator.Primary;
        }
        else if (colorSeed <= 6) // 30%
        {
            solarSystemColor = ColorGenerator.Secondary;
        }
        else if (colorSeed <= 8) // 20%
        {
            solarSystemColor = ColorGenerator.Mixed;
        }
        else if (colorSeed <= 9) // 10%
        {
            solarSystemColor = ColorGenerator.Contrast;
        }

        // ALPHA
        float alpha = GetSolarSystemAlpha(position);
        solarSystemColor = SetAlpha(solarSystemColor, alpha);

        return solarSystemColor;
    } // Nebula
    public static Color GetAmbientColor(ParticleType particleType)
    {
        Color color = new Color(1, 1, 1, 1);

        switch (particleType)
        {
            case ParticleType.Light:
                color = ColorGenerator.Contrast;
                break;
            case ParticleType.Center:
                color = ColorGenerator.Contrast;
                break;
            case ParticleType.Core:
                color = ColorGenerator.Contrast;
                break;
            case ParticleType.Mid:
                color = ColorGenerator.Contrast;
                break;
            case ParticleType.Outer:
                color = ColorGenerator.Contrast;
                break;
            default:
                break;
        }

        // ALPHA
        float alpha = GetAmbientAlpha(particleType);
        color = SetAlpha(color, alpha);

        return color;
    } // Ambient

    // Alpha Helper
    private static float GetSolarSystemAlpha(Vector3 position)
    {
        float alpha = 1;

        float multiplier = 3;

        // Calculate distance multiplier
        float distance = Vector3.Distance(Vector3.zero, position);
        float distanceMultiplier = Mathf.InverseLerp(GalaxyGenerator.Instance.Radius * 1.1f, 0, distance);

        // Galaxy Size
        switch (GalaxyGenerator.Instance.sizeType)
        {
            case GalaxyGenerator.SizeType.Tiny:
                alpha = 0.09f * multiplier;
                break;
            case GalaxyGenerator.SizeType.Small:
                alpha = 0.08f * multiplier;
                break;
            case GalaxyGenerator.SizeType.Medium:
                alpha = 0.07f * multiplier;
                break;
            case GalaxyGenerator.SizeType.Large:
                alpha = 0.06f * multiplier;
                break;
            case GalaxyGenerator.SizeType.Huge:
                alpha = 0.05f * multiplier;
                break;
            default:
                alpha = 0 / 255f;
                break;
        }

        // Galaxy Shape
        if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Ring)
        {
            alpha *= 1.5f;
        }

        return Mathf.Clamp01(alpha * distanceMultiplier);
    }
    private static float GetAmbientAlpha(ParticleType particleType)
    {
        float alpha = 1;

        if (particleType == ParticleType.Light)
        {
            alpha = 0.10f;
        }
        else if (particleType == ParticleType.Center)
        {
            alpha = 0.08f;
        }
        else if (particleType == ParticleType.Core)
        {
            alpha = 0.06f;
        }
        else if (particleType == ParticleType.Mid)
        {
            alpha = 0.04f;
        }
        else if (particleType == ParticleType.Outer)
        {
            alpha = 0.02f;
        }

        if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Ellipitical)
        {
            alpha = alpha * 0.75f;
        }

        return alpha;
    }
    private static Color SetAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}
