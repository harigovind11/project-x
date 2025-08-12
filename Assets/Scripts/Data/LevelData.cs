using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Memory Game/Level Data")]
public class LevelData : ScriptableObject
{
    public int columns = 4;
    public int rows = 3;
    public List<CardData> cardsToUse; // List of unique cards for this level
}