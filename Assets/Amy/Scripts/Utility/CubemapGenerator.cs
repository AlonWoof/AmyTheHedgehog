using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;

//////////////////////////////////////
//     ©2024 Jennifer Haden         //
//////////////////////////////////////

public class CubemapGenerator : MonoBehaviour
{

	public Camera mCamera;

	public int resWidth = 512; 
	public int resHeight = 512;

	public Texture2D tex;
	public Cubemap cb;

	// Start is called before the first frame update
	void Start()
	{
	        
	}
	
	// Update is called once per frame
	void Update()
	{

	}

	public void generateCubemapTextures()
    {
		cb = new Cubemap(resWidth, TextureFormat.RGB24, 0);

		mCamera.RenderToCubemap(cb);
		AssetDatabase.CreateAsset(cb, "Assets/Amy/Materials/Cubemaps/" + SceneManager.GetActiveScene().name + "_cubemap.cubemap");
	}

}
