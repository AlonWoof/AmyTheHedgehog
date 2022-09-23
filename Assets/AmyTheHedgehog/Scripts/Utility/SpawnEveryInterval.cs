using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEveryInterval : MonoBehaviour
{

    public float interval = 3.0f;
    float timeLeft;

    public GameObject objectToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        timeLeft = Random.Range(interval * 0.25f,interval);
    }

    // Update is called once per frame
    void Update()
    {
        if(timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
        }
        else
        {
            spawnObject();
            timeLeft = interval;
        }
    }

    void spawnObject()
    {
        GameObject inst = GameObject.Instantiate(objectToSpawn, transform.position, transform.rotation);
    }
}
