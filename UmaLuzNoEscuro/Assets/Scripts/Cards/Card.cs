using System;
using UnityEngine;

[Serializable]
public enum CardClassType
{
    Melee,
    Caster,
    Tank,
    Terrain,
}


// REVIEW - Not sure about using polymorphism here, maybe consider using a more composable approach
public abstract class Card : MonoBehaviour
{
    public CardInfo Info;
    public Turns Owner = Turns.Player1;

    public abstract void Cast(Vector3 position);
}
