using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerUI))]
public class DeckPlayer : MonoBehaviour
{
    private const int MAX_CARDS = 7;

    [SerializeField] private uint _startMoney;
    [SerializeField] private LayerMask _whatIsAssignable;

    public readonly List<Card> Deck = new(MAX_CARDS);
    private PlayerUI _ui;
    private Card _selectedCard;
    private Dictionary<Turns, uint> _currentMoney = new();

    private bool _isWaitingForCastPosition = false;

    private void Awake()
    {
        _ui = GetComponent<PlayerUI>();

        SetupDeck();
        GameManager.EndTurnEvent += SetupDeck;
    }

    private void Start()
    {
        _currentMoney.Add(Turns.Player1, _startMoney);
        _currentMoney.Add(Turns.Player2, _startMoney);
    }

    private void Update()
    {
        UpdateGUI();

        if (!_isWaitingForCastPosition)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hitInfo, _whatIsAssignable))
            {
                var randomOffset = Random.insideUnitSphere * .5f;
                randomOffset.y = 0f;

                _selectedCard.Cast(hitInfo.point + randomOffset);
                _currentMoney[GameManager.CurrentTurn] -= _selectedCard.Info.Cost;
                Deck.Remove(_selectedCard);
            }

            _selectedCard = null;
            _isWaitingForCastPosition = false;
        }
    }

    public void SetupNewCard(Card card)
    {
        var _buttonRef = card.GetComponent<Button>();

        _buttonRef.onClick.AddListener(delegate { HandleCast(card); });
        card.gameObject.SetActive(true);
        Deck.Add(card);
    }

    private void SetupDeck()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).TryGetComponent<Card>(out var card))
            {
                continue;
            }

            if (card.Owner != GameManager.CurrentTurn)
            {
                card.gameObject.SetActive(false);

                continue;
            }

            if (Deck.Contains(card))
            {
                card.gameObject.SetActive(true);

                continue;
            }

            SetupNewCard(card);
        }
    }

    /// <summary>
    /// On cast player callback, return true if move is valid false if not
    /// when false the selected card is not casted.
    /// </summary>
    /// <param name="card">Selected card</param>
    private void HandleCast(Card card)
    {
        if (card.Info.Cost > _currentMoney[GameManager.CurrentTurn])
        {
            return;
        }

        _selectedCard = card;
        _isWaitingForCastPosition = true;
    }

    private void UpdateGUI()
    {
        _ui.CurrentMoney = _currentMoney[GameManager.CurrentTurn];

        for (int i = 0; i < Deck.Count; i++)
        {
            Deck[i].transform.position = _ui.CardSlots[i].transform.position;
        }
    }

    private void OnDestroy()
    {
        GameManager.EndTurnEvent -= SetupDeck;
    }
}
