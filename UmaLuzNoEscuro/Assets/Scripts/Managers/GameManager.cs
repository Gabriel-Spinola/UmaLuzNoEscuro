using UnityEngine;
using Cinemachine;
using System;

public readonly struct GameTagsFields
{
    public const string PLAYER_1_TROOPS = "Player 1 Troops";
    public const string PLAYER_2_TROOPS = "Player 2 Troops";
    public const string NEUTRAL_OBJECTIVES = "Neutral Objectives";

    public static string[] AllTags = { PLAYER_1_TROOPS, PLAYER_2_TROOPS, NEUTRAL_OBJECTIVES };
}

public enum Turns
{
    Player1 = 1,
    Player2 = -1,
}

public enum GameState
{
    Paused,
    Turns,
    Simulating,
}

public class GameManager : MonoBehaviour
{
    public static GameState State = GameState.Turns;
    public static Turns CurrentTurn = Turns.Player1;
    public static event Action EndTurnEvent;

    [Header("UI References")]
    [SerializeField] private GameObject _cardPlayerObject;

    [Header("Lightning")]
    [SerializeField] private Transform _turn1Lightning;
    [SerializeField] private Transform _turn2Lightning;
    [SerializeField] private Transform _simulationLightning;

    [Header("AI")]
    [SerializeField] private GameObject[] _destroyOnTurnsPhase;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera _simulationCam;
    [SerializeField] private CinemachineVirtualCamera _player1Cam, _player2Cam;

    private int _turnsCount = 0;

    private void Awake()
    {
        _player1Cam.Priority = 1;
        _player2Cam.Priority = 0;
        _simulationCam.Priority = -2;
    }

    private void Update()
    {
        if (_turnsCount >= 2)
        {
            State = GameState.Simulating;

            _simulationCam.Priority = 2;

            foreach (var @object in _destroyOnTurnsPhase)
            {
                Destroy(@object);
            }
        }

        if (State is GameState.Simulating && _cardPlayerObject.activeSelf)
        {
            _cardPlayerObject.SetActive(false);
        }

        // TODO - State Control
        switch (State)
        {
            case GameState.Turns: break;

            case GameState.Simulating: break;
        }
    }

    public void EndTurn()
    {
        if (State is GameState.Simulating)
        {
            EndSimulation();

            _turnsCount = 0;
            CurrentTurn = Turns.Player1;
            _simulationCam.Priority = -2;
            _cardPlayerObject.SetActive(true);

            return;
        }

        CurrentTurn = (CurrentTurn is Turns.Player1)
            ? Turns.Player2
            : Turns.Player1;

        // NOTE - Cam Transition
        _player1Cam.Priority = (int)CurrentTurn;
        EndTurnEvent?.Invoke();
        _turnsCount++;
    }

    public static void EndSimulation() => State = GameState.Turns;
}
