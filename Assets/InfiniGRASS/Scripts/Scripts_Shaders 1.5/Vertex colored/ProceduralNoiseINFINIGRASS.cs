using UnityEngine;
using System.Collections;

namespace Artngame.INfiniDy {

public class ProceduralNoiseINFINIGRASS : MonoBehaviour {

	public float scale = 1.0f;
	public float speed = 1.0f;
	public bool recalculateNormals = false;
	private Vector3[] baseVertices;
		private PerlinINFINIGRASS  noise;

	public bool Colorize=false;
	public float Color_cycle_speed = 0.1f;

	MeshFilter meshFilter;

	//v1.5
	public bool RandomRot = false;
		public bool RandomScale = false;
		Transform this_transf;
		Vector3 start_scale;
		public	Vector3 Displace_factor = Vector3.zero;
		public	float Rot_disp_factor = 0.01f;

		public bool Update_collider = false;

		public bool ColorVertices = false;
		public float Vert_Y_Scale_factor = 10;
        public float raycastHeight = 0;
        public Color destColor = Color.black;
        public bool debugMe = false;


        public bool tintBase = false;
		public Color tineBaseColor = Color.black;
        public Color tintOffset = Color.black;


    void Start () {

		meshFilter = GetComponent("MeshFilter") as MeshFilter;

			noise = new PerlinINFINIGRASS ();

		register_time = Time.fixedTime;

			this_transf = this.transform;
			start_scale = this_transf.localScale;

			//v1.5
			if(RandomScale){
				this_transf.localScale = start_scale * Random.Range(0.1f,1.1f);
			}
			if(RandomRot){
				//Vector3 pos_original = this_transf.position;
				//this_transf.localPosition = Vector3.zero;
				float Rot_angle = Random.Range(-33,33);
				this_transf.rotation = Quaternion.Lerp(this_transf.rotation, Quaternion.AngleAxis(Rot_angle,new Vector3(Random.Range(-0.5f,1),Random.Range(-1,1),Random.Range(-1,0.5f))),Random.Range(0.2f,0.7f));
				//this_transf.position = pos_original;
				this_transf.position =this_transf.position + Displace_factor - new Vector3(0,-Mathf.Abs(Rot_angle)*Rot_disp_factor*0.1f	,0);
			}



			////////////

			if(scale!=0){
				Mesh mesh = meshFilter.mesh;
				
				if (baseVertices == null)
				{baseVertices = mesh.vertices;}
				
				Vector3[] vertices = new Vector3[baseVertices.Length];
				
				float timex = Time.time * speed + 0.1365143f * CHANGE_FACTOR + 0.11f;
				
				float timey = Time.time * speed + 1.21688f* CHANGE_FACTOR+ 0.12f;
				
				float timez = Time.time * speed + 2.5564f* CHANGE_FACTOR + 0.14f;
				
				for (int i=0;i<vertices.Length;i++)
					
				{
					
					Vector3 vertex = baseVertices[i];
					
					vertex.x += noise.Noise(timex + vertex.x, timex + vertex.y, timex + vertex.z) * scale;
					
					vertex.y += noise.Noise(timey + vertex.x, timey + vertex.y, timey + vertex.z) * scale;
					
					vertex.z += noise.Noise(timez + vertex.x, timez + vertex.y, timez + vertex.z) * scale;
					
					vertices[i] = vertex;
					
				}
				
				mesh.vertices = vertices;
				
				if (recalculateNormals) {
					
					mesh.RecalculateNormals();
					
				}
				
				
				mesh.RecalculateBounds();

				if(Update_collider){
					MeshCollider this_mesh_collider = this.gameObject.GetComponent<MeshCollider>();
					if(this_mesh_collider != null){
						this_mesh_collider.sharedMesh = mesh;
					}
				}

				if(ColorVertices){


					//raycast to grab color
					destColor = Color.black;
					RaycastHit hit = new RaycastHit();
					//if(Physics.Raycast(this_transf.position,-this_transf.up,out hit)){
					if(Physics.Raycast(this_transf.position+new Vector3(0, raycastHeight, 0),-Vector3.up,out hit)){

                        if (debugMe)
                        {
                            Debug.Log("Raycasting");
                        }

                        Renderer rend = hit.transform.GetComponent<Renderer>();
						if(rend!=null){
							Texture2D tex = rend.material.mainTexture as Texture2D;
							Vector2 pixelUV = hit.textureCoord;
							//Debug.Log("aaaa");
							if(tex != null){
								pixelUV.x*= tex.width;
								pixelUV.y*= tex.height;
                                if (debugMe)
                                {
                                    Debug.Log(tex.GetPixel((int)pixelUV.x, (int)pixelUV.y));
                                    destColor = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);
                                }
                                try {
									destColor = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);

                                    if (debugMe)
                                    {
                                        Debug.Log(destColor);
                                    }
								}
								//finally{
								//catch (UnityException e){
								catch{
									//e.Message.StartsWith
								}
								//Debug.Log(destColor);
							}
						}
					}

					if(tintBase){
						destColor = destColor * tineBaseColor ;
					}
                    destColor = destColor + tintOffset;

                    Color[] VertColors = new Color[baseVertices.Length];
					
					for(int i=0;i<VertColors.Length;i++){
						
						//VertColors[i] = Color.Lerp(Color.black,Color.white,vertices[i].y*Vert_Y_Scale_factor * this_transf.localScale.y);

						//VertColors[i] = Color.Lerp(Color.black,Color.white, (this_transf.localScale.y/2) + vertices[i].y);

						//VertColors[i] = Color.Lerp(Color.black,Color.white, (this_transf.localPosition.y + vertices[i].y/this_transf.localScale.y)*Vert_Y_Scale_factor );

						//VertColors[i] = Color.Lerp(Color.white,Color.black, (this_transf.localPosition.y + vertices[i].y/this_transf.localScale.y)*Vert_Y_Scale_factor );

						VertColors[i] = Color.Lerp(destColor,Color.white, ((this_transf.TransformPoint(vertices[i]).y - this_transf.parent.position.y)*Vert_Y_Scale_factor) / this_transf.localScale.y );

						//VertColors[i] = Color.Lerp(Color.black,Color.red,vertices[i].y*Vert_Y_Scale_factor);
						//VertColors[i] = Color.white;
					}
					
					mesh.colors = VertColors;
				}
				
			}

			if (!Enable_procedural) {
				this.enabled = false;
			}
	}

	public float CHANGE_FACTOR=0;

	private float register_time;
	public bool Deactivate_after=false;
	public float seconds=1f;

		public bool Enable_procedural = false;

	void Update () {
		if(Enable_procedural){
			if (!Deactivate_after | (Deactivate_after & ((Time.fixedTime - register_time) < seconds))) {
		
				if (scale != 0) {
					Mesh mesh = meshFilter.mesh;
		
					if (baseVertices == null) {
						baseVertices = mesh.vertices;
					}
		
					Vector3[] vertices = new Vector3[baseVertices.Length];
		
					float timex = Time.time * speed + 0.1365143f * CHANGE_FACTOR;
		
					float timey = Time.time * speed + 1.21688f * CHANGE_FACTOR;
		
					float timez = Time.time * speed + 2.5564f * CHANGE_FACTOR;
		
					for (int i = 0; i < vertices.Length; i++) {
			
						Vector3 vertex = baseVertices [i];
			
						vertex.x += noise.Noise (timex + vertex.x, timex + vertex.y, timex + vertex.z) * scale;
			
						vertex.y += noise.Noise (timey + vertex.x, timey + vertex.y, timey + vertex.z) * scale;
			
						vertex.z += noise.Noise (timez + vertex.x, timez + vertex.y, timez + vertex.z) * scale;
			
						vertices [i] = vertex;
			
					}

					mesh.vertices = vertices;

					if (recalculateNormals) {
				
						mesh.RecalculateNormals ();
				
					}
			
			
					mesh.RecalculateBounds ();


//					if(ColorVertices){
//						
//						Color[] VertColors = new Color[baseVertices.Length];
//						
//						for(int i=0;i<VertColors.Length;i++){
//							
//							VertColors[i] = Color.Lerp(Color.black,VertColors[i],0.99f);
//							//VertColors[i] = Color.black;
//						}
//						
//						mesh.colors = VertColors;
//					}

				}
	




				if (Colorize) {
					int RAND_1 = Random.Range (0, 3);
			
					if (RAND_1 == 0) {
						this.gameObject.GetComponent<Renderer> ().material.color = Color.Lerp (this.gameObject.GetComponent<Renderer> ().material.color, Color.blue, Color_cycle_speed);
					} else if (RAND_1 == 1) {
						this.gameObject.GetComponent<Renderer> ().material.color = Color.Lerp (this.gameObject.GetComponent<Renderer> ().material.color, Color.red, Color_cycle_speed);
					} else if (RAND_1 == 2) {
						this.gameObject.GetComponent<Renderer> ().material.color = Color.Lerp (this.gameObject.GetComponent<Renderer> ().material.color, Color.magenta, Color_cycle_speed);
					}
				}


			}
		
		}
	}
}

}