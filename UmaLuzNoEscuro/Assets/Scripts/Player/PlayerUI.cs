using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class PlayerUI : MonoBehaviour
{
    public Transform[] CardSlots;

    [SerializeField] private TMP_Text _TMP_currentMoney;
    [SerializeField] private Slider _healthBar;

    [HideInInspector] public uint CurrentMoney;
    [HideInInspector] public uint CurrentHealth;
    [HideInInspector] public uint MaxHealth;

    private void Start()
    {
        _healthBar.maxValue = MaxHealth;
        _healthBar.minValue = 0u;
    }

    private void Update()
    {
        _TMP_currentMoney.text = CurrentMoney.ToString();
        _healthBar.value = CurrentHealth;
    }
}
