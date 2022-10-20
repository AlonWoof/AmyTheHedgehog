using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMarker : MonoBehaviour
{

    public float soundRange = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyMe", 0.5f);        
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }

}
