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

    [Header("Stars")]
    [SerializeField] private GameObject[] stars; 

    [Header("Buttons")]
    [SerializeField] private GameObject nextLevelButton;


    public void Setup(bool didWin, int finalScore, int turnsRemaining)
    {
        gameObject.SetActive(true);

        if (didWin)
        {
            headerText.text = "You Won!";
            if (nextLevelButton != null) nextLevelButton.SetActive(true);
            
          
            foreach (var star in stars)
            {
                star.SetActive(true);
            }
        }
        else
        {
            headerText.text = "You Lost!";
            if (nextLevelButton != null) nextLevelButton.SetActive(false);

      
            foreach (var star in stars)
            {
                star.SetActive(false);
            }
        }
        
        totalScoreText.text = finalScore.ToString();
        levelScoreText.text = "Level Score: " + finalScore;
        remainingTurnsText.text = "Remaining Turns: " + turnsRemaining;
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
        GameStateManager.Instance.ChangeState(GameState.Gameplay);
    }



    public void OnNextLevelButton()
    {
        // For now, this can just restart the current level.
        // Later, you would add logic to load the next LevelData asset.
        OnRestartButton();
    }
}