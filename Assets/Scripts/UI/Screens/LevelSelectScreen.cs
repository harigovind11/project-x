using UnityEngine;
using System.Collections.Generic;

public class LevelSelectScreen : MonoBehaviour
{
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform buttonContainer;

    void OnEnable()
    {
        PopulateLevelButtons();
    }

    private void PopulateLevelButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        List<LevelData> allLevels = LevelManager.Instance.GetAllLevels();
        List<string> completedLevels = ProgressionManager.Instance.CurrentProgress.completedLevels;

        int highestUnlockedLevel = completedLevels.Count;

        for (int i = 0; i < allLevels.Count; i++)
        {
            GameObject buttonGO = Instantiate(levelButtonPrefab, buttonContainer);
            LevelButton levelButton = buttonGO.GetComponent<LevelButton>();

            bool isCompleted = completedLevels.Contains(allLevels[i].name);
            bool isLocked = i > highestUnlockedLevel;
            
            levelButton.Setup(i, isCompleted, isLocked);
        }
    }
}