using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	[System.Serializable]
	public class SuikaRadioSong
    {
		public string songTitle;
		public AudioClip clip;
    }

	public class SuikaRadio : MonoBehaviour
	{

		public AudioSource sound;
		public List<SuikaRadioSong> songs;
		public int currentSong = 0;

	    // Start is called before the first frame update
	    void Start()
	    {
			songs.Shuffle();
			sound.spatialBlend = 0.75f;
			sound.dopplerLevel = 0.0f;
			initSong();
		}
	
	    // Update is called once per frame
	    void Update()
	    {

			if (!sound.isPlaying)
				playNextSong();
	    }

		void playNextSong()
        {
			currentSong++;

			if(currentSong > songs.Count)
            {
				currentSong = 0;
				songs.Shuffle();
            }

			sound.time = 0.0f;
			sound.clip = songs[currentSong].clip;
			sound.Play();
        }

		void initSong()
        {
			//When radio is first created, we want a random position in the song.
			currentSong = -1;

			playNextSong();

			sound.time = Random.Range(0, sound.clip.length);
        }
	}
}
