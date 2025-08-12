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
    private bool isCheckingForMatch = false;

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
        isCheckingForMatch = false;
        flippedCards.Clear();

        CardGenerator.GenerateBoard(currentLevel);
    }
    private void HandleCardFlipped(CardView card)
    {

        if (isCheckingForMatch)
        {
            return;
        }

        card.Flip();
        flippedCards.Add(card);

        if (flippedCards.Count == 2)
        {
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
            card1.SetAsMatched();
            card2.SetAsMatched();

            //Todo : SFX MATCH FOUND

            EventManager.RaiseMatchFound(card1.CardData);
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
            // Todo : Trigger game won state, show UI, etc.
            EventManager.RaiseGameWon();
            Debug.Log("Game Won");
        }
    }
}