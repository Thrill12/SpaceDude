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

        if (musicSource.isPlaying)
        {
            isPlaying = true;
        }

        //Loops through all the songs in the list, making sure it doesn't play the same song
        //Twice in a row if there is more than 1 song.
        if (songs.Count > 1)
        {
            int songindex = Random.Range(0, songs.Count);
            musicSource.clip = songs[songindex];

            if(songindex == lastSong)
            {
                return;
            }
            else
            {
                musicSource.Play();
                lastSong = songindex;
            }
        }
        else
        {
            musicSource.clip = songs[0];
            musicSource.Play();
        }
    }
}
