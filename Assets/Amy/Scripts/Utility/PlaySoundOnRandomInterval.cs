using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jennifer Haden */
namespace Forest
{

	public class PlaySoundOnRandomInterval : MonoBehaviour
	{
        public AudioSource source;
        public List<AudioClip> clips;

        public float pitchMin = 1.0f;
        public float pitchMax = 1.0f;

        public float timeMin = 1.0f;
        public float timeMax = 30.0f;

        float timeLeft = 0.0f;

        // Start is called before the first frame update
        void Start()
    	{
            if (!source)
                source = GetComponent<AudioSource>();

            timeLeft = Random.Range(timeMin, timeMax);
        }

    	// Update is called once per frame
    	void Update()
    	{
            timeLeft -= Time.deltaTime;

    	    if(timeLeft < 0.0f)
            {
                playRandomClip();
                timeLeft = Random.Range(timeMin, timeMax);
            }
    	}

        public void playRandomClip()
        {
            source.clip = clips[Random.Range(0, clips.Count - 1)];
            source.pitch = Random.Range(pitchMin, pitchMax);
            source.Play();
        }
    }

}
