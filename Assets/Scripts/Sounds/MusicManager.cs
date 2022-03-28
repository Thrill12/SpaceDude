using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioClip menuTheme;

    public List<AudioClip> songs = new List<AudioClip>();

    public bool shouldLoop;

    private bool isPlaying;
    public AudioSource musicSource;
    public bool overrideSound = true;

    private int lastSong;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        FadeToSong(menuTheme, 0);
    }

    private void Update()
    {
        if (musicSource.isPlaying)
        {
            isPlaying = true;

            if(musicSource.clip == menuTheme)
            {
                shouldLoop = true;
                musicSource.loop = true;
            }
            else
            {
                shouldLoop = false;
                musicSource.loop = false;
            }
        }
        else
        {
            isPlaying = false;
        }

        if (isPlaying) return;

        if (overrideSound) return;

        if (shouldLoop)
        {
            FadeToSong(musicSource.clip, 0);
            return;
        }

        //Loops through all the songs in the list, making sure it doesn't play the same song
        //Twice in a row if there is more than 1 song.
        if (songs.Count > 1)
        {
            int songindex = Random.Range(0, songs.Count + 1);
            FadeToSong(songs[songindex], 1);

            if(songindex == lastSong)
            {
                return;
            }
            else
            {
                lastSong = songindex;
            }
        }
        else
        {
            FadeToSong(songs[0], 1);
        }
    }

    public void FadeMusic(float duration, float targetVolume)
    {
        StartCoroutine(FadeAudioSource.StartFade(musicSource, duration, targetVolume));
    }

    public void FadeToSong(AudioClip song, float duration)
    {
        overrideSound = true;
        FadeMusic(duration, 0);
        musicSource.clip = song;
        musicSource.Play();
        FadeMusic(duration, 1);
        overrideSound = false;
    }
}

public static class FadeAudioSource
{
    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}
