using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

public class TextureListAnimation : MonoBehaviour
{

	public int framerate = 30;
	public Texture[] textures;

	int currentFrame = 0;
	float timeSinceLastUpdate = 0.0f;

	Material mat;

	// Start is called before the first frame update
	void Start()
	{
		mat = GetComponent<Renderer>().materials[0];
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		if (!mat)
			return;



		timeSinceLastUpdate += Time.deltaTime;

		if(timeSinceLastUpdate > (1.0f/(float)framerate))
		{
			currentFrame++;
			timeSinceLastUpdate = 0.0f;

			if (currentFrame > (textures.Length - 1))
				currentFrame = 0;

			mat.SetTexture("_MainTex", textures[currentFrame]);
		}

		
	}
}
