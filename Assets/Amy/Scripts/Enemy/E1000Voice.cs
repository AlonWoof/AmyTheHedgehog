using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class E1000Voice : MonoBehaviour
	{
		public AudioClip[] SpottedVoice;
		public AudioClip[] PursuitVoice;
		public AudioClip[] SearchVoice;

		public AudioClip[] DamageVoice;
		public AudioClip[] DestroyedVoice;

		AudioSource voice;

	    // Start is called before the first frame update
	    void Start()
	    {
			voice = gameObject.GetComponent<AudioSource>();
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		public void playRandomClip(AudioClip[] clipList, bool canInterrupt = false)
        {
			int rng = Random.Range(0, clipList.Length);

			if (voice.isPlaying && !canInterrupt)
				return;

			voice.Stop();
			voice.PlayOneShot(clipList[rng]);
        }
	}
}
