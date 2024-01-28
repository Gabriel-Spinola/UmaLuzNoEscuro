using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerUI))]
public class DeckPlayer : MonoBehaviour
{
    private const int MAX_CARDS = 7;

    public static bool IsLightningAttackHappening = false;

    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private LayerMask _whatIsAssignable;
    [SerializeField] private CinemachineImpulseSource _lightningImpulseSource;

    [SerializeField] private GameObject _player1Lightning;
    [SerializeField] private GameObject _player2Lightning;

    public readonly Dictionary<Turns, List<Card>> Deck = new();

    private PlayerUI _ui;
    private Card _selectedCard;

    private bool _isWaitingForCastPosition = false;

    private void Awake()
    {
        _ui = GetComponent<PlayerUI>();
        SetupDeck();

        GameManager.EndTurnEvent += SetupDeck;
        GameManager.EndFullTurnEvent += OnEndOfFullTurn;
    }

    private void Start()
    {
        Deck.Add(Turns.Player1, new List<Card>(MAX_CARDS));
        Deck.Add(Turns.Player2, new List<Card>(MAX_CARDS));

        _playerController.LightningPower.Add(Turns.Player1, _playerController.StartLightningPower);
        _playerController.LightningPower.Add(Turns.Player2, _playerController.StartLightningPower);

        _playerController.CurrentHealth.Add(Turns.Player1, _playerController.StartHealth);
        _playerController.CurrentHealth.Add(Turns.Player2, _playerController.StartHealth);
    }

    private void Update()
    {
        UpdateGUI();

        if (!_isWaitingForCastPosition)
        {
            return;
        }

        Debug.Log("Waiting for assigment");
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hitInfo, Mathf.Infinity, _whatIsAssignable))
            {
                var randomOffset = UnityEngine.Random.insideUnitSphere * .5f;
                randomOffset.y = 0f;

                _selectedCard.Cast(hitInfo.point + randomOffset);
                _playerController.LightningPower[GameManager.CurrentTurn] -= _selectedCard.Info.Cost;
                Deck[GameManager.CurrentTurn].Remove(_selectedCard);
            }

            _selectedCard = null;
            _isWaitingForCastPosition = false;
        }
    }

    public void LightningAttack()
    {
        var targetPlayer = GameManager.CurrentTurn == Turns.Player1 ? Turns.Player2 : Turns.Player1;
        var particleSystem = GameManager.CurrentTurn == Turns.Player1 
            ? _player1Lightning.GetComponent<ParticleSystem>() 
            : _player2Lightning.GetComponent<ParticleSystem>();

        particleSystem.Play();
        IsLightningAttackHappening= particleSystem.isPlaying;

        _playerController.CurrentHealth[targetPlayer] -= _playerController.LightningDamage;
        _playerController.LightningPower[GameManager.CurrentTurn] -= _playerController.LightningAttackCost;

        _lightningImpulseSource.GenerateImpulse();
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
        Debug.Log("Handle cast");
        if (card.Info.Cost > _playerController.LightningPower[GameManager.CurrentTurn])
        {
            Debug.Log("Failed to casrt");
            return;
        }

        _selectedCard = card;
        _isWaitingForCastPosition = true;
    }

    private void UpdateGUI()
    {
        for (int i = 0; i < Deck[GameManager.CurrentTurn].Count; i++)
        {
            Deck[GameManager.CurrentTurn][i].transform.position = _ui.CardSlots[i].transform.position;
        }
    }

    private void OnEndOfFullTurn()
    {
        _playerController.LightningPower[Turns.Player1] += GameManager.GlobalTurnsCount + 2;
        _playerController.LightningPower[Turns.Player2] += GameManager.GlobalTurnsCount + 2;
    }

    private void OnDestroy()
    {
        GameManager.EndTurnEvent -= SetupDeck;
        GameManager.EndFullTurnEvent -= OnEndOfFullTurn;
    }
}
