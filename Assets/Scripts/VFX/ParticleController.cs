using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Handles particles in Galaxy view

public class ParticleController : MonoBehaviour
{
    public enum ParticleType
    {
        SolarSystem, // Nebula
        Center, // Ambient
        Core,
        Mid,
        Outer,
        Light
    }

    public TextureGenerator textureGenerator;

    // Nebula
    public ParticleSystem nebulaParticle;
    private Material nebulaMaterial;
    private Texture2D nebulaTexture;
    private float offsetThreshold = 0.025f;
    private float increaseAmount;
    private bool isOffsetIncreasing;

    // Ambient
    public ParticleSystem ambientParticle;

    void Start()
    {
        OnStart();
    }

    private void FixedUpdate()
    {
        UpdateNebulaMaterialOffset();
    }

    private void OnChangeView()
    {
        if (ViewController.ViewType == ViewType.Galaxy)
        {
            EmitNebulaParticles();
            EmitAmbientParticles();
        }
        else
        {
            ClearAllParticles();
        }
    }

    // Nebula
    private void EmitNebulaParticles()
    {
        ClearParticleSystem(nebulaParticle);

        // DECLARE
        var main = nebulaParticle.main;
        var emitParams = new ParticleSystem.EmitParams();
        System.Random rand = new System.Random(GalaxyGenerator.Instance.Seed);

        // Seed
        nebulaParticle.randomSeed = (uint)GalaxyGenerator.Instance.Seed;

        // MaxParticles
        int particleAmountPerSolarSystem = 1;
        main.maxParticles = GalaxyGenerator.Instance.Systems * particleAmountPerSolarSystem;

        // Lifetime
        main.startLifetime = Mathf.Infinity;

        // StartSize
        main.startSize3D = true;
        main.startSizeZ = 0.1f;

        // Material & Texture
        nebulaMaterial.mainTexture = nebulaTexture;

        // SOLARSYSTEM particles
        for (int i = 0; i < GalaxyGenerator.SolarSystemList.Count; i++)
        {
            int particleCount = 0;

            // Color of particle
            emitParams.startColor = ColorHelper.GetSolarSystemColor(rand, GalaxyGenerator.SolarSystemList[i].transform.position);

            // Size
            float sizeBase = 50 - (5 * GalaxyGenerator.Instance.NumArms) + (50 * ((int)GalaxyGenerator.Instance.sizeType + 1));
            float multiplier = 0.25f;
            float sizeMax = sizeBase + (sizeBase * multiplier);
            float sizeMin = sizeBase - (sizeBase * multiplier);
            float sizePercentageSeed = rand.Next(0, 100) / 100f; // Random

            float size = Mathf.Lerp(sizeMin, sizeMax, sizePercentageSeed);
            main.startSizeX = size;
            main.startSizeY = size;

            // Emit Particle
            float distanceFromCenter = Vector3.Distance(GalaxyGenerator.SolarSystemList[i].transform.position, Vector3.zero);
            int emitSeed = rand.Next(0, 10);
            int chanceValue = 0;

            //if (distanceFromCenter < GalaxyGenerator.Instance.Radius * 0.33f) // if solarSystem is within inner-percentage of radius, emit less particles to save performance (overdraw)
            //{
            //    if (GalaxyGenerator.Instance.sizeType == GalaxyGenerator.SizeType.Tiny) // if Tiny, 60% chance to emit
            //    {
            //        chanceValue = 5;
            //    }
            //    else if (GalaxyGenerator.Instance.sizeType == GalaxyGenerator.SizeType.Small) // 50% chance to emit
            //    {
            //        chanceValue = 4;
            //    }
            //    else if (GalaxyGenerator.Instance.sizeType == GalaxyGenerator.SizeType.Medium) // 40% chance to emit
            //    {
            //        chanceValue = 3;
            //    }
            //    else if (GalaxyGenerator.Instance.sizeType == GalaxyGenerator.SizeType.Large) // 30% chance to emit
            //    {
            //        chanceValue = 2;
            //    }
            //    else if (GalaxyGenerator.Instance.sizeType == GalaxyGenerator.SizeType.Huge) // 20% chance to emit
            //    {
            //        chanceValue = 1;
            //    }
            //}
            //else // if solarSystem in outer radius of galaxy
            //{
            //    if (GalaxyGenerator.Instance.sizeType == GalaxyGenerator.SizeType.Tiny) // if Tiny, 100% chance to emit
            //    {
            //        chanceValue = 9;
            //    }
            //    else if (GalaxyGenerator.Instance.sizeType == GalaxyGenerator.SizeType.Small) // 90% chance to emit
            //    {
            //        chanceValue = 8;
            //    }
            //    else if (GalaxyGenerator.Instance.sizeType == GalaxyGenerator.SizeType.Medium) // 80% chance to emit
            //    {
            //        chanceValue = 7;
            //    }
            //    else if (GalaxyGenerator.Instance.sizeType == GalaxyGenerator.SizeType.Large) // 70% chance to emit
            //    {
            //        chanceValue = 6;
            //    }
            //    else if (GalaxyGenerator.Instance.sizeType == GalaxyGenerator.SizeType.Huge) // 60% chance to emit
            //    {
            //        chanceValue = 5;
            //    }
            //}

            if (GalaxyGenerator.Instance.sizeType == GalaxyGenerator.SizeType.Tiny) // if Tiny, 100% chance to emit
            {
                chanceValue = 9;
            }
            else if (GalaxyGenerator.Instance.sizeType == GalaxyGenerator.SizeType.Small) // 90% chance to emit
            {
                chanceValue = 8;
            }
            else if (GalaxyGenerator.Instance.sizeType == GalaxyGenerator.SizeType.Medium) // 80% chance to emit
            {
                chanceValue = 7;
            }
            else if (GalaxyGenerator.Instance.sizeType == GalaxyGenerator.SizeType.Large) // 70% chance to emit
            {
                chanceValue = 6;
            }
            else if (GalaxyGenerator.Instance.sizeType == GalaxyGenerator.SizeType.Huge) // 60% chance to emit
            {
                chanceValue = 5;
            }

            if (emitSeed <= chanceValue)
            {
                do // FOR each individual particle at system
                {
                    // Rotation
                    int degrees = 30;
                    emitParams.rotation3D = new Vector3(0, 0, i * (degrees * (particleCount + 1)));

                    // Position of particle
                    float distancePercentage = Mathf.Abs((distanceFromCenter / GalaxyGenerator.Instance.Radius) - 1f);

                    float minimumHeight = -0.5f;
                    float maximumHeight = 5f * distancePercentage;

                    int heightSeed = rand.Next(0, 100);
                    float heightPercentage = heightSeed / 100f;

                    float height = Mathf.Lerp(minimumHeight, maximumHeight, heightPercentage);
                    emitParams.position = new Vector3(GalaxyGenerator.SolarSystemList[i].transform.position.x, GalaxyGenerator.SolarSystemList[i].transform.position.y, -height);

                    // Emit Particle
                    nebulaParticle.Emit(emitParams, 1);

                    particleCount++;

                } while (particleCount < particleAmountPerSolarSystem);
            }
        }

        nebulaParticle.Play();
    }
    private void GenerateNebulaTexture()
    {
        // Reset
        textureGenerator.SetPresetValues(TextureGenerator.TextureType.NebulaStar);
        textureGenerator.seed = GalaxyGenerator.Instance.Seed;

        nebulaTexture = textureGenerator.GenerateTexture(TextureGenerator.TextureType.NebulaStar);

        // Material
        nebulaMaterial.mainTextureOffset = new Vector2(0, 0);
    }
    private void UpdateNebulaMaterialOffset()
    {
        SetIncreaseAmount();

        if (increaseAmount > 0)
        {
            if (nebulaMaterial.mainTextureOffset.x >= offsetThreshold)
            {
                isOffsetIncreasing = false;

            }
            else if (nebulaMaterial.mainTextureOffset.x <= -offsetThreshold)
            {
                isOffsetIncreasing = true;
            }

            if (isOffsetIncreasing)
            {
                nebulaMaterial.mainTextureOffset += new Vector2(increaseAmount, 0);
            }
            else
            {
                nebulaMaterial.mainTextureOffset -= new Vector2(increaseAmount, 0);
            }
        }
    }
    private void SetIncreaseAmount()
    {
        float increaseConstant = 0.000001f;

        float barrier = offsetThreshold * 0.1f;
        float minBarrier = -offsetThreshold + barrier;
        float maxBarrier = offsetThreshold - barrier;

        float multiplier = 1;

        if (nebulaMaterial.mainTextureOffset.x <= minBarrier)
        {
            multiplier = Mathf.InverseLerp(-offsetThreshold, minBarrier, nebulaMaterial.mainTextureOffset.x);
        }
        else if (nebulaMaterial.mainTextureOffset.x >= maxBarrier)
        {
            multiplier = Mathf.InverseLerp(offsetThreshold, maxBarrier, nebulaMaterial.mainTextureOffset.x);
        }

        multiplier = Mathf.Clamp(multiplier, 0.1f, 1f);

        increaseAmount = increaseConstant * ((int)TimeController.Instance.SpeedType * 2) * multiplier;
    }

    // Ambient
    private void EmitAmbientParticles()
    {
        if (GalaxyGenerator.Instance.shapeType != GalaxyGenerator.ShapeType.Ring)
        {
            ClearParticleSystem(ambientParticle);

            // DECLARE
            var main = ambientParticle.main;
            var emitParams = new ParticleSystem.EmitParams();
            System.Random rand = new System.Random(GalaxyGenerator.Instance.Seed);

            // Seed
            ambientParticle.randomSeed = (uint)GalaxyGenerator.Instance.Seed;

            // MaxParticles
            int lightParticles = 1;
            int centerParticles = 1;
            int coreParticles = 1;
            int midParticles = 1;
            int outerParticles = 1;
            main.maxParticles = lightParticles + centerParticles + coreParticles + midParticles + outerParticles;

            // Lifetime
            main.startLifetime = Mathf.Infinity;

            // StartSize
            main.startSize3D = true;
            main.startSizeZ = 0.1f;

            // LIGHT particles
            for (int i = 0; i < lightParticles; i++)
            {
                // Size
                float sizeValue = 5f * ((int)GalaxyGenerator.Instance.sizeType + 1);
                main.startSizeX = sizeValue;
                main.startSizeY = sizeValue;

                // Position of particle
                emitParams.position = new Vector3(0, 0, ambientParticle.transform.position.z);

                // Color of particle
                emitParams.startColor = ColorHelper.GetAmbientColor(ParticleType.Light);

                // Rotation
                int degrees = 15;
                emitParams.rotation3D = new Vector3(0, 0, i * degrees);

                // Emit Particle
                ambientParticle.Emit(emitParams, 1);
            }

            // CENTER particles
            for (int i = 0; i < centerParticles; i++)
            {
                // Size
                float sizeValue = 25 * ((int)GalaxyGenerator.Instance.sizeType + 1);
                main.startSizeX = sizeValue;
                main.startSizeY = sizeValue;

                // Position of particle
                emitParams.position = new Vector3(0, 0, ambientParticle.transform.position.z);

                // Color of particle
                emitParams.startColor = ColorHelper.GetAmbientColor(ParticleType.Center);

                // Rotation
                int degrees = 15;
                emitParams.rotation3D = new Vector3(0, 0, i * degrees);

                // Emit Particle
                ambientParticle.Emit(emitParams, 1);
            }

            // CORE particles
            for (int i = 0; i < coreParticles; i++)
            {
                // Size
                float sizeValue = 50 * ((int)GalaxyGenerator.Instance.sizeType + 1);
                main.startSizeX = sizeValue;
                main.startSizeY = sizeValue;

                // Position of particle
                emitParams.position = new Vector3(0, 0, ambientParticle.transform.position.z);

                // Color of particle
                emitParams.startColor = ColorHelper.GetAmbientColor(ParticleType.Core);

                // Rotation
                int degrees = 15;
                emitParams.rotation3D = new Vector3(0, 0, i * degrees);

                // Emit Particle
                ambientParticle.Emit(emitParams, 1);
            }

            if (GalaxyGenerator.Instance.shapeType != GalaxyGenerator.ShapeType.Spiral || GalaxyGenerator.Instance.sizeType != GalaxyGenerator.SizeType.Tiny)
            {
                // MID particles
                for (int i = 0; i < midParticles; i++)
                {
                    // Size
                    float sizeValue = 100 * ((int)GalaxyGenerator.Instance.sizeType + 1);
                    main.startSizeX = sizeValue;
                    main.startSizeY = sizeValue;

                    // Position of particle
                    emitParams.position = new Vector3(0, 0, ambientParticle.transform.position.z);

                    // Color of particle
                    emitParams.startColor = ColorHelper.GetAmbientColor(ParticleType.Mid);

                    // Rotation
                    int degrees = 15;
                    emitParams.rotation3D = new Vector3(0, 0, i * degrees);

                    // Emit Particle
                    ambientParticle.Emit(emitParams, 1);
                }

                // OUTER particles
                for (int i = 0; i < outerParticles; i++)
                {
                    // Size
                    float sizeValue = 150 * ((int)GalaxyGenerator.Instance.sizeType + 1);
                    main.startSizeX = sizeValue;
                    main.startSizeY = sizeValue;

                    // Position of particle
                    emitParams.position = new Vector3(0, 0, ambientParticle.transform.position.z);

                    // Color of particle
                    emitParams.startColor = ColorHelper.GetAmbientColor(ParticleType.Outer);

                    // Rotation
                    int degrees = 15;
                    emitParams.rotation3D = new Vector3(0, 0, i * degrees);

                    // Emit Particle
                    ambientParticle.Emit(emitParams, 1);
                }
            }

            ambientParticle.Play();
        }
    }

    // Utility
    private void OnStart()
    {
        // Nebula
        nebulaMaterial = nebulaParticle.GetComponent<Renderer>().sharedMaterial;

        GalaxyGenerator.BeforeGenerate.AddListener(ClearAllParticles);
        GalaxyGenerator.PreAfterGenerate.AddListener(GenerateNebulaTexture);

        ViewController.ChangeView.AddListener(OnChangeView);
    }
    private void ClearParticleSystem(ParticleSystem particleSystem)
    {
        if (particleSystem != null)
        {
            particleSystem.Stop();
            particleSystem.Clear();
        }
    }
    private void ClearAllParticles()
    {
        ClearParticleSystem(nebulaParticle);
        ClearParticleSystem(ambientParticle);
    }

    private static class ColorHelper
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
}