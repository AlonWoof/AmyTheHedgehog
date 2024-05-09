using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class NowSavingText : MonoBehaviour
	{
		public Text txt;
		[Range(0, 10)]
		public int progress = 0;
		float wait = 0.1f;
		string fullMsg = "NOW SAVING";

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        if(wait > 0.0f)
            {
				wait -= Time.unscaledDeltaTime;
				return;
            }

			int prg = Mathf.Clamp(progress, 0, fullMsg.Length);

			txt.text = "<color=#5b97df>" + fullMsg.Substring(0, prg) + "</color>" + fullMsg.Substring(prg, fullMsg.Length - prg);

			wait = Random.Range(0.05f, 0.2f);
			
			if (progress > fullMsg.Length)
				Destroy(gameObject);

			progress++;
		}
	}
}
