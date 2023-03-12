using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public enum GameState
{
    MainMenu,
    Play
}
public class GameController : MonoBehaviour
{
    // Version
    [Header("Current Version")]
    [SerializeField] private int Major;
    [SerializeField] private int Minor;
    [SerializeField] private int Patch;
    [Header("Last Compatible Version")]
    [SerializeField] private int LastMajor;
    [SerializeField] private int LastMinor;
    [SerializeField] private int LastPatch;

    public Version Version
    {
        get { return new Version(Major, Minor, Patch); }
    }
    public Version LastCompatibleVersion
    {
        get { return new Version(LastMajor, LastMinor, LastPatch); }
    }

    // GameState
    [HideInInspector] public GameState GameState;
    public static UnityEvent ChangeGameState;

    public static GameController Instance
    {
        get { return instance; }
    }
    private static GameController instance;

    private void Awake()
    {
        OnAwake();
    }
    private void Start()
    {
        SetGameState(GameState.MainMenu);
    }

    // Game State
    public void SetGameState(GameState newGameState)
    {
        if (newGameState == GameState.MainMenu)
        {
            SceneManager.LoadScene("Galaxy");
            GalaxyGenerator.DestroyAllSystems();
        }

        GameState = newGameState;
        ChangeGameState.Invoke();
    }

    // Utility
    private void OnAwake()
    {
        DontDestroyOnLoad(gameObject);

        // Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Events
        if (ChangeGameState == null)
        {
            ChangeGameState = new UnityEvent();
        }
    }
    public bool IsVersionCompatible(Version version)
    {
        // MAJOR
        if (version.Major > LastCompatibleVersion.Major)
        {
            // Compatible
            return true;
        }
        else if (version.Major == LastCompatibleVersion.Major)
        {
            // MINOR
            if (version.Minor > LastCompatibleVersion.Minor)
            {
                // Compatible
                return true;
            }
            else if (version.Minor == LastCompatibleVersion.Minor)
            {
                // PATCH
                if (version.Patch >= LastCompatibleVersion.Patch)
                {
                    // Compatible
                    return true;
                }
                else
                {
                    // Incompatible
                    return false;
                }
            }
            else
            {
                // Incompatible
                return false;
            }
        }
        else
        {
            // Incompatible
            return false;
        }
    }
}

public struct Version
{
    public int Major;
    public int Minor;
    public int Patch;
    public string VersionString
    {
        get { return Major + "." + Minor + "." + Patch; }
    }

    public Version(int major, int minor, int patch)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }
}