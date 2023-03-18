using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ParticleType
{
    Light,
    Center,
    Core,
    Mid,
    Outer
}

public class GalaxyVFXController : MonoBehaviour
{
    [Header("Haze")]
    public ParticleSystem hazeParticleSystem;
    private Material hazeMaterial;
    private Texture2D hazeTexture;
    private List<HazeParticleData> hazeParticleDataList = new List<HazeParticleData>();
    [Header("Ambient")]
    public ParticleSystem ambientParticleSystem;
    [Header("CoreLight")]
    public GameObject coreLight;
    private Material coreLightMaterial;
    [Header("Settings")]
    // Alpha
    public bool IsUpdateAlpha;
    private const float alphaMin = 0.1f;
    private const float zoomThreshold = 0.5f;
    private const float alphaThreshold = 0.75f;
    private float lastZoomPercentage;
    // Neighbors
    //private const int neighborCount = 1; // Amount of closest neighbors considered to evaluate whether particle should emit
    //private const int neighborThreshold = 1; // If closest neighbors emits >= neighborThreshold, particle should not emit

    public int neighborCount;
    public int neighborThreshold;

    private void Start()
    {
        OnStart();
    }
    private void Update()
    {
        if (IsUpdateAlpha)
        {
            if (GameController.Instance.GameState == GameState.Play && ViewController.ViewType == ViewType.Galaxy)
            {
                if (PlayerCamera.Instance.ZoomPercentage != lastZoomPercentage)
                {
                    UpdateVFX();
                }
            }

            lastZoomPercentage = PlayerCamera.Instance.ZoomPercentage;
        }
    }

    private void InitializeVFX()
    {
        InitializeHazeParticles();
        InitializeAmbientParticles();
        InitializeCoreLight();
    }
    private void UpdateVFX()
    {
        UpdateHazeParticles();
        UpdateAmbientParticles();
        UpdateCoreLightAlpha();
    }
    private void DisableVFX()
    {
        Tools.ClearParticleSystem(hazeParticleSystem);
        Tools.ClearParticleSystem(ambientParticleSystem);
        coreLight.gameObject.SetActive(false);
    }

    // Haze
    private void InitializeHazeParticles()
    {
        Tools.ClearParticleSystem(hazeParticleSystem);
        hazeParticleDataList.Clear();

        // Declare variables
        var main = hazeParticleSystem.main;
        System.Random rand = new System.Random(GalaxyGenerator.Instance.Seed);

        // Seed
        hazeParticleSystem.randomSeed = (uint)GalaxyGenerator.Instance.Seed;

        // Max particles
        main.maxParticles = GalaxyGenerator.Instance.Systems;

        // Lifetime
        main.startLifetime = Mathf.Infinity;

        // StartSize
        main.startSize3D = true;
        main.startSizeZ = 0.1f;

        // Material
        hazeMaterial.mainTexture = hazeTexture;

        for (int i = 0; i < GalaxyGenerator.SolarSystemList.Count; i++)
        {
            HazeParticleData particleData = new HazeParticleData();

            // Color of particle
            Color color = ColorHelper.GetSolarSystemColor(rand, GalaxyGenerator.SolarSystemList[i].transform.position);
            particleData.color = color;

            // Size
            int sizeConstant = 50;
            float sizeBase = sizeConstant - (5 * GalaxyGenerator.Instance.NumArms) + (sizeConstant * ((int)GalaxyGenerator.Instance.sizeType + 1));
            float multiplier = 0.25f;
            float sizeMax = sizeBase + (sizeBase * multiplier);
            float sizeMin = sizeBase - (sizeBase * multiplier);
            float sizePercentageSeed = rand.Next(0, 100) / 100f; // Random

            float size = Mathf.Lerp(sizeMin, sizeMax, sizePercentageSeed);
            particleData.size = size;

            // Rotation
            int degrees = 30;
            particleData.rotation = new Vector3(0, 0, i * degrees);

            // Position
            particleData.position = new Vector3(GalaxyGenerator.SolarSystemList[i].transform.position.x, GalaxyGenerator.SolarSystemList[i].transform.position.y, 0);

            // Add particleData to particleDataList
            hazeParticleDataList.Add(particleData);
        }

        // Set neighorList and isEmit
        for (int i = 0; i < hazeParticleDataList.Count; i++)
        {
            HazeParticleData particleData = hazeParticleDataList[i];
            particleData.neighborList = new List<HazeParticleData>();

            // Sort all particles by distance
            List<HazeParticleData> sortByDistance = new List<HazeParticleData>(hazeParticleDataList);

            for (int k = sortByDistance.Count - 1; k >= 0; k--)
            {
                if (sortByDistance[k].position == particleData.position)
                {
                    sortByDistance.RemoveAt(k);
                }
            }

            sortByDistance = sortByDistance.OrderBy(o => Vector3.Distance(o.position, particleData.position)).ToList();

            // Set neighorList for particle
            int count = 0;

            if (neighborCount > sortByDistance.Count)
            {
                count = sortByDistance.Count;
            }
            else
            {
                count = neighborCount;
            }

            for (int j = 0; j < count; j++)
            {
                particleData.neighborList.Add(sortByDistance[j]);
            }

            // Set isEmit for particle
            int neighborEmitCount = 0;
            float distanceThreshold = GalaxyGenerator.Instance.Radius * 0.9f; // force emit if distance between closest neighbor is greater than distanceThreshold

            if (Vector3.Distance(particleData.position, Vector3.zero) > distanceThreshold) 
            {
                particleData.isEmit = true;
            }
            else
            {
                foreach (HazeParticleData neighbor in particleData.neighborList)
                {
                    if (neighbor.isEmit)
                    {
                        neighborEmitCount++;
                    }
                }

                if (neighborEmitCount < neighborThreshold)
                {
                    particleData.isEmit = true;
                }
                else
                {
                    particleData.isEmit = false;
                }
            }

            // Set particle
            hazeParticleDataList[i] = particleData;
        }

        // Remove particles that do not emit
        for (int i = hazeParticleDataList.Count - 1; i >= 0; i--)
        {
            if (!hazeParticleDataList[i].isEmit)
            {
                hazeParticleDataList.RemoveAt(i);
            }
        }

        EmitHazeParticles(); // Particles do not show on first frame of GameState.Play if Emit is not called in Initialize. This only matters when using the console to generate galaxies.
    }
    private void EmitHazeParticles()
    {
        Tools.ClearParticleSystem(hazeParticleSystem);

        // Declare variables
        var main = hazeParticleSystem.main;
        var emitParams = new ParticleSystem.EmitParams();
        System.Random rand = new System.Random(GalaxyGenerator.Instance.Seed);

        for (int i = 0; i < hazeParticleDataList.Count; i++)
        {
            // Emit particle
            if (hazeParticleDataList[i].isEmit)
            {
                // Color of particle
                emitParams.startColor = GetZoomPercentageColor(hazeParticleDataList[i].color);
                
                // Size
                main.startSizeX = hazeParticleDataList[i].size;
                main.startSizeY = hazeParticleDataList[i].size;

                // Rotation
                emitParams.rotation3D = hazeParticleDataList[i].rotation;

                // Position
                emitParams.position = hazeParticleDataList[i].position;

                // Emit Particle
                hazeParticleSystem.Emit(emitParams, 1);
            }
        }
    }
    private void UpdateHazeParticles()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[hazeParticleSystem.main.maxParticles];
        int currentAmount = hazeParticleSystem.GetParticles(particles);

        // Color
        for (int i = 0; i < currentAmount; i++)
        {
            particles[i].startColor = GetZoomPercentageColor(hazeParticleDataList[i].color);
        }

        hazeParticleSystem.SetParticles(particles, currentAmount);
    }
    private void SetHazeTexture()
    {
        hazeTexture = TextureGenerator.Instance.GenerateTexture(TextureGenerator.TextureType.Nebula, GalaxyGenerator.Instance.Seed);
    }

    // Ambient
    private void InitializeAmbientParticles()
    {
        Tools.ClearParticleSystem(ambientParticleSystem);

        // Declare variables
        var main = ambientParticleSystem.main;
        var emitParams = new ParticleSystem.EmitParams();
        System.Random rand = new System.Random(GalaxyGenerator.Instance.Seed);

        // Seed
        ambientParticleSystem.randomSeed = (uint)GalaxyGenerator.Instance.Seed;

        // MaxParticles
        main.maxParticles = 5;

        // Lifetime
        main.startLifetime = Mathf.Infinity;

        // StartSize
        main.startSize3D = true;
        main.startSizeZ = 0.1f;

        // Position
        emitParams.position = new Vector3(0, 0, ambientParticleSystem.transform.position.z);

        EmitAmbientParticles(); // Particles do not show on first frame of GameState.Play if Emit is not called in Initialize. This only matters when using the console to generate galaxies.
    }
    private void EmitAmbientParticles()
    {
        Tools.ClearParticleSystem(ambientParticleSystem);

        if (GalaxyGenerator.Instance.shapeType != GalaxyGenerator.ShapeType.Ring)
        {
            // Declare variables
            var main = ambientParticleSystem.main;
            var emitParams = new ParticleSystem.EmitParams();
            System.Random rand = new System.Random(GalaxyGenerator.Instance.Seed);

            // Size Light
            float sizeValue = 5f * ((int)GalaxyGenerator.Instance.sizeType + 1);
            main.startSizeX = sizeValue;
            main.startSizeY = sizeValue;

            // Color Light
            emitParams.startColor = GetZoomPercentageColor(ColorHelper.GetAmbientColor(ParticleType.Light));

            // Emit Light
            ambientParticleSystem.Emit(emitParams, 1);

            // Size Center
            sizeValue = 25 * ((int)GalaxyGenerator.Instance.sizeType + 1);
            main.startSizeX = sizeValue;
            main.startSizeY = sizeValue;

            // Color Center
            emitParams.startColor = GetZoomPercentageColor(ColorHelper.GetAmbientColor(ParticleType.Center));

            // Emit Center
            ambientParticleSystem.Emit(emitParams, 1);

            // Size Core
            sizeValue = 50 * ((int)GalaxyGenerator.Instance.sizeType + 1);
            main.startSizeX = sizeValue;
            main.startSizeY = sizeValue;

            // Color Core
            emitParams.startColor = GetZoomPercentageColor(ColorHelper.GetAmbientColor(ParticleType.Core));

            // Emit Core
            ambientParticleSystem.Emit(emitParams, 1);

            if (GalaxyGenerator.Instance.shapeType != GalaxyGenerator.ShapeType.Spiral || GalaxyGenerator.Instance.sizeType != SizeType.Tiny)
            {
                // Size Mid
                sizeValue = 100 * ((int)GalaxyGenerator.Instance.sizeType + 1);
                main.startSizeX = sizeValue;
                main.startSizeY = sizeValue;

                // Color Mid
                emitParams.startColor = GetZoomPercentageColor(ColorHelper.GetAmbientColor(ParticleType.Mid));

                // Emit Mid
                ambientParticleSystem.Emit(emitParams, 1);

                // Size Outer
                sizeValue = 150 * ((int)GalaxyGenerator.Instance.sizeType + 1);
                main.startSizeX = sizeValue;
                main.startSizeY = sizeValue;

                // Color Outer
                emitParams.startColor = GetZoomPercentageColor(ColorHelper.GetAmbientColor(ParticleType.Outer));

                // Emit Outer
                ambientParticleSystem.Emit(emitParams, 1);
            }
        }
    }
    private void UpdateAmbientParticles()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ambientParticleSystem.main.maxParticles];
        int currentAmount = ambientParticleSystem.GetParticles(particles);

        // Color
        for (int i = 0; i < currentAmount; i++)
        {
            switch (i)
            {
                case 0:
                    particles[i].startColor = GetZoomPercentageColor(ColorHelper.GetAmbientColor(ParticleType.Light));
                    break;
                case 1:
                    particles[i].startColor = GetZoomPercentageColor(ColorHelper.GetAmbientColor(ParticleType.Center));
                    break;
                case 2:
                    particles[i].startColor = GetZoomPercentageColor(ColorHelper.GetAmbientColor(ParticleType.Core));
                    break;
                case 3:
                    particles[i].startColor = GetZoomPercentageColor(ColorHelper.GetAmbientColor(ParticleType.Mid));
                    break;
                case 4:
                    particles[i].startColor = GetZoomPercentageColor(ColorHelper.GetAmbientColor(ParticleType.Outer));
                    break;
            }
        }

        ambientParticleSystem.SetParticles(particles, currentAmount);
    }

    // CoreLight 
    private void InitializeCoreLight()
    {
        coreLight.SetActive(true);

        SetCLSize();
        SetCLLight();
        SetCLColor();
        SetCLSimpleNoise();
    }
    private void SetCLSize()
    {
        float scale = 200 * ((int)GalaxyGenerator.Instance.sizeType + 1);

        coreLight.transform.localScale = new Vector3(scale, scale, 1);
    }
    private void SetCLLight()
    {
        if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Ring)
        {
            coreLightMaterial.SetFloat("_Light", 0.7f);
        }
        else
        {
            coreLightMaterial.SetFloat("_Light", 0.65f);
        }
    }
    private void SetCLColor()
    {
        coreLightMaterial.SetColor("_Color", ColorGenerator.Mixed);
    }
    private void SetCLSimpleNoise()
    {
        // Seed (Simple Noise Offset)
        System.Random rand = new System.Random(GalaxyGenerator.Instance.Seed);
        int seed = rand.Next(0, 10000);
        coreLightMaterial.SetFloat("_Seed", seed);

        // Noise Divider
        if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Spiral)
        {
            coreLightMaterial.SetFloat("_NoiseDivider", 15f);
        }
        else if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Ellipitical)
        {
            coreLightMaterial.SetFloat("_NoiseDivider", 15f);
        }
        else if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Ring)
        {
            coreLightMaterial.SetFloat("_NoiseDivider", 20f);
        }
    }
    private void UpdateCoreLightAlpha()
    {
        coreLightMaterial.SetFloat("_Alpha", GetZoomPercentageAlpha(1f));
    }

    // Events
    private void OnChangeView()
    {
        if (ViewController.ViewType != ViewType.Galaxy)
        {
            DisableVFX();
        }
        else
        {
            EmitHazeParticles();
            EmitAmbientParticles();
            coreLight.gameObject.SetActive(true);
        }
    }

    // Utility
    private void OnStart()
    {
        // Haze
        hazeMaterial = hazeParticleSystem.GetComponent<Renderer>().sharedMaterial;

        // CoreLight
        coreLightMaterial = coreLight.GetComponent<Renderer>().sharedMaterial;
        coreLight.SetActive(false);

        // Events
        GalaxyGenerator.PreAfterGenerate.AddListener(SetHazeTexture);
        GalaxyGenerator.AfterGenerate.AddListener(InitializeVFX);
        ViewController.ChangeView.AddListener(OnChangeView);
    }
    private Color GetZoomPercentageColor(Color color)
    {
        float alpha = GetZoomPercentageAlpha(color.a);

        return new Color(color.r, color.g, color.b, alpha);
    }
    private float GetZoomPercentageAlpha(float maxAlpha)
    {
        float newPercentage = 0;

        if (PlayerCamera.Instance.ZoomPercentage >= zoomThreshold)
        {
            float lerpValue = Mathf.InverseLerp(zoomThreshold, 1f, PlayerCamera.Instance.ZoomPercentage);
            newPercentage = Mathf.Lerp(alphaThreshold, 1f, lerpValue);
        }
        else
        {
            float lerpValue = Mathf.InverseLerp(0f, zoomThreshold, PlayerCamera.Instance.ZoomPercentage);
            newPercentage = Mathf.Lerp(0f, alphaThreshold, lerpValue);
        }

        float minAlpha = maxAlpha * alphaMin;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, newPercentage);

        return alpha;
    }

    public struct HazeParticleData
    {
        public bool isEmit;
        public Color color;
        public float size;
        public Vector3 position;
        public Vector3 rotation;
        public List<HazeParticleData> neighborList;
    }
}
