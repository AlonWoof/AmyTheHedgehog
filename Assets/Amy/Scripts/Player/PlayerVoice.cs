using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

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
        public AudioClip[] die;
        public AudioClip[] drown;

        public AudioClip[] jumping;
        public AudioClip[] altJumping;

        public AudioSource voiceSource;

        public float pitchMin = 1.0f;
        public float pitchMax = 1.0f;

    	// Start is called before the first frame update
    	void Start()
    	{
    	    if(!voiceSource)
            {
                GameObject inst = new GameObject("Voice");
                inst.transform.SetParent(gameObject.transform);
                inst.transform.position = transform.position + Vector3.up * 0.65f;

                voiceSource = inst.AddComponent<AudioSource>();
                voiceSource.outputAudioMixerGroup = GameManager.Instance.systemData.AUDIO_Group_Voice;
                voiceSource.spatialBlend = 0.0f;
            }
    	}

    	// Update is called once per frame
    	void Update()
    	{
    	    
    	}

        public void playVoice(AudioClip[] voices, bool canInterrupt = false)
        {
            if (!canInterrupt && voiceSource.isPlaying)
                return;

            if (voices == null || voices.Length == 0)
                return;

            voiceSource.clip = voices[Random.Range(0, voices.Length)];
            voiceSource.pitch = Random.Range(pitchMin, pitchMax);
            voiceSource.Play();
        }

        public void playVoiceDelayed(float delay, AudioClip[] voices, bool canInterrupt = false)
        {
            Timing.RunCoroutine(doDelayedVoice(delay, voices, canInterrupt));

        }

        IEnumerator<float> doDelayedVoice(float delay, AudioClip[] voices, bool canInterrupt = false)
        {
            yield return Timing.WaitForSeconds(delay);

            playVoice(voices, canInterrupt);
        }
	}

}
