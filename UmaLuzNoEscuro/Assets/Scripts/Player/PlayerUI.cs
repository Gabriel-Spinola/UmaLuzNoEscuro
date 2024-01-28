using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class PlayerUI : MonoBehaviour
{
    public Transform[] CardSlots;
    [SerializeField] private PlayerController _playerController;

    [SerializeField] private TMP_Text _TMP_currentMoney;
    [SerializeField] private Slider _healthBar;
    [SerializeField] private GameObject _lightningCard;

    private void Start()
    {
        _healthBar.maxValue = _playerController.StartHealth;
        _healthBar.minValue = 0u;
    }

    private void Update()
    {
        if (_playerController.LightningPower[GameManager.CurrentTurn] >= _playerController.LightningAttackThreshold)
        {
            _lightningCard.SetActive(true);
        }
        else if (_lightningCard.activeSelf)
        {
            _lightningCard.SetActive(false);
        }

        _TMP_currentMoney.text = _playerController.LightningPower[GameManager.CurrentTurn].ToString();
        _healthBar.value = _playerController.CurrentHealth[GameManager.CurrentTurn];
    }
}
