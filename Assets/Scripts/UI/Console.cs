using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Console : MonoBehaviour
{
    public GalaxyGenerator galaxyGenerator;
    public GameObject objectListParent;
    public Material material;

    // Console UI
    public TMP_InputField CommandInputField;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI resultText;
    // Command strings (string must be identical to respective method name)
    private string testString = "Test";
    private string debugString = "Debug";
    private string generateString = "Generate";
    private string generateRandomString = "GenerateRandom";
    private string generateIncrementString = "GenerateIncrement";
    private string spawnObjectsString = "SpawnObjects";
    private string destroyAllObjectsString = "DestroyAllObjects";
    private string spawnNebulaTextureString = "SpawnNebulaTexture";
    // Misc
    private float timer;
    private string activeCoroutine;
    private bool isCoroutineRunning;
    private List<GameObject> objectList = new List<GameObject>();

    // Panel Debug
    public GameObject PanelDebug;
    public TextMeshProUGUI VersionText;
    public TextMeshProUGUI FPSText;
    public TextMeshProUGUI ViewText;
    public TextMeshProUGUI SubViewText;
    public TextMeshProUGUI CameraPositionText;
    private float updateTime;
    public static bool DebugMode;

    private void Start()
    {
        OnStart();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ExecuteCommand();
        }

        timer += Time.unscaledDeltaTime;

        UpdatePanelDebug();
    }

    private void ExecuteCommand()
    {
        SetConsoleTextEmpty();

        string input = CommandInputField.text;
        feedbackText.color = new Color(205f / 255f, 205f / 255f, 205f / 255f);

        if (input == testString)
        {
            OnCommandExecute(testString);
        }
        else if (input == debugString)
        {
            OnCommandExecute(debugString);
        }
        else if (input == generateString)
        {
            OnCommandExecute(generateString);
        }
        else if (input == generateRandomString)
        {
            OnCommandExecute(generateRandomString);
        }
        else if (input == generateIncrementString)
        {
            OnCommandExecute(generateIncrementString);
        }
        else if (input == spawnObjectsString)
        {
            OnCommandExecute(spawnObjectsString);
        }
        else if (input == destroyAllObjectsString)
        {
            OnCommandExecute(destroyAllObjectsString);
        }
        else if (input == spawnNebulaTextureString)
        {
            OnCommandExecute(spawnNebulaTextureString);
        }
        else
        {
            if (isCoroutineRunning)
            {
                OnCommandCancel(activeCoroutine);
            }
            else
            {
                feedbackText.color = new Color(205f / 255f, 75f / 255f, 75f / 255f);
                feedbackText.text = "Unknown Command";
            }
        }
    }

    // Commands
    private IEnumerator Test()
    {
        feedbackText.text = testString;

        yield return null;

        OnCommandDone(testString);
    }
    private IEnumerator Debug()
    {
        feedbackText.text = debugString;

        if (!DebugMode)
        {
            DebugMode = true;
            PanelDebug.SetActive(true);
        }
        else if (DebugMode)
        {
            DebugMode = false;
            PanelDebug.SetActive(false);
        }

        yield return null;

        OnCommandDone(debugString);
    }
    private IEnumerator Generate()
    {
        int loopCount = 0;
        int generateAmount = 100;

        while (loopCount < generateAmount)
        {
            loopCount++;
            feedbackText.text = loopCount.ToString();
            galaxyGenerator.ExecuteGenerateWithRandomSeed();
            yield return null;
        }

        OnCommandDone(generateString);
    }
    private IEnumerator GenerateRandom()
    {
        int loopCount = 0;
        int generateAmount = 100;

        while (loopCount < generateAmount)
        {
            loopCount++;
            feedbackText.text = loopCount.ToString();
            galaxyGenerator.ExecuteGenerateWithRandomSettings();
            yield return null;
        }

        OnCommandDone(generateRandomString);
    }
    private IEnumerator GenerateIncrement()
    {
        int loopCount = 0;
        int generateAmount = 100;

        while (loopCount < generateAmount)
        {
            loopCount++;
            feedbackText.text = loopCount.ToString();

            galaxyGenerator.inputSeed = loopCount;
            galaxyGenerator.ExecuteGenerate(false);

            yield return null;
        }

        OnCommandDone(generateIncrementString);
    }
    private IEnumerator SpawnObjects()
    {
        System.Random rand = new System.Random();

        int loopCount = 0;
        int objectAmount = 100;

        while (loopCount < objectAmount)
        {
            loopCount++;
            feedbackText.text = loopCount.ToString();

            int x = rand.Next(0, 100);
            int y = rand.Next(0, 100);
            int z = 0;

            GameObject obj = null;
            int objectSeed = rand.Next(0, 2);

            if (objectSeed == 0)
            {
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            else
            {
                obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            }

            obj.transform.position = new Vector3(x, y, z);

            if (obj.TryGetComponent(out Renderer renderer))
            {
                renderer.material = material;
            }

            obj.transform.SetParent(objectListParent.transform, true);
            objectList.Add(obj);

            yield return null;
        }

        OnCommandDone(spawnObjectsString);
    }
    private IEnumerator DestroyAllObjects()
    {
        int loopCount = 0;

        foreach (GameObject objects in objectList)
        {
            loopCount++;
            feedbackText.text = loopCount.ToString();

            Destroy(objects);

            yield return null;
        }

        objectList.Clear();

        OnCommandDone(destroyAllObjectsString);
    }
    private IEnumerator SpawnNebulaTexture()
    {
        feedbackText.text = spawnNebulaTextureString;

        GameObject obj = null;
        bool shouldSpawn = true;

        // Get a pre-existing TextureGenerator
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i].TryGetComponent(out TextureGenerator gen))
            {
                obj = gen.gameObject;
                shouldSpawn = false;
                break;
            }
        }

        // Spawn TextureGenerator if does not exist
        if (shouldSpawn)
        {
            // Instantiate
            obj = GameObject.CreatePrimitive(PrimitiveType.Quad);

            // Rotation
            obj.transform.Rotate(new Vector3(180, 0, 0));

            // Material
            obj.GetComponent<Renderer>().material = material;

            // Set Parent & List
            obj.transform.SetParent(objectListParent.transform, true);
            objectList.Add(obj);

            // Add Components
            obj.AddComponent<TextureGenerator>();
            obj.AddComponent<DisplayTexture>();

            // TextureGenerator
            TextureGenerator textureGenerator = obj.GetComponent<TextureGenerator>();
            textureGenerator.textureType = TextureGenerator.TextureType.Nebula;
            textureGenerator.usePreset = true;
        }

        yield return null;

        // DisplayTexture
        DisplayTexture displayTexture = obj.GetComponent<DisplayTexture>();
        displayTexture.DrawTexture();

        OnCommandDone(spawnNebulaTextureString);
    }

    // Panel Debug
    private void UpdatePanelDebug()
    {
        if (PanelDebug.activeSelf)
        {
            UpdatePanelDebugText();
        }
    }
    private void UpdatePanelDebugText()
    {
        // Version
        VersionText.text = GameController.Instance.Version.VersionString;

        // FPS
        if (Time.time > updateTime)
        {
            float fps = (float)System.Math.Round(1 / Time.unscaledDeltaTime, 0);

            FPSText.text = fps.ToString();

            updateTime = Time.time + 0.2f;
        }

        // View
        if (ViewController.ViewType == ViewType.Galaxy)
        {
            ViewText.text = "Galaxy";
        }
        else if (ViewController.ViewType == ViewType.System)
        {
            ViewText.text = "System";
        }

        // Sub View
        if (ViewController.SubViewType == SubViewType.Low)
        {
            SubViewText.text = "Low";
        }
        else if (ViewController.SubViewType == SubViewType.High)
        {
            SubViewText.text = "High";
        }

        // Camera
        CameraPositionText.text = System.Math.Round(PlayerCamera.Instance.transform.position.x, 1) + ", " + System.Math.Round(PlayerCamera.Instance.transform.position.y, 1) + ", " + System.Math.Round(PlayerCamera.Instance.transform.position.z, 1).ToString();
    }

    // Input Field Event
    public void HandleCommandInputField(string input)
    {
        SetConsoleTextEmpty();
    }

    // Utility
    private void OnStart()
    {
        SetConsoleTextEmpty();

        PanelDebug.SetActive(false);
    }
    private void OnCommandExecute(string commandString)
    {
        if (!isCoroutineRunning)
        {
            timer = 0;
            StartCoroutine(commandString);
            activeCoroutine = commandString;
            isCoroutineRunning = true;
        }
        else
        {
            OnCommandCancel(commandString);
        }
    }
    private void OnCommandDone(string commandString)
    {
        StopCoroutine(commandString);
        isCoroutineRunning = false;
        resultText.text = System.Math.Round(timer, 2) + "s";
    }
    private void OnCommandCancel(string commandString)
    {
        StopCoroutine(commandString);
        isCoroutineRunning = false;
        resultText.text = "Cancel";
    }
    private void SetConsoleTextEmpty()
    {
        feedbackText.text = "";
        resultText.text = "";
    }
}
