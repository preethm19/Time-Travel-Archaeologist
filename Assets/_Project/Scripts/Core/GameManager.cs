using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Loading,
        Playing,
        TimeTraveling
    }

    public GameState CurrentState { get; private set; }

    [Header("Starting World")]
    [SerializeField] private string startingWorld = "SunTemple_Hub";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CurrentState = GameState.Loading;
    }

    private void Start()
    {
        LoadStartingWorld();
    }

    private void LoadStartingWorld()
    {
        if (SceneLoader.Instance == null)
        {
            Debug.LogError("GameManager: SceneLoader not found!");
            return;
        }

        SceneLoader.Instance.LoadWorld(startingWorld);
    }

    public void SetGameState(GameState newState)
    {
        CurrentState = newState;

        Debug.Log("Game State: " + CurrentState);
    }
}