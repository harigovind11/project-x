using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Memory Game/Card Data")]
public class CardData : ScriptableObject
{
    public int cardID;
    public Sprite cardSprite;
}