using UnityEngine;
using System;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public static event Action<GameState> OnGameStateChanged;
    public GameState CurrentState { get; private set; }

    [Header("State Dependencies")]
    [Tooltip("The GameplayManager that controls the game board.")]
    [SerializeField] private GameplayManager gameplayManager;

    [Header("UI Panels")]
    [SerializeField] private GameObject splashScreenPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private ResultScreen resultScreen;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        EventManager.OnGameWon += HandleGameWonEvent;
        EventManager.OnGameLost += HandleGameLostEvent;
        ChangeState(GameState.SplashScreen);
    }  void OnDestroy()
    {
        EventManager.OnGameWon -= HandleGameWonEvent;
        EventManager.OnGameLost -= HandleGameLostEvent;
    }
    public void ChangeState(GameState newState)
    {
        if (newState == CurrentState) return;

        CurrentState = newState;
        switch (newState)
        {
            case GameState.SplashScreen:
                HandleSplashScreen();
                break;
            case GameState.MainMenu:
                HandleMainMenu();
                break;
            case GameState.Gameplay:
                HandleGameplay();
                break;
            case GameState.Paused:
                HandlePaused();
                break;
            case GameState.GameWon:
                HandleGameWon();
                break;
            case GameState.GameLost:
                HandleGameLost();
                break;
        }
        OnGameStateChanged?.Invoke(newState);
        Debug.Log($"New Game State: {newState}");
    }
    private void DeactivateAllPanels()
    {
        if (splashScreenPanel != null) splashScreenPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        
    }

    // --- State Handler Methods ---
    private void HandleGameWonEvent()
    {
        ChangeState(GameState.GameWon);
    }

    private void HandleGameLostEvent()
    {
        ChangeState(GameState.GameLost);
    }
    private void HandleSplashScreen()
    {
        DeactivateAllPanels();
        if (splashScreenPanel != null) splashScreenPanel.SetActive(true);
    }

    private void HandleMainMenu()
    {
        DeactivateAllPanels();
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    private void HandleGameplay()
    {
        DeactivateAllPanels();
        if (gameplayPanel != null) gameplayPanel.SetActive(true);
    }

    private void HandlePaused()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
    }
    
    private void HandleGameWon()
    {
        Time.timeScale = 0f;
        if (resultScreen != null)
        {
            int finalScore = gameplayManager.GetCurrentScore(); 
            int turnsRemaining = gameplayManager.GetTurnsRemaining(); 
            resultScreen.Setup(true, finalScore, turnsRemaining);
        }
    }

    private void HandleGameLost()
    {
        Time.timeScale = 0f;
        if (resultScreen != null)
        {
            int finalScore = gameplayManager.GetCurrentScore();
            int turnsRemaining = 0; 
            resultScreen.Setup(false, finalScore, turnsRemaining);
        }
    }
}