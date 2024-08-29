using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
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

		public AudioSource source;

	    // Start is called before the first frame update
	    void Start()
	    {
			source = gameObject.GetComponent<AudioSource>();
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		public void playRandomClip(AudioClip[] clipList, bool canInterrupt = false)
        {
			int rng = Random.Range(0, clipList.Length);

			if (source.isPlaying && !canInterrupt)
				return;

			source.Stop();
			source.PlayOneShot(clipList[rng]);
        }


	}
}
