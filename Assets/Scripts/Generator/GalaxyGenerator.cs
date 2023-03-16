using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Generations system coordinates, instantiates systems

public class GalaxyGenerator : MonoBehaviour
{
    // Prefab
    public SolarSystem systemPrefab;
    // Generated SolarSystem list
    public static List<SolarSystem> SolarSystemList = new List<SolarSystem>();
    public static List<Satellite> SatelliteList = new List<Satellite>();

    public enum ShapeType
    {
        Spiral,
        Ellipitical,
        Ring,
    }

    public enum SizeType
    {
        Tiny,
        Small,
        Medium,
        Large,
        Huge
    }

    public enum ColorType
    {
        Red,
        Green,
        Blue,
    }

    // Variables
    [Header("Galaxy")]
    public ShapeType shapeType;
    public SizeType sizeType;
    public ColorType colorType;
    public int inputSeed; // ORIGINAL inputted seed on first generation attempt
    public int Seed; // actual seed (additional attempts add 1 to inputSeed)
    public string Name;
    public int Radius; // physical radius of galaxy
    public int Systems; // amount of systems in galaxy
    [Range(2, 6)] public int NumArms;
    private int rotation;

    [Header("Generation")]
    private const float distanceThreshold = 3f; // how close systems can be

    private int generateAttemptCount; // keep count of how many attempts to generate
    private const int maxGenerationAttempts = 10;
    private bool isGenerationFailure;

    private int collisionCount;
    private int collisionCountMax;

    public bool IsLoading;
    // Event
    public static UnityEvent BeforeGenerate;
    public static UnityEvent PreAfterGenerate;
    public static UnityEvent AfterGenerate;
    public static UnityEvent LateGenerate;
    // Static for reference
    public static GalaxyGenerator Instance;

    private void Awake()
    {
        OnAwake();
    }
    private void Start()
    {
        OnStart();
    }

    // Generate Map
    private void GenerateGalaxy(bool isFirstGenerateAttempt, bool isRandomSeed, bool isRandomSettings, bool isLoading)
    {
        IsLoading = isLoading;

        // GENERATE ATTEMPTS
        if (isFirstGenerateAttempt) // if firstTryToGenerate true, attempts = 1
        {
            generateAttemptCount = 1;
        }
        else // increment attempt
        {
            generateAttemptCount++;
        }

        // GENERATION
        if (generateAttemptCount <= maxGenerationAttempts)
        {
            BeforeGenerate.Invoke();

            OnGenerate(isFirstGenerateAttempt, isRandomSeed, isRandomSettings);

            if (shapeType == ShapeType.Spiral)
            {
                GenerateSpiral();
            }
            else if (shapeType == ShapeType.Ellipitical)
            {
                GenerateElliptical();
            }
            else if (shapeType == ShapeType.Ring)
            {
                GenerateRing();
            }

            if (!isGenerationFailure)
            {
                SystemGenerator.Instance.GenerateAllSolarSystems();
                ColorGenerator.GenerateGalaxyColors();
                Counter.SetTotalCounts(); // before AfterGenerate.Invoke() to have counts calculated before UI is updated

                PreAfterGenerate.Invoke();
                AfterGenerate.Invoke();
                LateGenerate.Invoke();
            }
            else // REGENERATE
            {
                GenerateGalaxy(false, false, false, IsLoading);
            }
        }
        else // SURPASSED MAXATTEMPTS, 
        {
            DestroyAllSystems(); // delete pre-existing map
            Debug.Log("Generation Error");
        }
    }
    private void OnGenerate(bool isFirstGenerateAttempt, bool isRandomSeed, bool isRandomSettings)
    {
        isGenerationFailure = false;

        // delete pre-existing map
        DestroyAllSystems();

        // get new random seed
        if (isFirstGenerateAttempt) // if first attempt
        {
            if (isRandomSeed)
            {
                System.Random r = new System.Random();
                int value = r.Next(0, 999999999);

                inputSeed = value;
            }

            Seed = inputSeed;
        }
        else // additional tries add 1 to the seed
        {
            Seed = Seed + 1;
        }

        // set systems & radius
        if (isRandomSettings)
        {
            SetRandomSettings();
        }

        if (!IsLoading)
        {
            FileGalaxy.CurrentSaveFolderName = null;
        }

        SetSize();
        SetColorType();
        SetRotationType();
        GenerateName();

        // Set collisionCount for generation
        collisionCount = 0;
        collisionCountMax = Systems * 10;

        // set camera bounds
        ViewController.SetCameraBounds(Radius);
    }
    private void SetRandomSettings()
    {
        System.Random rand = new System.Random();

        // SHAPE
        int shapeSeed = rand.Next(0, System.Enum.GetNames(typeof(ShapeType)).Length);
        shapeType = (ShapeType)shapeSeed;

        // SIZE
        int sizeSeed = rand.Next(0, System.Enum.GetNames(typeof(SizeType)).Length);
        sizeType = (SizeType)sizeSeed;

        // NUMARMS
        if (sizeType == SizeType.Tiny)
        {
            int armSeed = rand.Next(0, 2);

            if (armSeed == 0)
            {
                NumArms = 2;
            }
            else if (armSeed == 1)
            {
                NumArms = 3;
            }
        }
        else if (sizeType == SizeType.Small)
        {
            int armSeed = rand.Next(0, 3);

            if (armSeed == 0)
            {
                NumArms = 2;
            }
            else if (armSeed == 1)
            {
                NumArms = 3;
            }
            else if (armSeed == 2)
            {
                NumArms = 4;
            }
        }
        else if (sizeType == SizeType.Medium)
        {
            int armSeed = rand.Next(0, 4);

            if (armSeed == 0)
            {
                NumArms = 2;
            }
            else if (armSeed == 1)
            {
                NumArms = 3;
            }
            else if (armSeed == 2)
            {
                NumArms = 4;
            }
            else if (armSeed == 3)
            {
                NumArms = 5;
            }
        }
        else
        {
            int armSeed = rand.Next(0, 5);

            if (armSeed == 0)
            {
                NumArms = 2;
            }
            else if (armSeed == 1)
            {
                NumArms = 3;
            }
            else if (armSeed == 2)
            {
                NumArms = 4;
            }
            else if (armSeed == 3)
            {
                NumArms = 5;
            }
            else if (armSeed == 4)
            {
                NumArms = 6;
            }
        }
    }
    private void SetSize()
    {
        Radius = Systems / 2;

        switch (sizeType)
        {
            case SizeType.Tiny:
                Systems = 200;
                Radius = 100;
                break;
            case SizeType.Small:
                Systems = 400;
                Radius = 200;
                break;
            case SizeType.Medium:
                Systems = 600;
                Radius = 300;
                break;
            case SizeType.Large:
                Systems = 800;
                Radius = 400;
                break;
            case SizeType.Huge:
                Systems = 1000;
                Radius = 500;
                break;
        }
    }
    private void SetColorType()
    {
        System.Random rand = new System.Random(Seed);

        int colorSeed = rand.Next(0, 3);

        if (colorSeed == 0)
        {
            colorType = ColorType.Red;
        }
        else if (colorSeed == 1)
        {
            colorType = ColorType.Green;
        }
        else if (colorSeed == 2)
        {
            colorType = ColorType.Blue;
        }
    }
    private void SetRotationType()
    {
        System.Random rand = new System.Random(Seed);

        int rotationSeed = rand.Next(0, 4);

        if (rotationSeed == 0)
        {
            rotation = 270;
        }
        else if (rotationSeed == 1)
        {
            rotation = 315;
        }
        else if (rotationSeed == 2)
        {
            rotation = 360;
        }
        else if (rotationSeed == 3)
        {
            rotation = 405;
        }
    }
    private void GenerateName()
    {
        Name = NameGenerator.GenerateGalaxyName();
    }

    // Algorithms
    private void GenerateSpiral()
    {
        // SPIRAL, POSITION
        float standardDeviation = 2; // larger value gives more stars in outer rim (0.5f to 2f)
        //float[] positions = MirrorGaussianDistribution(standardDeviation);
        float[] positions = GaussianDistribution(standardDeviation);

        System.Random rand = new System.Random(Seed);

        float coreMultiplier = 0.07f; // larger multiplier makes supermassive blackhole bigger, keep between 0 - 1
        float armSeparationDistance = 2 * Mathf.PI / NumArms;
        float armOffsetMax = Radius * 0.4f; // max offset position from arm angles
        float radiusOfCore = Radius * 0.5f;

        // Arm rotation
        int armSeed = rand.Next(0, 2);
        float armRotation = 0;
        if (armSeed == 0)
        {
            armRotation = armSeparationDistance / 2f;
        }

        for (int i = 0; i < positions.Length; i++)
        {
            // Instantiate
            SolarSystem system = SpawnSystem(Vector3.zero);
            bool isCollision = false;

            do
            {
                isCollision = false;

                // Get Polar Position (Distance and Angle)
                float distance = 0;
                if (i < positions.Length * 0.75f) // First three quarters anywhere
                {
                    distance = Mathf.Lerp(Radius * coreMultiplier, Radius, positions[i]);
                }
                else // Last quarter beyond "radiusOfCore"
                {
                    distance = Mathf.Lerp(radiusOfCore, Radius, positions[i]);
                }
                float angle = rand.Next(0, 360);

                // Get Angle Offset
                float randomFloat = angle / 360; // get seeded float between 0-1
                float armOffset = randomFloat * armOffsetMax;
                armOffset = armOffset - armOffsetMax / 2;
                float offsetMultiplier = GetOffsetMultiplier();
                if (distance < radiusOfCore) // if core
                {
                    float tightnessMultiplier = 1.25f; // 1f is tight near core, 1.5f is loose near core
                    armOffset = armOffset * ((1 / distance) * tightnessMultiplier); // armOffset is proportional to distance from core
                }
                else // if arms
                {
                    armOffset = armOffset * offsetMultiplier; // fixed offset
                }

                float squaredArmOffset = Mathf.Pow(armOffset, 2f); // Denser at arm angle, less dense at armOffsetMax
                if (armOffset < 0)
                {
                    squaredArmOffset = squaredArmOffset * -1;
                }
                armOffset = squaredArmOffset;

                // Restrict angles to arms, then add angle offset
                angle = (((int)(angle / armSeparationDistance)) * armSeparationDistance) + armRotation + armOffset;

                // Set Cartesian Position (Coordinates)
                CartesianCoord cartesianCoord = Tools.ConvertPolarToCartesian(distance, angle);
                system.transform.position = new Vector3(cartesianCoord.x, cartesianCoord.y, 0);

                // Rotate system around center of galaxy
                float distanceFromCenter = Vector3.Distance(system.transform.position, transform.position);
                float rotationAngle = Mathf.Lerp(0, rotation, distanceFromCenter / Radius);

                if (Seed % 2 == 0) // if seed is even
                {
                    system.transform.RotateAround(transform.position, Vector3.forward, rotationAngle); // clockwise
                }
                else // odd
                {
                    system.transform.RotateAround(transform.position, Vector3.back, rotationAngle); // counterclockwise
                }

                // Check collision
                isCollision = CheckCollision(system);

            } while (isCollision && !isGenerationFailure);
        }
    }
    private void GenerateElliptical()
    {
        float standardDeviation = 2f; // larger value gives more stars in outer rim (0.5f to 2f)
        float[] positions = GaussianDistribution(standardDeviation);

        System.Random rand = new System.Random(Seed);

        float multiplier = 0.1f; // larger multiplier makes supermassive blackhole bigger, keep between 0-1

        // Arms
        for (int i = 0; i < positions.Length; i++)
        {
            // Instantiate
            SolarSystem system = SpawnSystem(Vector3.zero);
            bool isCollision = false;

            do
            {
                // Get Polar Position (Distance + Angle)
                float distance = Mathf.Lerp(Radius * multiplier, Radius, positions[i]);
                float angle = rand.Next(0, 360);

                // Get Cartesian Position (Coordinates)
                float x = Mathf.Cos(angle) * distance;
                float y = Mathf.Sin(angle) * distance;

                system.transform.position = new Vector3(x, y, 0);

                // Check collision
                isCollision = CheckCollision(system);

            } while (isCollision && !isGenerationFailure);
        }
    }
    private void GenerateRing()
    {
        float standardDeviation = 2f; // larger value gives more stars in outer rim (0.5f to 2f)
        float[] positions = GaussianDistribution(standardDeviation);

        System.Random rand = new System.Random(Seed);

        float multiplier = 0.5f; // larger multiplier makes supermassive blackhole bigger, keep between 0 - 1

        // Arms
        for (int i = 0; i < positions.Length; i++)
        {
            // Instantiate
            SolarSystem system = SpawnSystem(Vector3.zero);
            bool isCollision = false;

            do
            {
                // Get Polar Position (Distance + Angle)
                float distance = Mathf.Lerp(Radius * multiplier, Radius, positions[i]);
                float angle = rand.Next(0, 360);

                // Get Cartesian Position (Coordinates)
                float x = Mathf.Cos(angle) * distance;
                float y = Mathf.Sin(angle) * distance;

                system.transform.position = new Vector3(x, y, 0);

                // Check collision
                isCollision = CheckCollision(system);

            } while (isCollision && !isGenerationFailure);
        }
    }
    // Algorithm Helpers
    private bool CheckCollision(SolarSystem system)
    {
        bool isCollision = false;

        foreach (SolarSystem otherSystem in SolarSystemList)
        {
            if (otherSystem != system)
            {
                if (Vector3.Distance(otherSystem.transform.position, system.transform.position) < distanceThreshold)
                {
                    isCollision = true;
                }
            }
        }

        collisionCount++;

        if (collisionCount > collisionCountMax)
        {
            isGenerationFailure = true;
        }

        return isCollision;
    }
    private float GetOffsetMultiplier()
    {
        float offsetMultiplier = 0;

        switch (NumArms) // number of arms influence multiplier
        {
            case 2:
                offsetMultiplier = 0.007f;
                break;
            case 3:
                offsetMultiplier = 0.0055f;
                break;
            case 4:
                offsetMultiplier = 0.0045f;
                break;
            case 5:
                offsetMultiplier = 0.004f;
                break;
            case 6:
                offsetMultiplier = 0.0035f;
                break;
            default:
                offsetMultiplier = 0; // NULL
                break;
        }

        switch (sizeType) // physical radius of galaxy influences multiplier
        {
            case SizeType.Tiny:
                offsetMultiplier *= 6f;
                break;
            case SizeType.Small:
                offsetMultiplier *= 3f;
                break;
            case SizeType.Medium:
                offsetMultiplier *= 2f;
                break;
            case SizeType.Large:
                offsetMultiplier *= 1.5f;
                break;
            case SizeType.Huge:
                offsetMultiplier *= 1.25f;
                break;
        }

        return offsetMultiplier;
    }

    // Buttons
    public void ExecuteGenerate(bool isLoading)
    {
        GenerateGalaxy(true, false, false, isLoading);
    }
    public void ExecuteGenerateWithRandomSeed()
    {
        GenerateGalaxy(true, true, false, false);
    }
    public void ExecuteGenerateWithRandomSettings()
    {
        GenerateGalaxy(true, true, true, false);
    }

    // Utility
    private void OnAwake()
    {
        // Event
        if (BeforeGenerate == null)
        {
            BeforeGenerate = new UnityEvent();
        }

        if (PreAfterGenerate == null)
        {
            PreAfterGenerate = new UnityEvent();
        }

        if (AfterGenerate == null)
        {
            AfterGenerate = new UnityEvent();
        }

        if (LateGenerate == null)
        {
            LateGenerate = new UnityEvent();
        }

        Instance = this;
    }
    private void OnStart()
    {
        // Default (match UI dropdowns in generator settings)
        shapeType = ShapeType.Spiral;
        sizeType = SizeType.Medium;
        NumArms = 4;

        // Counter
        BeforeGenerate.AddListener(Counter.ResetAllCounts);
    }
    private float[] GaussianDistribution(float standardDeviation)
    {
        float[] array = new float[Systems];
        System.Random rand = new System.Random(Seed);

        // CODE FROM ONLINE FOR STANDARD DEVIATION (BELL CURVE) "Box-Muller"
        float mean = 0;
        // default standard deviation is 1, larger number is more dispersed

        for (int i = 0; i < Systems; i++)
        {
            do
            {
                double u1 = 1.0 - rand.NextDouble(); // uniform (0, 1) random doubles
                double u2 = 1.0 - rand.NextDouble();
                double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2); // random normal (0, 1)
                double randNormal = mean + standardDeviation * randStdNormal; // random normal (mean, stdDev^2)

                array[i] = (float)randNormal;
            } while (array[i] < -3 || array[i] > 3);
        }

        // Lerp value between 0 and 1
        for (int i = 0; i < Systems; i++)
        {
            array[i] = Mathf.InverseLerp(-3, 3, array[i]); // Standard Deviation (Bell Curve) is between -3 and 3
        }

        return array;
    }
    private float[] MirrorGaussianDistribution(float standardDeviation)
    {
        float[] positions = GaussianDistribution(standardDeviation);

        // "Mirror" values below 0.5f (0 becomes 1f, 0.25f becomes 0.75f, 0.49f becomes 0.51f)
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i] < 0.5f)
            {
                positions[i] = Mathf.Abs(positions[i] - 1);
            }
        }

        // InverseLerp values between 0.5f and 1f (0.5f becomes 0, 0.75f becomes 0.5f, 1f becomes 1f)
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = Mathf.InverseLerp(0.5f, 1f, positions[i]);
        }

        return positions; // 0 is most common, 1 is most rare
    }

    private SolarSystem SpawnSystem(Vector3 position)
    {
        SolarSystem system = Instantiate(systemPrefab, position, systemPrefab.transform.rotation);
        system.transform.SetParent(SystemGenerator.Instance.transform, true);
        SolarSystemList.Add(system);

        Counter.SystemCount++;

        return system;
    }
    public static void DestroyAllSystems()
    {
        foreach (SolarSystem system in SolarSystemList)
        {
            if (system != null)
            {
                Destroy(system.gameObject);
            }
        }

        SolarSystemList.Clear();
        SatelliteList.Clear();
    }
}

//public class GalaxyGenerator : MonoBehaviour
//{
//    // Prefab
//    public SolarSystem systemPrefab;
//    // Generated SolarSystem list
//    public static List<SolarSystem> SolarSystemList = new List<SolarSystem>();
//    public static List<Satellite> SatelliteList = new List<Satellite>();

//    public enum ShapeType
//    {
//        Spiral,
//        Ellipitical,
//        Ring,
//    }

//    public enum SizeType
//    {
//        Tiny,
//        Small,
//        Medium,
//        Large,
//        Huge
//    }

//    public enum ColorType
//    {
//        Red,
//        Green,
//        Blue,
//    }

//    // Variables
//    [Header("Generation")]
//    public ShapeType shapeType;
//    public SizeType sizeType;
//    public ColorType colorType;
//    public int inputSeed; // ORIGINAL inputted seed on first generation attempt
//    public int Seed; // actual seed (additional attempts add 1 to inputSeed)
//    public string Name;
//    public int Radius; // physical radius of galaxy
//    public int Systems; // amount of systems in galaxy
//    private int generateAttemptCount; // keep count of how many attempts to generate
//    [Header("Spiral")]
//    [Range(2, 6)] public int NumArms;
//    // Event
//    public static UnityEvent BeforeGenerate;
//    public static UnityEvent PreAfterGenerate;
//    public static UnityEvent AfterGenerate;
//    public static UnityEvent LateGenerate;
//    // Static for reference
//    public static GalaxyGenerator Instance;

//    private void Awake() 
//    {
//        OnAwake();
//    }
//    private void Start()
//    {
//        OnStart();
//    }

//    // Generate Map
//    private void GenerateGalaxy(bool isFirstGenerateAttempt, bool isRandomSeed, bool isRandomSettings, bool isLoading)
//    {
//        int maxAttempts = 10;

//        // GENERATE ATTEMPTS
//        if (isFirstGenerateAttempt) // if firstTryToGenerate true, attempts = 1
//        {
//            generateAttemptCount = 1;
//        }
//        else // increment attempt
//        {
//            generateAttemptCount++;
//        }

//        // GENERATION
//        if (generateAttemptCount <= maxAttempts)
//        {
//            BeforeGenerate.Invoke();

//            OnGenerate(isFirstGenerateAttempt, isRandomSeed, isRandomSettings, isLoading);

//            if (shapeType == ShapeType.Spiral)
//            {
//                GenerateSpiral();
//            }
//            else if (shapeType == ShapeType.Ellipitical)
//            {
//                GenerateElliptical();
//            }
//            else if (shapeType == ShapeType.Ring)
//            {
//                GenerateRing();
//            }

//            bool isGenerationAttemptFailure = SpreadCollidingSystems(isLoading);

//            if (!isGenerationAttemptFailure) // if generation attempt success
//            {
//                SystemGenerator.Instance.GenerateAllSolarSystems();
//                ColorGenerator.GenerateGalaxyColors();
//                Counter.SetTotalCounts(); // before AfterGenerate.Invoke() to have counts calculated before UI is updated

//                PreAfterGenerate.Invoke();
//                AfterGenerate.Invoke();
//                LateGenerate.Invoke();
//            }
//        }
//        else // SURPASSED MAXATTEMPTS, 
//        {
//            DestroyAllSystems(); // delete pre-existing map
//        }
//    }
//    private void OnGenerate(bool isFirstGenerateAttempt, bool isRandomSeed, bool isRandomSettings, bool isLoading)
//    {
//        // delete pre-existing map
//        DestroyAllSystems();

//        // get new random seed
//        if (isFirstGenerateAttempt) // if first attempt
//        {
//            if (isRandomSeed)
//            {
//                System.Random r = new System.Random();
//                int value = r.Next(0, 999999999);

//                inputSeed = value;
//            }

//            Seed = inputSeed;
//        }
//        else // additional tries add 1 to the seed
//        {
//            Seed = Seed + 1;
//        }

//        // set systems & radius
//        if (isRandomSettings)
//        {
//            SetRandomSettings();
//        }

//        if (!isLoading)
//        {
//            FileGalaxy.CurrentSaveFolderName = null;
//        }

//        SetSize();
//        SetColorType();
//        GenerateName();

//        // set camera bounds
//        ViewController.SetCameraBounds(Radius);
//    }
//    private void SetRandomSettings()
//    {
//        System.Random rand = new System.Random();

//        // SHAPE
//        int shapeSeed = rand.Next(0, System.Enum.GetNames(typeof(ShapeType)).Length);
//        shapeType = (ShapeType)shapeSeed;

//        // SIZE
//        int sizeSeed = rand.Next(0, System.Enum.GetNames(typeof(SizeType)).Length);
//        sizeType = (SizeType)sizeSeed;

//        // NUMARMS
//        if (sizeType == SizeType.Tiny)
//        {
//            int armSeed = rand.Next(0, 2);

//            if (armSeed == 0)
//            {
//                NumArms = 2;
//            }
//            else if (armSeed == 1)
//            {
//                NumArms = 3;
//            }
//        }
//        else if (sizeType == SizeType.Small)
//        {
//            int armSeed = rand.Next(0, 3);

//            if (armSeed == 0)
//            {
//                NumArms = 2;
//            }
//            else if (armSeed == 1)
//            {
//                NumArms = 3;
//            }
//            else if (armSeed == 2)
//            {
//                NumArms = 4;
//            }
//        }
//        else if (sizeType == SizeType.Medium)
//        {
//            int armSeed = rand.Next(0, 4);

//            if (armSeed == 0)
//            {
//                NumArms = 2;
//            }
//            else if (armSeed == 1)
//            {
//                NumArms = 3;
//            }
//            else if (armSeed == 2)
//            {
//                NumArms = 4;
//            }
//            else if (armSeed == 3)
//            {
//                NumArms = 5;
//            }
//        }
//        else
//        {
//            int armSeed = rand.Next(0, 5);

//            if (armSeed == 0)
//            {
//                NumArms = 2;
//            }
//            else if (armSeed == 1)
//            {
//                NumArms = 3;
//            }
//            else if (armSeed == 2)
//            {
//                NumArms = 4;
//            }
//            else if (armSeed == 3)
//            {
//                NumArms = 5;
//            }
//            else if (armSeed == 4)
//            {
//                NumArms = 6;
//            }
//        }
//    }
//    private void SetSize()
//    {
//        if (sizeType == SizeType.Tiny)
//        {
//            Systems = 200;
//            Radius = Systems / 2;
//        }
//        else if (sizeType == SizeType.Small)
//        {
//            Systems = 400;
//            Radius = Systems / 2;
//        }
//        else if (sizeType == SizeType.Medium)
//        {
//            Systems = 600;
//            Radius = Systems / 2;
//        }
//        else if (sizeType == SizeType.Large)
//        {
//            Systems = 800;
//            Radius = Systems / 2;
//        }
//        else if (sizeType == SizeType.Huge)
//        {
//            Systems = 1000;
//            Radius = Systems / 2;
//        }
//    }
//    private void SetColorType()
//    {
//        System.Random rand = new System.Random(Seed);

//        int colorSeed = rand.Next(0, 3);

//        if (colorSeed == 0)
//        {
//            colorType = ColorType.Red;
//        }
//        else if (colorSeed == 1)
//        {
//            colorType = ColorType.Green;
//        }
//        else if (colorSeed == 2)
//        {
//            colorType = ColorType.Blue;
//        }

//        // DEBUG
//        colorType = ColorType.Blue;
//    }
//    private void GenerateName()
//    {
//        Name = NameGenerator.GenerateGalaxyName();
//    }
//    private bool SpreadCollidingSystems(bool isLoading)
//    {
//        bool isGenerationFail = false;
//        int collisionAmount; // how many systems overlap

//        for (int i = 0; i < SolarSystemList.Count; i++)
//        {
//            int loopCount = 0; // how many times systemList[i] tries to MOVE away from otherSystems

//            do
//            {
//                loopCount++;

//                if (loopCount > 1000) // if systemList[i] tried to MOVE away from otherSystems 1000 times
//                {
//                    isGenerationFail = true;
//                    GenerateGalaxy(false, false, false, isLoading); // RETRY GENERATE ATTEMPT WITH SEED + 1

//                    break;
//                }

//                collisionAmount = 0;

//                foreach (SolarSystem otherSystem in SolarSystemList)
//                {
//                    if (SolarSystemList[i].transform.position != otherSystem.transform.position) // do not check distance of systemList[i] to itself
//                    {
//                        float distance = Vector3.Distance(SolarSystemList[i].transform.position, otherSystem.transform.position);

//                        if (distance < 2f) // if systemList[i] is too close to otherSystem
//                        {
//                            collisionAmount++;

//                            // Move systemList[i] away from otherSystem
//                            Vector3 direction = (SolarSystemList[i].transform.position - otherSystem.transform.position).normalized;
//                            SolarSystemList[i].transform.position = SolarSystemList[i].transform.position + direction;
//                        }
//                    }
//                }
//            } while (collisionAmount > 0); // stop spreading systems when there are 0 overlapping

//            if (isGenerationFail)
//            {
//                break;
//            }
//        }

//        return isGenerationFail;
//    }

//    // Generate Spiral
//    private void GenerateSpiral()
//    {
//        // SPIRAL, POSITION
//        float standardDeviation = 2; // larger value gives more stars in outer rim (0.5f to 2f)
//        float[] positions = MirrorGaussianDistribution(standardDeviation);

//        System.Random rand = new System.Random(Seed);

//        float multiplier = 0.075f; // larger multiplier makes supermassive blackhole bigger, keep between 0 - 1

//        float armSeparationDistance = 2 * Mathf.PI / NumArms;
//        float armOffsetMax = Radius / 2; // max offset position from arm angles

//        for (int i = 0; i < positions.Length; i++)
//        {
//            // Get Polar Position (Distance + Angle)
//            float distance = Mathf.Lerp(Radius * multiplier, Radius, positions[i]); // WITH MirrorGaussianDistribution: -radius to radius is MOST outer rim & SOME core & FEW other outer rim (two arms), 0 to radius is MOST core & FEW outer rim (one arm)
//            float angle = rand.Next(0, 360);

//            // Get Angle Offset
//            float randomFloat = angle / 360; // get seeded float between 0-1
//            float armOffset = randomFloat * armOffsetMax;
//            armOffset = armOffset - armOffsetMax / 2;
//            float radiusOfCore = Radius / (5 * (Systems / 1000f)); // X * systems / 1000f, larger X is smaller radius (core)
//            float offsetMultiplier = GetOffsetMultiplier();
//            if (distance < radiusOfCore) // if core
//            {
//                armOffset = armOffset * (1 / distance); // offset is proportional to distance from core
//            }
//            else // if arms
//            {
//                armOffset = armOffset * offsetMultiplier; // fixed offset
//            }
//            float squaredArmOffset = Mathf.Pow(armOffset, 2);
//            if (armOffset < 0) // Denser at arm angle, less dense at armOffsetMax
//            {
//                squaredArmOffset = squaredArmOffset * -1;
//            }
//            armOffset = squaredArmOffset;

//            // Restrict angles to arms, then add angle offset
//            angle = (int)(angle / armSeparationDistance) * armSeparationDistance + armOffset;

//            // Set Cartesian Position (Coordinates)
//            CartesianCoord cartesianCoord = Tools.ConvertPolarToCartesian(distance, angle);

//            // Instantiate
//            SolarSystem system = SpawnSystem(new Vector3(cartesianCoord.x, cartesianCoord.y, 0));
//        }

//        // ROTATION
//        foreach (SolarSystem system in SolarSystemList)
//        {
//            // ROTATE systems proportional to distance from center of galaxy (stars rotate more farther out)
//            int maxAngleToRotate = 360;
//            float distanceFromCenter = Vector3.Distance(system.transform.position, transform.position);
//            float angle = Mathf.Lerp(0, maxAngleToRotate, distanceFromCenter / Radius);

//            if (Seed % 2 == 0) // if seed is even
//            {
//                system.transform.RotateAround(this.transform.position, Vector3.forward, angle); // rotate clockwise
//            }
//            else // odd
//            {
//                system.transform.RotateAround(this.transform.position, Vector3.back, angle); // rotate counterclockwise
//            }
//        }
//    }
//    private float GetOffsetMultiplier()
//    {
//        float offsetMultiplier = 0;

//        switch (NumArms) // number of arms influence multiplier
//        {
//            case 2:
//                offsetMultiplier = 0.007f;
//                break;
//            case 3:
//                offsetMultiplier = 0.0055f;
//                break;
//            case 4:
//                offsetMultiplier = 0.0045f;
//                break;
//            case 5:
//                offsetMultiplier = 0.004f;
//                break;
//            case 6:
//                offsetMultiplier = 0.0035f;
//                break;
//            default:
//                offsetMultiplier = 0; // NULL
//                break;
//        }

//        switch (sizeType) // physical radius of galaxy influences multiplier
//        {
//            case SizeType.Tiny:
//                offsetMultiplier *= 6f;
//                break;
//            case SizeType.Small:
//                offsetMultiplier *= 3f;
//                break;
//            case SizeType.Medium:
//                offsetMultiplier *= 2f;
//                break;
//            case SizeType.Large:
//                offsetMultiplier *= 1.5f;
//                break;
//            case SizeType.Huge:
//                offsetMultiplier *= 1.25f;
//                break;
//        }

//        return offsetMultiplier;
//    }

//    // Generate Elliptical
//    private void GenerateElliptical()
//    {
//        float standardDeviation = 2; // larger value gives more stars in outer rim (0.5f to 2f)
//        float[] positions = GaussianDistribution(standardDeviation);

//        System.Random rand = new System.Random(Seed);

//        float multiplier = 0.1f; // larger multiplier makes supermassive blackhole bigger, keep between 0-1

//        // Arms
//        for (int i = 0; i < positions.Length; i++)
//        {
//            // Get Polar Position (Distance + Angle)
//            float distance = Mathf.Lerp(Radius * multiplier, Radius, positions[i]);
//            float angle = rand.Next(0, 360);

//            // Get Cartesian Position (Coordinates)
//            float x = Mathf.Cos(angle) * distance;
//            float y = Mathf.Sin(angle) * distance;

//            // Instantiate
//            SolarSystem system = SpawnSystem(new Vector3(x, y, 0));
//        }
//    }

//    // Generate Ring
//    private void GenerateRing()
//    {
//        float standardDeviation = 2f; // larger value gives more stars in outer rim (0.5f to 2f)
//        float[] positions = GaussianDistribution(standardDeviation);

//        System.Random rand = new System.Random(Seed);

//        float multiplier = 0.5f; // larger multiplier makes supermassive blackhole bigger, keep between 0 - 1

//        // Arms
//        for (int i = 0; i < positions.Length; i++)
//        {
//            // Get Polar Position (Distance + Angle)
//            float distance = Mathf.Lerp(Radius * multiplier, Radius, positions[i]);
//            float angle = rand.Next(0, 360);

//            // Get Cartesian Position (Coordinates)
//            float x = Mathf.Cos(angle) * distance;
//            float y = Mathf.Sin(angle) * distance;

//            // Instantiate
//            SolarSystem system = SpawnSystem(new Vector3(x, y, 0));
//        }
//    }

//    // Buttons
//    public void ExecuteGenerate(bool isLoading)
//    {
//        GenerateGalaxy(true, false, false, isLoading);
//    }
//    public void ExecuteGenerateWithRandomSeed()
//    {
//        GenerateGalaxy(true, true, false, false);
//    }
//    public void ExecuteGenerateWithRandomSettings()
//    {
//        GenerateGalaxy(true, true, true, false);
//    }

//    // Utility
//    private void OnAwake()
//    {
//        // Event
//        if (BeforeGenerate == null)
//        {
//            BeforeGenerate = new UnityEvent();
//        }

//        if (PreAfterGenerate == null)
//        {
//            PreAfterGenerate = new UnityEvent();
//        }

//        if (AfterGenerate == null)
//        {
//            AfterGenerate = new UnityEvent();
//        }

//        if (LateGenerate == null)
//        {
//            LateGenerate = new UnityEvent();
//        }

//        Instance = this;
//    }
//    private void OnStart()
//    {
//        // Default (match UI dropdowns in generator settings)
//        shapeType = ShapeType.Spiral;
//        sizeType = SizeType.Medium;
//        NumArms = 4;

//        // Counter
//        BeforeGenerate.AddListener(Counter.ResetAllCounts);
//    }
//    private float[] GaussianDistribution(float standardDeviation)
//    {
//        float[] array = new float[Systems];
//        System.Random rand = new System.Random(Seed);

//        // CODE FROM ONLINE FOR STANDARD DEVIATION (BELL CURVE) "Box-Muller"
//        float mean = 0;
//        // default standard deviation is 1, larger number is more dispersed

//        for (int i = 0; i < Systems; i++)
//        {
//            do
//            {
//                double u1 = 1.0 - rand.NextDouble(); // uniform (0, 1) random doubles
//                double u2 = 1.0 - rand.NextDouble();
//                double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2); // random normal (0, 1)
//                double randNormal = mean + standardDeviation * randStdNormal; // random normal (mean, stdDev^2)

//                array[i] = (float)randNormal;
//            } while (array[i] < -3 || array[i] > 3);
//        }

//        // Lerp value between 0 and 1
//        for (int i = 0; i < Systems; i++)
//        {
//            array[i] = Mathf.InverseLerp(-3, 3, array[i]); // Standard Deviation (Bell Curve) is between -3 and 3
//        }

//        return array;
//    }
//    private float[] MirrorGaussianDistribution(float standardDeviation)
//    {
//        float[] positions = GaussianDistribution(standardDeviation);

//        // "Mirror" values below 0.5f (0 becomes 1f, 0.25f becomes 0.75f, 0.49f becomes 0.51f)
//        for (int i = 0; i < positions.Length; i++)
//        {
//            if (positions[i] < 0.5f)
//            {
//                positions[i] = Mathf.Abs(positions[i] - 1);
//            }
//        }

//        // InverseLerp values between 0.5f and 1f (0.5f becomes 0, 0.75f becomes 0.5f, 1f becomes 1f)
//        for (int i = 0; i < positions.Length; i++)
//        {
//            positions[i] = Mathf.InverseLerp(0.5f, 1f, positions[i]);
//        }

//        return positions; // 0 is most common, 1 is most rare
//    }

//    private SolarSystem SpawnSystem(Vector3 position)
//    {
//        SolarSystem system = Instantiate(systemPrefab, position, systemPrefab.transform.rotation);
//        system.transform.SetParent(SystemGenerator.Instance.transform, true);
//        SolarSystemList.Add(system);

//        Counter.SystemCount++;

//        return system;
//    }
//    public static void DestroyAllSystems()
//    {
//        foreach (SolarSystem system in SolarSystemList)
//        {
//            if (system != null)
//            {
//                Destroy(system.gameObject);
//            }
//        }

//        SolarSystemList.Clear();
//        SatelliteList.Clear();
//    }
//}