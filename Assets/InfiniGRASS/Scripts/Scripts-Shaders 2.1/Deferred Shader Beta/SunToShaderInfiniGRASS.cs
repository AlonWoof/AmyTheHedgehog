using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SunToShaderInfiniGRASS : MonoBehaviour {

	public List<Material> GrassMaterial = new  List<Material>();
	public Transform sunLight;
	public Light SunLight;

	bool useDirection;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (SunLight == null) {
			SunLight = sunLight.gameObject.GetComponent<Light> ();
		} else {
			for (int i = 0; i < GrassMaterial.Count; i++) {
				if (useDirection || Camera.main == null) {
					GrassMaterial [i].SetVector ("_LightDir", sunLight.position);
					GrassMaterial [i].SetVector ("_LightColor", new Vector4 (SunLight.color.r, SunLight.color.g, SunLight.color.b, SunLight.intensity));
				} else {
					GrassMaterial [i].SetVector ("_LightDir", Camera.main.transform.position - 10000 * sunLight.forward);
					GrassMaterial [i].SetVector ("_LightColor", new Vector4 (SunLight.color.r, SunLight.color.g, SunLight.color.b, SunLight.intensity));
				}
			}
		}
	}
}
