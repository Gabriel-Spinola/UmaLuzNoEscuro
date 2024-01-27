using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Card Game/New Card", order = 1)]
public class CardInfo : ScriptableObject
{
    [Header("Config")]
    public string CardName;
    public uint Cost;
    public CardClassType CardType;
    public GameObject WorldObject;

    [Header("Stats")]
    public float Health;
    public float AttackDamage;
}