using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerUI))]
public class DeckPlayer : MonoBehaviour
{
    private const int MAX_CARDS = 7;

    [SerializeField] private uint _startMoney;
    [SerializeField] private uint _startHealth;
    [SerializeField] private LayerMask _whatIsAssignable;

    public readonly Dictionary<Turns, List<Card>> Deck = new();
    private readonly Dictionary<Turns, uint> _currentHealth = new();
    private readonly Dictionary<Turns, uint> _currentMoney = new();

    private PlayerUI _ui;
    private Card _selectedCard;

    private bool _isWaitingForCastPosition = false;

    private void Awake()
    {
        _ui = GetComponent<PlayerUI>();
        SetupUI();

        SetupDeck();
        GameManager.EndTurnEvent += SetupDeck;
    }

    private void Start()
    {
        Deck.Add(Turns.Player1, new List<Card>(MAX_CARDS));
        Deck.Add(Turns.Player2, new List<Card>(MAX_CARDS));

        _currentMoney.Add(Turns.Player1, _startMoney);
        _currentMoney.Add(Turns.Player2, _startMoney);

        _currentHealth.Add(Turns.Player1, _startHealth);
        _currentHealth.Add(Turns.Player2, _startHealth);
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
                Deck[GameManager.CurrentTurn].Remove(_selectedCard);
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
        Deck[GameManager.CurrentTurn].Add(card);
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

            if (Deck[GameManager.CurrentTurn].Contains(card))
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

    private void SetupUI()
    {
        _ui.MaxHealth = _startHealth;
    }

    private void UpdateGUI()
    {
        _ui.CurrentHealth = _currentHealth[GameManager.CurrentTurn];
        _ui.CurrentMoney = _currentMoney[GameManager.CurrentTurn];

        for (int i = 0; i < Deck[GameManager.CurrentTurn].Count; i++)
        {
            Deck[GameManager.CurrentTurn][i].transform.position = _ui.CardSlots[i].transform.position;
        }
    }

    private void OnDestroy()
    {
        GameManager.EndTurnEvent -= SetupDeck;
    }
}
