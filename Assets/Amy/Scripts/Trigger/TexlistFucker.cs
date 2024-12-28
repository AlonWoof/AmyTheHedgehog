using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class TexlistFucker : MonoBehaviour
	{
		public List<Material> worldMaterials;
		public List<Texture> worldTexlists;

	    // Start is called before the first frame update
	    void Start()
	    {
			populateWorldTexlist();
			fuckTexlists();
		}

		void populateWorldTexlist()
        {
			if (worldTexlists == null)
				worldTexlists = new List<Texture>();

			if (worldMaterials == null)
				worldMaterials = new List<Material>();

			worldTexlists.Clear();
			worldMaterials.Clear();


			foreach(Renderer r in FindObjectsOfType<Renderer>())
            {
				int rng = Random.Range(0, 100);

				if (rng > 50)
					continue;

				foreach(Material m in r.materials)
				{
					Texture t = m.mainTexture;

					if (t != null)
					{
							worldTexlists.Add(t);
							worldMaterials.Add(m);
					}
				}

            }
        }

		void fuckTexlists()
        {

			worldTexlists.Shuffle();
			int count = 0;

			foreach(Material m in worldMaterials)
			{
				m.SetTexture("_MainTex", worldTexlists[count]);
				count++;

				if (count > worldTexlists.Count)
					count = 0;
			}
        }
	
	}
}
