using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralObjectiveSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _monster;

    private bool _isSpawned;

    private void Update()
    {
        if (GameManager.GlobalTurnsCount == 2 && !_isSpawned)
        {
            Instantiate(_monster, transform.position, Quaternion.identity);

            _isSpawned = true;  
        }
    }
}
