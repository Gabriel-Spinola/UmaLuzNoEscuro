using System;
using TMPro;
using UnityEngine;


[Serializable]
public class PlayerUI : MonoBehaviour
{
    public Transform[] CardSlots;

    [SerializeField] private TMP_Text _TMP_currentMoney;
    [HideInInspector] public uint CurrentMoney;

    private void Update()
    {
        _TMP_currentMoney.text = CurrentMoney.ToString();
    }
}
