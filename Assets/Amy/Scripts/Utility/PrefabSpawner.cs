using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnPrefab()
    {
        GameObject inst = GameObject.Instantiate(prefabToSpawn, transform.position, transform.rotation);
    }    
}
