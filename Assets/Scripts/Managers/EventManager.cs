using System;

public static class EventManager
{
    // Gameplay Events
    public static event Action<CardView> OnCardFlipped;
    public static void RaiseCardFlipped(CardView card) => OnCardFlipped?.Invoke(card);

    public static event Action<CardData> OnMatchFound;
    public static void RaiseMatchFound(CardData cardData) => OnMatchFound?.Invoke(cardData);

    public static event Action OnMatchFailed;
    public static void RaiseMatchFailed() => OnMatchFailed?.Invoke();

    // Game State Events
    public static event Action OnGameWon;
    public static void RaiseGameWon() => OnGameWon?.Invoke();

    public static event Action<int> OnScoreUpdated;
    public static void RaiseScoreUpdated(int newScore) => OnScoreUpdated?.Invoke(newScore);

    public static event Action<int, int> OnTurnUpdated;
    public static void RaiseTurnUpdated(int turnsTaken, int maxTurns) => OnTurnUpdated?.Invoke(turnsTaken, maxTurns);
    public static event Action<int> OnComboUpdated;
    public static void RaiseComboUpdated(int multiplier) => OnComboUpdated?.Invoke(multiplier);
    public static event Action OnGameLost;
    public static void RaiseGameLost() => OnGameLost?.Invoke();

}