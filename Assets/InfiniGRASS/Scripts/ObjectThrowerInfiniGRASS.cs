using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Artngame.INfiniDy;

public class ObjectThrowerInfiniGRASS : MonoBehaviour {

	public GameObject ObjectToThrow;
	public InfiniGRASSManager GrassManager;//provide new hero as objecles approach grass

	// Use this for initialization
	void Start () {
	
	}
	public AudioClip Chop;
	public AudioClip Bash;
	public AudioSource Basher;

	public float Distance = 25;

	float timer;
	public float force_power =0.002f;
	public bool Change_player = true;

	void Update () {

		if( Input.GetMouseButtonDown(0)){
			GameObject To_THROW = (GameObject)Instantiate (ObjectToThrow, this.transform.position + this.transform.forward *Distance+ this.transform.up *Distance, Quaternion.identity);

			To_THROW.GetComponent<Rigidbody> ().AddForce (Camera.main.transform.forward * force_power *0.2f + this.transform.up * force_power/1124);
			To_THROW.GetComponent<MoveItemOverGroundInfiniGRASS> ().enabled = true;

			if(Change_player){
				GrassManager.player = To_THROW;
				for(int i=0;i<GrassManager.Grasses.Count;i++){
					GrassManager.Grasses[i].player = To_THROW;
				}
			}
		}
	}
}
