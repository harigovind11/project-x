using System.Collections.Generic;

[System.Serializable]
public class PlayerProgress
{
    public int totalScore;
    public List<string> completedLevels;

    public PlayerProgress()
    {
        totalScore = 0;
        completedLevels = new List<string>();
    }
}