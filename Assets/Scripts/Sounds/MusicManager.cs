using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public List<AudioClip> songs = new List<AudioClip>();

    private bool isPlaying;
    public AudioSource musicSource;

    private int lastSong;

    private void Update()
    {
        if (isPlaying) return;

        if (songs.Count > 1)
        {

            int songindex = Random.Range(0, songs.Count);
            musicSource.clip = songs[songindex];

            if(songindex == lastSong)
            {
                isPlaying = false;
                return;
            }
            else
            {
                isPlaying=true;
                musicSource.Play();
                lastSong = songindex;
            }
        }
        else
        {
            isPlaying = true;
            musicSource.clip = songs[0];
            musicSource.Play();
        }
    }
}
