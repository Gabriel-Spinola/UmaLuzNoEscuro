using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Game Rules")]
    public uint LightningAttackThreshold;
    public uint LightningAttackCost;
    public uint LightningDamage;
    public uint StartLightningPower;
    public uint StartHealth;

    public readonly Dictionary<Turns, uint> LightningPower = new();
    public readonly Dictionary<Turns, uint> CurrentHealth = new();

    public void AddLightningPoints(Turns player, uint points)
    {
        Debug.Log("Received of power");
        LightningPower[player] += points;
    }
}
