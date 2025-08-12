// File: Scripts/Managers/EventManager.cs
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
}