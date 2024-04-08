using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class TextureFucker : MonoBehaviour
	{

		public List<Texture> possibleTextures;

		List<Material> allMaterials;

		// Start is called before the first frame update
		void Start()
		{
			getAllMaterials();

			foreach (Material m in allMaterials)
			{
				flickerTexture(m);
			}
		}

		public void getAllMaterials()
		{
			allMaterials = new List<Material>();

			foreach (GameObject g in FindObjectsOfType<GameObject>())
			{
				if (g.layer == LayerMask.NameToLayer("Collision"))
				{
					foreach (Renderer r in g.GetComponentsInChildren<Renderer>())
					{
						foreach (Material m in r.materials)
							allMaterials.Add(m);
					}
				}
			}
		}

		// Update is called once per frame
		void Update()
		{
			
		}

		public void flickerAllTextures()
		{
			foreach (Material m in allMaterials)
			{
				//if (Random.Range(0, 3000) < 10)
				flickerTexture(m);

			}
		}

		public void flickerTexture(Material m)
		{
			m.SetTexture("_MainTex", possibleTextures[Random.Range(0, possibleTextures.Count)]);

			if (Random.Range(0, 100) < 50)
				m.SetColor("_Color", Color.Lerp(new Color(Random.Range(0.25f, 1.0f), Random.Range(0.25f, 1.0f), Random.Range(0.25f, 1.0f), Random.Range(0.25f, 1.0f)), Color.gray, 0.5f));
			else
				m.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 0.5f));

			if (Random.Range(0, 100) < 50)
				m.SetTextureScale("_MainTex", new Vector2(Random.Range(-3, 3), Random.Range(-3, 3)));
			else
				m.SetTextureScale("_MainTex", new Vector2(1.0f, 1.0f));
		}


	}
}
