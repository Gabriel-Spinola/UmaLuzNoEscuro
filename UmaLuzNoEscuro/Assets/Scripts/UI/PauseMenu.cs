
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _GUI;

    private void OnEnable()
    {
        _GUI.SetActive(false);
    }

    private void OnDisable()
    {
        _GUI.SetActive(true);
    }

    public void OnStart()
    {
        GameManager.State = GameManager.TurnBeforePause;
    }

    public void OnOpenMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
