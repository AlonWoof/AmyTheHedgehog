using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////


public class SoundEffectSpawner : MonoBehaviour
{
	public AudioClip mClip;
	public AudioSource src;
	float timeLeft = 0.0f;

	static SoundEffectSpawner Spawn(AudioClip clip)
    {
		GameObject inst = new GameObject(clip.name);

		SoundEffectSpawner ret = inst.AddComponent<SoundEffectSpawner>();


		ret.mClip = clip;
		ret.src = inst.AddComponent<AudioSource>();

		return ret;
    }

	// Start is called before the first frame update
	void Start()
	{
		if (!mClip)
			return;

		timeLeft = mClip.length * 1.1f;


		src.PlayOneShot(mClip);
		
	}
	
	// Update is called once per frame
	void Update()
	{
		timeLeft -= Time.deltaTime;

		if(timeLeft < 0.0f)
        {
			Destroy(gameObject);
        }
	}

	void playClip()
    {

    }
}
