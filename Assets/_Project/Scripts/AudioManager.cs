using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance = null;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
                if (_instance == null)
                {
                    string goName = typeof(AudioManager).Name;
                    GameObject go = GameObject.Find(goName);

                    if (go == null)
                    {
                        go = new GameObject
                        {
                            name = goName
                        };
                    }

                    _instance = go.AddComponent<AudioManager>();
                }
            }

            return _instance;
        }
    }

    [SerializeField, Range(0f, 1f)] private float masterVolume = 1;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 1;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1;

    private int activeMusicIndex;
    private AudioSource[] musicSources;
    private SoundLibrary library;

    private void Awake()
    {
        musicVolume *= 0.1f;
        library = GetComponent<SoundLibrary>();
        musicSources = new AudioSource[2];
        for (int i = 0; i < 2; i++)
        {
            GameObject musicSource = new GameObject("MusicSource: " + (i + 1));
            musicSources[i] = musicSource.AddComponent<AudioSource>();
            musicSource.transform.parent = transform;
        }
    }

    public void PlayMusic(AudioClip clip, float fadeDuriation = 1)
    {
        activeMusicIndex = 1 - activeMusicIndex;
        musicSources[activeMusicIndex].clip = clip;
        musicSources[activeMusicIndex].Play();

        StartCoroutine(AnimateMusicCrossfade(fadeDuriation));
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolume * masterVolume);
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null) 
            AudioSource.PlayClipAtPoint(clip, transform.position, sfxVolume * masterVolume);
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(library.GetClipFromName(soundName), pos);
    }

    public void PlaySound(string soundName)
    {
        PlaySound(library.GetClipFromName(soundName), transform.position);
    }

    private IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.unscaledDeltaTime * 1 / duration;
            musicSources[activeMusicIndex].volume = Mathf.Lerp(0, musicVolume * masterVolume, percent);
            musicSources[1 - activeMusicIndex].volume = Mathf.Lerp(musicVolume * masterVolume, 0, percent);
            yield return null;
        }

    }
}
