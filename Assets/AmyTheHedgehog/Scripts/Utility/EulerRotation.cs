using UnityEngine;
using System.Collections;


public class EulerRotation : MonoBehaviour
{


    public Vector3 rotation;
    public float speed = 1.0f;

    Vector3 myRotation;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        myRotation = transform.localRotation.eulerAngles;

        myRotation += rotation * (Time.deltaTime * speed);

        transform.localRotation = Quaternion.Euler(myRotation);
	}


}
