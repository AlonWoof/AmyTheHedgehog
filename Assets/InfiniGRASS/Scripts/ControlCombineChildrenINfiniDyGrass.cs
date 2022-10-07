using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Artngame.INfiniDy {

	[ExecuteInEditMode]
	public class ControlCombineChildrenINfiniDyGrass : MonoBehaviour {

        //v1.7 - Above 64K meshes enable - v1.9.9.7
        public bool enableAbove64KMesh = false;

		//v1.6
		bool noThreading = false;//override multithreading setting for WebGL

		//v1.2
		public bool realtime = false;//for rocks-fences

		//v1.1 - follow objects paintedOn
		public bool FollowObject = false;//enable when grass cant be parnted to PaintedOn object
		public bool ObjectPosInit = false;
		public bool look_at_direction = false;
		public InfiniGRASSManager GrassManager;

		public float MinTranslation = 0.3f;
		public float MinRotation = 0.3f;

		Vector3 Prev_position;
		Quaternion Prev_rotation;
		Vector3 Prev_scale;

		Transform thisTransf;
		[HideInInspector]
		public Vector3 Registered_initial_position;
		[HideInInspector]
		public Quaternion Registered_initial_rotation;
		[HideInInspector]
		public Vector3 Registered_initial_scale;

		[HideInInspector]
		public List<Vector3> Registered_paint_positions = new List<Vector3>(); 
		[HideInInspector]
		public List<Vector3> Updated_Registered_paint_positions = new List<Vector3>(); 
		[HideInInspector]
		public List<Vector3> Registered_paint_rotations=new List<Vector3>();
		//[HideInInspector]
		//public List<Vector3> Registered_initial_scale;


		//InfiniGRASS
		public bool got_LOD_dist;//dont change LOD during play time
		public bool Tag_based_player = false;//otherwise search for camera
		public string Player_tag = "Player";
		public int Type=0;
		public Transform PaintedOn;//object first item in batch was painted on

		public struct Meshy {
			
			public string name;
			public Vector3[] vertices;
			public Vector3[] normals;
			public Color[] colors;
			public Vector2[] uv;
			public Vector2[] uv1;
			public Vector4[] tangents;
			public int[] triangles;
			public bool thread_ended;
		}
		
		List<Meshy> MeshList = new List<Meshy>();
		List<int> MeshIDList = new List<int>();//fill with 1 when thread registered for MeshList item is done
		
		public string Save_Name = "InfiniTree1";
		public string Save_Dir = "Assets/InfiniTREE/savedMesh/";
		
		public int Added_item_count;
		public List<Transform> Added_items;
		public List<INfiniDyGrassField> Added_items_handles;
		public int Max_items;
		public float Deactivate_hero_dist = 360;
		public float Deactivate_hero_offset = 5;
		public float Deactivate_hero_distCUT = 580;

		//infiniGRASS - more LODs
		public float Deactivate_hero_dist1 = 460;
		public float Deactivate_hero_dist2 = 520;

		public bool DeactivateLOD = false;
		public Material LODMaterial;//material that holds the LOD sprite
		
		public bool generateTriangleStrips = true;
		public bool Auto_Disable=false;		
		public int skip_every_N_frame=0;		
		bool run_once=false;
		public bool Multithreaded=false;
		public bool MTmethod2 = false;
		// ControlCombineChildrenINfiniDy.Meshy mesh;
		
		bool mesh_initalized = false;
		public bool is_moving = false;
		public bool doing_batching = false;//signal to tree scripts to not use interactive mode


		//v1.2
		public void Full_reset(){
			//reset everything

			if (started & Application.isPlaying) {

				batching_initialized = false;
				mesh_initalized = false;
				MakeActive = true;

				is_moving = false;
				doing_batching = false;
				run_once=false;
				is_active = false;

				//Multithreaded = true;
				//v1.6
				Multithreaded = !noThreading; //Multithreaded = true;


				threads_started = 0;
				threads_ended = 0;
				all_threads_started = false;

				started = false;

				Component[] filters  = GetComponentsInChildren<MeshFilter>(true);//  GetComponentsInChildren(typeof(MeshFilter));
				for (int i=0;i<filters.Length;i++) {
					if(filters[i].gameObject.name != "Combined mesh64" & filters[i].gameObject.name != "Combined mesh"){

						Renderer curRenderer  = filters[i].gameObject.GetComponentsInChildren<Renderer>(true)[0];
						
						if (curRenderer != null && curRenderer.sharedMaterial !=null && curRenderer.sharedMaterial.name.Contains("LOD") && curRenderer.enabled ) {
							curRenderer.enabled = false;
						}else{
							curRenderer.enabled = true;
						}

					}else{
						DestroyImmediate(filters[i].gameObject);
					}
				}

				if(Destroy_list != null){
					for(int i=0;i<Destroy_list.Count;i++){						
						if(Destroy_list[i] != null){
							DestroyImmediate(Destroy_list[i]);
							}
					}
					Destroy_list.Clear();
				}

				if(Destroy_list_MF != null){
				for(int i=0;i<Destroy_list_MF.Count;i++){						
					if(Destroy_list_MF[i] != null){
						DestroyImmediate(Destroy_list_MF[i].gameObject);
					}
				}
				Destroy_list_MF.Clear();
				}

				if(Destroy_list_MF64 != null){
				for(int i=0;i<Destroy_list_MF64.Count;i++){						
					if(Destroy_list_MF64[i] != null){
						DestroyImmediate(Destroy_list_MF64[i].gameObject);
					}
				}
				Destroy_list_MF64.Clear();
					}

				if(Destroy_list64 != null){
					for(int i=0;i<Destroy_list64.Count;i++){						
						if(Destroy_list64[i] != null){
							DestroyImmediate(Destroy_list64[i]);
						}
					}
					Destroy_list64.Clear();
				}
				MeshList.Clear();
				MeshIDList.Clear();
				Material_list.Clear();
				Splits.Clear();
					
				
			}

		}

		void Start () {

			if (!realtime | (realtime & Application.isPlaying)) {

				if(Application.isPlaying){
					//started = false;
				}

				//v1.2
				Full_reset ();

				thisTransf = transform;

				//v1.1 - init follow object
				if (FollowObject && PaintedOn != null) {
					Updated_Registered_paint_positions.Add (transform.position);
					Registered_paint_positions.Add (transform.position);
					Registered_paint_rotations.Add (thisTransf.up);
					//Registered_initial_scale
				}

				//Infinigrass
				if (Tag_based_player) {
					if (player == null) {
						player = GameObject.FindGameObjectWithTag (Player_tag);
					}
				} else {

					if (player == null) {
						if (Application.isPlaying) {
							if(Camera.main != null){
								player = Camera.main.gameObject;
							}else{
								//Debug.Log("Please add a Main Camera to the scene or use the tag based player option");
							}
						} else {
							if (Camera.current != null) {
								player = Camera.current.gameObject;
							} else {
								if(Camera.main != null){
									player = Camera.main.gameObject;
								}
							}
						}
					}
					if (player == null) {
						if (Camera.current != null) {
							player = Camera.current.gameObject;
						}
					}

				}
			
				if (Added_items == null) {
					Added_items = new List<Transform> ();
				}
				if (Added_items_handles == null) {
					Added_items_handles = new List<INfiniDyGrassField> ();
				}
			
				if (!mesh_initalized) {
					//mesh = new ControlCombineChildrenINfiniDy.Meshy();
					mesh_initalized = true;
				}
			
				if (Destroy_list == null) {
					Destroy_list = new List<GameObject> ();
				}
				if (Destroy_list_MF == null) {
					Destroy_list_MF = new List<MeshFilter> ();
				}
			
				if (Destroy_list64 == null) {
					Destroy_list64 = new List<GameObject> ();
				}
				if (Destroy_list_MF64 == null) {
					Destroy_list_MF64 = new List<MeshFilter> ();
				}
			
				Component[] filters = GetComponentsInChildren (typeof(MeshFilter));
				//v1.7
				if (Self_dynamic_enable & !run_once) {
					if (Children_list != null) {
						if (filters.Length != Children_list.Count) {
						
							Children_list.Clear ();
							Positions_list.Clear ();
						
							if (Self_dynamic_check_rot) {
								Rotations_list.Clear ();
							}
							if (Self_dynamic_check_scale) {
								Scale_list.Clear ();
							}						
						}
					}
				}
			
				for (int i=0; i<filters.Length; i++) {
					Renderer curRenderer = filters [i].GetComponent<Renderer> ();
				
					//v1.7
					if (Self_dynamic_enable & !run_once) {
						if (Children_list != null) {
							if (filters.Length != Children_list.Count) {
								Children_list.Add (filters [i].gameObject.transform);
								Positions_list.Add (filters [i].gameObject.transform.position);
								Rotations_list.Add (filters [i].gameObject.transform.rotation);
								Scale_list.Add (filters [i].gameObject.transform.localScale);
							}
						}
					}
				
					if (curRenderer != null && !curRenderer.enabled) {	
						if (curRenderer.sharedMaterial.name.Contains ("LOD")) {//LOD
						
							curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;//.castShadows = false;
							curRenderer.receiveShadows = false;
						} else {
							curRenderer.enabled = true;
						}
					}
				}				
				//run_once = true;
				previous_children_count = transform.childCount; 			
				self_dyn_enable_time = Time.fixedTime;
			}

			//v1.4 - update tag player status
//			if (GrassManager != null) {
//				Tag_based_player = GrassManager.Tag_based_player;
//			} else {
//				//Debug.Log("as");
//			}
		}
		
		public bool MakeActive=false;
		private List<GameObject> Destroy_list;
		private List<MeshFilter> Destroy_list_MF;
		private List<GameObject> Destroy_list64;
		private List<MeshFilter> Destroy_list_MF64;
		int count_frames;
		
		//v1.7
		private List<Vector3> Positions_list;
		private List<Quaternion> Rotations_list;
		private List<Vector3> Scale_list;
		private List<Transform> Children_list;
		public bool Self_dynamic_enable=false;
		public bool Self_dynamic_check_rot=false;
		public bool Self_dynamic_check_scale=false;
		
		public bool is_active=false;
		public GameObject player;
		
		public float Min_dist = 0.2f; //minimal move distance to trigger mesh update
		int previous_children_count;
		public float stop_self_dyn_after = 0; //let objects settle and then stop self_dyn after x secs, if > 0
		float self_dyn_enable_time;
		public bool Decombine = false;
		public bool Decombined = false;//flag a recent decombination
		public bool Erase_tree = false;
		public List<int> Erase_Tree_IDs;
		
		int threads_started = 0;
		int threads_ended = 0;
		bool all_threads_started = false;
		public bool batching_initialized = false;
		
		List<int> Splits = new List<int>();//List to keep split count per material, for multithreading handling after threads are over
		int Material_count = 0;
		List<Material> Material_list = new List<Material>();
		
		bool Hero_far=true;
		public bool LODed = false;

		//v1.2
		public bool started = false;

		//v1.3
		public int LOD_Level=0;//0 is no LOD, 1-2-3 is LOD0-1-2, 4 is cutoff

		//InfiniGRASS
		void Update(){

			//v1.6
			if (GrassManager != null && Application.isPlaying && GrassManager.noThreading) {
				noThreading = true;
			}

			//v1.4 - update tag player status
//			if (GrassManager != null) {
//				Tag_based_player = GrassManager.Tag_based_player;
//			} else {
//				//Debug.Log("as");
//			}

			if (!realtime | (realtime & Application.isPlaying)) {
			//v1.2
				//Debug.Log(started);
			if (!started & !Application.isPlaying) {
				Start ();
				started = true;
			}

			if (started & !Application.isPlaying) {
				Component[] filters = GetComponentsInChildren (typeof(MeshFilter));
				for (int i=0; i<filters.Length; i++) {
					if (filters [i].gameObject.name != "Combined mesh64" & filters [i].gameObject.name != "Combined mesh") {
						filters [i].gameObject.GetComponent<MeshRenderer> ().enabled = false;
					}
				}
			}
			

				//v1.3
				if (GrassManager == null) {
					if (Added_items_handles.Count > 0 && Added_items_handles [0] != null) {//v1.1a
						GrassManager = Added_items_handles [0].GrassManager;
					}
					//v1.6
					if (GrassManager != null && Application.isPlaying && GrassManager.noThreading) {
						noThreading = true;
					}
				}

			//v1.1 - clean up when follow object and object is deleted
			if (FollowObject && ObjectPosInit && PaintedOn == null && Added_items.Count > 0) {
							
				//v1.1
//				if (GrassManager == null) {
//					if (Added_items_handles.Count > 0 && Added_items_handles [0] != null) {//v1.1a
//						GrassManager = Added_items_handles [0].GrassManager;
//					}
//				}

				if (GrassManager != null) {

					//v1.2
					if(!Application.isPlaying){
						for(int j=GrassManager.DynamicCombiners.Count-1;j>=0;j--){
							if(GrassManager.DynamicCombiners[j] != null && GrassManager.DynamicCombiners[j] == this.gameObject){
								GrassManager.DynamicCombiners.RemoveAt(j);
							}
						}
						for(int j=GrassManager.StaticCombiners.Count-1;j>=0;j--){
							if(GrassManager.StaticCombiners[j] != null && GrassManager.StaticCombiners[j] == this.gameObject){
								GrassManager.StaticCombiners.RemoveAt(j);
							}
						}

						for (int j = Added_items_handles.Count-1; j>=0; j--) {
							if(Added_items_handles[j] != null && Added_items_handles[j].gameObject != null){
								DestroyImmediate(Added_items_handles[j].gameObject);
							}
						}
					}

					for (int j = GrassManager.Grasses.Count -1; j>=0; j--) {
						if (GrassManager.Grasses [j] == null) {
							GrassManager.Grasses.RemoveAt (j);
							GrassManager.GrassesType.RemoveAt (j);
						}
					}

					for (int j=0; j<GrassManager.Grasses.Count; j++) {
						GrassManager.Grasses [j].Grass_Holder_Index = j;
					}
				}
				
				//v.1.2
				if(Application.isPlaying){
					Destroy (this.gameObject);
				}else{
					DestroyImmediate (this.gameObject);
				}
			}

			//LOD - get from first added and never change
			if (!got_LOD_dist && Added_items_handles.Count > 0 && Added_items_handles [0] != null) {
				if (Deactivate_hero_dist != Added_items_handles [0].LOD_distance) {
					Deactivate_hero_dist = Added_items_handles [0].LOD_distance;
				}
				if (Deactivate_hero_dist1 != Added_items_handles [0].LOD_distance1) {
					Deactivate_hero_dist1 = Added_items_handles [0].LOD_distance1;
				}
				if (Deactivate_hero_dist2 != Added_items_handles [0].LOD_distance2) {
					Deactivate_hero_dist2 = Added_items_handles [0].LOD_distance2;
				}
				if (Deactivate_hero_distCUT != Added_items_handles [0].Cutoff_distance) {
					Deactivate_hero_distCUT = Added_items_handles [0].Cutoff_distance;
				}
				got_LOD_dist = true;
			}

			if (Added_items.Count == 0 & batching_initialized & Max_items > 0) {
				batching_initialized = false;
				MakeActive = false;
				is_active = false;
				Decombined = false;
				mesh_initalized = false;

				//v1.2
				if (Application.isPlaying) {
					Destroy (this.gameObject);
				} else {
					DestroyImmediate (this.gameObject);
				}
				//Debug.Log("init");
			}

			//v1.3 - clean up if away from hero and in gradual grow mode
			if(Application.isPlaying && GrassManager != null && GrassManager.UnGrown){
					if(GrassManager.GradualGrowth & GrassManager.UseDistFromPlayer & GrassManager.GradualRecreate & LOD_Level == 5){

						//erase elements
						for (int j = Added_items_handles.Count -1; j>=0; j--) {
							INfiniDyGrassField forest = Added_items_handles[j];
							//EXTRAS
							GameObject LEAF_POOL = new GameObject ();
							LEAF_POOL.transform.parent = forest.transform;
							forest.Leaf_pool = LEAF_POOL;							
							forest.Combiner = null;
							forest.Grow_in_Editor = false;
							forest.growth_over = false;
							forest.Registered_Brances.Clear ();//
							//forest.root_tree = null;
							forest.Branch_grew.Clear ();
							forest.Registered_Leaves.Clear ();//
							forest.Registered_Leaves_Rot.Clear ();//
							forest.batching_ended = false;
							forest.Branch_levels.Clear ();
							forest.BranchID_per_level.Clear ();
							//forest.Grass_Holder_Index = 0;
							forest.Grow_level = 0;
							forest.Grow_tree_ended = false;
							forest.Health = forest.Max_Health;
							forest.is_moving = false;
							forest.Leaf_belongs_to_branch.Clear ();
							forest.scaleVectors.Clear ();
							forest.Leaves.Clear ();
							forest.Tree_Holder_Index = 0;
							forest.Grow_tree = false;
							forest.rotation_over = false;							
							forest.Forest_holder = null;
							
							forest.Grow_tree_speed = 1000;
							forest.Restart = true;

							//v1.3
							forest.gameObject.SetActive(false);							

						}

//						for (int j = Added_items_handles.Count-1; j>=0; j--) {
//							DestroyImmediate(Added_items_handles[j].gameObject);
//						}
//
//						for (int j = GrassManager.Grasses.Count -1; j>=0; j--) {
//							if (GrassManager.Grasses [j] == null) {
//								GrassManager.Grasses.RemoveAt (j);
//								GrassManager.GrassesType.RemoveAt (j);
//							}
//						}
//						
//						for (int j=0; j<GrassManager.Grasses.Count; j++) {
//							GrassManager.Grasses [j].Grass_Holder_Index = j;
//						}

						Destroy (this.gameObject);
					}
			}

		  }//REAL TIME CHECK
		}

		public void LateUpdate(){

			//v1.6
			if (GrassManager != null && Application.isPlaying && GrassManager.noThreading) {
				noThreading = true;
			}

			if (!realtime | (realtime & Application.isPlaying)) {
				//v1.2
				if (!Application.isPlaying) {
			
					Multithreaded = false;
			
				} else {
			
					//Multithreaded = true;	
					Multithreaded = !noThreading;//v1.6
				}

				//v1.1 -  follow object
				if (FollowObject && PaintedOn != null) {
					if (!ObjectPosInit) {
						Registered_initial_position = PaintedOn.position;
						Registered_initial_rotation = PaintedOn.rotation;
						Registered_initial_scale = PaintedOn.localScale;
						ObjectPosInit = true;

						Updated_Registered_paint_positions.Add (transform.position);
						Registered_paint_positions.Add (transform.position);
						Registered_paint_rotations.Add (thisTransf.up);

						Prev_position = PaintedOn.position;
						Prev_rotation = PaintedOn.rotation;
						Prev_scale = PaintedOn.localScale;

						if (Added_items_handles.Count > 0 && Added_items_handles [0] != null) {//v1.1a
							GrassManager = Added_items_handles [0].GrassManager;
						}
					}

					//for(int i=0;i<Added_items.Count;i++){		//DO IT WHOLE

					if (Vector3.Distance (Prev_position, PaintedOn.position) > MinTranslation
						| Quaternion.Angle (Prev_rotation, PaintedOn.rotation) > MinRotation
						| Prev_scale != PaintedOn.localScale) {


						Vector3 FIND_moved_toZERO = Registered_paint_positions [0] - Registered_initial_position;
					
						Vector3 FIXED_ROT = PaintedOn.eulerAngles;
					
						Quaternion relative = Quaternion.Euler (FIXED_ROT) * Quaternion.Inverse (Registered_initial_rotation);
						Vector3 FIND_rotated = relative * (FIND_moved_toZERO);  //+ Emitter_objects[counter_regsitered].gameObject.transform.eulerAngles ;
					
						Vector3 FIND_scaled = new Vector3 (FIND_rotated.x * (PaintedOn.localScale.x / Registered_initial_scale.x),
					                                  FIND_rotated.y * (PaintedOn.localScale.y / Registered_initial_scale.y),
					                                  FIND_rotated.z * (PaintedOn.localScale.z / Registered_initial_scale.z));
					
						Vector3 FIND_re_translated = FIND_scaled + PaintedOn.position;
						Vector3 FIND_moved_pos = FIND_re_translated;
					
						Vector3 FIND_moved_normal_toZERO = Registered_paint_rotations [0];
					
						//Vector3 FIND_rotated_normal = relative*(FIND_moved_normal_toZERO);

						thisTransf.position = FIND_moved_pos; 
					
						if (!look_at_direction) {
							thisTransf.rotation = relative * Quaternion.FromToRotation (Vector3.up, FIND_moved_normal_toZERO); 
						} else {
						
							//v1.4
							//if(!look_at_normal){
							Vector3 Motion_vec = FIND_moved_pos - Updated_Registered_paint_positions [0];
						
							Quaternion New_rot = Quaternion.identity;
							if (Motion_vec != Vector3.zero) {
								New_rot = Quaternion.LookRotation (1 * Motion_vec);
								thisTransf.rotation = Quaternion.Slerp (thisTransf.rotation, New_rot, Time.deltaTime * 16);
							}
						}

						//v1.3
						if(!Application.isPlaying){
							for(int i=0;i<Added_items_handles.Count;i++){
								if(Updated_Registered_paint_positions [0] != FIND_moved_pos){
									//Added_items_handles[i].transform.position = Added_items_handles[i].transform.position - (Updated_Registered_paint_positions [0]-FIND_moved_pos);
									if(Added_items_handles[i].PaintedOnOBJ != null){
										Added_items_handles[i].transform.parent = Added_items_handles[i].PaintedOnOBJ;
									}
								}
								//Added_items_handles[i].transform.rotation = thisTransf.rotation;
							}
						}


						Updated_Registered_paint_positions [0] = FIND_moved_pos;

						Prev_position = PaintedOn.position;
						Prev_rotation = PaintedOn.rotation;
						Prev_scale = PaintedOn.localScale;

						//Debug.Log("done");
					}
					//}
			
				}



				//Infinigrass
				if (Tag_based_player) {
					if (player == null) {
						player = GameObject.FindGameObjectWithTag (Player_tag);
					}
				} else {
					if (player == null) {
						if (Application.isPlaying) {
							if(Camera.main != null){//v1.4
								player = Camera.main.gameObject;
							}else{
								//Debug.Log("Please add a Main Camera to the scene or use the tag based player option");
							}
						} else {
							//v1.2
							if (Camera.current != null) {
								player = Camera.current.gameObject;
							} else {
								if(Camera.main != null){//v1.4
									player = Camera.main.gameObject;
								}
							}
						}
					}
					if (player == null) {
						if (Camera.current != null) {
							player = Camera.current.gameObject;
						}
					}
//					if (Application.isPlaying) { //v1.4
//						if(Camera.main != null){
//							player = Camera.main.gameObject;
//						}
//					}
				}

				if(!Application.isPlaying){
					if (Camera.current != null) {
						player = Camera.current.gameObject;
					}
				}

				//v1.4c
				if(player == null){
					//Debug.Log("no player assigned");
					return;
				}

						
				Hero_far = true;
			
				if (Application.isPlaying && Added_items != null) {//v1.4
					for (int i=0; i<Added_items.Count; i++) {
						if (Added_items [i] != null) {
							if (Vector3.Distance (Added_items [i].position, player.transform.position) < Deactivate_hero_dist) {
								Hero_far = false;
							}
						}
					}
				}
			
				if (Hero_far) {
					//Debug.Log ("Hero Far");
				}
			
				//LODed = false;  //v1.4f
			
				//v1.1 - Fix case where handles are not available
				bool checked_handled = false;
			
				if (Added_items_handles.Count > 0) {
					if (Added_items_handles [Added_items_handles.Count - 1].Grow_tree_ended) {
						checked_handled = true;
					}
				}
			
				//v1.3
				LOD_Level = 0;

				if (DeactivateLOD
					& Hero_far 
					& !is_moving 
					& !Decombined 
					& Added_items.Count > 0 
					& (Destroy_list.Count > 0 || Destroy_list64.Count > 0) //){ //v1.7
					& !MakeActive 
					& Added_items.Count == Added_item_count 
					& checked_handled

				    & Application.isPlaying //v1.2
			   //& Added_items_handles[Added_items_handles.Count-1].Grow_tree_ended
			   ) {
				
					bool Hero_far2 = true;
					for (int i=0; i<Added_items.Count; i++) {
						if (Vector3.Distance (Added_items [i].position, player.transform.position) < (Deactivate_hero_dist + Deactivate_hero_offset)) {
							Hero_far2 = false;
						}
					}
				
					if (Hero_far2) {
						//Debug.Log ("Hero Far2");
					}
				
					if (!Hero_far2) {
					
						LODed = false;
						//LOD
						//re-enable meshes in the threhold region, before system gets fully activated
						if (Destroy_list_MF.Count > 0) {
							for (int i=0; i<Destroy_list_MF.Count; i++) {
							
								if (Destroy_list_MF [i] != null) {
									if (Destroy_list_MF [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD")) {
										Destroy_list_MF [i].gameObject.SetActive (false);
									} else {
										Destroy_list_MF [i].gameObject.SetActive (true);
									}
								}
							}
						}
						if (Destroy_list_MF64.Count > 0) {
							for (int i=0; i<Destroy_list_MF64.Count; i++) {
							
								if (Destroy_list_MF64 [i] != null) {
									if (Destroy_list_MF64 [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD")) {
										Destroy_list_MF64 [i].gameObject.SetActive (false);
									} else {
										Destroy_list_MF64 [i].gameObject.SetActive (true);
									}
								}
							}
						}
					} else {
						//deactivate and activate LODs

						//v1.5
						bool Hero_farCUT = true;

						bool Hero_farLOD1 = true;
						bool Hero_farLOD2 = true;

						if (Added_items != null) {
							for (int i=0; i<Added_items.Count; i++) {
								if (Added_items [i] != null) {

									//v1.3
									float distA = Vector3.Distance (Added_items [i].position, player.transform.position);
									if (distA > Deactivate_hero_distCUT*1.5f) {
										LOD_Level = 5;
									}
									if (distA < Deactivate_hero_distCUT) {
										Hero_farCUT = false;
									}
									if (distA < Deactivate_hero_dist1) {
										Hero_farLOD1 = false;
									}
									if (distA < Deactivate_hero_dist2) {
										Hero_farLOD2 = false;
									}
								}
							}
						}

						if (Hero_farCUT) {
							if (Destroy_list_MF.Count > 0) {
								for (int i=0; i<Destroy_list_MF.Count; i++) {
								
									if (Destroy_list_MF [i] != null) {
										if (Destroy_list_MF [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD")) {
											Destroy_list_MF [i].gameObject.SetActive (false);
										} else {
											Destroy_list_MF [i].gameObject.SetActive (false);
										}
									}
								}
							}
							if (Destroy_list_MF64.Count > 0) {
								for (int i=0; i<Destroy_list_MF64.Count; i++) {
								
									if (Destroy_list_MF64 [i] != null) {
										if (Destroy_list_MF64 [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD")) {
											Destroy_list_MF64 [i].gameObject.SetActive (false);
										} else {
											Destroy_list_MF64 [i].gameObject.SetActive (false);
										}
									}
								}
							}

							//v1.3
							if(LOD_Level != 5){
								LOD_Level = 4;
							}

						} else {

							//INFINIGRASS - added more lods
							if (Hero_farLOD2) {
								if (Destroy_list_MF.Count > 0) {
									for (int i=0; i<Destroy_list_MF.Count; i++) {
									
										if (Destroy_list_MF [i] != null) {
											if (Destroy_list_MF [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD2")) {
												Destroy_list_MF [i].gameObject.SetActive (true);
											} else {
												Destroy_list_MF [i].gameObject.SetActive (false);
											}
										}
									}
								}
								if (Destroy_list_MF64.Count > 0) {
									for (int i=0; i<Destroy_list_MF64.Count; i++) {
									
										if (Destroy_list_MF64 [i] != null) {
											if (Destroy_list_MF64 [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD2")) {
												Destroy_list_MF64 [i].gameObject.SetActive (true);
											} else {
												Destroy_list_MF64 [i].gameObject.SetActive (false);
											}
										}
									}
								}

								//v1.3
								LOD_Level = 3;

							} else if (Hero_farLOD1) {
								if (Destroy_list_MF.Count > 0) {
									for (int i=0; i<Destroy_list_MF.Count; i++) {
									
										if (Destroy_list_MF [i] != null) {
											if (Destroy_list_MF [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD1")) {
												Destroy_list_MF [i].gameObject.SetActive (true);
											} else {
												Destroy_list_MF [i].gameObject.SetActive (false);
											}
										}
									}
								}
								if (Destroy_list_MF64.Count > 0) {
									for (int i=0; i<Destroy_list_MF64.Count; i++) {
									
										if (Destroy_list_MF64 [i] != null) {
											if (Destroy_list_MF64 [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD1")) {
												Destroy_list_MF64 [i].gameObject.SetActive (true);
											} else {
												Destroy_list_MF64 [i].gameObject.SetActive (false);
											}
										}
									}
								}

								//v1.3
								LOD_Level = 2;

							} else {
								if (Destroy_list_MF.Count > 0) {
									for (int i=0; i<Destroy_list_MF.Count; i++) {
									
										if (Destroy_list_MF [i] != null) {
											if (Destroy_list_MF [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD0")) {
												Destroy_list_MF [i].gameObject.SetActive (true);
											} else {
												Destroy_list_MF [i].gameObject.SetActive (false);
											}
										}
									}
								}
								if (Destroy_list_MF64.Count > 0) {
									for (int i=0; i<Destroy_list_MF64.Count; i++) {
									
										if (Destroy_list_MF64 [i] != null) {
											if (Destroy_list_MF64 [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD0")) {
												Destroy_list_MF64 [i].gameObject.SetActive (true);
											} else {
												Destroy_list_MF64 [i].gameObject.SetActive (false);
											}
										}
									}
								}

								//v1.3
								LOD_Level = 1;
							}



//						if(Destroy_list_MF.Count>0){
//							for(int i=0;i<Destroy_list_MF.Count;i++){
//								
//								if(Destroy_list_MF[i]!=null){
//									if(Destroy_list_MF[i].gameObject.GetComponent<Renderer>().sharedMaterial.name.Contains("LOD")){
//										Destroy_list_MF[i].gameObject.SetActive(true);
//									}else{
//										Destroy_list_MF[i].gameObject.SetActive(false);
//									}
//								}
//							}
//						}
//						if(Destroy_list_MF64.Count>0){
//							for(int i=0;i<Destroy_list_MF64.Count;i++){
//								
//								if(Destroy_list_MF64[i]!=null){
//									if(Destroy_list_MF64[i].gameObject.GetComponent<Renderer>().sharedMaterial.name.Contains("LOD")){
//										Destroy_list_MF64[i].gameObject.SetActive(true);
//									}else{
//										Destroy_list_MF64[i].gameObject.SetActive(false);
//									}
//								}
//							}
//						}
						}
						LODed = true;
					}
					//reacivate just before threshold is passed
				} else {
				
					if(LODed){
						//Debug.Log("en");
						if (Destroy_list_MF.Count > 0) {
							for (int i=0; i<Destroy_list_MF.Count; i++) {
								
								if (Destroy_list_MF [i] != null) {
									if (Destroy_list_MF [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD")) {
										Destroy_list_MF [i].gameObject.SetActive (false);
									} else {
										Destroy_list_MF [i].gameObject.SetActive (true);
									}
								}
							}
						}
						if (Destroy_list_MF64.Count > 0) {
							for (int i=0; i<Destroy_list_MF64.Count; i++) {
								
								if (Destroy_list_MF64 [i] != null) {
									if (Destroy_list_MF64 [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD")) {
										Destroy_list_MF64 [i].gameObject.SetActive (false);
									} else {
										Destroy_list_MF64 [i].gameObject.SetActive (true);
									}
								}
							}
						}
					}

					LODed = false;
				
					is_active = false;//start with false, if active signal it
				
					if (Self_dynamic_enable) {
					
						if (stop_self_dyn_after > 0) {
							if (Time.fixedTime - self_dyn_enable_time > stop_self_dyn_after) {
								self_dyn_enable_time = Time.fixedTime;
								Self_dynamic_enable = false;
							}
						}				
					
						if (Children_list != null) {
							//v1.7 check if items in list are null and remove from both lists.
							for (int i=Children_list.Count-1; i>=0; i--) {
								if (Children_list [i] != null) {
									if (Children_list [i].gameObject == null) {
										Children_list.RemoveAt (i);
										Positions_list.RemoveAt (i);
									}
								}
							}
							//if item changed position
							for (int i=Children_list.Count-1; i>=0; i--) {
								if (Children_list [i] != null) {
								
									if (Vector3.Distance (Children_list [i].position, Positions_list [i]) > Min_dist) {
										MakeActive = true;
										Positions_list [i] = Children_list [i].position; //save new pos
										//Debug.Log ("ID ="+i);
										//Restore();
										//v1.6
										Multithreaded = false;
										Self_dynamic_enable = true;
									}
									if (Self_dynamic_check_rot) {
										if (Rotations_list [i] != Children_list [i].rotation) {
											MakeActive = true;
											Rotations_list [i] = Children_list [i].rotation; //save new rot
										}
									}
									if (Self_dynamic_check_scale) {
										if (Scale_list [i] != Children_list [i].localScale) {
											MakeActive = true;
											Scale_list [i] = Children_list [i].localScale; //save new scale
										}
									}
								}
							}					
						
							//if item has been added					
							int child_count = transform.childCount; //v1.8.1 Check PREVIOUS COUNT and not children count
						
							if (child_count != previous_children_count) {						
								previous_children_count = child_count;
								MakeActive = true;
							}
						
						} else {
							Children_list = new List<Transform> ();
							Positions_list = new List<Vector3> ();
							Rotations_list = new List<Quaternion> ();
							Scale_list = new List<Vector3> ();

							//v1.6
							if (!run_once & Self_dynamic_enable) {
								Component[] filters = GetComponentsInChildren (typeof(MeshFilter));
								//v1.7
								if (Self_dynamic_enable & !run_once) {
									if (Children_list != null) {
										if (filters.Length != Children_list.Count) {
										
											Children_list.Clear ();
											Positions_list.Clear ();
										
											if (Self_dynamic_check_rot) {
												Rotations_list.Clear ();
											}
											if (Self_dynamic_check_scale) {
												Scale_list.Clear ();
											}						
										}
									}
								}
							
								for (int i=0; i<filters.Length; i++) {
									Renderer curRenderer = filters [i].GetComponent<Renderer> ();
								
									//v1.7
									if (Self_dynamic_enable & !run_once) {
										if (Children_list != null) {
											if (filters.Length != Children_list.Count) {
												Children_list.Add (filters [i].gameObject.transform);
												Positions_list.Add (filters [i].gameObject.transform.position);
												Rotations_list.Add (filters [i].gameObject.transform.rotation);
												Scale_list.Add (filters [i].gameObject.transform.localScale);
											}
										}
									}
								
									if (curRenderer != null && !curRenderer.enabled) {	
										if (curRenderer.sharedMaterial.name.Contains ("LOD")) {//LOD
										
											curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;//.castShadows = false;
											curRenderer.receiveShadows = false;
										} else {
											curRenderer.enabled = true;
										}
									}
								}
								run_once = true;
							}

							Debug.Log ("LISTS INITIALIZED");
						}
					}			
				
					if (Erase_tree) {
						Erase_tree = false;
						Restore ();//de combine all
						for (int i=Erase_Tree_IDs.Count-1; i>=0; i--) {
							if (Erase_Tree_IDs [i] <= Added_items.Count) {
								DestroyImmediate (Added_items [Erase_Tree_IDs [i] - 1].gameObject);
								Added_items.RemoveAt (Erase_Tree_IDs [i] - 1);
								DestroyImmediate (Added_items_handles [Erase_Tree_IDs [i] - 1].transform.gameObject);
								Added_items_handles.RemoveAt (Erase_Tree_IDs [i] - 1);
							}
						}
					}
				
					if (Decombine) {
					
						//Debug.Log ("INNER DECOMB");
					
						Decombine = false; //disable immediately
						Decombined = true;
					
						Start (); //reenable meshfilters
						MakeActive = false;
						is_active = false;
						Self_dynamic_enable = false;
					
						//clean up
						MeshFilter filter1 = this.gameObject.GetComponent (typeof(MeshFilter)) as MeshFilter;
						if (filter1 != null) {
							Mesh meshD = filter1.sharedMesh;
						
							DestroyImmediate (meshD, true);
							DestroyImmediate (filter1, true);
						} else {					
							if (Destroy_list.Count > 0) {
								for (int i=0; i<Destroy_list.Count; i++) {
									MeshFilter filter11 = Destroy_list [i].GetComponent (typeof(MeshFilter)) as MeshFilter;
									if (filter11 != null) {
										Mesh meshD = filter11.sharedMesh;// this.gameObject.GetComponent <MeshFilter>().sharedMesh;
									
										DestroyImmediate (meshD, true);
										DestroyImmediate (filter11, true);
									}
								}
								for (int i=Destroy_list.Count-1; i>=0; i--) {
									Destroy_list_MF.RemoveAt (i);
									DestroyImmediate (Destroy_list [i]);
									Destroy_list.RemoveAt (i);
								}
							}
							if (Destroy_list64.Count > 0) {
								for (int i=0; i<Destroy_list64.Count; i++) {
								
									if (Destroy_list_MF64 [i] != null) {
										Mesh meshD = Destroy_list_MF64 [i].sharedMesh;
									
										DestroyImmediate (meshD, true);
										DestroyImmediate (Destroy_list_MF64 [i], true);
									}
								}
								for (int i=Destroy_list64.Count-1; i>=0; i--) {
									Destroy_list_MF64.RemoveAt (i);
									DestroyImmediate (Destroy_list64 [i]);
									Destroy_list64.RemoveAt (i);
								}
							}
						}				
					}
				
				
					if (MakeActive) {
					
						//LOD - reenable LOD groups to count in combine
						if (Destroy_list_MF64.Count > 0) {
							for (int i=0; i<Destroy_list_MF64.Count; i++) {
							
								if (Destroy_list_MF64 [i] != null) {
									if (Destroy_list_MF64 [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD")) {
										Destroy_list_MF64 [i].gameObject.SetActive (true);
									} else {
									
									}
								}
							}
						}
						if (Destroy_list_MF.Count > 0) {
							for (int i=0; i<Destroy_list_MF.Count; i++) {
							
								if (Destroy_list_MF [i] != null) {
									if (Destroy_list_MF [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD")) {
									
										Destroy_list_MF [i].gameObject.SetActive (true);
									} else {
									
									}
								}
							}
						}
					
					
					
						is_active = true;//start with false, if active signal it
					
						if (Auto_Disable) {
							if (!Multithreaded) {
								MakeActive = false;
							}
						}
					
						if (skip_every_N_frame > 0) {
							if (count_frames >= skip_every_N_frame) { 
								count_frames = 0; 
							
								return;
							} else {
								count_frames = count_frames + 1;
							}
						
						}
					
						//activate children
						if (1 == 1) {
						
						
							if ((!batching_initialized & Multithreaded) | !Multithreaded) {
								batching_initialized = true;											
							
								if (Multithreaded & MTmethod2) { // if 2ond method, works best if vertices > 65K, but is a bit slower
									Start (); //1. reactivate all and disabled already combined in 2.
								}
							
								if (!Multithreaded) {
									Start ();
								
									MeshFilter filter1 = this.gameObject.GetComponent (typeof(MeshFilter)) as MeshFilter;
									if (filter1 != null) {
										Mesh meshD = filter1.sharedMesh;
									
										DestroyImmediate (meshD, true);
										DestroyImmediate (filter1, true);
									} else {
									
										if (Destroy_list.Count > 0) {
											for (int i=0; i<Destroy_list.Count; i++) {
												MeshFilter filter11 = Destroy_list [i].GetComponent (typeof(MeshFilter)) as MeshFilter;
												if (filter11 != null) {
													Mesh meshD = filter11.sharedMesh;
												
													DestroyImmediate (meshD, true);
													DestroyImmediate (filter11, true);
												}
											}
											for (int i=Destroy_list.Count-1; i>=0; i--) {
												Destroy_list_MF.RemoveAt (i);
												DestroyImmediate (Destroy_list [i]);
												Destroy_list.RemoveAt (i);
											}
										}						
									}
								} else if (MTmethod2) { //We cant destroy them to avoid flicker from dissapearence until threads work is over, BUT must be disabled OR WILL BE ADDED to the mix !!!!
								
									//2. disable previously combined
								
									MeshFilter filter1 = this.gameObject.GetComponent (typeof(MeshFilter)) as MeshFilter;
									if (filter1 != null) {								
										filter1.gameObject.SetActive (false);
									} else {								
										if (Destroy_list.Count > 0) {
											for (int i=0; i<Destroy_list.Count; i++) {
												MeshFilter filter11 = Destroy_list [i].GetComponent (typeof(MeshFilter)) as MeshFilter;
												if (filter11 != null) {											
													filter11.gameObject.SetActive (false);
												}
											}									
										}						
									}
								}
							
								Component[] filters = GetComponentsInChildren (typeof(MeshFilter));					
							
								if (Multithreaded & MTmethod2) { //3. re-enable previously combined to avoid flicker while waiting for threads to end
									MeshFilter filter1 = this.gameObject.GetComponent (typeof(MeshFilter)) as MeshFilter;
									if (filter1 != null) {
										filter1.gameObject.SetActive (true);
									} else {								
										if (Destroy_list.Count > 0) {
											for (int i=0; i<Destroy_list.Count; i++) {
												MeshFilter filter11 = Destroy_list [i].GetComponent (typeof(MeshFilter)) as MeshFilter;
												if (filter11 != null) {
													filter11.gameObject.SetActive (true);
												}
											}
										}						
									}
								}
							
								if (filters != null | 1 == 0) {
									if (filters.Length > 0 | 1 == 0) {
									
										Matrix4x4 myTransform = transform.worldToLocalMatrix;
										Hashtable materialToMesh = new Hashtable ();							
										//Debug.Log ("Filters count = "+filters.Length);
									
										int Group_start = 0;
										int Group_end = filters.Length;									
									
										for (int i=Group_start; i<Group_end; i++) {									
										
											MeshFilter filter = (MeshFilter)filters [i];

											//	filter.sharedMesh = new Mesh();

											if (filter.sharedMesh == null) {
												Debug.Log (filter.name);
												//return;
											}

                                            //v1.7
                                            int maxVerts = 32000;
                                            if (enableAbove64KMesh)
                                            {
                                                maxVerts = 3200000;
                                            }

                                            if (!Multithreaded | (Multithreaded & (
										                                      (filter.sharedMesh != null && filter.sharedMesh.vertexCount <= maxVerts && 
												filter.gameObject.name != "Combined mesh64") | MTmethod2))) {
											
												//Debug.Log (filter.gameObject.name);
											
												Renderer curRenderer = filters [i].GetComponent<Renderer> ();
											
												//LOD - check if material is LOD AND has no shadows, must be combined !!! so enable it
												// then disable it AND enable shadows to mark it, when decombine disable renderer AND disable shadows in LODs
												bool is_LOD = false;
												if (curRenderer.sharedMaterial.name.Contains ("LOD") & curRenderer.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.On//.castShadows 
													& !curRenderer.receiveShadows) {
													curRenderer.enabled = true;
													is_LOD = true;
												}
											
												MeshCombineUtilityINfiniDyGrass.MeshInstance instance = new MeshCombineUtilityINfiniDyGrass.MeshInstance ();
												instance.mesh = filter.sharedMesh;
												if (curRenderer != null && curRenderer.enabled && instance.mesh != null) {
													instance.transform = myTransform * filter.transform.localToWorldMatrix;
												
													Material[] materials = curRenderer.sharedMaterials;
													for (int m=0; m<materials.Length; m++) {
														instance.subMeshIndex = System.Math.Min (m, instance.mesh.subMeshCount - 1);
													
														ArrayList objects = (ArrayList)materialToMesh [materials [m]];
														if (objects != null) {
															objects.Add (instance);
														} else {
															objects = new ArrayList ();
															objects.Add (instance);
															materialToMesh.Add (materials [m], objects);
														}
													}
												
													if (!Multithreaded | (Multithreaded & MTmethod2)) {
														curRenderer.enabled = false;
													}
												
													//LOD
													if (is_LOD) {
														curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;//.castShadows = true;
														curRenderer.receiveShadows = true;
														curRenderer.enabled = false;
													}
												
												}
											} else {
												//Debug.Log (">64k");
											}
										}								
									
									
									
										int List_increase = 0;
										//int mesh_counter=0;
										int counted_entries = 1;
										foreach (DictionaryEntry de  in materialToMesh) {
										
											Material_count++;
											Material_list.Add ((Material)de.Key);
										
											ArrayList elements = (ArrayList)de.Value;
											MeshCombineUtilityINfiniDyGrass.MeshInstance[] instances = (MeshCombineUtilityINfiniDyGrass.MeshInstance[])elements.ToArray (typeof(MeshCombineUtilityINfiniDyGrass.MeshInstance));
										
											List<int> Split_index = new List<int> ();
											Split_index.Add (0);
										
											int vertexes_count = 0;
											for (int i=0; i<instances.Length; i++) {										
											
												vertexes_count = vertexes_count + instances [i].mesh.vertexCount;// filter.sharedMesh.vertexCount;

                                                //v1.7
                                                int maxVerts = 64000;
                                                if (enableAbove64KMesh)
                                                {
                                                    maxVerts = 6400000;
                                                }

												if (vertexes_count > maxVerts) { //64000) { //v1.7
													vertexes_count = 0;
													Split_index.Add (i);
													Debug.Log ("Split at =" + i);
												}
											}								
											//Debug.Log ("Matrial ID ="+de.Key+" "+de.Value);
										
											Splits.Add (Split_index.Count);
											for (int j=0; j<Split_index.Count; j++) {	
											
												if (!Multithreaded) {
												
													if (j < Split_index.Count - 1) {
														Group_start = Split_index [j];
														Group_end = Split_index [j + 1] - 1;
													} else {
														Group_start = Split_index [j];
														Group_end = instances.Length;
													}
												
													MeshCombineUtilityINfiniDyGrass.MeshInstance[] instances_Split = new MeshCombineUtilityINfiniDyGrass.MeshInstance[Group_end - Group_start];
													for (int k=0; k<(Group_end-Group_start); k++) {
														instances_Split [k] = instances [Group_start + k - 0];
													}									
												
													string name = "Combined mesh";											
													if (j < Split_index.Count - 1) {
														name = "Combined mesh64";
													}
												
													GameObject go = new GameObject (name);
													go.transform.parent = transform;
													go.transform.localScale = Vector3.one;
													go.transform.localRotation = Quaternion.identity;
													go.transform.localPosition = Vector3.zero;
													go.AddComponent (typeof(MeshFilter));
													go.AddComponent<MeshRenderer> ();
													go.GetComponent<Renderer> ().material = (Material)de.Key;
													MeshFilter filter = (MeshFilter)go.GetComponent (typeof(MeshFilter));

													//v1.2
													if (Application.isPlaying) {
                                                        if (enableAbove64KMesh && filter.mesh != null)//v1.7
                                                        {
                                                            filter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; //v1.7
                                                        }
                                                        filter.mesh = MeshCombineUtilityINfiniDyGrass.Combine (instances_Split, generateTriangleStrips);
													} else {
                                                        if (enableAbove64KMesh && filter.sharedMesh != null)//v1.7
                                                        {
                                                            filter.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; //v1.7
                                                        }
                                                        filter.sharedMesh = MeshCombineUtilityINfiniDyGrass.Combine (instances_Split, generateTriangleStrips);
													}

													Destroy_list.Add (go);
													Destroy_list_MF.Add (filter);
												
												} else { //MULTITHREADED											
												
													if (j < Split_index.Count - 1) {
														Group_start = Split_index [j];
														Group_end = Split_index [j + 1] - 1;
													} else {
														Group_start = Split_index [j];
														Group_end = instances.Length;
													}
												
													MeshCombineUtilityINfiniDyGrass.MeshInstance[] instances_Split = new MeshCombineUtilityINfiniDyGrass.MeshInstance[Group_end - Group_start];
													for (int k=0; k<(Group_end-Group_start); k++) {
														instances_Split [k] = instances [Group_start + k - 0];
													}
												
													if (!all_threads_started) {
													
														int vertexCount = 0;
														int triangleCount = 0;
														List<int> Combine_Mesh_vertexCount = new List<int> ();
														List<Vector3[]> Combine_Mesh_vertices = new List<Vector3[]> ();
														List<Vector3[]> Combine_Mesh_normals = new List<Vector3[]> ();
														List<Vector4[]> Combine_Mesh_tangets = new List<Vector4[]> ();
													
														List<Vector2[]> Combine_Mesh_uv = new List<Vector2[]> ();
														List<Vector2[]> Combine_Mesh_uv1 = new List<Vector2[]> ();
														List<Color[]> Combine_Mesh_colors = new List<Color[]> ();
														List<int[]> Combine_Mesh_triangles = new List<int[]> ();
														List<int> Has_mesh = new List<int> ();
													
														int count = 0;
														foreach (MeshCombineUtilityINfiniDyGrass.MeshInstance combine in instances_Split) {
															if (combine.mesh) {												
																vertexCount += combine.mesh.vertexCount;												
															}
															//Combine_Mesh_vertexCount[count] = combine.mesh.vertexCount;
															Combine_Mesh_vertexCount.Add (combine.mesh.vertexCount);
															Combine_Mesh_vertices.Add (combine.mesh.vertices);
															Combine_Mesh_normals.Add (combine.mesh.normals);
															Combine_Mesh_tangets.Add (combine.mesh.tangents);
														
															Combine_Mesh_uv.Add (combine.mesh.uv);
															Combine_Mesh_uv1.Add (combine.mesh.uv2);
															Combine_Mesh_colors.Add (combine.mesh.colors);
															Combine_Mesh_triangles.Add (combine.mesh.GetTriangles (combine.subMeshIndex));
														
															count++;
														}
													
														foreach (MeshCombineUtilityINfiniDyGrass.MeshInstance combine in instances_Split) {											
															if (combine.mesh) {												
																triangleCount += combine.mesh.GetTriangles (combine.subMeshIndex).Length;
																Has_mesh.Add (1);
															} else {
																Has_mesh.Add (0);
															}											
														}
													
														ControlCombineChildrenINfiniDyGrass.Meshy mesh = new ControlCombineChildrenINfiniDyGrass.Meshy ();
														mesh.thread_ended = false;
														MeshList.Add (mesh);
														MeshIDList.Add (0); //signal this thread is not done yet
													
														int counter_mesh = List_increase;
														LoomINfiniDyGRASS.RunAsync (() => {
														
															MeshList [counter_mesh] = MeshCombineUtilityINfiniDyGrass.CombineM (counter_mesh, Has_mesh, all_threads_started, instances_Split, 
														                                                             generateTriangleStrips, vertexCount, triangleCount,
														                                                             Combine_Mesh_vertexCount, Combine_Mesh_vertices, Combine_Mesh_normals,
														                                                             Combine_Mesh_tangets, Combine_Mesh_uv, Combine_Mesh_uv1, Combine_Mesh_colors, Combine_Mesh_triangles);
														
														
														
														});
													
														List_increase++;
													
														//thread_started = true;
														threads_started++;
													
													
														if (counted_entries == materialToMesh.Count & j == (Split_index.Count - 1)) {											
														
															all_threads_started = true;
															//mesh_counter = 0;
															//break;
														}
													
													}									
												
												}// END MULTITHREADED ELSE
											
												//Destroy_list.Add(go);
											}
											counted_entries++;
										}//END MATRIAL FOR
									
									}
								}//END check filters array if exists
							
							}// END if !batching_initialized
						
							int threads_ended_real = 0;
							for (int i=0; i<MeshIDList.Count; i++) {
								if (MeshIDList [i] == 0) {
								
								}
								if (MeshList [i].thread_ended) {
									threads_ended_real++;
								}
							}
						
							if (threads_ended_real >= threads_started & Multithreaded & all_threads_started & 1 == 1) {
							
								if (Multithreaded & 1 == 1) {
									//Start ();
								
									MeshFilter filter1 = this.gameObject.GetComponent (typeof(MeshFilter)) as MeshFilter;
									if (filter1 != null) {
										Mesh meshD = filter1.sharedMesh;
									
										DestroyImmediate (meshD, true);
										DestroyImmediate (filter1, true);
									
									} else {
									
										if (Destroy_list.Count > 0) {
											for (int i=0; i<Destroy_list.Count; i++) {
												MeshFilter filter11 = Destroy_list [i].GetComponent (typeof(MeshFilter)) as MeshFilter;
												if (filter11 != null) {
													Mesh meshD = filter11.sharedMesh;// this.gameObject.GetComponent <MeshFilter>().sharedMesh;
												
													DestroyImmediate (meshD, true);
													DestroyImmediate (filter11, true);											
												}
											}
											for (int i=Destroy_list.Count-1; i>=0; i--) {
												Destroy_list_MF.RemoveAt (i);
												DestroyImmediate (Destroy_list [i]);
												Destroy_list.RemoveAt (i);
											}
										}						
									}
								}
							
								//disable filters NOW 
							
								Component[] filters = GetComponentsInChildren (typeof(MeshFilter));	
								int Group_start = 0;
								int Group_end = filters.Length;						
							
								for (int i=Group_start; i<Group_end; i++) {
									MeshFilter filter = (MeshFilter)filters [i];
									Renderer curRenderer = filters [i].GetComponent<Renderer> ();
								
									if (curRenderer != null && curRenderer.enabled) {																
									
										if (filter.gameObject.name == "Combined mesh64" & !MTmethod2) {
										
										} else {
											curRenderer.enabled = false;
										}						
									
									}
								}					
							
								int mesh_counter = 0;
								for (int i = 0; i<Material_list.Count; i++) {
								
									for (int j=0; j<Splits[i]; j++) {
									
										if (MeshList [mesh_counter].vertices != null) {
											//Debug.Log ("Vertex count = "+MeshList[mesh_counter].vertices.Length);
											//Debug.Log ("Mesh counter ="+mesh_counter+" IDs = "+MeshIDList[mesh_counter]);
										}
									
										if (MeshList [mesh_counter].vertices != null & MeshIDList [mesh_counter] != 1) { 
										
											MeshIDList [mesh_counter] = 1;

                                            //v1.7
                                            int maxVerts = 32000;
                                            if (enableAbove64KMesh)
                                            {
                                                maxVerts = 3200000;
                                            }

                                            string name = "Combined mesh";									
											if (MeshList [mesh_counter].vertices.Length > maxVerts) {
												name = "Combined mesh64";
											}
										
											GameObject go = new GameObject (name);
											go.transform.parent = transform;
											go.transform.localScale = Vector3.one;
											go.transform.localRotation = Quaternion.identity;
											go.transform.localPosition = Vector3.zero;
											go.AddComponent (typeof(MeshFilter));
											go.AddComponent<MeshRenderer> ();
											go.GetComponent<Renderer> ().material = Material_list [i];//(Material)de.Key;
											MeshFilter filter = (MeshFilter)go.GetComponent (typeof(MeshFilter));
										
											if (MeshList [mesh_counter].vertices.Length > 0) {

                                                //v1.2
                                                if (Application.isPlaying)
                                                {
                                                    if (enableAbove64KMesh && filter.mesh != null)//v1.7
                                                    {
                                                        filter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; //v1.7
                                                    }
													filter.mesh.name = MeshList [mesh_counter].name;			
													filter.mesh.vertices = MeshList [mesh_counter].vertices;			
													filter.mesh.normals = MeshList [mesh_counter].normals;			
													filter.mesh.colors = MeshList [mesh_counter].colors;			
													filter.mesh.uv = MeshList [mesh_counter].uv;			
													filter.mesh.uv2 = MeshList [mesh_counter].uv1;			
													filter.mesh.tangents = MeshList [mesh_counter].tangents;			
													filter.mesh.triangles = MeshList [mesh_counter].triangles;	
												} else {
                                                    if (filter.sharedMesh != null)
                                                    {
                                                        if (enableAbove64KMesh && filter.sharedMesh != null)//v1.7
                                                        {
                                                            filter.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; //v1.7
                                                        }
                                                        filter.sharedMesh.name =
													MeshList [mesh_counter].name;			
														filter.sharedMesh.vertices = MeshList [mesh_counter].vertices;			
														filter.sharedMesh.normals = MeshList [mesh_counter].normals;			
														filter.sharedMesh.colors = MeshList [mesh_counter].colors;			
														filter.sharedMesh.uv = MeshList [mesh_counter].uv;			
														filter.sharedMesh.uv2 = MeshList [mesh_counter].uv1;			
														filter.sharedMesh.tangents = MeshList [mesh_counter].tangents;			
														filter.sharedMesh.triangles = MeshList [mesh_counter].triangles;	
													}
												}
												//thread_started = false; 
											
												threads_ended = threads_ended + 1;
											}

                                            //v1.7
                                            //int maxVerts = 32000;
                                            //if (enableAbove64KMesh)
                                            //{
                                            //    maxVerts = 3200000;
                                            //}

                                            if (MeshList [mesh_counter].vertices.Length <= maxVerts) { //v1.7
												Destroy_list.Add (go);
												Destroy_list_MF.Add (filter);
											} else {
												Destroy_list64.Add (go);
												Destroy_list_MF64.Add (filter);
											}
										}
										mesh_counter++;								
									}							
								}						
							
								Splits.Clear ();
								Splits = new List<int> ();
								Material_count = 0;
								Material_list.Clear ();
								Material_list = new List<Material> (); 						
							
								batching_initialized = false;
							
								threads_ended = 0;
								threads_started = 0;
								MeshList.Clear ();
								MeshList = new List<Meshy> ();
							
								MeshIDList.Clear ();
								MeshIDList = new List<int> ();
								//MeshList.
								all_threads_started = false;
								mesh_counter = 0;
								if (Auto_Disable) {
									MakeActive = false;
								}				
							
							}			
						
						
						}//end if use bones
						//END IF BONES
					
						//LOD - reenable LOD groups to count in combine
						if (Destroy_list_MF64.Count > 0) {
							for (int i=0; i<Destroy_list_MF64.Count; i++) {
							
								if (Destroy_list_MF64 [i] != null) {
									if (Destroy_list_MF64 [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD")) {
										Destroy_list_MF64 [i].gameObject.SetActive (false);
									} else {
									
									}
								}
							}
						}
						if (Destroy_list_MF.Count > 0) {
							for (int i=0; i<Destroy_list_MF.Count; i++) {
							
								if (Destroy_list_MF [i] != null) {
									if (Destroy_list_MF [i].gameObject.GetComponent<Renderer> ().sharedMaterial.name.Contains ("LOD")) {
										Destroy_list_MF [i].gameObject.SetActive (false);
									} else {
									
									}
								}
							}
						}
					
					}//END MAKEACTIVE
				
				
				}//END hero check

				//v1.1
//			if (PaintedOn != null) {
//				Prev_position = PaintedOn.position;
//				Prev_rotation = PaintedOn.rotation;
//				Prev_scale = PaintedOn.localScale;
//			}
			}
		}//END UPDATE
		
		public void Restore(){
			Decombine = false; //disable immediately
			Decombined = true;
			
			//Debug.Log ("OUTER DECOMB");
			
			Start (); //reenable meshfilters
			MakeActive = false;
			is_active = false;
			Self_dynamic_enable = false;
			
			//clean up
			MeshFilter filter1  = this.gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
			if(filter1!=null){
				Mesh meshD = filter1.sharedMesh;
				
				DestroyImmediate(meshD,true);
				DestroyImmediate(filter1,true);
			}else{					
				if(Destroy_list.Count>0){
					for(int i=0;i<Destroy_list.Count;i++){
						MeshFilter filter11  = Destroy_list[i].GetComponent(typeof(MeshFilter)) as MeshFilter;
						if(filter11!=null){
							Mesh meshD = filter11.sharedMesh;
							
							DestroyImmediate(meshD,true);
							DestroyImmediate(filter11,true);
						}
					}
					for(int i=Destroy_list.Count-1;i>=0;i--){
						Destroy_list_MF.RemoveAt(i);
						DestroyImmediate(Destroy_list[i]);
						Destroy_list.RemoveAt(i);
					}
				}
				if(Destroy_list64.Count>0){
					for(int i=0;i<Destroy_list64.Count;i++){
						
						if(Destroy_list_MF64[i]!=null){
							Mesh meshD = Destroy_list_MF64[i].sharedMesh;
							
							DestroyImmediate(meshD,true);
							DestroyImmediate(Destroy_list_MF64[i],true);
						}
					}
					for(int i=Destroy_list64.Count-1;i>=0;i--){
						Destroy_list_MF64.RemoveAt(i);
						DestroyImmediate(Destroy_list64[i]);
						Destroy_list64.RemoveAt(i);
					}
				}
			}				
		}
		
	}
}