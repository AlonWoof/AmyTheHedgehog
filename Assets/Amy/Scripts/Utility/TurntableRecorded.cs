using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class TurntableRecorded : MonoBehaviour
	{
		public int numFrames = 60;


	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {

			if (Input.GetKeyDown(KeyCode.F11))
				Timing.RunCoroutine(doCapture());
		}

		IEnumerator<float> doCapture()
        {
			float rotSeg = 360.0f / ((float)numFrames);


			float initialRot = transform.rotation.eulerAngles.y;
			float rot = initialRot;

			int frame = 0;

			while (rot < (initialRot + 360.0f))
			{
				transform.rotation = Quaternion.Euler(0, rot, 0);

				string screenLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
				screenLocation += "/TurnTable/";

				ScreenCapture.CaptureScreenshot(screenLocation + frame + ".png", 2);

				rot += rotSeg;
				frame++;

				yield return 0f;
			}
		}


	}
}
