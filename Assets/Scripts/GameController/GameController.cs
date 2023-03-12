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
    public VersionScriptableObject VersionObj;

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
        if (VersionObj != null)
        {
            SetGameState(GameState.MainMenu);
        }
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

        VersionObj = Tools.GetVersionObject();
    }
}