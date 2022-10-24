using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotVoices : MonoBehaviour
{

    public AudioClip[] voices_alertModeStart;
    public AudioClip[] voices_searchModeStart;
    public AudioClip[] voices_patrolModeResume;


    public AudioClip[] voices_searchTaunts;

    public AudioSource mVoice;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    public void playVoice(AudioClip[] voices)
    {
        int r = Random.Range(0, voices.Length);

        mVoice.PlayOneShot(voices[r]);
    }

    public void playPatrolModeVoice()
    {
        mVoice.spatialBlend = 0.5f;
        playVoice(voices_patrolModeResume);
    }

    public void playAlertModeVoice()
    {
        mVoice.spatialBlend = 0.5f;
        playVoice(voices_alertModeStart);
    }

    public void playSearchModeVoice()
    {
        mVoice.spatialBlend = 0.5f;
        playVoice(voices_searchModeStart);
    }

    public void playSearchModeTaunt()
    {
        mVoice.spatialBlend = 1.0f;
        playVoice(voices_searchTaunts);
    }
}
