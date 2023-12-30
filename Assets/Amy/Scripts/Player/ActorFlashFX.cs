using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class ActorFlashFX : MonoBehaviour
	{

		List<Material> mats;

		public Color mColor;

        private void Awake()
        {
			mColor = Color.black;
			mColor.a = 0.0f;

			mats = new List<Material>();

            foreach(Renderer r in GetComponentsInChildren<Renderer>())
            {
				foreach(Material m in r.materials)
                {
					mats.Add(m);
                }
            }
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
			updateColor();

		}

		void updateColor()
        {
			foreach(Material m in mats)
            {
				m.SetColor("_FlashColor", mColor);
            }
        }
	}
}
