using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jason Haden */
namespace Amy
{

	public class PlayerVoice : MonoBehaviour
	{

        public AudioClip[] basicAttack;
        public AudioClip[] powerAttack;

        public AudioClip[] smallPain;
        public AudioClip[] largePain;

        public AudioClip[] falling;

        public AudioClip[] jumping;

        public AudioSource voiceSource;

        public float pitchMin = 1.0f;
        public float pitchMax = 1.0f;

    	// Start is called before the first frame update
    	void Start()
    	{
    	    
    	}

    	// Update is called once per frame
    	void Update()
    	{
    	    
    	}

        public void playVoice(AudioClip[] voices, bool canInterrupt = false)
        {
            if (!canInterrupt && voiceSource.isPlaying)
                return;

            voiceSource.clip = voices[Random.Range(0, voices.Length - 1)];
            voiceSource.pitch = Random.Range(pitchMin, pitchMax);
            voiceSource.Play();
        }
	}

}
