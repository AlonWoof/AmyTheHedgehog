using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2022 Jason Haden */
namespace Amy
{

	public class BallModeModel : MonoBehaviour
	{


        public List<GameObject> mainModels;
        public GameObject ballModeModel;

        bool wasBallMode = true;
        public bool isBallMode = false;

    	// Start is called before the first frame update
    	void Start()
    	{
    	    
    	}

    	// Update is called once per frame
    	void Update()
    	{
    	    if(isBallMode != wasBallMode)
            {
                foreach(GameObject g in mainModels)
                {
                    g.SetActive(!isBallMode);
                }

                ballModeModel.SetActive(isBallMode);

                wasBallMode = isBallMode;
            }
    	}
	}

}
