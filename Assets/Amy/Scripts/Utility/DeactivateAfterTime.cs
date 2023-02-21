using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateAfterTime : MonoBehaviour
{

    public float timeToDeactivate = 1.0f;

    float timeLeft = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        timeLeft = timeToDeactivate;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0)
            timeLeft -= Time.deltaTime;

        if (timeLeft <= 0)
            gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        timeLeft = timeToDeactivate;
    }
}
