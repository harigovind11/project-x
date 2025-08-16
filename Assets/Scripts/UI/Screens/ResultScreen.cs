using UnityEngine;
using TMPro;

public class ResultScreen : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private TextMeshProUGUI headerText;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI levelScoreText; 
    [SerializeField] private TextMeshProUGUI remainingTurnsText; 
    [SerializeField] private TextMeshProUGUI comboEarnedText;

    [Header("Stars")]
    [SerializeField] private GameObject[] stars; 

    [Header("Buttons")]
    [SerializeField] private GameObject nextLevelButton;


    public void Setup(bool didWin, int levelScore,int totalScore, int turnsRemaining,int comboEarned)
    {
        gameObject.SetActive(true);

        if (didWin)
        {
            headerText.text = "Won!";
            if (nextLevelButton != null) nextLevelButton.SetActive(true);
            
          
            foreach (var star in stars)
            {
                star.SetActive(true);
            }
        }
        else
        {
            headerText.text = "Lost!";
            if (nextLevelButton != null) nextLevelButton.SetActive(false);

      
            foreach (var star in stars)
            {
                star.SetActive(false);
            }
        }
        
        levelScoreText.text =levelScore.ToString();
        totalScoreText.text = totalScore.ToString();
        remainingTurnsText.text = turnsRemaining.ToString();
        comboEarnedText.text= comboEarned.ToString();
    }

    // --- Button Methods ---
    
    public void OnHomeButton()
    {
        Time.timeScale = 1f;
        GameStateManager.Instance.ChangeState(GameState.MainMenu);
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        LevelManager.Instance.RestartCurrentLevel();
    }
    
    public void OnNextLevelButton()
    {
        Time.timeScale = 1f;
        LevelManager.Instance.LoadNextLevel();
    }
}