using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameplayManager : MonoBehaviour
{
    [Header("Level Configuration")]
    [SerializeField] private LevelData currentLevel;

    [Header("Dependencies")]
    [SerializeField] private CardGenerator CardGenerator;

    [Header("Game Feel")]
    [SerializeField] private float delayBeforeHidingMismatch = 0.8f;

    // --- Private Game State ---
    private List<CardView> flippedCards = new List<CardView>();
    private int matchesFound = 0;
    private int score = 0;
    private int turnsTaken = 0;
    private bool isCheckingForMatch = false;
    private bool isGameActive = true;

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
        StartNewGame();
    }

    #endregion

    void StartNewGame()
    {
        matchesFound = 0;
        score = 0;
        turnsTaken = 0;
        isGameActive = true;
        isCheckingForMatch = false;
        flippedCards.Clear();

        CardGenerator.GenerateBoard(currentLevel);
        EventManager.RaiseScoreUpdated(score);
        EventManager.RaiseTurnUpdated(turnsTaken, currentLevel.maxNumberOfTurns);
    }
    private void HandleCardFlipped(CardView card)
    {

        if (!isGameActive || isCheckingForMatch)
        {
            return;
        }

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
        // Block player input while we check the pair.
        isCheckingForMatch = true;

        // Wait for a moment so the player has time to see the second card.
        yield return new WaitForSeconds(delayBeforeHidingMismatch);

        CardView card1 = flippedCards[0];
        CardView card2 = flippedCards[1];

        // --- THE MATCHING LOGIC ---
        // We compare the unique cardID from our CardData ScriptableObjects.
        if (card1.CardData.cardID == card2.CardData.cardID)
        {

            matchesFound++;
            score++;
            card1.SetAsMatched();
            card2.SetAsMatched();

            //Todo : SFX MATCH FOUND

            EventManager.RaiseMatchFound(card1.CardData);
            EventManager.RaiseScoreUpdated(score);
        }
        else
        {
            //Todo : SFX MISMATCH

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
            EventManager.RaiseGameWon();
            // Todo : Trigger game won state, show UI, etc.
            Debug.Log("Game Won");
        }
        else if (currentLevel.maxNumberOfTurns > 0 && turnsTaken >= currentLevel.maxNumberOfTurns)
        {
            isGameActive = false;
            EventManager.RaiseGameLost();
            // Todo : Trigger game lost state, show UI, etc.
            Debug.Log("Game Lost");
        }
    }
}