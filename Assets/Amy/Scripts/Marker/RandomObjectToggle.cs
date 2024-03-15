using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class RandomObjectToggle : MonoBehaviour
	{

		public List<GameObject> randomObjectList;

        private void Awake()
        {
			foreach(GameObject g in randomObjectList)
            {
				g.SetActive(false);
            }

			randomObjectList[Random.Range(0, randomObjectList.Count)].SetActive(true);
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
