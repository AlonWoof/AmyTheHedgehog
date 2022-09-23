using UnityEngine;
using System.Collections;

public class TimedDestroy : MonoBehaviour
{

    public GameObject deathFX;

    public float lifetime = 10.0f;
    public float maxLifetime = 10.0f;

    public bool randomRange = false;
    public bool destroyOnDisable = false;
    public bool infiniteTime = false;

	// Use this for initialization
	void Start ()
    {
	    if(randomRange)
        {
            lifetime = Random.Range(lifetime, maxLifetime);
        }
	}

    void OnDisable()
    {
        if (destroyOnDisable)
            GameObject.Destroy(gameObject);
    }

    public void instantDestroy()
    {
        lifetime = 0;
        infiniteTime = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //A ticking timebomb.

        if (lifetime > 0)
        {
            if(!infiniteTime)
                lifetime -= Time.deltaTime;
        }
        else
        {

            if (deathFX)
            {
                GameObject inst = GameObject.Instantiate(deathFX);
                inst.transform.position = transform.position;
                inst.transform.rotation = transform.rotation;
                inst.transform.localScale = transform.localScale;
            }

            GameObject.Destroy(gameObject);
        }
	}


}
