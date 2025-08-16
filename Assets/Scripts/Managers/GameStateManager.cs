using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public static event Action<GameState> OnGameStateChanged;
    public GameState CurrentState { get; private set; }

    [Header("State Dependencies")]
    [Tooltip("The GameplayManager that controls the game board.")]
    [SerializeField] private GameplayManager gameplayManager;
    [SerializeField] private CardGenerator cardGenerator;

    [Header("UI Panels")]
    [SerializeField] private List<UIPanel> uiPanels;
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
    }
    void OnDestroy()
    {
        EventManager.OnGameWon -= HandleGameWonEvent;
        EventManager.OnGameLost -= HandleGameLostEvent;
    }
    public void ChangeState(GameState newState)
    {
        if (newState == CurrentState) return;
        if (CurrentState == GameState.Gameplay)
        {
            if (cardGenerator != null)
            {
                cardGenerator.ClearBoard();
            }
        }
        CurrentState = newState;
        switch (newState)
        {
            case GameState.SplashScreen:
                HandleSplashScreen();
                break;
            case GameState.MainMenu:
                HandleMainMenu();
                break;
            case GameState.LevelSelect:
                HandleLevelSelect();
                break;
            case GameState.Gameplay:
                HandleGameplay();
                break;
            case GameState.Paused:
                HandlePaused();
                break;
            case GameState.ResultScreen:
                ActivatePanelForState(GameState.ResultScreen);
                break;
        }
        OnGameStateChanged?.Invoke(newState);
        Debug.Log($"New Game State: {newState}");
    }
    private void DeactivateAllPanels()
    {
        // Loop through every panel in our list and deactivate it.
        foreach (var panel in uiPanels)
        {
            if (panel.panelObject != null)
            {
                panel.panelObject.SetActive(false);
            }
        }
    }

    // --- State Handler Methods ---
    private void HandleSplashScreen()
    {
        StartCoroutine(SplashScreenCoroutine());
    }


    private void HandleMainMenu()
    {
        Time.timeScale = 1f;
        ActivatePanelForState(GameState.MainMenu);
    }

    private void HandleLevelSelect()
    {
        ActivatePanelForState(GameState.LevelSelect);
    }

    private void HandleGameplay()
    {
        Time.timeScale = 1f;
        ActivatePanelForState(GameState.Gameplay);
        LevelData levelToPlay = LevelManager.Instance.GetCurrentLevelData();
        if (gameplayManager != null && levelToPlay != null)
        {
            gameplayManager.StartNewGame(levelToPlay);
        }
        else
        {
            Debug.LogError("Could not find a level to play!");
            ChangeState(GameState.MainMenu);
        }
    }

    private void HandlePaused()
    {
        Time.timeScale = 0f;

        UIPanel pausePanel = uiPanels.Find(p => p.state == GameState.Paused);
        if (pausePanel != null && pausePanel.panelObject != null)
        {
            pausePanel.panelObject.SetActive(true);
        }
    }
    private void HandleGameWonEvent()
    { 
        ProgressionManager.Instance.MarkLevelAsCompleted(gameplayManager.GetCurrentLevelName());
        
        int levelScore = gameplayManager.GetCurrentScore();
        int turnsRemaining = gameplayManager.GetTurnsRemaining();
        int combosEarned = gameplayManager.GetCombosEarned();
        
        ProgressionManager.Instance.AddToTotalScore(levelScore);
        int newTotalScore = ProgressionManager.Instance.CurrentProgress.totalScore;
        
        ChangeState(GameState.ResultScreen);
        resultScreen.Setup(true,levelScore,newTotalScore,turnsRemaining,combosEarned);
    }

    private void HandleGameLostEvent()
    {

        int levelScore = gameplayManager.GetCurrentScore();
        int totalScore = ProgressionManager.Instance.CurrentProgress.totalScore;
        int combosEarned = gameplayManager.GetCombosEarned();

        ChangeState(GameState.ResultScreen);

        resultScreen.Setup(false,levelScore,totalScore,0,combosEarned);
    }

    private IEnumerator SplashScreenCoroutine()
    {
        ActivatePanelForState(GameState.SplashScreen);
        yield return new WaitForSeconds(2f);
        ChangeState(GameState.MainMenu);
    }

    // Helper Methods
    private void ActivatePanelForState(GameState state)
    {

        DeactivateAllPanels();

        // Then, find the correct panel in our list that matches the new state.
        UIPanel panelToActivate = uiPanels.Find(p => p.state == state);

        if (panelToActivate != null && panelToActivate.panelObject != null)
        {
            panelToActivate.panelObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No UI Panel found for state: " + state);
        }
    }
}

[System.Serializable]
public class UIPanel
{
    public GameState state;
    public GameObject panelObject;
}