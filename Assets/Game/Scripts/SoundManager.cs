using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundManager>();
            }
            return _instance;
        }
    }
    public SoundAudioClip[] soundAudioClipArray;
    private List<GameObject> audioSources = new List<GameObject>();

    private void Start()
    {
        DontDestroyOnLoad(this);
        SpawnSound();
    }

    [System.Serializable]
    public class SoundAudioClip
    {
        public Sound sound;
        public AudioClip audioClip;
    }

    public void SpawnSound()
    {
        for (int i = 0; i < soundAudioClipArray.Length; i++)
        {
            GameObject soundGameObject = new GameObject(soundAudioClipArray[i].sound.ToString());
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            soundGameObject.transform.parent = this.transform;
            audioSources.Add(soundGameObject);
        }
    }

    public void PlaySound(Sound sound)
    {
        if (audioSources != null)
        {
            foreach (GameObject audioSource in audioSources)
            {
                if (audioSource.name == sound.ToString())
                {
                    var playAudio = audioSource.GetComponent<AudioSource>();
                    if (playAudio.clip == null)
                    {
                        playAudio.clip = GetAudioClip(sound);
                    }
                    SoundChanger(playAudio, sound);
                }
            }
        }
    }

    private void SoundChanger(AudioSource audioSource, Sound sound)
    {
        switch (sound)
        {
            default:
                audioSource.Play();
                break;
        }
    }

    private AudioClip GetAudioClip(Sound sound)
    {
        foreach (SoundAudioClip soundAudioClip in soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        return null;
    }
}
public enum Sound
{
    BlockDestroy,
    Win
}