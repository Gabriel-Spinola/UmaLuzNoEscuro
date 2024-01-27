using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    [SerializeField] private DeckPlayer _cardPlayer;
    [Tooltip("Add only UI card prefabs")]
    [SerializeField] private List<GameObject> _possibleCards;

    private void OnEnable()
    {
        foreach (var card in _possibleCards)
        {
            Debug.Assert(card.TryGetComponent<Card>(out var _));
        }
    }

    public void DrawRandomCard()
    {
        if (_cardPlayer.Deck.Count >= _cardPlayer.Deck.Capacity)
        {
            // TODO - Add out of space popup
            return;
        }

        var selectedCard = _possibleCards[Random.Range(0, _possibleCards.Count)];
        var card = Instantiate(selectedCard, _cardPlayer.transform).GetComponent<Card>();

        card.Owner = GameManager.CurrentTurn;
        _cardPlayer.SetupNewCard(card);
    }
}