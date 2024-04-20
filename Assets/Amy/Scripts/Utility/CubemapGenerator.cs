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

	public int resWidth = 512; 
	public int resHeight = 512;

	public Cubemap cb;
	public Texture2D normalMap;
	public Camera cam;
	
	// Start is called before the first frame update
	void Start()
	{
	        
	}
	
	// Update is called once per frame
	void Update()
	{

	}

	void CreatePortalMaterial()
	{
#if UNITY_EDITOR
		// Create a simple material asset

		Cubemap cb = (Cubemap)AssetDatabase.LoadAssetAtPath("Assets/Amy/Materials/Cubemaps/" + SceneManager.GetActiveScene().name + "_cubemap.cubemap", typeof(Cubemap));

		Material material = new Material(Shader.Find("AlonWoof/Amy/DoorPortal"));
		material.SetTexture("_MainTex", cb);
		material.mainTexture = cb;
		material.SetTexture("_Normal", normalMap);
		material.SetTexture("_Normal1", normalMap);


		AssetDatabase.CreateAsset(material, "Assets/Amy/Materials/Cubemaps/" + SceneManager.GetActiveScene().name + "_portal" + ".mat");

		// Print the path of the created asset
		Debug.Log(AssetDatabase.GetAssetPath(material));
	#endif
	}


	public void generateCubemapTextures()
    {
	#if UNITY_EDITOR
		cb = new Cubemap(resWidth, TextureFormat.RGB24, 0);
		cam.gameObject.SetActive(true);


		cam.transform.position = transform.position;
		cam.transform.rotation = transform.rotation;

		//cam.tag = "";
		cam.RenderToCubemap(cb);
		

		AssetDatabase.CreateAsset(cb, "Assets/Amy/Materials/Cubemaps/" + SceneManager.GetActiveScene().name + "_cubemap.cubemap");

		cam.gameObject.SetActive(false);
		CreatePortalMaterial();

	#endif
	}
	
}
