using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public bool trigger = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(trigger)
        {
            spawnPrefab();
            trigger = false;
        }
    }

    public void spawnPrefab()
    {
        GameObject inst = GameObject.Instantiate(prefabToSpawn, transform.position, transform.rotation);
    }    
}
