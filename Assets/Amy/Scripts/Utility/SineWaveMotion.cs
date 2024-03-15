using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jennifer Haden */
public class SineWaveMotion : MonoBehaviour
{
    public float waveStrength = 1.0f;
    public float waveSpeed = 1.0f;

    Vector3 homePos;

    public bool useFixedUpdate = true;

    private void Start()
    {
        homePos = transform.position;
    }

    private void Update()
    {
        if (useFixedUpdate)
            return;

        float motion = Mathf.Sin(Time.time * waveSpeed);

        transform.position = homePos + Vector3.up * (motion * waveStrength);


    }

    private void FixedUpdate()
    {
        if (!useFixedUpdate)
            return;

        float motion = Mathf.Sin(Time.fixedTime * waveSpeed);

        transform.position = homePos + Vector3.up * (motion * waveStrength);
    }


}

