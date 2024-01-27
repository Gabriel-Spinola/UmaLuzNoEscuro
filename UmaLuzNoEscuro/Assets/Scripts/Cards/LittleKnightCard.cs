using TMPro;
using UnityEngine;

public class LittleKnightCard : Card
{
    private void Awake()
    {
        GetComponentInChildren<TMP_Text>().text = Info.CardName;
    }

    public override sealed void Cast(Vector3 position)
    {
        var npc = Instantiate(Info.WorldObject, position, Quaternion.identity).GetComponent<NPCController>();
        npc.Card = this;
        npc.tag = Owner == Turns.Player1 ? GameTagsFields.PLAYER_1_TROOPS : GameTagsFields.PLAYER_2_TROOPS;
        npc.Agent.destination = position;

        Destroy(gameObject);
    }
}
