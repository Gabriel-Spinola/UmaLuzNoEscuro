using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _credits;
    [SerializeField] private GameObject _mainMenu;

    public void OnStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ToggleCredits()
    {
        _credits.SetActive(!_credits.activeSelf);
        _mainMenu.SetActive(!_mainMenu.activeSelf); 
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
