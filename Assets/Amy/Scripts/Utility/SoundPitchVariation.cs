using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPitchVariation : MonoBehaviour
{
    public bool playOnAwake = true;

    public float pitchMin = 1.0f;
    public float pitchMax = 1.0f;

    public List<AudioClip> clips;

    public AudioSource source;

    // Start is called before the first frame update
    void Awake()
    {
        if (!source)
            source = GetComponent<AudioSource>();

        if (playOnAwake)
            playRandomClip();
    }

    public void playRandomClip()
    {
        source.clip = clips[Random.Range(0, clips.Count - 1)];
        source.pitch = Random.Range(pitchMin, pitchMax);
        source.Play();
    }
}
