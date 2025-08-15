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
    private LevelData currentLevel;
    private List<CardView> flippedCards = new List<CardView>();
    private int matchesFound = 0;
    private int score = 0;
    private int turnsTaken = 0;
    private bool isCheckingForMatch = false;
    private bool isGameActive = true;
    
    private int comboThreshold = 2;
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
    

    #endregion
    
    public void StartNewGame(LevelData levelToPlay)
    {
        currentLevel = levelToPlay;
        StartCoroutine(GameStartSequenceCoroutine());
    }
    
    private IEnumerator GameStartSequenceCoroutine()
    {
        matchesFound = 0;
        score = 0;
        turnsTaken = 0;
        isGameActive = false; 
        isCheckingForMatch = false;
        flippedCards.Clear();
        comboMultiplier = 1;
        consecutiveMatches = 0;

        EventManager.RaiseNewGameStarted();
        EventManager.RaiseScoreUpdated(score);
        EventManager.RaiseTurnUpdated(turnsTaken, currentLevel.maxNumberOfTurns);
        
        cardGenerator.GenerateBoard(currentLevel);
        
        yield return new WaitForSeconds(0.5f);

        CardView[] cardsOnBoard = FindObjectsOfType<CardView>();
        foreach (var card in cardsOnBoard)
        {
            card.Flip();
        }
        yield return new WaitForSeconds(previewDuration);

        foreach (var card in cardsOnBoard)
        {
            card.Flip();
        }
        
        yield return new WaitForSeconds(0.5f);

        isGameActive = true;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.Gameplay)
        {
            StartNewGame(currentLevel);
        }
    }

    private void HandleCardFlipped(CardView card)
    {
        if (!isGameActive || isCheckingForMatch)
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
            StartCoroutine(CheckForMatchCoroutine());
        }
    }

    private IEnumerator CheckForMatchCoroutine()
    {
        isCheckingForMatch = true;
        yield return new WaitForSeconds(delayBeforeHidingMismatch);

        CardView card1 = flippedCards[0];
        CardView card2 = flippedCards[1];

        if (card1.CardData.cardID == card2.CardData.cardID)
        {
            matchesFound++;
            consecutiveMatches++;
            if (consecutiveMatches >= comboThreshold)
            {
                comboMultiplier++;
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
            consecutiveMatches = 0;
            comboMultiplier = 1;
            
            card1.Flip();
            card2.Flip();
            EventManager.RaiseMatchFailed();
        }

        flippedCards.Clear();
        isCheckingForMatch = false;

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
        return comboMultiplier;
    }
    public string GetCurrentLevelName()
    {
        return currentLevel.name;
    }
}