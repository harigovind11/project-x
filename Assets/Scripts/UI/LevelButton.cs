using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject completedIcon;
    [SerializeField] private GameObject lockedIcon;
    
    private int levelIndex;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    public void Setup(int levelIdx, bool isCompleted, bool isLocked)
    {
        levelIndex = levelIdx;
        levelText.text = (levelIndex + 1).ToString(); 

        if (completedIcon != null) completedIcon.SetActive(isCompleted);
        if (lockedIcon != null) lockedIcon.SetActive(isLocked);

        button.interactable = !isLocked;
    }

    private void OnButtonClick()
    {
        Debug.Log("Loading Level: " + (levelIndex + 1));
        // Tell the LevelManager to load the specific level we selected.
        LevelManager.Instance.LoadSpecificLevel(levelIndex);
    }
}