using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePrefabIG : MonoBehaviour
{

    public GameObject prefab;
    public GameObject prefabPREGROWN;
    public float forwardLength = 30;
    public float upLength = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 posFrontCamera = Camera.main.transform.position + new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z)* forwardLength + Vector3.up * upLength;
            Instantiate(prefab, posFrontCamera, Quaternion.identity);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 posFrontCamera = Camera.main.transform.position + new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z) * forwardLength + Vector3.up * upLength;
            Instantiate(prefabPREGROWN, posFrontCamera, Quaternion.identity);
        }
    }
}
