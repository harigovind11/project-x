using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }

    public PlayerProgress CurrentProgress { get; private set; }
    
    [SerializeField] private SaveLoadManager saveLoadManager;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPlayerProgress();
        }
    }

    public void MarkLevelAsCompleted(string levelName)
    {
        if (!CurrentProgress.completedLevels.Contains(levelName))
        {
            CurrentProgress.completedLevels.Add(levelName);
            SavePlayerProgress();
        }
    }

    public void AddToTotalScore(int scoreToAdd)
    {
        CurrentProgress.totalScore += scoreToAdd;
        SavePlayerProgress();
    }

    private void SavePlayerProgress()
    {
        saveLoadManager.SaveProgress(CurrentProgress);
    }

    private void LoadPlayerProgress()
    {
        CurrentProgress = saveLoadManager.LoadProgress();
    }
}