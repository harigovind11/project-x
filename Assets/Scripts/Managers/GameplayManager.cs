using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameplayManager : MonoBehaviour
{

    [Header("Dependencies")]
    [SerializeField] private CardGenerator cardGenerator; 

    [Header("Game Feel")]
    [SerializeField] private float delayBeforeHidingMismatch = 0.8f;
    [SerializeField] private float previewDuration = 2.0f; 

    // --- Private Game State ---
  
    private List<CardView> flippedCards = new List<CardView>();
    private Queue<CardView> flippedCardsToProcess = new Queue<CardView>();
    private LevelData currentLevel;
    
    private int matchesFound = 0;
    private int score = 0;
    private int turnsTaken = 0;
    private bool isProcessingPair  = false;
    private bool isGameActive = true;
    
    private int comboThreshold = 2;
    private int combosEarned;
    private int comboMultiplier;
    private int consecutiveMatches;

    #region --- Unity Methods & Event Handling ---

    void OnEnable()
    {
        EventManager.OnCardFlipped += HandleCardFlipped;
        GameStateManager.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDisable()
    {
        EventManager.OnCardFlipped -= HandleCardFlipped;
        GameStateManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Update()
    {
        if (!isProcessingPair && flippedCardsToProcess.Count >= 2)
        {
            CardView card1 = flippedCardsToProcess.Dequeue();
            CardView card2 = flippedCardsToProcess.Dequeue();
            StartCoroutine((ProcessFlippedCardsCoroutine(card1, card2)));
        }
    }

    #endregion
    
    public void StartNewGame(LevelData levelToPlay)
    {
        currentLevel = levelToPlay;
        StartCoroutine(GameStartSequenceCoroutine());
    }
    
    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.Gameplay)
        {
            StartNewGame(LevelManager.Instance.GetCurrentLevelData());
        }
    }
    
    private void HandleCardFlipped(CardView card)
    {
        if (!isGameActive ||
            flippedCards.Contains(card) ||
            flippedCardsToProcess.Contains(card)||
            !card.gameObject.activeInHierarchy)
        {
            return;
        }
        if(flippedCards.Count>0&& flippedCards[0] == card) return;
        EventManager.RaisePlayerFlipSound();
        card.Flip();
        flippedCards.Add(card);

        if (flippedCards.Count == 2)
        {
            turnsTaken++;
            EventManager.RaiseTurnUpdated(turnsTaken, currentLevel.maxNumberOfTurns);
           flippedCardsToProcess.Enqueue(flippedCards[0]);
           flippedCardsToProcess.Enqueue(flippedCards[1]);
           
           flippedCards.Clear();
        }
    }

    private IEnumerator ProcessFlippedCardsCoroutine(CardView card1, CardView card2)
    {  if (card1 == null || card2 == null)
        {
            yield break; 
        }
        isProcessingPair = true;

        yield return new WaitForSeconds(delayBeforeHidingMismatch);

        if (card1.CardData.cardID == card2.CardData.cardID)
        {
            // --- MATCH FOUND ---
            matchesFound++;
            consecutiveMatches++;
            if (consecutiveMatches >= comboThreshold)
            {
                comboMultiplier++;
                combosEarned++;
                EventManager.RaiseComboUpdated(comboMultiplier);
            }
            score += 1; 
            card1.SetAsMatched();
            card2.SetAsMatched();
            EventManager.RaiseMatchFound(card1.CardData);
            EventManager.RaiseScoreUpdated(score);
        }
        else
        {
            // --- MISMATCH ---
            consecutiveMatches = 0;
            if (comboMultiplier > 1)
            {
                comboMultiplier = 1;
                EventManager.RaiseComboUpdated(comboMultiplier);
            }
            card1.Flip();
            card2.Flip();
            EventManager.RaiseMatchFailed();
        }
        CheckEndGameConditions();
        isProcessingPair = false;
    }

    private void CheckEndGameConditions()
    {
        int totalPairsInLevel = currentLevel.columns * currentLevel.rows / 2;
        if (matchesFound >= totalPairsInLevel)
        {
            isGameActive = false;
            EndGame(true); // Game Won
        }
        else if (currentLevel.maxNumberOfTurns > 0 && turnsTaken >= currentLevel.maxNumberOfTurns)
        {
            isGameActive = false;
            EndGame(false); // Game Lost
        }
    }
        
    private void EndGame(bool didWin)
    {
        if (didWin)
        {
            EventManager.RaiseGameWon();
        }
        else
        {
            EventManager.RaiseGameLost();
        }
    }
    
    // Starter Methods
    private IEnumerator GameStartSequenceCoroutine()
    {
        flippedCards.Clear();
        flippedCardsToProcess.Clear();
        isProcessingPair = false;
        
        matchesFound = 0;
        score = 0;
        turnsTaken = 0;
        isGameActive = false; 
        combosEarned = 0;
        comboMultiplier = 1;
        consecutiveMatches = 0;

        EventManager.RaiseNewGameStarted();
        EventManager.RaiseScoreUpdated(score);
        EventManager.RaiseTurnUpdated(turnsTaken, currentLevel.maxNumberOfTurns);
        
        cardGenerator.ClearBoard();
        cardGenerator.GenerateBoard(currentLevel);
        
        yield return new WaitForSeconds(0.5f);

        CardView[] cardsOnBoard = FindObjectsOfType<CardView>();
        
        foreach (var card in cardsOnBoard) card.Flip();
      
        yield return new WaitForSeconds(previewDuration);

        foreach (var card in cardsOnBoard) card.Flip();
        
        yield return new WaitForSeconds(0.5f);

        isGameActive = true;
    }

    // Helper Methods
    public int GetCurrentScore()
    {
        return score;
    }

    public int GetTurnsRemaining()
    {
        if (currentLevel.maxNumberOfTurns == 0) return 0;
        return currentLevel.maxNumberOfTurns - turnsTaken;
    }

    public int GetCombosEarned()
    {
        return combosEarned;
    }
    public string GetCurrentLevelName()
    {
        return currentLevel.name;
    }
}