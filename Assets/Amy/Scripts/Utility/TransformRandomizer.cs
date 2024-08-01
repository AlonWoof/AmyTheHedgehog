using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//     ©2024 Jennifer Haden         //
//////////////////////////////////////

public class TransformRandomizer : MonoBehaviour
{
    public Vector3 minScale = Vector3.one;
    public Vector3 maxScale = Vector3.one;

    public bool randomizeScale = false;
    public bool randomizeYrot = false;

	public void randomizeTransforms()
    {
        foreach(Transform t in GetComponentsInChildren<Transform>())
        {
            if (t == transform)
                continue;

            if (t.transform.parent != transform)
                continue;

            if (randomizeScale)
            {
                Vector3 newScale = new Vector3(Random.Range(minScale.x, maxScale.x),
                    Random.Range(minScale.y, maxScale.y),
                    Random.Range(minScale.z, maxScale.z));

                t.localScale = newScale;
            }

            if(randomizeYrot)
            {
                Vector3 euler = t.transform.rotation.eulerAngles;
                euler.y = Random.Range(0, 360.0f);

                t.transform.rotation = Quaternion.Euler(euler);
            }
        }
    }
}
