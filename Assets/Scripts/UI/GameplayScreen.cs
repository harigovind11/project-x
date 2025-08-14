using System.Collections;
using UnityEngine;
using TMPro;

public class GameplayScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI turnsText;
    [Header("Combo Effect")]
    [SerializeField] private TextMeshProUGUI comboText; 
    [SerializeField] private CanvasGroup comboCanvasGroup; 
    [SerializeField] private float comboEffectDuration = 1.5f; 
    [SerializeField] private float comboMoveUpDistance = 60f; 
    [SerializeField] private float comboScaleFactor = 1.2f;
    
    private Coroutine comboCoroutine;
    void OnEnable()
    {
        EventManager.OnScoreUpdated += UpdateScoreText;
        EventManager.OnTurnUpdated += UpdateTurnsText;
        EventManager.OnComboUpdated += ShowComboEffect;
    }

    void OnDisable()
    {
        EventManager.OnScoreUpdated -= UpdateScoreText;
        EventManager.OnTurnUpdated -= UpdateTurnsText;
        EventManager.OnComboUpdated -= ShowComboEffect;
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
    
    private void ShowComboEffect(int multiplier)
    {
        if (multiplier > 1)
        {
            if (comboCoroutine != null)
            {
                StopCoroutine(comboCoroutine);
            }
            comboCoroutine = StartCoroutine(ComboAnimationCoroutine(multiplier));
        }
        else
        {
            comboCanvasGroup.alpha = 0;
        }
    }

    private IEnumerator ComboAnimationCoroutine(int multiplier)
    {
        comboText.text = $"{multiplier}x Combo!";
        comboCanvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;
        Vector3 startPosition = comboText.transform.localPosition;
        Vector3 targetPosition = startPosition + Vector3.up * comboMoveUpDistance;
        Vector3 startScale = Vector3.one;
        Vector3 targetScale = Vector3.one * comboScaleFactor;
        
        float elapsedTime = 0f;
        while (elapsedTime < comboEffectDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / comboEffectDuration;
            
            comboText.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, progress);
            comboText.transform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            comboCanvasGroup.alpha = Mathf.Lerp(1f, 0f, progress); 

            yield return null;
        }
        comboCanvasGroup.alpha = 0f;
    }

}