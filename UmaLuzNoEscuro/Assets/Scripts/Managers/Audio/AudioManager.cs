using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://youtu.be/C65ExBy6WPA
public class AudioManager : MonoBehaviour
{
    public static AudioManager _I = null;

    public enum AudioChannel
    {
        Master,
        SFX,
        Music
    }

    public float MasterVolumePercent { get; private set; }
    public float MusicVolumePercent { get; private set; }
    public float SFXVolumePercent { get; private set; }

    private Dictionary<string, bool> _isFadedDictionary = new Dictionary<string, bool>();

    private AudioSource[] _musicSources = null;
    private AudioSource _SFX2DSource = null;
    private SoundLibrary _library = null;

    private int _activeMusicSource;

    private void Awake()
    {
        if (_I != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _I = this;

            DontDestroyOnLoad(gameObject);

            _library = GetComponent<SoundLibrary>();
            _musicSources = new AudioSource[2];

            for (int i = 0; i < _musicSources.Length; i++)
            {
                GameObject newMusicSource = new GameObject($"Music Source {i + 1}");

                _musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }

            foreach (AudioSource music in _musicSources)
            {
                _isFadedDictionary.Add(music.name, false);
            }

            GameObject newSFX2DSource = new GameObject("2D Sound Effect Source");

            _SFX2DSource = newSFX2DSource.AddComponent<AudioSource>();
            newSFX2DSource.transform.parent = transform;

            MasterVolumePercent = PlayerPrefs.GetFloat("master vol", 1f);
            SFXVolumePercent = PlayerPrefs.GetFloat("sfx vol", 1f);
            MusicVolumePercent = PlayerPrefs.GetFloat("music vol", 1f);
        }
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                MasterVolumePercent = volumePercent;
                break;

            case AudioChannel.SFX:
                SFXVolumePercent = volumePercent;
                break;

            case AudioChannel.Music:
                MusicVolumePercent = volumePercent;
                break;
        }

        for (int i = 0; i < _musicSources.Length; i++)
        {
            if (!_isFadedDictionary[_musicSources[i].name])
            {
                _musicSources[i].volume = MusicVolumePercent * MasterVolumePercent;
            }
        }

        PlayerPrefs.SetFloat("master vol", MasterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", SFXVolumePercent);
        PlayerPrefs.SetFloat("music vol", MusicVolumePercent);
        PlayerPrefs.Save();
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1f, float pitch = 1f)
    {
        _activeMusicSource = 1 - _activeMusicSource;

        _musicSources[_activeMusicSource].clip = clip;
        _musicSources[_activeMusicSource].pitch = pitch;
        _musicSources[_activeMusicSource].priority = 5;
        _musicSources[_activeMusicSource].Play();

        StartCoroutine(MusicCrossfade(fadeDuration));
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, SFXVolumePercent * MasterVolumePercent);
        }
        else
        {
            Debug.LogWarning($"Can't Play \"{clip.name}\" Audio Clip");
        }
    }

    public void PlaySound(string clip, Vector3 pos)
    {
        PlaySound(_library.GetClipFromName(clip), pos);
    }

    public void PlaySound2D(string soundName, float pitch = 1f, int priority = 128)
    {
        _SFX2DSource.pitch = pitch;
        _SFX2DSource.priority = priority;
        _SFX2DSource.PlayOneShot(_library.GetClipFromName(soundName), SFXVolumePercent * MasterVolumePercent);
    }

    private IEnumerator MusicCrossfade(float duration)
    {
        float percent = 0f;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;

            _musicSources[_activeMusicSource].volume = Mathf.Lerp(0f, MusicVolumePercent * MasterVolumePercent, percent);
            _musicSources[1 - _activeMusicSource].volume = Mathf.Lerp(MusicVolumePercent * MasterVolumePercent, 0f, percent);

            _isFadedDictionary[_musicSources[_activeMusicSource].name] = false;
            _isFadedDictionary[_musicSources[1 - _activeMusicSource].name] = true;

            yield return null;
        }
    }
}