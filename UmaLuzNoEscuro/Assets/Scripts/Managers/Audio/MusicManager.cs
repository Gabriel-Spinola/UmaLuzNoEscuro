using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private SceneTheme[] _sceneThemes;

    private string _sceneName;
    private AudioClip _clipHolder = null;
    private float _fadeHolder = 0f;
    private float _pitchHolder = 0f;
    private bool _restart = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Shows a Unity warning, but doesn't cause any error.
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
        {
            string newSceneName = scene.name;

            if (newSceneName != _sceneName)
            {
                _sceneName = newSceneName;

                Invoke(nameof(PlayMusic), .2f);
            }
            else
            {
                Debug.LogWarning($"Theme: {newSceneName} not found!");
            }
        };
    }

    private void Start()
    {
        InvokeRepeating(nameof(Restart), 0f, _clipHolder.length);
    }

    void PlayMusic()
    {
        AudioClip clipToPlay = null;
        float fadeDuration = 0f;
        float pitch = 0f;

        for (int i = 0; i < _sceneThemes.Length; i++)
        {
            if (_sceneName == _sceneThemes[i].name)
            {
                clipToPlay = _sceneThemes[i].theme;
                fadeDuration = _sceneThemes[i].fadeDuration;
                pitch = _sceneThemes[i].pitch;
            }
        }

        if (clipToPlay != null)
        {
            _clipHolder = clipToPlay;
            _fadeHolder = fadeDuration;
            _pitchHolder = pitch;
            _restart = true;

            AudioManager._I.PlayMusic(clipToPlay, fadeDuration, pitch);

            Invoke(nameof(PlayMusic), clipToPlay.length);
        }
    }

    public void Restart()
    {
        if (_restart)
        {
            AudioManager._I.PlayMusic(_clipHolder, _fadeHolder, _pitchHolder);

            Invoke(nameof(PlayMusic), _clipHolder.length);

            _restart = false;
        }
    }

    [System.Serializable]
    public class SceneTheme
    {
        public string name;
        public AudioClip theme;
        public float fadeDuration;
        public float pitch;
    }
}