using System.Collections.Generic;

[System.Serializable]
public class PlayerProgress
{
    public List<string> completedLevels;

    public PlayerProgress()
    {
        completedLevels = new List<string>();
    }
}