using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    public SoundGroup[] _soundGroups;

    private Dictionary<string, AudioClip[]> _groupDictionary = new();

    private void Awake()
    {
        foreach (SoundGroup group in _soundGroups) {
            _groupDictionary.Add(group.groupID, group.group);
        }
    }

    public AudioClip GetClipFromName(string name)
    {
        if (_groupDictionary.ContainsKey(name)) {
            AudioClip[] sounds = _groupDictionary[name];

            return sounds[Random.Range(0, sounds.Length)];
        }

        Debug.LogWarning($"Sound: { name } not found!");

        return null;
    }

    [System.Serializable]
    public class SoundGroup
    {
        public string groupID;
        public AudioClip[] group;
    }
}