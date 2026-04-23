using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class YumeNudeToggler : MonoBehaviour
	{

		public GameObject clothedBody;
		public GameObject nudeBody;

		public float nudeChance = 0.1f;

	    // Start is called before the first frame update
	    void Start()
	    {
			changeBody(false);
			rollRandomChance();

		}

        private void OnEnable()
        {
			rollRandomChance();
		}

        void rollRandomChance()
        {

			if (Random.Range(0, 1.0f) < nudeChance)
				changeBody(true);
			else
				changeBody(false);
		}
	
	    // Update is called once per frame
	    void Update()
	    {
			if (Input.GetKeyDown(KeyCode.F6))
				rollRandomChance();

		}

		void changeBody(bool nakie)
        {
			clothedBody.SetActive(!nakie);
			nudeBody.SetActive(nakie);
        }
	}
}
