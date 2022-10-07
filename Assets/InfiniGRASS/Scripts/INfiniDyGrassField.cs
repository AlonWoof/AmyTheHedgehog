using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Artngame.INfiniDy {
	
	[ExecuteInEditMode]
	public class INfiniDyGrassField : MonoBehaviour {

        //v1.7 - above 64K - v1.9.9.7
        public bool enableAbove64KMesh = false;

        //v1.9.9.2 - add even scaling option
        public bool evenScaleGrass = false;

		//v1.7 
		public bool AddChopHandler = false;

		//v1.6
		public bool customRot = false;
		public bool lerpRot = false;
		public float lerpRotAmount = 0.5f;
		public bool HorizontAligned = false;//check if brush is aligned along the forward vector than the up vector
		public bool RandomRotRight = false;//random rotation based on right vector 
		public float RandomRotRightMin = -30;
		public float RandomRotRightMax = 30;

		//v1.5 - InfiniGRASS - Performance enchancment by removing original meshes after combined for static grass & disable grower script and colliders overhead
		public bool Disable_after_growth = false;
		public bool WhenCombinerFull = true; // false only when no extra real time painting is required
		public bool Eliminate_original_mesh = false;

		//v1.4 - scale grabbed
		public float ScaleMin_grabbed = 1;//scale factor for play mode grabbed grass

		//v1.3 - restart when eliminated in play mode
		public bool Restart = false;

		//v1.1a
		public bool RotTowards = false;//rotate towards a vector
		public Vector3 RotVector = new Vector3 (0, -1, 0);

		//v1.1
		public bool RandomRot = false;//randomize rotation, for low grass randomize to give a better volume feeling at spots
		public float RandRotMin = -360;
		public float RandRotMax = 360;
		public bool GroupByObject = false;//enable grouping by object, search for batchers that have same object or create one
		public bool ParentToObject = false;//parent batcher to object grass was painted on, otherwise control by other script
		public bool ParentWhileGrowing = false;//parent to paintedOn while growing and this script so collider moves along
		public bool MoveWithObject = false;
		public LayerMask RaycastLayers = -1;
		public bool AvoidOwnColl = false;//avoid the collider when planting

		//GROUPING - if tag based is active (so grass can be planted without the grass manager) or GrassManager is null
		public bool Tag_based = false;//use tags to create combiners
		public InfiniGRASSManager GrassManager;//use combiners lists to group grass(dynamic - static), so multiple grass managers can exist
		public int Type=0;

		//PLAYER
		public bool Tag_based_player = false;//otherwise search for camera
		public string Player_tag = "Player";

		//LOD
		public float LOD_distance = 200;//pass from editor and is read in combiner
		public float LOD_distance1 = 230;//pass from editor and is read in combiner
		public float LOD_distance2 = 260;//pass from editor and is read in combiner
		public float Cutoff_distance = 300;//pass from editor and is read in combiner

		//INFINIGRASS
		public bool Init_rot = false;//apply initial rotation
		public Vector3 Intial_Up_Vector = new Vector3(0,1,0);
		public float MinAvoidDist = 2;//distance to raycast around branch to see if touches terrain or should be scaled down
		public float MinScaleAvoidDist = 4;//distance to raycast around branch to see if touches terrain or should be avoided creation
		public float InteractionSpeed = 1;//define speed of grass interaction
		public float InteractSpeedThres = 0.5f;//define lower speed that will affect grass

		public bool enable_colliders_editor = true;//enable colliders to avoid grass insertion close to other patches
		public bool enable_colliders_playmode = true;//enable non interactive items colliders to avoid grass painting close to other patches or disable for performance
		public Collider GrassCollider;
		public Vector3 colliderScale = new Vector3(3,2,3);
		public Vector2 PosSpread = new Vector2 (8, 8);
		public bool ScaleWhole = false;//scale whole grass than individual, may have Y offset problem

		public float max_ray_dist = 10;
		public Transform PaintedOnOBJ;//object grass is painted on
		public bool GridOnNormal = false;//rotate grass paint grid based on hit normal

		//v1.6 CHOP
		[HideInInspector]
		public float Max_Health = 100;
		[HideInInspector]
		public bool Chop = false;
		[HideInInspector]
		public float max_fall_ammount = 80;

		//InfiniGRASS enum interactions
		public enum GrassAction {Flatten, Chop, None};
		public GrassAction EnableAction = GrassAction.Flatten;
				
		public bool no_shadows_on_dynamic = false;
		bool removed_shadows = false;
		public int Rm_shad_above_lvl = 5;
		public bool no_leaf_shadows = false;
		public bool Rm_receive = true;
		public bool Rm_cast = true;
		
		//
		[HideInInspector]
		public bool Range_leaf_scale=false;
		[HideInInspector]
		public Vector2 Leaf_scale_range = new Vector2(0.5f,0.8f);//leaf scaling randomization and control
		//

		float fall_timer;
		[HideInInspector]
		public float fall_max_time=4;
		[HideInInspector]
		public float fall_speed = 1;
		//Vector3 direction_randomize;
		[HideInInspector]
		public bool Rot_toward_normal = false;
		[HideInInspector]
		public float Rot_speed = 5f;
		[HideInInspector]
		public Vector3 direction_normal = new Vector3(0,1,0);//rotate towards normal, insert normal on instantiation
		[HideInInspector]
		public float max_rot_time=0.5f;
		
		public GameObject player;
		public float Interaction_thres = 15;
		public float Interaction_offset = 5;
		public bool Enable_LOD=false;
		public bool Grow_in_Editor=false;
		bool Has_Run_once = false;//check for editor mode

		public bool InstaGrowLeaves = false;
		public bool Leaves_local_space=true;//parent leaves to brances
		
		[HideInInspector]
		public bool is_grass=false;//grass system activation
		
		//[HideInInspector]
		INfiniDyGrass infiniDyTree;
		public float StartRadi = 0.1f;
		public float StartLength = 1;
		[HideInInspector]
		public float Length_scale=1;
		[HideInInspector]
		public float Extend_scale=1.2f;
		[HideInInspector]
		public Vector2 Min_max_spread = new Vector2(45,45);
		[HideInInspector]
		public bool Spread_Z_separate=false;
		[HideInInspector]
		public Vector2 Min_max_spread_Z = new Vector2(45,45);
		[HideInInspector]
		public bool Spread_Y_separate=false;
		[HideInInspector]
		public Vector2 Min_max_spread_Y = new Vector2(45,45);
		//[HideInInspector]
		public Vector2 Min_Max_Branching = new Vector2(1,4);
		
		//v1.2 per level max
		public List<int> Max_branches_per_level;
		
		public Vector2 Min_Max_Radi = new Vector2(0.2f,0.3f);
		public float lower_length_by = 0.8f;
		public float lower_radi_by = 0.7f;	
		
		public bool Grow_tree = false;

		public float Start_tree_scale = 0.5f;
		public float End_scale = 1;
		//public float Random_scale_bound=0;//end scale randomly around 1
		public float Grow_tree_speed = 0.1f;
		
		//Vector3 prev_root_pos;
		//Vector3 prev_root_rot;
		//Vector3 prev_root_scale;
		
		public float Update_every = 0.02f;
		float last_update_time;
		float start_time;
		
		//new
		//public Color Branch_color;
		public List<INfiniDyGrass> Trees;
		//public bool Animated=false;
		Vector3 Tree_pos ;		
		
		public string BarkPrefab;
		public GameObject BarkPrefabOBJ;//v1.5
		//[HideInInspector]
		public List<string> LeafPrefabs;
		//[HideInInspector]
		public List<string> BranchPrefabs;

		//[HideInInspector]
		public List<GameObject> LeafPrefabsOBJ;//v1.5
		//[HideInInspector]
		public List<GameObject> BranchPrefabsOBJ;//v1.5
		//	public bool Alernate_Leaves=false; // get more leaf types and alternate them - //v1.2 removed
		//	public bool Brance_prefab_per_level=false; - //v1.2 removed
		public List<int> BranchID_per_level;

		[HideInInspector]
		public int Grow_leaves_slow_factor=10;

		//[HideInInspector]
		public float Grow_speed=2f;

		[HideInInspector]
		public float Growth_start_div = 2;
		[HideInInspector]
		public float Growth_angle_div = 2;
		[HideInInspector]
		public float Leaf_start_div = 2;
		[HideInInspector]
		public bool Smooth_leaf_growth = false;
		[HideInInspector]
		public Vector2 Leaf_level_min_max = new Vector2(4,8);
		[HideInInspector]
		public int Leaves_per_branch =10;

		[HideInInspector]
		public bool Use_height=false;//add per height properties

		[HideInInspector]
		public Vector2 Leaf_height_min_max = new Vector2(0,18);
		
		//v1.2 
		[HideInInspector]
		public float Leaf_dist_factor = 0.29f; // control leaf distance

		[HideInInspector]
		public bool CreateForest = true;
		//public int Tree_count = 2;
		
		public bool Interactive_tree=false;//put in lower count holders
		bool Tree_status_previous;
		public int Max_interact_holder_items = 12;
		public int Max_trees_per_group = 12;//for non interactive trees
		public float Max_tree_dist = 15;
		
		void Create_new_pool(string name, int Max_interact_holder_items){
			
			GameObject Instance = new GameObject();			
			Instance.transform.localPosition = Vector3.zero;
			Instance.transform.localEulerAngles= Vector3.zero;
			Instance.name = name;
	//		Instance.tag = name;

			if (Tag_based | GrassManager == null) {
				Instance.tag = name;
			} else {
				
			}


			Forest_holder = Instance;
			
			//find one with empty slots, add tree to slots number
			Combiner = Forest_holder.GetComponent(typeof(ControlCombineChildrenINfiniDyGrass)) as ControlCombineChildrenINfiniDyGrass;
			if(Combiner == null){
				Forest_holder.AddComponent(typeof(ControlCombineChildrenINfiniDyGrass));
				Combiner = Forest_holder.GetComponent(typeof(ControlCombineChildrenINfiniDyGrass)) as ControlCombineChildrenINfiniDyGrass;

                //v1.9.9.7
                if (enableAbove64KMesh)
                {
                    Combiner.enableAbove64KMesh = true;
                }
			}
			if(Combiner != null){

				if (GrassManager != null) {
					if(name =="INfiniDyForestInter"){
						GrassManager.DynamicCombiners.Add(Forest_holder);
					}
					if(name =="INfiniDyForest"){
						GrassManager.StaticCombiners.Add(Forest_holder);
					}
				}

				Combiner.Auto_Disable=true;
				Combiner.MakeActive = false;
				Combiner.Multithreaded = Multithreaded;
				
				Combiner.Max_items = Max_interact_holder_items;
				Combiner.Added_item_count++;
				
				if(Enable_LOD){
					Combiner.DeactivateLOD = true;
				}
				
				if(Combiner.Added_items == null){
					Combiner.Added_items = new List<Transform>();
				}
				Combiner.Added_items.Add(root_tree.transform);
				//INFINIGRASS
				Tree_Holder_Index = Combiner.Added_items.Count-1;
				
				if(Combiner.Added_items_handles == null){
					Combiner.Added_items_handles = new List<INfiniDyGrassField>();
				}
				Combiner.Added_items_handles.Add(this);

				Combiner.Type = Type;
				//v1.1
				Combiner.PaintedOn = PaintedOnOBJ;
				if(ParentToObject){
					Combiner.transform.parent = PaintedOnOBJ;
				}else{
					if(MoveWithObject){
						Combiner.FollowObject = true;
					}
				}
			}			
		}
		
		public void Start () {

            //v2.0
#if UNITY_EDITOR
            int stage = GameObjectTypeLoggingInfiniGRASS.postStageInformation(this.gameObject);
            if (stage == 4)
            {
                Debug.Log("Grass is in prefab edit mode");
                return;
            }
#endif

            //Debug.Log ("started");

            if (Application.isPlaying) {
			
				rotation_over = false;

				//v1.3 - parent the script to object
				if(MoveWithObject && PaintedOnOBJ != null){
					this.gameObject.transform.parent = PaintedOnOBJ;
				}

			}

			//helpers for undo
			if (!Application.isPlaying) {
			
				//if has grassmanager, but no root tree, regrow in editor
				if(GrassManager != null && root_tree == null && Leaf_pool == null){

					Combiner = null;
					Grow_in_Editor = false;
					growth_over = false;
					Registered_Brances.Clear ();//
					//forest.root_tree = null;
					Branch_grew.Clear ();
					Registered_Leaves.Clear ();//
					Registered_Leaves_Rot.Clear ();//
					batching_ended = false;
					Branch_levels.Clear ();
					BranchID_per_level.Clear ();
					//forest.Grass_Holder_Index = 0;
					Grow_level = 0;
					Grow_tree_ended = false;
					Health = Max_Health;
					is_moving = false;
					Leaf_belongs_to_branch.Clear ();
					scaleVectors.Clear ();
					Leaves.Clear ();
					//forest.Tree_Holder_Index = 0;										
					Forest_holder = null;										
					//MakeActive = false;
					//int index = forest.Tree_Holder_Index; 

					Grow_in_Editor = true;
					GameObject LEAF_POOL = new GameObject ();
					LEAF_POOL.transform.parent = transform;
					Leaf_pool = LEAF_POOL;
					transform.localScale = Vector3.one;
					GrassManager.Grasses.Add(this);

					Grass_Holder_Index = GrassManager.Grasses.Count-1;

					GrassManager.GrassesType.Add(Type);
				}
			
			}


			if(Application.isPlaying){

				//v1.1
				if(MoveWithObject && PaintedOnOBJ != null && Grow_tree){//v1.1a
					//ParentToObject = true;
					ParentWhileGrowing = true;
					GroupByObject = true;
				}

				StartP ();

				//v1.1
				if(ParentWhileGrowing){
					this.transform.parent = PaintedOnOBJ;
					root_tree.transform.parent = PaintedOnOBJ;
				}

			
				//Debug.Log("aaa00");
				//v1.1a
				//if(!Grow_tree & GrassManager.UnGrown){ //v1.2a  //if(!Grow_tree){	//v1.7
				if(!Grow_tree & GrassManager.UnGrown){ 
					if( Registered_Brances[0].localScale.x < End_scale){
						for (int i = 0; i<Registered_Brances.Count; i++) {
							//Registered_Brances[i].localScale += Vector3.one*Grow_speed*Time.deltaTime;  //v1.7.5
							//Registered_Brances[i].transform.localScale = Registered_Brances[i].transform.localScale*End_scale; 
							Registered_Brances[i].localScale = End_scale*Registered_Brances[i].localScale;  //v1.7.5
						}
						//Debug.Log("aaa0");
					}else if(Registered_Brances[0].localScale.x >= End_scale){ //v1.7.5 also check equal
						Registered_Brances[0].localScale = End_scale*Vector3.one;
						for (int i = 1; i<Registered_Brances.Count; i++) {
								Registered_Brances[i].localScale = End_scale*Registered_Brances[i].localScale; 
						//Registered_Brances[i].localScale = End_scale*Vector3.one; 
						//Registered_Brances[i].localScale -= Vector3.one*Grow_speed*Time.deltaTime; 
						}
						//Debug.Log("aaa");
					}
				}
			}

			//v1.6 - Run lateupdate to align trees proper in editor
			//if (!Application.isPlaying) {
				//LateUpdateINNER();
				//Debug.Log ("fd");
			//}

		}

		GameObject[] Forest_holder1;

		void StartP () {			



			Health=Max_Health;
			
			//	if((Application.isPlaying & !Grow_in_Editor) | (!Application.isPlaying & Grow_in_Editor & !Has_Run_once & Registered_Brances.Count < 1)){
			
			//v1.2 - remove the run_once check here, in case start does not run fully in editor, so update can re-run until it is created properly
			if((Application.isPlaying & !Grow_in_Editor) | (!Application.isPlaying & Grow_in_Editor  & Registered_Brances.Count < 1 & !Grow_tree_ended)){
				
				//Has_Run_once = true; //v1.2 move to end
				//Debug.Log ("start over");
				//player = GameObject.FindGameObjectWithTag("Player");

				//Infinigrass
				if (Tag_based_player) {
					if (player == null) {
						player = GameObject.FindGameObjectWithTag (Player_tag);
					}
				} else {
					if (player == null) {
						if(Camera.main != null){
							player = Camera.main.gameObject;
						}
					}
				}

				if(player != null){
					Prev_hero_pos = player.transform.position; //INFINIGRASS
				}

				if(Trees == null){
					Trees = new List<INfiniDyGrass>();
				}
				
				last_update_time = Time.fixedTime;
				start_time = Time.fixedTime;
				
				infiniDyTree = new INfiniDyGrass();
				infiniDyTree.is_root = true;
				Tree_pos = this.gameObject.transform.position;
				infiniDyTree.INfiniDyForestC = this;
				infiniDyTree.generateTree(StartRadi,StartLength,Color.blue,Tree_pos,null);


				//v1.6 - Run lateupdate to align trees proper in editor
				//if (!Application.isPlaying) {
					//LateUpdateINNER();
					//Debug.Log ("fd");
				//}

				infiniDyTree.INfiniDyForestOBJ = this.gameObject;
				infiniDyTree.Level = 1;
				

				//v1.2
				if(!Application.isPlaying & 1==1){
					for(int i=GrassManager.DynamicCombiners.Count-1;i>=0;i--){
						if(GrassManager.DynamicCombiners[i] == null){
							GrassManager.DynamicCombiners.RemoveAt(i);
						}
					}
					for(int i=GrassManager.StaticCombiners.Count-1;i>=0;i--){
						if(GrassManager.StaticCombiners[i] == null){
							GrassManager.StaticCombiners.RemoveAt(i);
						}
					}
				}

				//check if forest holder has been defined by scene item, if not search for tag "INfiniDyForest" and if not found create object with it.
				if(Forest_holder == null){
					//Debug.Log ("1asasas");
					if(Interactive_tree){

						//InfniGRASS
						if(Tag_based | GrassManager == null){
							Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForestInter");
						}else{
							Forest_holder1 = GrassManager.DynamicCombiners.ToArray();
							//Debug.Log(Forest_holder1.Length);
						}

						if(Forest_holder1 == null | Forest_holder1.Length < 1){
							Create_new_pool("INfiniDyForestInter", Max_interact_holder_items);
							//Debug.Log ("asasas");
						}else{
							//find one with empty slots, add tree to slots number
							bool found = false;
							for(int i=0;i<Forest_holder1.Length;i++){
								Combiner = Forest_holder1[i].GetComponent(typeof(ControlCombineChildrenINfiniDyGrass)) as ControlCombineChildrenINfiniDyGrass;
								
								//check if far away from other trees
								bool Enter = true;
								for(int k = 0;k<Combiner.Added_items.Count;k++){
									if(Vector3.Distance(root_tree.transform.position,Combiner.Added_items[k].position) > Max_tree_dist){
										Enter = false;
									}
								}

								//Debug.Log("Enter = "+Enter);

								//v1.2
								if(!Combiner.gameObject.activeInHierarchy | !Combiner.enabled){
									Enter=false;
								}

								//if near, add tree / grass to exiting combiner
								//if(Enter & Combiner.Added_item_count < Combiner.Max_items &  Combiner.Max_items == Max_interact_holder_items & !Combiner.LODed){
								if(Enter & Combiner.Added_item_count < Combiner.Max_items &  Combiner.Max_items == Max_interact_holder_items & !Combiner.LODed 
								   & Combiner.Type == Type & (!GroupByObject | (GroupByObject & (Combiner.PaintedOn == PaintedOnOBJ) ) ) ){


									Forest_holder = Forest_holder1[i];
									Combiner.Added_item_count++;
									
									if(Enable_LOD){
										Combiner.DeactivateLOD = true;
									}
									
									if(Combiner.Added_items == null){
										Combiner.Added_items = new List<Transform>();
									}
									Combiner.Added_items.Add(root_tree.transform);
									//INFINIGRASS
									Tree_Holder_Index = Combiner.Added_items.Count-1;
									
									if(Combiner.Added_items_handles == null){
										Combiner.Added_items_handles = new List<INfiniDyGrassField>();
									}
									Combiner.Added_items_handles.Add(this);
									
									found=true;
									break;
								}
							}
							if(!found){
								Create_new_pool("INfiniDyForestInter", Max_interact_holder_items);
								//Debug.Log ("2asasas");
							}
						}
					}else{
						//GameObject[] Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForest");

						if(Tag_based | GrassManager == null){
							Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForest");
						}else{
							Forest_holder1 = GrassManager.StaticCombiners.ToArray();
							//Debug.Log(Forest_holder1.Length);
						}


						if(Forest_holder1 == null | Forest_holder1.Length < 1){
							Create_new_pool("INfiniDyForest", Max_trees_per_group); //Debug.Log ("OOO");
						}else{
							//find one with empty slots, add tree to slots number
							bool found = false;
							for(int i=0;i<Forest_holder1.Length;i++){
								Combiner = Forest_holder1[i].GetComponent(typeof(ControlCombineChildrenINfiniDyGrass)) as ControlCombineChildrenINfiniDyGrass;
								
								//check if far away from other trees
								bool Enter = true;
								for(int k = 0;k<Combiner.Added_items.Count;k++){
									if(Vector3.Distance(root_tree.transform.position,Combiner.Added_items[k].position) > Max_tree_dist){
										Enter = false;
									}
								}

								//v1.2
								if(!Combiner.gameObject.activeInHierarchy | !Combiner.enabled){
									Enter=false;
								}
								
								if(Enter & Combiner.Added_item_count < Combiner.Max_items &  Combiner.Max_items == Max_trees_per_group & !Combiner.LODed
								   & Combiner.Type == Type  & (!GroupByObject | (GroupByObject & (Combiner.PaintedOn == PaintedOnOBJ) ) ) ){


									Forest_holder = Forest_holder1[i];
									Combiner.Added_item_count++;
									
									if(Enable_LOD){
										Combiner.DeactivateLOD = true;
									}
									
									if(Combiner.Added_items == null){
										Combiner.Added_items = new List<Transform>();
									}
									Combiner.Added_items.Add(root_tree.transform);
									//INFINIGRASS
									Tree_Holder_Index = Combiner.Added_items.Count-1;
									
									if(Combiner.Added_items_handles == null){
										Combiner.Added_items_handles = new List<INfiniDyGrassField>();
									}
									Combiner.Added_items_handles.Add(this);
									
									found=true;
									break;
								}
							}
							if(!found){
								Create_new_pool("INfiniDyForest", Max_trees_per_group);
							}
						}
					}//END ELSE					
				}
				
				//prev_root_pos = this.transform.position;
				//prev_root_rot = this.transform.eulerAngles;
				//prev_root_scale = this.transform.localScale;
				
				Tree_status_previous = Interactive_tree;//initial status saved to check against later
				
				if(Grow_tree){
					//root_tree.transform.localScale = Start_tree_scale*Vector3.one;	//INFNIGRASS	
					for (int i = 0; i<Registered_Brances.Count;i++){
						Registered_Brances[i].localScale = Registered_Brances[i].localScale * Start_tree_scale;
					}
				}
				
				if(InstaGrowLeaves){
					for (int i = 0; i<Registered_Brances.Count;i++){
						
						if(Registered_Brances[i] != null){
							if(Grow_level == Branch_levels[i] | 1==1){
								if(Registered_Leaves.Count != Leaves.Count){
									
									//int Diff = Registered_Leaves.Count - Leaves.Count;									
									for (int j = 0; j<Registered_Leaves.Count;j++){
										if(Leaf_belongs_to_branch[j] == i){
											
											int Index = 0;
											GameObject Leaf = null;// = new GameObject();
											//Leaf.name = "leaf";

											//if(Alernate_Leaves ){ //v1.2 removed bool option, grab by list count
											if(LeafPrefabs.Count > 0){
												if(LeafPrefabs.Count > 1){
													Index = Random.Range(0,LeafPrefabs.Count);
												}
											}else{
												if(LeafPrefabsOBJ.Count > 1){
													Index = Random.Range(0,LeafPrefabsOBJ.Count);
												}
											}											

											if(LeafPrefabs.Count > 0){
												Leaf =(GameObject)Object.Instantiate(Resources.Load (LeafPrefabs[Index]));
												Leaf.name = "Leaf";
											}else{

												//InfiniGRASS
												if(LeafPrefabsOBJ.Count > Index && LeafPrefabsOBJ[Index] != null){
													Leaf =(GameObject)Object.Instantiate(LeafPrefabsOBJ[Index]);
												}else{
													Leaf = new GameObject();
												}
												Leaf.name = "Leaf";
											}

											//v1.2
											Leaf.transform.position = Registered_Leaves[j];//Registered_Leaves[Leaves.Count];
											Leaf.transform.rotation = Registered_Leaves_Rot[j];//Registered_Leaves_Rot[Leaves.Count];
											
											if(!Leaves_local_space){
												if(!is_grass){
													Leaf.transform.parent = Registered_Brances[i];
													Leaf.transform.localPosition = new Vector3(0,Mathf.Abs(Leaf.transform.localPosition.y),0);
												}
												Leaf.transform.parent = Leaf_pool.transform;
											}else{
												Leaf.transform.parent = Registered_Brances[i];
												if(!is_grass){
													Leaf.transform.localPosition = new Vector3(0,Mathf.Abs(Leaf.transform.localPosition.y),0);
												}
											}											
											
											//Debug.Log ("AAA");
											//v1.5
											if(Range_leaf_scale){
												Vector3 Scale = Leaf.transform.localScale;
												float Randomizer = Random.Range(Leaf_scale_range.x,Leaf_scale_range.y);
												Leaves_scales.Add(Scale * Randomizer);
											}else{
												Leaves_scales.Add(Leaf.transform.localScale);
											}
											
											Leaf.transform.localScale = Leaf.transform.localScale / Leaf_start_div;
											Leaves.Add(Leaf.transform);								
											
											if(Random.Range(1,Grow_leaves_slow_factor)==2){
												//break;
											}
										}
										if(Smooth_leaf_growth){
											if(j < Leaves.Count){
												if(Leaves[j] != null){
													if(Leaves[j].transform.localScale.x < Leaves_scales[j].x){
														Leaves[j].transform.localScale = Leaves[j].transform.localScale + 0.1f*Grow_speed*Time.deltaTime*new Vector3(1,1,1); 
													}
												}
											}
										}
									}
								}
								Branch_grew[i] = 1;
							}}}
				}

//				if (Grow_in_Editor && growth_over && !Grow_tree_ended) {
//					if (!Application.isPlaying) {
//						LateUpdateINNER();
//						Debug.Log ("NOW="+Registered_Brances.Count);
//					}
//					root_tree.transform.parent = Forest_holder.transform;
//					Combiner.MakeActive  =  true;
//					Grow_tree_ended = true;
//				}

				if(Grow_in_Editor & !growth_over){
				//if(Grow_in_Editor){
					//register all growth over flags here, prepare combiner ? and parent tree

					//INFINIGRASS
					if(Registered_Brances.Count == 0){
						ScaleWhole = true;
						//Debug.Log ("fdAAAA");
					}
					if(ScaleWhole  | Registered_Brances.Count == 0){
						if(root_tree.transform.localScale.x != End_scale){
							root_tree.transform.localScale = End_scale*Vector3.one;
						}else{
							root_tree.transform.localScale = End_scale*Vector3.one;// MAKE IT EXACTLY same, so self dynamic baching does not kick in !!!
						}
					}else{
                        if (evenScaleGrass) //v1.9.9.2
                        {
                            for (int i = 1; i < Registered_Brances.Count; i++)
                            {  //v1.7.5 must start at 1, not 0
                                Registered_Brances[i].localScale = End_scale * Vector3.one;//v1.9.9.2
                            }
                        }
                        else
                        {
                            if (Registered_Brances[0].localScale.x < End_scale)
                            {
                                Registered_Brances[0].localScale = End_scale * Vector3.one * ScaleMin_grabbed;//v1.7.5
                                for (int i = 1; i < Registered_Brances.Count; i++)
                                {
                                    //Registered_Brances[i].transform.localScale += Vector3.one*0.1f*Time.deltaTime; 
                                    Registered_Brances[i].localScale = Registered_Brances[i].localScale * End_scale * ScaleMin_grabbed;
                                }
                            }
                            else
                            {
                                Registered_Brances[0].localScale = End_scale * Vector3.one;
                                for (int i = 1; i < Registered_Brances.Count; i++)
                                {  //v1.7.5 must start at 1, not 0
                                    Registered_Brances[i].localScale = End_scale * Registered_Brances[i].localScale;
                                }
                            }
                        }
					}

					//v1.6
					//Combiner.MakeActive  =  false;

					//Combiner.Decombine = true;
					//v1.6 - Run lateupdate to align trees proper in editor
//					if (!Application.isPlaying) {
//						LateUpdateINNER();
//						Debug.Log (Registered_Brances.Count);
//					}
					if (Application.isPlaying) {					
						root_tree.transform.parent = Forest_holder.transform;
					}

					//root_tree.transform.parent = Forest_holder.transform;




					//Combiner.Restore ();
					//v1.2
					Combiner.MakeActive  =  true;
					//Combiner.MakeActive  =  false;
					//root_tree.transform.parent = Forest_holder.transform;


					
					Grow_tree_ended = true;
					
					growth_over = true;
				}
				
				Has_Run_once = true; //v1.2 moved to end
			}//END if app is playing check
			
			if(Grow_in_Editor){
				
				Grow_tree_ended = true;
				
				growth_over = true;
			}


			if (GrassCollider == null) {
				//add collider
				this.gameObject.AddComponent<BoxCollider> ().isTrigger = true;
			//	transform.localScale = new Vector3 (transform.localScale.x * colliderScale.x, transform.localScale.y * colliderScale.y, transform.localScale.z * colliderScale.z); //v1.2a
			//	transform.localScale = colliderScale;

				//v1.7
				this.gameObject.AddComponent<GrassChopCollider> ().TreeHandler = this;
				if (AddChopHandler) {
					//this.gameObject.AddComponent<GrassChopCollider> ().TreeHandler = this;
				}
				GrassCollider = this.gameObject.GetComponent<BoxCollider> ();

				//v1.2a - scale collider
				(GrassCollider as BoxCollider).size = colliderScale;

				//v1.4 - if parented, scale accordingly
				//if(this.transform.parent != null && this.transform.parent.name != "Grass Holder"){
				//	(GrassCollider as BoxCollider).size = new Vector3(colliderScale.x/this.transform.parent.localScale.x,colliderScale.y/this.transform.parent.localScale.y,colliderScale.z/this.transform.parent.localScale.z);
				//}

				if (Application.isPlaying) {
					if (!Interactive_tree) {
						if (enable_colliders_playmode) {
							if (!GrassCollider.enabled) {
								GrassCollider.enabled = true;
							}
						} else {
							if (GrassCollider.enabled) {
								GrassCollider.enabled = false;
							}
						}
					} else {
						if (!GrassCollider.enabled) {
							GrassCollider.enabled = true;
						}
					}
				} else {
					if (enable_colliders_editor) {
						if (!GrassCollider.enabled) {
							GrassCollider.enabled = true;	//enable in editor
						}
					}
				}

				//GrassCollider.enabled = false;
			} else {
			
				if(this.transform.localScale != Vector3.one && this.transform.parent.name == "Grass Holder"){//v1.4
					colliderScale = this.transform.localScale;
					this.transform.localScale = Vector3.one;
				}
				//transform.localScale = colliderScale;
				//v1.2a - scale collider
				(GrassCollider as BoxCollider).size = colliderScale;

				//v1.4 - if parented, scale accordingly
				//if(this.transform.parent != null && this.transform.parent.name != "Grass Holder"){
					//(GrassCollider as BoxCollider).size = new Vector3(colliderScale.x/this.transform.parent.localScale.x,colliderScale.y/this.transform.parent.localScale.y,colliderScale.z/this.transform.parent.localScale.z);
				//}
			
			}

			//v1.6 - Run lateupdate to align trees proper in editor
			//if (!Application.isPlaying) {
				//LateUpdateINNER();
				//Debug.Log ("fd");
			//}

		}


		public bool rotation_over = false;

		//v1.6
		//INFNIGRASS - update normal towards rotation after grown
		void LateUpdate(){

            //v2.0
#if UNITY_EDITOR
            int stage = GameObjectTypeLoggingInfiniGRASS.postStageInformation(this.gameObject);
            if (stage == 4)
            {
                Debug.Log("Grass is in prefab edit mode");
                return;
            }
#endif

            //INFINIGRASS  - rotate to normal
            if (!rotation_over) {
				LateUpdateINNER ();
				rotation_over = true;

				//v1.6 - put in batching later, so it has the right rotation
				if (!Application.isPlaying) {					
					root_tree.transform.parent = Forest_holder.transform;
				}
			}
		}

		//INFNIGRASS - update normal towards rotation after grown
		public void LateUpdateINNER(){

			//INFINIGRASS  - rotate to normal
			//if (!rotation_over) {

					for (int i = 0; i<Registered_Brances.Count; i++) {							

						//v1.6
						if (customRot) {
							if (RotTowards) {
								if (lerpRot) {
									if (HorizontAligned) {
										Registered_Brances [i].forward = Vector3.Lerp (Registered_Brances [i].forward, RotVector, lerpRotAmount);
									} else {
										Registered_Brances [i].up = Vector3.Lerp (Registered_Brances [i].up, RotVector, lerpRotAmount);
									}
								} else {
									if (HorizontAligned) {
										Registered_Brances [i].forward = RotVector;
									} else {
										Registered_Brances [i].up = RotVector;
									}
								}
								//Debug.Log ("aaa");
							} else {
		//								if (lerpRot) {
		//									Registered_Brances [i].forward = Vector3.Lerp (Registered_Brances [i].forward, RotVector, lerpRotAmount);
		//								} else {
		//									Registered_Brances [i].forward = RotVector;
		//								}
							}
							if (RandomRot) {
								Registered_Brances [i].Rotate (Registered_Brances [i].up, Random.Range (RandRotMin, RandRotMax));
							}
							if (RandomRotRight) {
								Registered_Brances [i].Rotate (Registered_Brances [i].right, Random.Range (RandomRotRightMin, RandomRotRightMax));
							}

						} else {
					
							Ray ray = new Ray (Registered_Brances [i].position +  Intial_Up_Vector * max_ray_dist, - Intial_Up_Vector);

							RaycastHit hit = new RaycastHit ();
							//if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {

							if(!AvoidOwnColl){
								RaycastLayers = -1;
							}

							if (Physics.Raycast (ray, out hit, Mathf.Infinity,RaycastLayers)) { //v1.1

								//Registered_Brances [i].up = hit.normal;

								if(RotTowards & hit.normal.y <=0){
									//Registered_Brances[i].Rotate(Registered_Brances[i].up,RotVector);
									//Registered_Brances[i].forward = RotVector;
									Registered_Brances[i].LookAt(hit.point + RotVector,hit.normal);
								}else{
									//v1.2
									if(RotTowards){	
										//Registered_Brances[i].LookAt(hit.point + Vector3.Lerp(Vector3.Lerp(hit.normal,new Vector3(hit.point.x,0,hit.point.z),0.5f),RotVector,0.5f),hit.normal);	
										Registered_Brances[i].LookAt(hit.point + RotVector,hit.normal);
									}else{
										Registered_Brances [i].up = hit.normal;
									}
								}

								//v1.1
								if(RandomRot){
									Registered_Brances[i].Rotate(Registered_Brances[i].up,Random.Range(RandRotMin,RandRotMax));
									//Registered_Brances[i].forward = Vector3.Lerp(Registered_Brances[i].forward,Vector3.Cross(Registered_Brances[i].forward,Registered_Brances[i].up),Random.Range(0,1));
									//Registered_Brances[i].forward = Vector3.Lerp(Registered_Brances[i].forward,Registered_Brances[i].right,Random.Range(0,1));
								}
								//v1.6
								if (RandomRotRight) {
									Registered_Brances [i].Rotate (Registered_Brances [i].right, Random.Range (RandomRotRightMin, RandomRotRightMax));
								}
								//Debug.Log(this.gameObject.name);

							}
						}

					}
				//rotation_over = true;
			//}

		}

		[HideInInspector]
		public bool Grow_angles = false; 
		
		[HideInInspector]
		public List<int> Branch_levels;
		[HideInInspector]
		public List<Vector3> scaleVectors;
		
		[HideInInspector]
		public List<Transform> Registered_Brances;
		[HideInInspector]
		public List<Vector3> Start_Scales;
		[HideInInspector]
		public List<Vector3> Registered_Leaves; //register leaves from each Branch and instantiate them in update here, then rotate them 
		[HideInInspector]
		public List<Quaternion> Registered_Leaves_Rot;
		[HideInInspector]
		public List<int> Leaf_belongs_to_branch;
		public GameObject Leaf_pool; //parent them herePlayer
		[HideInInspector]
		public List<Transform> Leaves; //keep instantiated leaves transofrms here
		[HideInInspector]
		public List<Vector3> Leaves_scales;
		
		int Branches_done;
		 
		[HideInInspector]
		public int Height_levels = 1;
		[HideInInspector]
		public float Height_separation = 1;
		[HideInInspector]
		public float Height_reduce = 0.05f;
		[HideInInspector]
		public float Level1Decline = 0;
		[HideInInspector]
		public bool Reduce_angle_with_height = false;
		
		public bool Multithreaded = false;
		[HideInInspector]
		public Vector3 Height_offset = new Vector3(0,0,0);
		[HideInInspector]
		public bool Decouple_from_bark = false;

		//INFINIGRASS
		public int Tree_Holder_Index = 0;//keep index in holder script list
		public int Grass_Holder_Index = 0;//index in grass manager
		
		public void Update () {

            //v2.0
#if UNITY_EDITOR
            int stage = GameObjectTypeLoggingInfiniGRASS.postStageInformation(this.gameObject);
            if (stage == 4)
            {
                Debug.Log("Grass is in prefab edit mode");
                return;
            }
#endif

            //v1.3
            if (Restart) {
			
				Start ();
				Restart=false;
			
			}

			//v1.1 - Check if combiner is null and destroy if it is
			if (Combiner == null & Grow_tree_ended) {
			//if (Combiner == null && Application.isPlaying) {
				//if(PaintedOnOBJ != null){ //v1.2
					//remove from script
					GrassManager.Grasses.RemoveAt (Grass_Holder_Index);
					GrassManager.GrassesType.RemoveAt (Grass_Holder_Index);
					for (int i=0; i<GrassManager.Grasses.Count; i++) {
						GrassManager.Grasses [i].Grass_Holder_Index = i;
					}
					DestroyImmediate(this.gameObject);
				//}
			}


			//v1.4 - out of check
			//Infinigrass
			if (Tag_based_player) {
				if (player == null) {
					player = GameObject.FindGameObjectWithTag (Player_tag);
					
				}
			} else {
				if (player == null) {
						if(Camera.main != null){
							player = Camera.main.gameObject;
						}
					
				}
			}

			//INFINIGRASS
			if(Interactive_tree | (!Interactive_tree && !growth_over && !Grow_tree_ended)){ 

			if (GrassCollider != null) {
				if(Application.isPlaying){
					if(!Interactive_tree) {
						if(enable_colliders_playmode){
							if(!GrassCollider.enabled){
								GrassCollider.enabled = true;
							}
						}else{
							if(GrassCollider.enabled){
								GrassCollider.enabled = false;
							}
						}
					}else{
						if(!GrassCollider.enabled){
							GrassCollider.enabled = true;
						}
					}
				}else{
					if(enable_colliders_editor){
						if(!GrassCollider.enabled){
							GrassCollider.enabled = true;	//enable in editor
						}
					}
				}
			}


			//if((Grow_in_Editor & !Application.isPlaying & !Has_Run_once)){
			//v1.2
			if((Grow_in_Editor & !Application.isPlaying & (!Has_Run_once | (Has_Run_once & root_tree == null)) )){
				//Start ();
				
				//reset dividers, full grow at start
				Leaf_start_div = 1;
				Growth_angle_div = 1;
				Growth_start_div = 1;
				Grow_tree = false;
				Smooth_leaf_growth = false;
				InstaGrowLeaves = true;
				
				StartP (); //v1.2

				//INFINIGRASS

					//v1.1a
					if(!Grow_tree & !Has_Run_once){ //v1.5 - fix scaling issue when painting in editor
						if( Registered_Brances[0].localScale.x < End_scale){
							for (int i = 0; i<Registered_Brances.Count; i++) {
								Registered_Brances[i].localScale += Vector3.one*Grow_speed*Time.deltaTime; 
								//Registered_Brances[i].transform.localScale = Registered_Brances[i].transform.localScale*End_scale; 
							}
						}else if(Registered_Brances[0].localScale.x > End_scale){
							Registered_Brances[0].localScale = End_scale*Vector3.one;
							for (int i = 1; i<Registered_Brances.Count; i++) {
								Registered_Brances[i].localScale = End_scale*Registered_Brances[i].localScale; 
								//Registered_Brances[i].localScale = End_scale*Vector3.one; 
								//Registered_Brances[i].localScale -= Vector3.one*Grow_speed*Time.deltaTime; 
							}
							//Debug.Log("aaa");
						}
					}
					
					//if (!Application.isPlaying) {//rotate here if application not playing
						//Debug.Log(Registered_Brances.Count);
						for (int i = 0; i<Registered_Brances.Count; i++) {
							
							
							Ray ray = new Ray (Registered_Brances [i].position +  Intial_Up_Vector * max_ray_dist, - Intial_Up_Vector);
							
							RaycastHit hit = new RaycastHit ();
							//if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
							
							if(!AvoidOwnColl){
								RaycastLayers = -1;
							}
							
							if (Physics.Raycast (ray, out hit, Mathf.Infinity,RaycastLayers)) { //v1.1
								
								//Registered_Brances [i].up = hit.normal;
								
								if(RotTowards & hit.normal.y <=0){
									//Registered_Brances[i].Rotate(Registered_Brances[i].up,RotVector);
									//Registered_Brances[i].forward = RotVector;
									Registered_Brances[i].LookAt(hit.point + RotVector,hit.normal);
								}else{
									//v1.2
									if(RotTowards){	
										Registered_Brances[i].LookAt(hit.point + RotVector,hit.normal);									
									}else{
										Registered_Brances [i].up = hit.normal;
									}
								}
								
								//v1.1
								if(RandomRot){
									Registered_Brances[i].Rotate(Registered_Brances[i].up,Random.Range(RandRotMin,RandRotMax));
									//Registered_Brances[i].forward = Vector3.Lerp(Registered_Brances[i].forward,Vector3.Cross(Registered_Brances[i].forward,Registered_Brances[i].up),Random.Range(0,1));
									//Registered_Brances[i].forward = Vector3.Lerp(Registered_Brances[i].forward,Registered_Brances[i].right,Random.Range(0,1));
								}
								
								//Debug.Log(this.gameObject.name);
								
							}						
						}
						//rotation_over = true;
					//}


				foreach(GrassChopCollider collider_script in root_tree.GetComponentsInChildren<GrassChopCollider>(true))
				{
					collider_script.TreeHandler = this;
				}

			}
			
			if(Application.isPlaying){
				
				if(Time.fixedTime-last_update_time > Update_every)
				{					
					last_update_time = Time.fixedTime;
					
					bool All_Branches_done = true;
					for (int i = 0; i < Branch_grew.Count; i++){
						if(Branch_grew[i] ==0){
							All_Branches_done = false;
						}
					}
					
					if(!growth_over){
						
						for (int i = 0; i<Registered_Brances.Count;i++){
							if(Registered_Brances[i] != null){
								if(Grow_level == Branch_levels[i]){
									if(Registered_Brances[i].localScale.x < scaleVectors[i].x*Extend_scale 
									   | Registered_Brances[i].localScale.y < scaleVectors[i].y*Extend_scale 
									   |Registered_Brances[i].localScale.z < scaleVectors[i].z*Extend_scale ){
										
										if(Grow_level < 0 | 1==0){
											//Registered_Brances[i].localScale = new Vector3(Registered_Brances[i].localScale.x+0.01f, Registered_Brances[i].localScale.y+0.01f, Registered_Brances[i].localScale.z+0.01f);
										}else{											
											//Registered_Brances[i].localScale = Vector3.Lerp(Registered_Brances[i].localScale, scaleVectors[i]*Extend_scale*2, Grow_speed*Time.deltaTime);	

											//v1.2a
											Registered_Brances[i].localScale = Vector3.Lerp(Registered_Brances[i].localScale, Registered_Brances[i].localScale*End_scale, Grow_speed*Time.deltaTime);
										}
									}else{
										bool leaves_grown=true;
										if(Registered_Leaves.Count != Leaves.Count){										
																				
											
											for (int j = 0; j<Registered_Leaves.Count;j++){
												if(Leaf_belongs_to_branch[j] == i){													


													//v1.5
													int Index = 0;
													GameObject Leaf = null;
													
													//if(Alernate_Leaves ){ //v1.2 removed bool option, grab by list count
													if(LeafPrefabs.Count > 0){
														if(LeafPrefabs.Count > 1){
															Index = Random.Range(0,LeafPrefabs.Count);
														}
													}else{
														if(LeafPrefabsOBJ.Count > 1){
															Index = Random.Range(0,LeafPrefabsOBJ.Count);
														}
													}											
													
													if(LeafPrefabs.Count > 0){
														Leaf =(GameObject)Object.Instantiate(Resources.Load (LeafPrefabs[Index]));
														Leaf.name = "Leaf";
													}else{

														//InfiniGRASS
														if(LeafPrefabsOBJ.Count > Index && LeafPrefabsOBJ[Index] != null){
															Leaf =(GameObject)Object.Instantiate(LeafPrefabsOBJ[Index]);
														}else{
															Leaf = new GameObject();
														}
														Leaf.name = "Leaf";
													}
													
													//v1.2
													Leaf.transform.position = Registered_Leaves[j];
													Leaf.transform.rotation = Registered_Leaves_Rot[j];
													
													if(!Leaves_local_space){
														if(!is_grass){
															Leaf.transform.parent = Registered_Brances[i];
															Leaf.transform.localPosition = new Vector3(0,Mathf.Abs(Leaf.transform.localPosition.y),0);
														}
														Leaf.transform.parent = Leaf_pool.transform;
													}else{
														Leaf.transform.parent = Registered_Brances[i];
														if(!is_grass){
															Leaf.transform.localPosition = new Vector3(0,Mathf.Abs(Leaf.transform.localPosition.y),0);
														}
													}												
																										
													//v1.5
													if(Range_leaf_scale){
														Vector3 Scale = Leaf.transform.localScale;
														float Randomizer = Random.Range(Leaf_scale_range.x,Leaf_scale_range.y);
														Leaves_scales.Add(Scale * Randomizer);
													}else{
														Leaves_scales.Add(Leaf.transform.localScale);
													}
													//Leaves_scales.Add(Leaf.transform.localScale);
													
													Leaf.transform.localScale = Leaf.transform.localScale / Leaf_start_div;
													Leaves.Add(Leaf.transform);
													
													if(Random.Range(1,Grow_leaves_slow_factor)==2){
														//break;
													}
												}												
											}
										}
										//v1.2
										for (int j = 0; j<Registered_Leaves.Count;j++){
											if(Leaf_belongs_to_branch[j] == i){
												if(Smooth_leaf_growth){
													if(j < Leaves.Count){
														if(Leaves[j] != null){
															if(Leaves[j].transform.localScale.x < Leaves_scales[j].x
															   |Leaves[j].transform.localScale.y < Leaves_scales[j].y
															   |Leaves[j].transform.localScale.z < Leaves_scales[j].z
															   ){
																//Leaves[j].transform.localScale = Leaves[j].transform.localScale + 110.1f*Grow_speed*Time.deltaTime*new Vector3(1,1,1); 
																Leaves[j].transform.localScale = Vector3.Lerp(Leaves[j].transform.localScale,Leaves_scales[j]*2, 5*Grow_speed*Time.deltaTime);
																All_Branches_done = false;
																leaves_grown = false;
															}
														}
													}
												}
											}}
										if(leaves_grown){
											Branch_grew[i] = 1;
										}
									}
								}
							}
						}						
						
						if(Grow_level>5){
							Grow_level=0;			
						}else{
							Grow_level=Grow_level+1;
						}						
					}
					//See if leaves and branches have stopped growing
					
					if(Grow_tree & !Grow_tree_ended){
						
						//INFNIGRASS
						//bool ScaleWhole1 = ScaleWhole;
						if(Registered_Brances.Count == 0){
							ScaleWhole = true;
						}
						if(ScaleWhole | Registered_Brances.Count == 0){
							if(root_tree.transform.localScale.x != End_scale){
								root_tree.transform.localScale = root_tree.transform.localScale + Time.deltaTime*Vector3.one*Grow_tree_speed;
							}else{
								root_tree.transform.localScale = End_scale*Vector3.one;// MAKE IT EXACTLY same, so self dynamic baching does not kick in !!!
							}						
						}else{
							if( Registered_Brances[0].localScale.x < End_scale){
								for (int i = 0; i<Registered_Brances.Count; i++) {
									Registered_Brances[i].localScale += Vector3.one*Grow_speed*Time.deltaTime; 
									//Registered_Brances[i].transform.localScale = Registered_Brances[i].transform.localScale*End_scale; 
								}
							}else if(Registered_Brances[0].localScale.x >= End_scale){ //v1.7.5
		//v1.7						Registered_Brances[0].localScale = End_scale*Vector3.one;
									Registered_Brances[0].localScale = End_scale*Vector3.one;

								//for (int i = 1; i<Registered_Brances.Count; i++) {
								//		Registered_Brances[i].localScale = End_scale*Registered_Brances[i].localScale; 
										//Registered_Brances[i].localScale = End_scale*Vector3.one; 
										//Registered_Brances[i].localScale -= Vector3.one*Grow_speed*Time.deltaTime; 
								//}
									//Debug.Log("aaa");
							}
						}
					}					
					
						//v1.2
						if(root_tree == null || (Registered_Brances.Count > 0 && Registered_Brances[0] == null)){
							return;
						}

					if(Grow_tree_ended | (!Grow_tree_ended & Leaves.Count >= Registered_Leaves.Count & All_Branches_done & (!Grow_tree | (Grow_tree & 

					                                                                                (ScaleWhole & root_tree.transform.localScale.x >= End_scale) |
					                                                                                (!ScaleWhole & Registered_Brances.Count >0 && Registered_Brances[0].localScale.x >= End_scale)

					                                                                                )
					                                                                  ) )){
						
						if(!Combiner.doing_batching & !batching_ended){
							Combiner.doing_batching = true;//signal other trees to stop dynamic mode						
						}
						
						Grow_tree_ended = true;							
						
						//v1.2 CHOP
						if(!passed_chop){
							passed_chop = true;							

//							if(GrassCollider == null){
//								//add collider
//								this.gameObject.AddComponent<BoxCollider>().isTrigger = true;
//								transform.localScale = new Vector3(transform.localScale.x*colliderScale.x,transform.localScale.y*colliderScale.y,transform.localScale.z*colliderScale.z);
//								this.gameObject.AddComponent<GrassChopCollider>().TreeHandler = this;
//								GrassCollider = this.gameObject.GetComponent<BoxCollider>();
//							}

							foreach(GrassChopCollider collider_script in root_tree.GetComponentsInChildren<GrassChopCollider>(true))
							{
								collider_script.TreeHandler = this;
							}							
						}
						
						if(!CreateForest){
							if(!growth_over){
                                    if (Combiner == null)
                                    {
                                        root_tree.AddComponent(typeof(ControlCombineChildrenINfiniDyGrass));
                                        Combiner = root_tree.GetComponent(typeof(ControlCombineChildrenINfiniDyGrass)) as ControlCombineChildrenINfiniDyGrass;
                                        Combiner.Auto_Disable = true;
                                        Combiner.MakeActive = true;

                                        //v1.9.9.7
                                        if (enableAbove64KMesh)
                                        {
                                            Combiner.enableAbove64KMesh = true;
                                        }
                                    }
							}
						}else{
							if(!growth_over){
								//copy tree, randomize, parent to forest pool, combine there
								root_tree.transform.parent = Forest_holder.transform;
								
								Combiner = Forest_holder.GetComponent(typeof(ControlCombineChildrenINfiniDyGrass)) as ControlCombineChildrenINfiniDyGrass;
								if(Combiner == null){
									Forest_holder.AddComponent(typeof(ControlCombineChildrenINfiniDyGrass));
									Combiner = Forest_holder.GetComponent(typeof(ControlCombineChildrenINfiniDyGrass)) as ControlCombineChildrenINfiniDyGrass;
								}
								if(Combiner != null){
									
									Combiner.Auto_Disable=true;										
									Combiner.MakeActive = true;									
									Combiner.Multithreaded = Multithreaded;

                                        //v1.9.9.7
                                        if (enableAbove64KMesh)
                                        {
                                            Combiner.enableAbove64KMesh = true;
                                        }
                                    }
							}
						}
						
						//Start wind
						if(Enable_wind && Leaf_mat !=null){
							if(Leaf_mat.HasProperty("_Scale")){
								Leaf_mat.SetVector("_Scale",new Vector4(1-Wind_strength*Mathf.Cos (Time.fixedTime*Wind_freq),1-Wind_strength*Mathf.Sin (Time.fixedTime*Wind_freq),1-Wind_strength*Mathf.Cos (Time.fixedTime*Wind_freq),1));
							}
						}else{
							if(Leaf_mat !=null && Leaf_mat.HasProperty("_Scale")){
								Leaf_mat.SetVector("_Scale",new Vector4(1,1,1,1));
							}
						}
						growth_over = true;						
						
						//
						if(Combiner != null){
							Combiner.Auto_Disable=true;
							
							//bool rebatch = false;							
//							if(batching_ended & !Combiner.doing_batching & !Combiner.LODed){
//								if( Vector3.Distance(root_tree.transform.position, prev_root_pos) > 0.25f & !Combiner.batching_initialized){
//									//rebatch = true;
//									prev_root_pos = root_tree.transform.position;									
//								}								
//								if( Vector3.Distance(root_tree.transform.eulerAngles, prev_root_rot) > 0.25f & !Combiner.batching_initialized){
//									//rebatch = true;
//									prev_root_rot = root_tree.transform.eulerAngles;							
//								} 
//								if( Vector3.Distance(root_tree.transform.localScale, prev_root_scale) > 0.25f & !Combiner.batching_initialized){
//									//rebatch = true;
//									prev_root_scale = root_tree.transform.localScale;
//								}
//							}else{
//								prev_root_pos = root_tree.transform.position;
//								prev_root_rot = root_tree.transform.eulerAngles;
//								prev_root_scale = root_tree.transform.localScale;
//								//rebatch = false;
//							}
							
							Combiner.Multithreaded = Multithreaded;
							
							/////check if tree status changed, decombine container, parent tree to other type container (or make one if not exist) and parent there (decombine too if exists)
							if(Tree_status_previous != Interactive_tree & !Combiner.doing_batching  & !Combiner.LODed){
								
								Tree_status_previous = Interactive_tree;
								
								if(!Interactive_tree){
									
									Combiner.Restore();
									Combiner.Decombine = false;
									Combiner.Decombined = false;
									Combiner.MakeActive = true;
									
									//GameObject[] Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForest");

									if(Tag_based | GrassManager == null){
										Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForest");
									}else{
										Forest_holder1 = GrassManager.StaticCombiners.ToArray();
										//Debug.Log(Forest_holder1.Length);
									}

									if(Forest_holder1 == null | Forest_holder1.Length < 1){
										Create_new_pool("INfiniDyForest", Max_trees_per_group);
										root_tree.transform.parent = Forest_holder.transform;
										Combiner.MakeActive = true;Combiner.batching_initialized = false;
									}else{
										//find one with empty slots, add tree to slots number
										bool found = false;
										for(int i=0;i<Forest_holder1.Length;i++){
											Combiner = Forest_holder1[i].GetComponent(typeof(ControlCombineChildrenINfiniDyGrass)) as ControlCombineChildrenINfiniDyGrass;
											
											//check if far away from other trees
											bool Enter = true;
											for(int k = 0;k<Combiner.Added_items.Count;k++){
												if(Vector3.Distance(root_tree.transform.position,Combiner.Added_items[k].position) > Max_tree_dist){
													Enter = false;
												}
											}
											
											//v1.2
											if(!Combiner.gameObject.activeInHierarchy | !Combiner.enabled){
												Enter=false;
											}

											if(Enter & Combiner.Added_item_count < Combiner.Max_items &  Combiner.Max_items == Max_trees_per_group & !Combiner.LODed
												   & Combiner.Type == Type  & (!GroupByObject | (GroupByObject & (Combiner.PaintedOn == PaintedOnOBJ) ) )){


												Forest_holder = Forest_holder1[i];
												root_tree.transform.parent = Forest_holder.transform;
												Combiner.Added_item_count++;
												
												if(Enable_LOD){
													Combiner.DeactivateLOD = true;
												}
												
												if(Combiner.Added_items == null){
													Combiner.Added_items = new List<Transform>();
												}
												Combiner.Added_items.Add(root_tree.transform);
												//INFINIGRASS
												Tree_Holder_Index = Combiner.Added_items.Count-1;
												
												if(Combiner.Added_items_handles == null){
													Combiner.Added_items_handles = new List<INfiniDyGrassField>();
												}
												Combiner.Added_items_handles.Add(this);
												
												Combiner.Decombine = false;
												Combiner.Decombined = false;
												Combiner.MakeActive = true;
												Combiner.batching_initialized = false;
												found=true;
												break;
											}
										}
										if(!found){
											Create_new_pool("INfiniDyForest", Max_trees_per_group);
											root_tree.transform.parent = Forest_holder.transform;
											Combiner.MakeActive = true;Combiner.batching_initialized = false;
										}
									}
								}else{
									
								}
							}							
						}//END check if combiner						
						
						//tree rot
						//rotate tree towards normal
						if(Rot_toward_normal & growth_over & !Combiner.doing_batching  & !Combiner.LODed){
							
							bool found = false;//did not find decombined flag
							//GameObject[] Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForestInter");

							if(Tag_based | GrassManager == null){
								Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForestInter");
							}else{
								Forest_holder1 = GrassManager.DynamicCombiners.ToArray();
							}

							if(Forest_holder1 == null | Forest_holder1.Length < 1){
								
							}else{
								//find one with empty slots, add tree to slots number								
								for(int i=0;i<Forest_holder1.Length;i++){
									ControlCombineChildrenINfiniDyGrass Combiner1 = Forest_holder1[i].GetComponent(typeof(ControlCombineChildrenINfiniDyGrass)) as ControlCombineChildrenINfiniDyGrass;
									if(Combiner1.Decombined & Combiner1 != Combiner){										
										found=true; // if it finds even one, dont do it										
									}
								}							
							}						
							
							//check within the same combiner group as this tree, if one of the trees is close enough
							bool Enter = false;
							for(int k = 0;k<Combiner.Added_items.Count;k++){
								//if(Vector3.Distance(Hero.transform.position,Combiner.Added_items[k].position) < Interaction_thres){
								Enter = true;
								//}
							}
							
							if(Enter & !found){
								
								is_moving = true;
								Combiner.is_moving = is_moving;
								//start tree rotation
								if(!Combiner.Decombined){
									Combiner.Restore();
								}
								//do rotation								
								root_tree.transform.rotation = Quaternion.Lerp(root_tree.transform.rotation, 
								                                               Quaternion.Euler(direction_normal.x,direction_normal.y,direction_normal.z),Rot_speed * Time.deltaTime); 
								
								//if(Vector3.Distance(root_tree.transform.rotation == Quaternion.Euler(direction_normal.x,direction_normal.y,direction_normal.z)){
								if(Time.fixedTime - start_time > max_rot_time){
									Rot_toward_normal = false;
									
									Restore_shadows();
									
									is_moving = false;
									Combiner.is_moving = is_moving;
									
									if(Combiner.Decombined){
										Combiner.Decombined = false;
										Combiner.MakeActive=true;
									}
									
									start_time = Time.fixedTime; //reload, if needs real time turn in game
								}									
								
								Remove_shadows();
								
							}else{
								
								Restore_shadows();
								
								is_moving = false;
								Combiner.is_moving = is_moving;
								
								if(Combiner.Decombined){
									Combiner.Decombined = false;
									Combiner.MakeActive=true;
								}
							}
						}						
						
						//PUSH TREES sample code
						if(Interactive_tree & !Combiner.doing_batching  & !Combiner.LODed & 1==1){
														
							//v1.2 CHOP
							//if(!Chop){
							if(EnableAction == GrassAction.None){
								bool found = false;//did not find decombined flag
								//GameObject[] Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForestInter");

								if(Tag_based | GrassManager == null){
									Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForestInter");
								}else{
									Forest_holder1 = GrassManager.DynamicCombiners.ToArray();
								}


								if(Forest_holder1 == null | Forest_holder1.Length < 1){
									
								}else{
									//find one with empty slots, add tree to slots number
									
									for(int i=0;i<Forest_holder1.Length;i++){
										ControlCombineChildrenINfiniDyGrass Combiner1 = Forest_holder1[i].GetComponent(typeof(ControlCombineChildrenINfiniDyGrass)) as ControlCombineChildrenINfiniDyGrass;
										if(Combiner1.Decombined & Combiner1 != Combiner){											
											found=true; // if it finds even one, dont do it											
										}
									}							
								}						
								
								//check within the same combiner group as this tree, if one of the trees is close enough
								bool Enter = false;
								for(int k = 0;k<Combiner.Added_items.Count;k++){
									if(Vector3.Distance(player.transform.position,Combiner.Added_items[k].position) < Interaction_thres){
										Enter = true;
									}
								}
								
								if(Enter & !found){
									
									is_moving = true;
									Combiner.is_moving = is_moving;
									//start tree rotation
									if(!Combiner.Decombined){
										Combiner.Restore();
									}
									//do rotation
									float margin = 0.05f;
									if(!is_grass){
										if(Vector3.Distance( player.transform.position, root_tree.transform.position) < (Interaction_thres - Interaction_offset)){
											if(Mathf.Abs(0.1f*Mathf.Cos(0.7f*Time.fixedTime))<margin){
												root_tree.transform.Rotate(Vector3.up+new Vector3(0.3f,0,0),margin);
											}else{
												root_tree.transform.Rotate(Vector3.up+new Vector3(0.3f,0,0),0.1f*Mathf.Cos(0.7f*Time.fixedTime));
											}
										}
									}
									
									if(Vector3.Distance( player.transform.position, root_tree.transform.position) < (Interaction_thres - Interaction_offset)){
										if(Mathf.Abs(0.23f*Mathf.Cos(0.7f*Time.fixedTime))<margin){
											for(int i =0;i<Registered_Brances.Count;i++){
												if(Branch_levels[i] <2){
													Registered_Brances[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),margin);
												}
											}
										}else{
											for(int i =0;i<Registered_Brances.Count;i++){
												if(Branch_levels[i] <2){
													Registered_Brances[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),0.23f*Mathf.Cos(0.7f*Time.fixedTime));
												}
											}
										}
									}
									
									if(Vector3.Distance( player.transform.position, root_tree.transform.position) < Interaction_thres){
										if(Mathf.Abs(0.1f*Mathf.Cos(0.7f*Time.fixedTime))<margin){
											for(int i =0;i<Leaves.Count;i++){
												Leaves[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),margin);
											}
										}else{	
											for(int i =0;i<Leaves.Count;i++){
												Leaves[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),0.1f*Mathf.Cos(0.7f*Time.fixedTime));
											}
										}
									}	
									
									Remove_shadows();
									
								}else{
									
									Restore_shadows();
									
									is_moving = false;
									Combiner.is_moving = is_moving;
									
									if(Combiner.Decombined){
										Combiner.Decombined = false;
										Combiner.MakeActive=true;
									}
								}
							}//END if not CHOP
							else{////v1.2 CHOP

									if(EnableAction == GrassAction.Chop){
										
										if(Health <=(8*Max_Health/10) & !start_falling){
											
											//			start_falling = true;
											fall_ammount = 0;
											
											//
											fall_timer = Time.fixedTime;
											//direction_randomize = Vector3.right+new Vector3(Random.Range(-5.3f,10.1f),Random.Range(0,0.1f),Random.Range(-5.3f,10.1f));
										}
										
										//									bool found = false;//did not find decombined flag
										//									GameObject[] Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForestInter");
										//									if(Forest_holder1 == null | Forest_holder1.Length < 1){
										//										
										//									}else{
										//										//find one with empty slots, add tree to slots number
										//										
										//										for(int i=0;i<Forest_holder1.Length;i++){
										//											ControlCombineChildrenINfiniDyGrass Combiner1 = Forest_holder1[i].GetComponent(typeof(ControlCombineChildrenINfiniDyGrass)) as ControlCombineChildrenINfiniDyGrass;
										//											if(Combiner1.Decombined & Combiner1 != Combiner){											
										//												found=true; // if it finds even one, dont do it											
										//											}
										//										}							
										//									}
										
										
										//		if(!found && Vector3.Distance( player.transform.position, root_tree.transform.position) < Interaction_thres - Interaction_offset){ //INFINIGRASS
										if( Vector3.Distance( player.transform.position, root_tree.transform.position) < Interaction_thres - Interaction_offset){ //INFINIGRASS
											
											//rotate tree
											is_moving = true;
											Combiner.is_moving = is_moving;
											//start tree rotation
											if(!Combiner.Decombined){
												Combiner.Restore();
											}
											//do rotation
											
											
											if(root_tree.transform.eulerAngles.x < 89){
												fall_ammount = fall_ammount+1.5f+(0.01f)*Mathf.Cos(0.7f*Time.fixedTime);
											}
											
											
											//INFINIGRASS
											Vector3 Player_motion_direction = (player.transform.position - Prev_hero_pos);
											float speed = (player.transform.position - Prev_hero_pos).magnitude / Time.deltaTime;
											for(int i =0;i<Registered_Brances.Count;i++){
												
												float dist = Vector3.Distance( player.transform.position, Registered_Brances[i].position);
												if( dist < Interaction_thres*1){
													
													if(Branch_levels[i] >=0){

														//v1.3 - harvest/chop
														if(Registered_Brances[i].position.y < transform.position.y + 0.01f*22115*End_scale){
															Registered_Brances[i].position = Registered_Brances[i].position +fall_ammount*Vector3.one+ (Vector3.up - Player_motion_direction)*0.01f*2200*Time.deltaTime;
														}else{
															Registered_Brances[i].position = Registered_Brances[i].position + 0.01f*10000*Vector3.up;
														}
														
														if(speed > InteractSpeedThres){
															
															Registered_Brances[i].up = Vector3.Lerp(Registered_Brances[i].up, Player_motion_direction, InteractionSpeed * Time.deltaTime);
															
															if(Registered_Brances[i].localScale.y > 0.01f){
																
																Registered_Brances[i].localScale = Vector3.Lerp(Registered_Brances[i].localScale,Registered_Brances[i].localScale*0.1f*(((Interaction_thres - Interaction_offset)/dist)),Time.deltaTime);
																
															}
														}
													}
												}										
											}
											
											if(!start_falling){
												//Remove_shadows();
											}
											
											//if(fall_ammount > max_fall_ammount | (Time.fixedTime - fall_timer) > fall_max_time){
											if((Time.fixedTime - fall_timer) > fall_max_time){
												fall_ended = true;
											}
											
											start_falling = true;//INFINIGRASS
										}else if(Vector3.Distance( player.transform.position, root_tree.transform.position) > Interaction_thres ){
											
											//if(fall_ended & start_falling){//INFINIGRASS
											if(start_falling){
												is_moving = false;
												Combiner.is_moving = is_moving;
												
												if(Combiner.Decombined){
													Combiner.Decombined = false;
													Combiner.MakeActive=true;
												}
												
												start_falling = false;//INFINIGRASS
												//v1.2
												//Restore_shadows();//END shadow removal
											}
										}
										
										if(Vector3.Distance( player.transform.position, root_tree.transform.position) < Interaction_thres * 10){
											if(!shadows_removed){
												Remove_shadows();
												shadows_removed = true;
											}
										}else if(Vector3.Distance( player.transform.position, root_tree.transform.position) > Interaction_thres * 10 + Interaction_offset ){
											if(shadows_removed){
												Restore_shadows();
												shadows_removed = false;
											}
										}									
										
									}//END CHOP

								if(EnableAction == GrassAction.Flatten){

									if(Health <=(8*Max_Health/10) & !start_falling){
										
							//			start_falling = true;
										fall_ammount = 0;
										
										//
										fall_timer = Time.fixedTime;
										//direction_randomize = Vector3.right+new Vector3(Random.Range(-5.3f,10.1f),Random.Range(0,0.1f),Random.Range(-5.3f,10.1f));
									}

//									bool found = false;//did not find decombined flag
//									GameObject[] Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForestInter");
//									if(Forest_holder1 == null | Forest_holder1.Length < 1){
//										
//									}else{
//										//find one with empty slots, add tree to slots number
//										
//										for(int i=0;i<Forest_holder1.Length;i++){
//											ControlCombineChildrenINfiniDyGrass Combiner1 = Forest_holder1[i].GetComponent(typeof(ControlCombineChildrenINfiniDyGrass)) as ControlCombineChildrenINfiniDyGrass;
//											if(Combiner1.Decombined & Combiner1 != Combiner){											
//												found=true; // if it finds even one, dont do it											
//											}
//										}							
//									}

																
							//		if(!found && Vector3.Distance( player.transform.position, root_tree.transform.position) < Interaction_thres - Interaction_offset){ //INFINIGRASS
									if( Vector3.Distance( player.transform.position, root_tree.transform.position) < Interaction_thres - Interaction_offset){ //INFINIGRASS

										//rotate tree
										is_moving = true;
										Combiner.is_moving = is_moving;
										//start tree rotation
										if(!Combiner.Decombined){
											Combiner.Restore();
										}
										//do rotation

										
										if(root_tree.transform.eulerAngles.x < 89){
											fall_ammount = fall_ammount+1.5f+(0.01f)*Mathf.Cos(0.7f*Time.fixedTime);
										}


										//INFINIGRASS
										Vector3 Player_motion_direction = (player.transform.position - Prev_hero_pos);
										float speed = (player.transform.position - Prev_hero_pos).magnitude / Time.deltaTime;
										for(int i =0;i<Registered_Brances.Count;i++){

											float dist = Vector3.Distance( player.transform.position, Registered_Brances[i].position);
											if( dist < Interaction_thres*1){

												if(Branch_levels[i] >=0){

													if(speed > InteractSpeedThres){

														Registered_Brances[i].up = Vector3.Lerp(Registered_Brances[i].up, Player_motion_direction, InteractionSpeed * Time.deltaTime);

														if(Registered_Brances[i].localScale.y > 0.01f){
														
															Registered_Brances[i].localScale = Vector3.Lerp(Registered_Brances[i].localScale,Registered_Brances[i].localScale*0.1f*(((Interaction_thres - Interaction_offset)/dist)),Time.deltaTime);

														}
													}
												}
											}										
										}

										if(!start_falling){
											//Remove_shadows();
										}
										
										//if(fall_ammount > max_fall_ammount | (Time.fixedTime - fall_timer) > fall_max_time){
										if((Time.fixedTime - fall_timer) > fall_max_time){
											fall_ended = true;
										}

										start_falling = true;//INFINIGRASS
									}else if(Vector3.Distance( player.transform.position, root_tree.transform.position) > Interaction_thres ){
									
										//if(fall_ended & start_falling){//INFINIGRASS
										if(start_falling){
											is_moving = false;
											Combiner.is_moving = is_moving;
											
											if(Combiner.Decombined){
												Combiner.Decombined = false;
												Combiner.MakeActive=true;
											}

											start_falling = false;//INFINIGRASS
											//v1.2
											//Restore_shadows();//END shadow removal
										}
									}

									if(Vector3.Distance( player.transform.position, root_tree.transform.position) < Interaction_thres * 10){
										if(!shadows_removed){
											Remove_shadows();
											shadows_removed = true;
										}
									}else if(Vector3.Distance( player.transform.position, root_tree.transform.position) > Interaction_thres * 10 + Interaction_offset ){
										if(shadows_removed){
											Restore_shadows();
											shadows_removed = false;
										}
									}									
									
								}//END FLATTEN
							}//END IF ACTION
						}else{
							is_moving = false;
							Combiner.is_moving = is_moving;
							if(Combiner.Decombined){
								
							}
						}
						
						if(batching_ended){
							Combiner.doing_batching = false;//signal other trees to stop dynamic mode						
						}
						
						batching_ended = true; // use this when returns here, to enable dynamic trees, NOT BEFORE since will break batching !!!
					}// END if growh ended
					
				}//END TIME CHECK
				
			}//END if app is playing

				if(player != null){
					Prev_hero_pos = player.transform.position;
				}

			}

			//v1.5 - InfiniGRASS - Performance enchancment by removing original meshes after combined for static grass & disable grower script and colliders overhead
			if (!Interactive_tree && Application.isPlaying && Disable_after_growth && Combiner != null) {
				//if(Application.isPlaying && Grow_tree_ended && !Interactive_tree && batching_ended && !Combiner.doing_batching){
				//if(Application.isPlaying && Grow_tree_ended && !Interactive_tree && batching_ended && Combiner.Max_items == Combiner.Added_item_count){
				//if(Application.isPlaying && Grow_tree_ended && !Interactive_tree && (!WhenCombinerFull || (WhenCombinerFull && Combiner.Max_items == Combiner.Added_item_count)) && !Combiner.batching_initialized){
				//if (All_batching_ended && All_growing_ended && (!WhenCombinerFull || (WhenCombinerFull && Combiner.Max_items == Combiner.Added_item_count)) && !Combiner.batching_initialized) {

				bool WhenCombinerFull1 = WhenCombinerFull;
				if (GrassManager.GradualGrowth) {
					WhenCombinerFull1 = true; //always true if in gradual mode
				}

				if ( All_growing_ended && (!WhenCombinerFull1 || (WhenCombinerFull1 && Combiner.Max_items == Combiner.Added_item_count)) ) {
				//if ( All_growing_ended && (!WhenCombinerFull || (WhenCombinerFull && Combiner.Max_items == Combiner.Added_item_count)) ) {
					if (Eliminate_original_mesh) {
						for (int i = root_tree.transform.childCount - 1; i >= 0; --i) {
							GameObject.Destroy (root_tree.transform.GetChild (i).gameObject);
						}
						root_tree.transform.DetachChildren ();
						if (GrassManager.GradualGrowth) {
							Registered_Brances.Clear ();
							Registered_Leaves.Clear ();
							Registered_Leaves_Rot.Clear ();
						}
					}

					Combiner.Max_items = Combiner.Added_item_count;//dont accept more

					if (!GrassManager.GradualGrowth) {
						GrassCollider.enabled = false;
						this.enabled = false;
					}
				}

				//bool All_batching_ended = true;
				//bool All_growing_ended = true;
//				for (int i = 0; i < Combiner.Added_items_handles.Count; i++) {
//					//if (!Combiner.Added_items_handles [i].batching_ended) {
//					//		All_batching_ended = false;
//					//}
//					if (!Combiner.Added_items_handles [i].Grow_tree_ended) {
//						All_growing_ended = false;
//					}
//				}
				bool All_growing_ended1 = true;
				for (int i = 0; i < Combiner.Added_items_handles.Count; i++) {
					//if (!Combiner.Added_items_handles [i].batching_ended) {
					//		All_batching_ended = false;
					//}
					if (!Combiner.Added_items_handles [i].Grow_tree_ended) {
						All_growing_ended1 = false;
					}
				}
				if (All_growing_ended1) {
					All_growing_ended = true; //deffer for next frame
				}
			
			}

		}// END UPDATE

		bool All_growing_ended = false;
		
		void Remove_shadows(){

			//Debug.Log ("Removed shadows");
			
			//v1.2
			if(!removed_shadows & no_shadows_on_dynamic){
				removed_shadows = true;
				//remove shadows
				if(Rm_shad_above_lvl < 0){
					Component[] filters  = root_tree.GetComponentsInChildren(typeof(MeshFilter));
					for (int i=0;i<filters.Length;i++) {
						Renderer curRenderer  = filters[i].GetComponent<Renderer>();
						if (curRenderer != null && curRenderer.enabled ) {	
							if(curRenderer.sharedMaterial.name.Contains("LOD")){//LOD
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;//.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}else{
								//curRenderer.enabled = true;
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;//.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}
						}
					}
				}else{
					for(int i =0;i<Registered_Brances.Count;i++){
						if(Branch_levels[i] >= Rm_shad_above_lvl){
							//if(Rm_shad_per_lvl[Branch_levels[i]] == 1){
							Component[] filters  = Registered_Brances[i].GetComponentsInChildren(typeof(MeshFilter));
							for (int j=0;j<filters.Length;j++) {
								Renderer curRenderer  = filters[j].GetComponent<Renderer>();
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;//.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}
							//}
						}
					}
					if(no_leaf_shadows){
						for(int i =0;i<Leaves.Count;i++){
							Component[] filters  = Leaves[i].GetComponentsInChildren(typeof(MeshFilter));
							for (int j=0;j<filters.Length;j++) {
								Renderer curRenderer  = filters[j].GetComponent<Renderer>();
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;//.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}
						}
					}
				}
			}
			
		}
		
		void Restore_shadows(){

			//Debug.Log ("restored shadows");

			//v1.2
			if(removed_shadows & no_shadows_on_dynamic){
				removed_shadows = false;
				//remove shadows
				if(Rm_shad_above_lvl < 0){
					Component[] filters  = root_tree.GetComponentsInChildren(typeof(MeshFilter));
					for (int i=0;i<filters.Length;i++) {
						Renderer curRenderer  = filters[i].GetComponent<Renderer>();
						if (curRenderer != null && curRenderer.enabled ) {	
							if(curRenderer.sharedMaterial.name.Contains("LOD")){//LOD													
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;//.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}else{
								//curRenderer.enabled = true;
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;//.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}
						}
					}
				}else{
					for(int i =0;i<Registered_Brances.Count;i++){
						if(Branch_levels[i] >= Rm_shad_above_lvl){
							//if(Rm_shad_per_lvl[Branch_levels[i]] == 1){
							Component[] filters  = Registered_Brances[i].GetComponentsInChildren(typeof(MeshFilter));
							for (int j=0;j<filters.Length;j++) {
								Renderer curRenderer  = filters[j].GetComponent<Renderer>();
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;//.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}
							//}
						}
					}
					if(no_leaf_shadows){
						for(int i =0;i<Leaves.Count;i++){
							Component[] filters  = Leaves[i].GetComponentsInChildren(typeof(MeshFilter));
							for (int j=0;j<filters.Length;j++) {
								Renderer curRenderer  = filters[j].GetComponent<Renderer>();
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;//.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}
						}
					}
				}
			}
		}		

		public GameObject Forest_holder;
		[HideInInspector]
		public Material Leaf_mat;
		[HideInInspector]
		public bool Enable_wind=false;
		[HideInInspector]
		public float Wind_freq=1;
		[HideInInspector]
		public float Wind_strength = 0.01f;
		[HideInInspector]
		public 	bool batching_ended=false;
		[HideInInspector]
		public bool is_moving = false;

		//v1.6 DEBUG
		[HideInInspector]
		public float Health;
		[HideInInspector]
		public bool start_falling = false;
		[HideInInspector]
		public float fall_ammount = 0;
		[HideInInspector]
		public bool fall_ended = false;
		[HideInInspector]
		public bool passed_chop = false;

		public GameObject root_tree;
		//[HideInInspector]
		public bool Grow_tree_ended = false;
		[HideInInspector]
		public List<int> Branch_grew;
		[HideInInspector]
		public int Grow_level = 0;
		[HideInInspector]
		public bool growth_over = false;
		public ControlCombineChildrenINfiniDyGrass Combiner;

		//INFINIGRASS
		public bool shadows_removed = false;
		Vector3 Prev_hero_pos = Vector3.zero;
	}
}