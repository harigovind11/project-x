using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Levels")]
    [SerializeField] private List<LevelData> levels;

    private int currentLevelIndex = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
            DetermineCurrentLevel();
        }
    }
    private void DetermineCurrentLevel()
    {
        List<string> completedLevels = ProgressionManager.Instance.CurrentProgress.completedLevels;

        int lastCompletedIndex = -1;
        if (completedLevels.Count > 0)
        {
            string lastCompletedName = completedLevels.Last();
            lastCompletedIndex = levels.FindIndex(level => level.name == lastCompletedName);
        }

        currentLevelIndex = lastCompletedIndex + 1;

        if (currentLevelIndex >= levels.Count)
        {
            currentLevelIndex = levels.Count - 1; 
        }
    }

    public LevelData GetCurrentLevelData()
    {
        if (currentLevelIndex < levels.Count)
        {
            return levels[currentLevelIndex];
        }
        return null;
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= levels.Count)
        {
            Debug.Log("Congratulations! All levels completed!");
            GameStateManager.Instance.ChangeState(GameState.MainMenu);
        }
        else
        {
            GameStateManager.Instance.ChangeState(GameState.Gameplay);
        }
    }

    public void RestartCurrentLevel()
    {
        GameStateManager.Instance.ChangeState(GameState.Gameplay);
    }
    
    public List<LevelData> GetAllLevels()
    {
        return levels;
    }

    public void LoadSpecificLevel(int levelIdx)
    {
        if (levelIdx < levels.Count)
        {
            currentLevelIndex = levelIdx;
            GameStateManager.Instance.ChangeState(GameState.Gameplay);
        }
    }
}