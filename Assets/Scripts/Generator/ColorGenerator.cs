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
