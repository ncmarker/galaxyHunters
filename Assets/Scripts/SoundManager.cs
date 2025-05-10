using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private AudioSource soundEffectSource;
    private AudioSource backgroundMusicSource;
    private Dictionary<string, AudioClip> soundClips = new Dictionary<string, AudioClip>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        soundEffectSource = gameObject.AddComponent<AudioSource>();

        backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        backgroundMusicSource.loop = true;

        LoadAllSounds();
    }

    // loads all audio clips from resources folder
    private void LoadAllSounds()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds");
        foreach (AudioClip clip in clips)
        {
            soundClips[clip.name] = clip; 
        }
    }

    // play one-off sound effects
    public void PlaySound(string soundName)
    {
        if (GameManager.Instance && GameManager.Instance.GetIsPaused()) return;
        Debug.Log("PLAY SOUND: " + soundName);

        if (soundClips.TryGetValue(soundName, out AudioClip clip))
        {
            soundEffectSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Sound not found: " + soundName);
        }
    }

    // play background music
    public void PlayBackgroundMusic(string musicName)
    {
        if (soundClips.TryGetValue(musicName, out AudioClip clip))
        {
            backgroundMusicSource.clip = clip;
            backgroundMusicSource.Play();
        }
        else
        {
            Debug.LogWarning("Background music not found: " + musicName);
        }
    }

    public void StopBackgroundMusic()
    {
        backgroundMusicSource.Stop();
    }
}
