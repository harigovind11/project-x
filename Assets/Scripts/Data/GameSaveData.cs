using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    public int score;
    public int turnsTaken;

    public List<CardSaveState> cardStates;

    public GameSaveData()
    {
        cardStates = new List<CardSaveState>();
    }
}

[System.Serializable]
public class CardSaveState
{
    public int cardID;
    public bool isMatched;
}