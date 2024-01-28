using System;
using TMPro;
using UnityEngine;

[Serializable]
public enum CardClassType
{
    Melee,
    Caster,
    Tank,
    Ranged,
}

public class Card : MonoBehaviour
{
    public CardInfo Info;

    [SerializeField] private TMP_Text _TMP_damage;
    [SerializeField] private TMP_Text _TMP_health;

    [HideInInspector] public Turns Owner = Turns.Player1;

    private void Awake()
    {
        GetComponentInChildren<TMP_Text>().text = Info.CardName;
    }

    private void Start()
    {
        _TMP_damage.text = Info.AttackDamage.ToString();
        _TMP_health.text = Info.Health.ToString();
    }

    public void Cast(Vector3 position)
    {
        var npc = Instantiate(Info.WorldObject, position, Quaternion.identity).GetComponent<NPCController>();
        npc.Card = this;
        npc.tag = Owner == Turns.Player1 ? GameTagsFields.PLAYER_1_TROOPS : GameTagsFields.PLAYER_2_TROOPS;
        npc.Agent.destination = position;

        Destroy(gameObject);
    }
}
