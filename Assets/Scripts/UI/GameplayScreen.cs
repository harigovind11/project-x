using UnityEngine;
using TMPro;

public class GameplayScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI turnsText;

    void OnEnable()
    {
        EventManager.OnScoreUpdated += UpdateScoreText;
        EventManager.OnTurnUpdated += UpdateTurnsText;
    }

    void OnDisable()
    {
        EventManager.OnScoreUpdated -= UpdateScoreText;
        EventManager.OnTurnUpdated -= UpdateTurnsText;
    }

    private void UpdateScoreText(int newScore)
    {
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
                turnsText.text = $"{turnsTaken}/{maxTurns}";
            }
            else
            {
                turnsText.text = $"{turnsTaken}";
            }
        }
    }
    


}