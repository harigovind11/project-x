using UnityEngine;
using TMPro;

public class GameplayScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI turnsText;
    // [SerializeField] private GameObject winPanel;
    // [SerializeField] private GameObject losePanel;

    // void Start()
    // {
    //
    //     if (winPanel != null) winPanel.SetActive(false);
    //     if (losePanel != null) losePanel.SetActive(false);
    // }

    void OnEnable()
    {
        EventManager.OnScoreUpdated += UpdateScoreText;
        EventManager.OnTurnUpdated += UpdateTurnsText;
        // EventManager.OnGameWon += ShowWinPanel;
        // EventManager.OnGameLost += ShowLosePanel;
    }

    void OnDisable()
    {
        EventManager.OnScoreUpdated -= UpdateScoreText;
        EventManager.OnTurnUpdated -= UpdateTurnsText;
        // EventManager.OnGameWon -= ShowWinPanel;
        // EventManager.OnGameLost -= ShowLosePanel;
    }

    private void UpdateScoreText(int newScore)
    {
        Debug.Log("UIManager: Received new score: " + newScore); // <-- ADD THIS
        if (scoreText != null)
        {
            scoreText.text = $"{newScore}";
        }
    }


    private void UpdateTurnsText(int turnsTaken, int maxTurns)
    {
        if (turnsText != null)
        {
            if (maxTurns > 0)
            {
                turnsText.text = $"{turnsTaken} / {maxTurns}";
            }
            else
            {
                turnsText.text = $"{turnsTaken}";
            }
        }
    }

    // private void ShowWinPanel()
    // {
    //     if (winPanel != null) winPanel.SetActive(true);
    // }
    //
    // private void ShowLosePanel()
    // {
    //     if (losePanel != null) losePanel.SetActive(true);
    // }
}