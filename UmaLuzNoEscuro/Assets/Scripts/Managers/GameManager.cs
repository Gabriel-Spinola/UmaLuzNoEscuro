using UnityEngine;
using Cinemachine;
using System;
using Unity.AI.Navigation;
using System.Threading.Tasks;

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
    public static GameState TurnBeforePause = GameState.Turns;

    [Header("UI References")]
    [SerializeField] private GameObject _cardPlayerObject;
    [SerializeField] private GameObject _pauseMenu;

    [Header("Lightning")]
    [SerializeField] private GameObject _turn1Lightning;
    [SerializeField] private GameObject _turn2Lightning;
    [SerializeField] private GameObject _simulationLightning;

    [Header("AI")]
    [SerializeField] private NavMeshSurface _navSurface;
    [SerializeField] private GameObject[] _destroyOnTurnsPhase;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera _simulationCam;
    [SerializeField] private CinemachineVirtualCamera _player1Cam, _player2Cam;

    private int _turnsCount = 0;

    public static uint GlobalTurnsCount = 0;

    private void Awake()
    {
        _player1Cam.Priority = 1;
        _player2Cam.Priority = 0;
        _simulationCam.Priority = -2;

        _turn1Lightning.SetActive(CurrentTurn is Turns.Player1);
        _turn2Lightning.SetActive(CurrentTurn is Turns.Player2);

        _pauseMenu.SetActive(false);
    }

    private async void Update()
    {
        GlobalTurnsCount++;

        if (DeckPlayer.IsLightningAttackHappening)
        {
            await ActiveAndDeactivateLights();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && State == GameState.Paused)
        {
            State = TurnBeforePause;
            Time.timeScale = 1f;
            _pauseMenu.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && State != GameState.Paused)
        {
            TurnBeforePause = State;
            State = GameState.Paused;
            Time.timeScale = 0f;

            _pauseMenu.SetActive(true);

            return;
        }

        if (_turnsCount >= 2)
        {
            State = GameState.Simulating;

            _simulationCam.Priority = 2;
        }

        if (State is GameState.Simulating && _cardPlayerObject.activeSelf)
        {
            _cardPlayerObject.SetActive(false);
        }

        switch (State)
        {
            case GameState.Turns:
                foreach (var obj in _destroyOnTurnsPhase)
                {
                    if (obj.activeSelf)
                    {
                        continue;
                    }

                    obj.SetActive(true);
                    _navSurface.BuildNavMesh();
                }
                break;

            case GameState.Simulating:
                {
                    foreach (var obj in _destroyOnTurnsPhase)
                    {
                        if (!obj.activeSelf)
                        {
                            continue;
                        }

                        obj.SetActive(false);
                        _navSurface.BuildNavMesh();
                    }
                }
                break;
        }
    }

    public async void EndTurn()
    {
        if (State is GameState.Simulating)
        {
            EndSimulation();


            _turnsCount = 0;
            CurrentTurn = Turns.Player1;
            _simulationCam.Priority = -2;
            _cardPlayerObject.SetActive(true);
            await TurnsLightsOn();
            return;
        }

        CurrentTurn = (CurrentTurn is Turns.Player1)
            ? Turns.Player2
            : Turns.Player1;

        // NOTE - Cam Transition
        _player1Cam.Priority = (int)CurrentTurn;
        EndTurnEvent?.Invoke();
        _turnsCount++;

        await TurnsLightsOn();
    }

    private async Task TurnsLightsOn()
    {
        await Task.Delay(600);

        _turn1Lightning.SetActive(CurrentTurn is Turns.Player1);
        _turn2Lightning.SetActive(CurrentTurn is Turns.Player2);

        if (State is GameState.Simulating)
        {
            _simulationLightning.SetActive(true);

            _turn1Lightning.SetActive(false);
            _turn2Lightning.SetActive(false);
        }
        else
        {
            _simulationLightning.SetActive(false);
        }
    }

    public async Task ActiveAndDeactivateLights()
    {
        _turn1Lightning.SetActive(false);
        _turn2Lightning.SetActive(false);
        _simulationLightning.SetActive(false);

        await Task.Delay(1000);

        _turn1Lightning.SetActive(true);
        _turn2Lightning.SetActive(true);
        _simulationLightning.SetActive(true);
        
        DeckPlayer.IsLightningAttackHappening = false;
    }

    public static void EndSimulation() => State = GameState.Turns;
}
