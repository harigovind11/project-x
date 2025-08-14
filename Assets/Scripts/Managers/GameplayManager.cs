using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameplayManager : MonoBehaviour
{
    [Header("Level Configuration")]
    [SerializeField] private LevelData currentLevel;

    [Header("Dependencies")]
    [SerializeField] private CardGenerator _cardGenerator;
    [SerializeField] private SaveLoadManager _saveLoadManager;

    [Header("Game Feel")]
    [SerializeField] private float delayBeforeHidingMismatch = 0.8f;

    // --- Private Game State ---
    private List<CardView> _flippedCards = new List<CardView>();
    private int _matchesFound = 0;
    private int _score = 0;
    private int _turnsTaken = 0;
    private bool _isCheckingForMatch = false;
    private bool _isGameActive = true;

    #region --- Unity Methods & Event Handling ---

    void OnEnable()
    {
        EventManager.OnCardFlipped += HandleCardFlipped;
    }

    void OnDisable()
    {
        EventManager.OnCardFlipped -= HandleCardFlipped;
    }

    void Start()
    {
        if (_saveLoadManager.DoesSaveFileExist())
        {
            RestoreGameState();
        }
        else
        {
            StartNewGame();
        }

    }

    #endregion

    void StartNewGame()
    {
        _matchesFound = 0;
        _score = 0;
        _turnsTaken = 0;
        _isGameActive = true;
        _isCheckingForMatch = false;
        _flippedCards.Clear();

        _cardGenerator.GenerateBoard(currentLevel);
        EventManager.RaiseScoreUpdated(_score);
        EventManager.RaiseTurnUpdated(_turnsTaken, currentLevel.maxNumberOfTurns);
    }

    void RestoreGameState()
    {
        GameSaveData saveData = _saveLoadManager.LoadGame();

        _score = saveData.score;
        _turnsTaken = saveData.turnsTaken;
        _matchesFound = saveData.cardStates.Count(card => card.isMatched) / 2; 

        _cardGenerator.GenerateBoardFromSave(currentLevel,saveData.cardStates);
        
        EventManager.RaiseScoreUpdated(_score);
        EventManager.RaiseTurnUpdated(_turnsTaken, currentLevel.maxNumberOfTurns);
    }
    private void HandleCardFlipped(CardView card)
    {

        if (!_isGameActive || _isCheckingForMatch)
        {
            return;
        }

        card.Flip();
        _flippedCards.Add(card);

        if (_flippedCards.Count == 2)
        {
            _turnsTaken++;
            EventManager.RaiseTurnUpdated(_turnsTaken, currentLevel.maxNumberOfTurns);
            StartCoroutine(CheckForMatchCoroutine());
        }
    }


    private IEnumerator CheckForMatchCoroutine()
    {
        // Block player input while we check the pair.
        _isCheckingForMatch = true;
        yield return new WaitForSeconds(delayBeforeHidingMismatch);

        CardView card1 = _flippedCards[0];
        CardView card2 = _flippedCards[1];

        // --- THE MATCHING LOGIC ---
        // We compare the unique cardID from our CardData ScriptableObjects.
        if (card1.CardData.cardID == card2.CardData.cardID)
        {
            _matchesFound++;
            _score++;
            card1.SetAsMatched();
            card2.SetAsMatched();
            EventManager.RaiseMatchFound(card1.CardData);
            EventManager.RaiseScoreUpdated(_score);
            SaveCurrentGame();
        }
        else
        {
            card1.Flip();
            card2.Flip();
            EventManager.RaiseMatchFailed();
        }


        _flippedCards.Clear();
        _isCheckingForMatch = false;


        int totalPairsInLevel = currentLevel.columns * currentLevel.rows / 2;
        bool hasWon = _matchesFound >= totalPairsInLevel;
        
        if (hasWon)
        {
            _isGameActive = false;
            // Todo : Trigger game won state, show UI, etc.
            EndGame(true);
        }
        else if (currentLevel.maxNumberOfTurns > 0 && _turnsTaken >= currentLevel.maxNumberOfTurns)
        {
            _isGameActive = false;
            // Todo : Trigger game lost state, show UI, etc.
            EndGame(false);
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
    private void SaveCurrentGame()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.score = this._score;
        saveData.turnsTaken = this._turnsTaken;
        
        // Get the state of all cards from the board
        var cardsOnBoard = FindObjectsOfType<CardView>();
        foreach (var card in cardsOnBoard)
        {
            saveData.cardStates.Add(new CardSaveState { 
                cardID = card.CardData.cardID, 
                isMatched = card.IsMatched
            });
        }
        
        _saveLoadManager.SaveGame(saveData);
    }
    
    // Helper Methods
    public int GetCurrentScore()
    {
        return _score;
    }

    public int GetTurnsRemaining()
    {
        return currentLevel.maxNumberOfTurns - _turnsTaken;
    }
}