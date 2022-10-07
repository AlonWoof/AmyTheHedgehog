using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Artngame.INfiniDy {

	//v1.4 - etxra materials
	[System.Serializable]
	public class InfiniGRASS_ExtraBrushMaterials{
		public List<Material> ExtraMaterials = new List<Material>(); 
	}

	//v1.4 - brush settings
	[System.Serializable]
	public class InfiniGRASS_BrushSettings{

		public float min_scale;//
		public float max_scale;//
		public float Min_density;
		public float Max_density;
		public float Min_spread;//
		public float Max_spread;//
		public float SpecularPower;									
		public float Cutoff_distance;//
		public float LOD_distance;//
		public float LOD_distance1;//
		public float LOD_distance2;	//	
		public float AmplifyWind;//
		public float WindTurbulence;//
		public float min_grass_patch_dist;//
		public float Stop_Motion_distance;//
		public bool Override_density;//
		public bool Override_spread;//
		public float MinAvoidDist;//
		public float MinScaleAvoidDist;//
		public float SphereCastRadius;//
		public float Grass_Fade_distance;//
		public float Collider_scale;//

		public float rayCastDist;
		public float InteractionSpeed;
		public float InteractSpeedThres;
		public float Interaction_thres;
		public float Max_tree_dist = 450;//v1.4.6
		public bool Disable_after_growth = false;//v1.5
		public bool WhenCombinerFull = true;//v1.5
		public bool Eliminate_original_mesh = false;//v1.5
		public float Interaction_offset;
		public float RandRotMin;
		public float RandRotMax;
		public bool  RandomRot;

		public bool GroupByObject;
		public bool ParentToObject;
		public bool MoveWithObject;
		public bool AvoidOwnColl;

		public bool AdaptOnTerrain;
		public int Max_interactive_group_members;
		public int Max_static_group_members;
		public bool Interactive;
		public bool GridOnNormal;

        //v1.9.9.7
        public bool enableAbove64KMesh;
    }


[ExecuteInEditMode]
public class InfiniGRASSManager : MonoBehaviour {

        //v1.7 - above 64K - v1.9.9.7
        public bool enableAbove64KMesh = false;

        //v1.9.8
        public bool addPresetBrushes = true;

        //v1.7.8
        public bool UseTabs = true;
		public int currentTab =0;
		public int basicAdvancedToggle=0;

		//v1.7.7
		public bool leftMousePaint=false;

		//v1.7.5
		public bool UpdateMaterials = true;

		//v1.7
		public bool ApplyPresets=true;//apply presets when icons pressed

		//v1.6
		//[SerializeField]
		public bool noThreading = false;//override multithreading setting for WebGL

		//v1.5 - slope -height control for mass plant
		public float Min_Height = -600;
		public float Max_Height = 6000;
		public float Vary_Height = 0;
		public bool Height_control = false;
		public float Min_Slope = 0;
		public float Max_Slope = 90;
		public float Vary_Slope = 0;
		public bool Slope_control = false;

		//v1.5
		public bool ShaderBasedInteract = true;
		Vector3 prev_pos;
		Vector3 SpeedVec = Vector3.zero;
		public float ShaderBInteractSpeed=2;

		//v1.4f - playr control
		public bool resetPlayer = false;
		public bool recheckPlayer = false;//control in inspector - rest for scripting
		public bool resetInteractor = false;
		public bool applyInteractor = false;

		//v1.4 - LOD control
		public bool Apply_LOD_to_all = false;
		public Transform Interactor;

		//v1.4 - grab grass
		public Transform play_mode_grass_holder;
		public Transform Editor_holder;//choose equivalent object from play mode in editor
		public float Scale_grabbed = 1;
		public float ScaleMin_grabbed = 1;

		//v1.4.6
		public bool ScalePerTypes = false;
		public List<float> ScalePerType = new List<float> ();

		//v1.4 - settings for each mass placement brush
		public bool GrabSettingsMassPlace = false;//use settings structure to give brush props on mass placement
		public List<InfiniGRASS_BrushSettings> BrushSettings = new List<InfiniGRASS_BrushSettings>();

		///v1.3 - Gradual growth in play mode when ungrown
		public bool GradualGrowth = false;
		public bool GradualRecreate = false;
		public bool UseDistFromPlayer = true;//enable only when close to hero
		public float EnableDist = 600;//set this higher than cutoff distance of grass

		public List<int> BrushTypePerSplat = new List<int>();//use -1 to define no grass placement
		public List<int> SubTypePerSplat = new List<int>();

		public Transform MassPlantAreaCornerA;
		public Transform MassPlantAreaCornerB;
		public int MassPlantDensity = 10;//how many grass patches per meter

		//v1.2a
		public float WorldScale = 20;
		public float prev_WorldScale = 20;

		//v1.2
		public int prev_brush=0;
		public Vector3 prev_cam_pos;

		//v1.1 - TERRAIN ADAPT
		public bool AdaptOnTerrain = false;//find if there is active terrain, asapt size based on color
		public bool AdaptOnColor = false;
		public Terrain currentTerrain;//assign the active terrain here, check if changed and reassign terrain and data
		public TerrainData Tdata;
		public Vector3 Tpos;
		public List<float> ScalePerTexture = new List<float>();//define what scale grass will receive on each terrain texture. Must be as long as textures on terrain
		int prev_type;

		//v1.1
		public bool RandomRot = false;//randomize rotation, for low grass randomize to give a better volume feeling at spots
		public float RandRotMin = -360;
		public float RandRotMax = 360;
		public bool GroupByObject = true;//enable grouping by object, search for batchers that have same object or create one
		public bool ParentToObject = false;//parent batcher to object grass was painted on, otherwise control by other script
		public bool MoveWithObject = false;
		public bool AvoidOwnColl = true;

		public bool PaintonTag = false;//paint on objects tagged as "PPaint"
		public bool Tag_based_player = false;//otherwise search for camera
		public bool Tag_based_player_prev = false;//otherwise search for camera
		public string Player_tag = "Player";
		public string Player_tag_prev = "Player";

		public bool UnGrown = false;//signal ungrown system in editor

		public List<GameObject> DynamicCombiners = new List<GameObject>();
		public List<GameObject> StaticCombiners = new List<GameObject>();

		public bool Enable_real_time_paint = false;//real time painting toggle
		public bool Enable_real_time_erase = false;//real time painting toggle

		public float SphereCastRadius = 15;//radius when mass erasing
		public bool MassErase = false;
		public bool Erasing = false;
		public bool Looking = false;
		public bool ActivateHelp = true;

		public List<Transform> Fences = new List<Transform> ();
		List<Transform> RealTimeFences = new List<Transform> ();//keep real time fences here and batch individually per fence

		public float WindTurbulence = 1;

		public bool GridOnNormal = true;//rotate grass paint grid based on hit normal
		public float rayCastDist = 10;//define max raycast distance

		public float MinAvoidDist = 2;//distance to raycast around branch to see if touches terrain or should be scaled down
		public float MinScaleAvoidDist = 4;//distance to raycast around branch to see if touches terrain or should be avoided creation
		public float InteractionSpeed = 1;//define speed of grass interaction
		public float InteractSpeedThres = 0.5f;

		//v1.4
		public float Interaction_thres = 20;
		public float Max_tree_dist = 450;//v1.4.6
		public bool applyPerfToAll = false;//v1.5
		public bool Disable_after_growth = false;//v1.5
		public bool WhenCombinerFull = true;//v1.5
		public bool Eliminate_original_mesh = false;//v1.5
		public float Interaction_offset = 1f;

		public bool CleanUp = true;//clean up low on brnaches instances
		public int MinBranches = 2;//minimal branches to avoid clean up

		public bool enable_colliders_editor = true;//enable colliders to avoid grass insertion close to other patches
		public bool enable_colliders_playmode = true;//pass to grass field on instantiation

		public float Min_density = 1;
		public float Max_density = 5;
		public float Min_spread = 1;
		public float Max_spread = 5;
		public bool Override_density = false;
		public bool Override_spread = false;

		public bool Preview_wind = false;//preview wind in editor
		public float Grow_speed = 2;//pass custom growth speed on instantiated
		public float Editor_view_dist = 300;//disablr far away in editor
        public bool autoSetEditorViewDist = true;//v1.9.9.8d

        public bool Grass_painting = false;
		public bool Rock_painting = false;
		public bool Fence_painting = false;
		public float Collider_scale = 1;//global collider scale
		public float Gizmo_scale = 2;

		public Transform windzone;
		public WindZone windzoneScript;
		public float AmplifyWind = 1f;
		bool Fence_start_added = false;
		public bool GizmosOn = true;
		public bool EditorCollidersOn = false;

		public bool ShaderWind = true;
		public bool TintGrass = false;
		public float TintPower = 0;
		public float TintFrequency = 0.1f;//v1.1
		public Color tintColor = Color.white;
		public float SpecularPower = 1;

		//grass
		public int Max_interactive_group_members = 6;
		public int Max_static_group_members = 9;
		public float LOD_distance = 350;
		public float LOD_distance1 = 430;
		public float LOD_distance2 = 460;
		public float Cutoff_distance = 530;

		public bool Interactive = false;//define if instantiated will be interactive or not
		public bool ApplyInteractive=false;//apply interactive to specific grass selector type
		public bool apply_to_all = false;//apply to all types !!!

		public int Grass_selector;

		public void Apply_Interactive(){
			for (int i=0; i<Grasses.Count; i++) {
				if(apply_to_all){
					Grasses[i].Interactive_tree = Interactive;
					//v1.4
					Grasses[i].Interaction_thres = Interaction_thres;
					Grasses[i].Interaction_offset = Interaction_offset;
					Grasses[i].InteractionSpeed = InteractionSpeed;
					Grasses[i].InteractSpeedThres = InteractSpeedThres;
					//Grasses[i].stopm = InteractSpeedThres;
				}else{
					if(GrassesType[i] == Grass_selector){
						Grasses[i].Interactive_tree = Interactive;
						//v1.4
						Grasses[i].Interaction_thres = Interaction_thres;
						Grasses[i].Interaction_offset = Interaction_offset;
						Grasses[i].InteractionSpeed = InteractionSpeed;
						Grasses[i].InteractSpeedThres = InteractSpeedThres;
					}
				}
			}		
		}
		//v1.5
		public void Apply_Performance(){
			for (int i=0; i<Grasses.Count; i++) {
				if(applyPerfToAll){					
					Grasses[i].Disable_after_growth = Disable_after_growth;
					Grasses[i].WhenCombinerFull = WhenCombinerFull;
					Grasses[i].Eliminate_original_mesh = Eliminate_original_mesh;
				}else{
					if(GrassesType[i] == Grass_selector){						
						Grasses[i].Disable_after_growth = Disable_after_growth;
						Grasses[i].WhenCombinerFull = WhenCombinerFull;
						Grasses[i].Eliminate_original_mesh = Eliminate_original_mesh;
					}
				}
			}		
		}
		public void Apply_Batch_Dist(){
			for (int i=0; i<Grasses.Count; i++) {
				//if(apply_to_all){
					Grasses[i].Max_tree_dist = Max_tree_dist;					
//				}else{
//					if(GrassesType[i] == Grass_selector){
//						Grasses[i].Max_tree_dist = Max_tree_dist;						
//					}
//				}
			}		
		}

		//[SerializeField, Range(0, 20)]
		[Range(0, 20)]
		public float min_grass_patch_dist = 2;//distance between grasss patches drawn on mouse click/drag

		public float fence_scale = 1;

		public float min_scale = 0.5f;
		public float max_scale = 1;

		public List<Transform> Fence_poles = new List<Transform>();
		public List<Transform> Fence_poles_tmp = new List<Transform>();

		public List<Transform> Fence_poles_midA = new List<Transform>();
		public List<Transform> Fence_poles_midA_tmp = new List<Transform>();

		public List<INfiniDyGrassField> Grasses = new List<INfiniDyGrassField>();
		public List<int> GrassesType = new List<int>();

		public GameObject GrassPrefab;
		public GameObject player;

		public GameObject FencesHolder;
		public GameObject RocksHolder;
		public GameObject GrassHolder;
		public GameObject GrassBatchHolder;

		public float Grass_Fade_distance = 500;//Grass fade distance, set in grass materials based on camera
		public float Stop_Motion_distance = 15;//stop grass motion during interaction and let script take over

		public List<Material> GrassMaterials = new List<Material>();
		public List<InfiniGRASS_ExtraBrushMaterials> ExtraGrassMaterials = new List<InfiniGRASS_ExtraBrushMaterials>();//v1.4 - support more than one material per brush

		public List<int> Overide_seg_detail = new List<int>();
		public List<GameObject> GrassPrefabs = new List<GameObject>();

		//v1.3 - icons
		public List<Texture2D> GrassPrefabsIcons = new List<Texture2D>();
		public List<Texture2D> RocksPrefabsIcons = new List<Texture2D>(); //v1.4.6
		public List<Texture2D> FencePrefabsIcons = new List<Texture2D>(); //v1.4.6
		public List<string> GrassPrefabsNames = new List<string>(); //v1.4.6

		public List<GameObject> RockPrefabs = new List<GameObject>();
		public List<GameObject> FencePrefabs = new List<GameObject>();
		public List<GameObject> FenceMidPrefabs = new List<GameObject>();

	// Use this for initialization
	void Start () {

			//v1.7 - restore !grow_tree mode, when ungrown, relates to ungrow in editor option and line 390 in grassfield script
			if (UnGrown) {
				for (int i = 0; i < Grasses.Count; i++) {
					Grasses [i].Grow_tree = false;
				}
			}

			//v1.4 - tag based player
			if (Application.isPlaying) {
				Tag_based_player_prev = Tag_based_player;
				Player_tag_prev = Player_tag;
				for(int i=0;i<Grasses.Count;i++){
					Grasses[i].Tag_based_player = Tag_based_player;
					Grasses[i].Player_tag = Player_tag;
				}
				for(int i=0;i<DynamicCombiners.Count;i++){
					if(DynamicCombiners[i] != null){
						DynamicCombiners[i].GetComponent<ControlCombineChildrenINfiniDyGrass>().Tag_based_player = Tag_based_player;
						DynamicCombiners[i].GetComponent<ControlCombineChildrenINfiniDyGrass>().Player_tag = Player_tag;
					}
				}
				for(int i=0;i<StaticCombiners.Count;i++){
					if(StaticCombiners[i] != null){
						StaticCombiners[i].GetComponent<ControlCombineChildrenINfiniDyGrass>().Tag_based_player = Tag_based_player;
						StaticCombiners[i].GetComponent<ControlCombineChildrenINfiniDyGrass>().Player_tag = Player_tag;
					}
				}
			}

			//v1.4
			if (ExtraGrassMaterials == null) {
				ExtraGrassMaterials = new List<InfiniGRASS_ExtraBrushMaterials> ();
			}

			if (Application.isPlaying) {
				for(int j=DynamicCombiners.Count-1;j>=0;j--){
					if(!DynamicCombiners[j].gameObject.activeInHierarchy){
						DynamicCombiners[j].gameObject.SetActive(true);
					}
				}
				for(int j=StaticCombiners.Count-1;j>=0;j--){
					if(!StaticCombiners[j].gameObject.activeInHierarchy){
						StaticCombiners[j].gameObject.SetActive(true);
					}
				}
			}

			if (Camera.current != null) {
				prev_cam_pos = Camera.current.transform.position;
			}
			//v1.2
			FencesHolder.GetComponent<ControlCombineChildrenINfiniDyGrass>().realtime = true;
			RocksHolder.GetComponent<ControlCombineChildrenINfiniDyGrass>().realtime = true;

		prev_type = Grass_selector;
		if(Grass_selector < GrassMaterials.Count){
			if(GrassMaterials[Grass_selector].HasProperty("_OceanCenter") & GrassMaterials[Grass_selector].HasProperty("_SpecularPower") ){
				TintPower =  GrassMaterials[Grass_selector].GetFloat("_TintPower");
				tintColor = GrassMaterials[Grass_selector].GetColor("_Color");
			}
			if(GrassMaterials[Grass_selector].HasProperty("_TintFrequency")) {
				TintFrequency =  GrassMaterials[Grass_selector].GetFloat("_TintFrequency");
			}
			if(GrassMaterials[Grass_selector].HasProperty("_OceanCenter") & GrassMaterials[Grass_selector].HasProperty("_SpecularPower") ){
				SpecularPower = GrassMaterials[Grass_selector].GetFloat("_SpecularPower");
			}
			//v1.5
			if(GrassMaterials[Grass_selector].HasProperty("_FadeThreshold")){
				Grass_Fade_distance = GrassMaterials[Grass_selector].GetFloat("_FadeThreshold");
			}
			if(GrassMaterials[Grass_selector].HasProperty("_StopMotionThreshold")){
				Stop_Motion_distance =GrassMaterials[Grass_selector].GetFloat("_StopMotionThreshold");
			}
		}

		if (Application.isPlaying) {

			//v1.3
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

//			if(Tag_based_player){
//				player = GameObject.FindGameObjectWithTag ("Player");
//			}else{
//				player = Camera.main.gameObject;
//			}
				if(Camera.main != null){
					Camera_transf = Camera.main.transform;
				}


				//v1.5
				if (player != null) {
					prev_pos = player.transform.position;
				}

			//enable all grass again, after editor LOD
			for (int i=0; i<Grasses.Count; i++) {

					//parent to moving objects
					if(Grasses[i].PaintedOnOBJ != null){
						Grasses[i].transform.parent = Grasses[i].PaintedOnOBJ;
					}

					//v1.3
					if(!GradualGrowth){
						Grasses[i].gameObject.SetActive(true);
					}
					if(GradualGrowth & UnGrown & player != null){
						//for (int i=0; i<Grasses.Count; i++) {					
							if(Vector3.Distance(player.transform.position,Grasses[i].gameObject.transform.position) < EnableDist){
								//make appear gradually
								if(!Grasses[i].gameObject.activeInHierarchy){
									Grasses[i].Grow_tree_speed = 1000;
									Grasses[i].gameObject.SetActive(true);
									//Debug.Log("activated:"+i);
									//if(Random.Range(1,3)==2){
									//	break;
									//}
								}
							}
						//}
					}

				if(Grasses [i].root_tree != null){
					Grasses [i].root_tree.SetActive (true);
				}
			}
		}
	}


		public void EnableColliders(){
			for (int i=0; i<Grasses.Count; i++) {
				Grasses[i].GrassCollider.enabled = true;
				Grasses[i].enable_colliders_editor = true;
			}
		}
		public void DisableColliders(){
			for (int i=0; i<Grasses.Count; i++) {
				Grasses[i].GrassCollider.enabled = false;
				Grasses[i].enable_colliders_editor = false;
			}
		}

		void OnDrawGizmos(){

			if (GizmosOn) {
				for (int i=0; i<Grasses.Count; i++) {
					if(Grasses[i] != null){
						Gizmos.DrawCube (Grasses [i].transform.position, Gizmo_scale * Vector3.one);
					}

				}

				//target circle - removed in v1.4 
//				if(Grass_painting & 1==0){
//					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
//					RaycastHit hit = new RaycastHit ();
//					if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
//						Gizmos.DrawCube (hit.point, Gizmo_scale *3* Vector3.one);
//						if(MassErase){
//							Gizmos.DrawWireSphere(hit.point,SphereCastRadius);
//						}else{
//							Gizmos.DrawWireSphere(hit.point,15);
//						}
//					}
//				}
			}
		}

		Transform Camera_transf;

	// Update is called once per frame
	public void Update () {

			//v1.4
			if (Application.isPlaying) {

				//v1.4f - interactor real time control
				if(resetInteractor){

					for (int i=0; i<Grasses.Count; i++) {
						if(apply_to_all){
							Grasses[i].player = null;
						}else{
							if(GrassesType[i] == Grass_selector){
								Grasses[i].player = null;
							}
						}
					}

					resetInteractor = false;
				}
				if(applyInteractor){

					if(Interactor != null){
						for (int i=0; i<Grasses.Count; i++) {
							if(apply_to_all){
								Grasses[i].player = Interactor.gameObject;
							}else{
								if(GrassesType[i] == Grass_selector){
									Grasses[i].player = Interactor.gameObject;
								}
							}
						}
					}else{
						Debug.Log("Please enter a gameobject in the Interactor variable");
					}

					applyInteractor = false;
				}

				//v1.4f - recheck player
				if(recheckPlayer){
					//if player changed, reset
					if (Tag_based_player) {
						
						GameObject Newplayer = GameObject.FindGameObjectWithTag (Player_tag);
						if(Newplayer != null && player != null && player != Newplayer){
							resetPlayer = true;
						}
					} else {
						if(Camera.main != null && player != null && player != Camera.main.gameObject){
							resetPlayer = true;
						}						
					}
				}

				//v1.4c
				if((Tag_based_player != Tag_based_player_prev) | (Player_tag != Player_tag_prev) | resetPlayer)
				{
					resetPlayer = false;//v1.4f

					player = null;
					for(int i=0;i<Grasses.Count;i++){
						Grasses[i].Tag_based_player = Tag_based_player;
						Grasses[i].Player_tag = Player_tag;
					}
					for(int i=0;i<DynamicCombiners.Count;i++){
						if(DynamicCombiners[i] != null){
							DynamicCombiners[i].GetComponent<ControlCombineChildrenINfiniDyGrass>().Tag_based_player = Tag_based_player;
							DynamicCombiners[i].GetComponent<ControlCombineChildrenINfiniDyGrass>().Player_tag = Player_tag;
								DynamicCombiners[i].GetComponent<ControlCombineChildrenINfiniDyGrass>().player = null;
						}
					}
					for(int i=0;i<StaticCombiners.Count;i++){
						if(StaticCombiners[i] != null){
							StaticCombiners[i].GetComponent<ControlCombineChildrenINfiniDyGrass>().Tag_based_player = Tag_based_player;
							StaticCombiners[i].GetComponent<ControlCombineChildrenINfiniDyGrass>().Player_tag = Player_tag;
								StaticCombiners[i].GetComponent<ControlCombineChildrenINfiniDyGrass>().player = null;
						}
					}
					Tag_based_player_prev = Tag_based_player;
					Player_tag_prev = Player_tag;
				}

				//if((Tag_based_player != Tag_based_player_prev) | (Player_tag != Player_tag_prev)){
				{
					for(int i=0;i<Grasses.Count;i++){
						Grasses[i].Tag_based_player = Tag_based_player;
						Grasses[i].Player_tag = Player_tag;
					}
					for(int i=0;i<DynamicCombiners.Count;i++){
						if(DynamicCombiners[i] != null){
							DynamicCombiners[i].GetComponent<ControlCombineChildrenINfiniDyGrass>().Tag_based_player = Tag_based_player;
							DynamicCombiners[i].GetComponent<ControlCombineChildrenINfiniDyGrass>().Player_tag = Player_tag;
						}
					}
					for(int i=0;i<StaticCombiners.Count;i++){
						if(StaticCombiners[i] != null){
							StaticCombiners[i].GetComponent<ControlCombineChildrenINfiniDyGrass>().Tag_based_player = Tag_based_player;
							StaticCombiners[i].GetComponent<ControlCombineChildrenINfiniDyGrass>().Player_tag = Player_tag;
						}
					}
					//Tag_based_player_prev = Tag_based_player;
					//Player_tag_prev = Player_tag;
				}
			}

			//v1.3
			if (Application.isPlaying && player == null) {
				if (Tag_based_player) {

					player = GameObject.FindGameObjectWithTag (Player_tag);

				} else {

					if (Camera.main != null) {
						player = Camera.main.gameObject;
					}

				}

				//enable all grass again, after editor LOD
				for (int i=0; i<Grasses.Count; i++) {

					if(GradualGrowth & UnGrown & player != null){
						//for (int i=0; i<Grasses.Count; i++) {					
						if(Vector3.Distance(player.transform.position,Grasses[i].gameObject.transform.position) < EnableDist){
							//make appear gradually
							if(!Grasses[i].gameObject.activeInHierarchy){
								Grasses[i].Grow_tree_speed = 1000;
								Grasses[i].gameObject.SetActive(true);
								//Debug.Log("activated:"+i);
								//if(Random.Range(1,3)==2){
								//	break;
								//}
							}
						}
						//}
					}					

				}
			}


			//v1.1 - terrain adapt
			//if (AdaptOnTerrain) { //v1.7.6
			
				if(currentTerrain == null | (currentTerrain != null && Terrain.activeTerrain != null && currentTerrain != Terrain.activeTerrain )){
					if(Terrain.activeTerrain != null){
						currentTerrain = Terrain.activeTerrain;
						Tdata = currentTerrain.terrainData;
						Tpos = currentTerrain.transform.position;
					}
				}
				if(currentTerrain != null){
					//Tdata
					if(Tdata == null){
						Tdata = currentTerrain.terrainData;
						Tpos = currentTerrain.transform.position;
					}
					//if(Tpos == Vector3.zero){

					//}

					//v1.4
					if(!Application.isPlaying){
						//v1.7
						if (Terrain.activeTerrain != null) {
							currentTerrain = Terrain.activeTerrain;
							Tpos = currentTerrain.transform.position;
							Tdata = currentTerrain.terrainData;
						}
					}
				}
			
			//}


			if (Application.isPlaying) {

				//v1.3 rescale brushes
				bool enter_sizing = false;
				if (WorldScale <= 0) {
					WorldScale=0.05f;
				}
				if (WorldScale != prev_WorldScale) {			
					prev_WorldScale = WorldScale;	
					enter_sizing = true;
				}
				if((prev_brush != Grass_selector | enter_sizing) ){

					prev_brush = Grass_selector;
					enter_sizing = false;

					//Grass
					if (Grass_selector == 0) {
						Min_density = 1;
						Max_density = 4;
						//SpecularPower = 4;
						Min_spread = 7;
						Max_spread = 9;
						min_scale = 0.4f;
						max_scale = 0.6f;
						
						Cutoff_distance = 530;
						LOD_distance = 520;
						LOD_distance1 = 523;
						LOD_distance2 = 527;
						
						RandomRot = false;
					}
					//Vertex grass
					if (Grass_selector == 1) {
						min_scale = 0.4f;
						max_scale = 0.8f;
						Min_density = 2.0f;
						Max_density = 3.0f;
						Min_spread = 7;
						Max_spread = 9;
						//SpecularPower = 4;
						
//						Cutoff_distance = 530;
//						LOD_distance = 520;
//						LOD_distance1 = 523;
//						LOD_distance2 = 527;
						
						RandomRot = false;
					}
					//Red flowers
					if (Grass_selector== 2) {
						min_scale = 0.8f;
						max_scale = 0.9f;
						Min_density = 1.0f;
						Max_density = 1.0f;
						Min_spread = 7;
						Max_spread = 10;
						//SpecularPower = 4;
						
//						Cutoff_distance = 530;
//						LOD_distance = 520;
//						LOD_distance1 = 523;
//						LOD_distance2 = 527;
						
						RandomRot = true;
					}
					//Wheet
					if (Grass_selector == 3) {
						min_scale = 1.0f;
						max_scale = 1.5f;
						Min_density = 1.0f;
						Max_density = 1.0f;
						Min_spread = 15;
						Max_spread = 20;
						//SpecularPower = 4;
						
//						Cutoff_distance = 530;
//						LOD_distance = 520;
//						LOD_distance1 = 523;
//						LOD_distance2 = 527;
						
						RandomRot = false;
					}
					//Detailed vertex
					if (Grass_selector == 4) {
						min_scale = 1.0f;
						max_scale = 1.2f;
						Min_density = 1.0f;
						Max_density = 3.0f;
						Min_spread = 7;
						Max_spread = 10;
						//SpecularPower = 4;
						
//						Cutoff_distance = 530;
//						LOD_distance = 520;
//						LOD_distance1 = 523;
//						LOD_distance2 = 527;
						
						RandomRot = false;
					}
					
					//Simple vertex
					if (Grass_selector == 5) {
						min_scale = 0.5f;
						max_scale = 1.0f;
						Min_density = 2.0f;
						Max_density = 3.0f;
						Min_spread = 7;
						Max_spread = 10;
						//SpecularPower = 4;
						
//						Cutoff_distance = 530;
//						LOD_distance = 520;
//						LOD_distance1 = 523;
//						LOD_distance2 = 527;
						
						RandomRot = false;
					}
					//White flowers
					if (Grass_selector == 6) {
						min_scale = 0.6f;
						max_scale = 0.9f;
						Min_density = 1.0f;
						Max_density = 1.0f;
						Min_spread = 7;
						Max_spread = 10;
						//SpecularPower = 4;
						
//						Cutoff_distance = 530;
//						LOD_distance = 520;
//						LOD_distance1 = 523;
//						LOD_distance2 = 527;
						
						RandomRot = true;
					}
					//Curved grass
					if (Grass_selector == 7) {
						min_scale = 0.5f;
						max_scale = 1.5f;
						Min_density = 1.0f;
						Max_density = 4.0f;
						Min_spread = 7;
						Max_spread = 8;
						//SpecularPower = 4;
						
//						Cutoff_distance = 530;
//						LOD_distance = 520;
//						LOD_distance1 = 523;
//						LOD_distance2 = 527;
						
						RandomRot = false;
					}
					//Low grass - FOR LIGHT DEMO without Sky Master and real time use
					if (Grass_selector == 8) {
						min_scale = 1.2f;
						max_scale = 1.3f;
						Min_density = 1.0f;
						Max_density = 3.0f;
						Min_spread = 4;
						Max_spread = 6;
						//SpecularPower = 4;
						Collider_scale = 0.4f;
						
//						Cutoff_distance = 530;
//						LOD_distance = 520;
//						LOD_distance1 = 523;
//						LOD_distance2 = 527;
						
						RandomRot = false;
					}
					//Vines
					if (Grass_selector == 9) {
						min_scale = 1.5f;
						max_scale = 1.5f;
						Min_density = 3.0f;
						Max_density = 3.0f;
						Min_spread = 7;
						Max_spread = 7;
						//SpecularPower = 4;
						
//						Cutoff_distance = 530;
//						LOD_distance = 520;
//						LOD_distance1 = 523;
//						LOD_distance2 = 527;
						
						RandomRot = false;
					}
					
					//Mushrooms Brown and red
					if (Grass_selector == 10 | Grass_selector == 11) {
						min_scale = 0.4f;
						max_scale = 1.0f;
						Min_density = 1.0f;
						Max_density = 4.0f;
						Min_spread = 7;
						Max_spread = 9;
						//SpecularPower = 4;
						
//						Cutoff_distance = 530;
//						LOD_distance = 80;
//						LOD_distance1 = 120;
//						LOD_distance2 = 520;
						
						RandomRot = false;
					}
					//Ground leaves
					if (Grass_selector == 12) {
						min_scale = 0.5f;
						max_scale = 0.8f;
						Min_density = 1.0f;
						Max_density = 3.0f;
						Min_spread = 7;
						Max_spread = 11;
						//SpecularPower = 4;
						
//						Cutoff_distance = 530;
//						LOD_distance = 520;
//						LOD_distance1 = 523;
//						LOD_distance2 = 527;
						
						RandomRot = true;
					}
					//Noisy grass
					if (Grass_selector == 13) {
						min_scale = 0.5f;
						max_scale = 1.5f;
						Min_density = 2.0f;
						Max_density = 3.0f;
						Min_spread = 7;
						Max_spread = 9;
						//SpecularPower = 4;
						
//						Cutoff_distance = 530;
//						LOD_distance = 520;
//						LOD_distance1 = 523;
//						LOD_distance2 = 527;
						
						RandomRot = true;
					}
					//Rocks
					if (Grass_selector == 14) {
						min_scale = 0.7f;
						max_scale = 1.2f;
						Min_density = 1.0f;
						Max_density = 3.0f;
						Min_spread = 7;
						Max_spread = 11;
						//SpecularPower = 4;							
						
//						Cutoff_distance = 520;
//						LOD_distance = 220;
//						LOD_distance1 = 270;
//						LOD_distance2 = 410;
						
						RandomRot = false;
					}

					//v1.4
					if (Grass_selector > 14) {
						min_scale = 0.7f;
						max_scale = 0.9f;
						Min_density = 2.0f;
						Max_density = 4.0f;
						Min_spread = 4;
						Max_spread = 5;
						RandomRot = false;
					}
					
					min_scale = min_scale*0.55f;
					max_scale = max_scale*0.55f;

					//v1.2a
					min_scale = min_scale*(WorldScale/20);
					max_scale =  max_scale*(WorldScale/20);
					//	Min_density =  Min_density*(WorldScale/20);
					//	Max_density =  Max_density*(WorldScale/20);
					Min_spread =  Min_spread*(WorldScale/20);
					Max_spread =  Max_spread*(WorldScale/20);
					//SpecularPower = 4;									
//					Cutoff_distance = Cutoff_distance*(WorldScale/20);
//					LOD_distance =  LOD_distance*(WorldScale/20);
//					LOD_distance1 =  LOD_distance1*(WorldScale/20);
//					LOD_distance2 =  LOD_distance2*(WorldScale/20);
					
					//AmplifyWind = 1*(WorldScale/20); //v1.7.8d
					//WindTurbulence = 0.5f*(WorldScale/20); //v1.7.8d
					
				//	Editor_view_dist = 4500*(WorldScale/20);
					min_grass_patch_dist = 1;
					//Stop_Motion_distance = 20*(WorldScale/20); //v1.7.8d
					
					for(int i=0;i<GrassMaterials.Count;i++){
						if (GrassMaterials [i].HasProperty ("_SmoothMotionFactor")) { //v1.7.8d
							GrassMaterials [i].SetFloat ("_SmoothMotionFactor", 255);
						}
						//GrassMaterials[i].SetVector("_TimeControl1",new Vector4(2,1,1,0)); //v1.7.8d
						if(GrassMaterials[i].HasProperty("_TimeControl1")){ //v1.7.8d
							Vector4 timeVec = GrassMaterials[i].GetVector("_TimeControl1"); //v1.7.8d
							GrassMaterials[i].SetVector("_TimeControl1",new Vector4(2,timeVec.y,timeVec.z,0)); //v1.7.8d
						}
					}
					Override_density = true;
					Override_spread = true;
					MinAvoidDist = 2/10;
					MinScaleAvoidDist= 4/10;
					SphereCastRadius = (WorldScale/20)*50;
					//Grass_Fade_distance = Cutoff_distance - 60*((WorldScale/20));
					//Gizmo_scale = (WorldScale/20)*3;
					Collider_scale = (WorldScale/20);
					//END v1.2a
				}


				//v1.3
				if(GradualGrowth & UnGrown){
					for (int i=0; i<Grasses.Count; i++) {

						int Randomizer = UnityEngine.Random.Range(1,3);
						bool did_addition=false;

						if(Randomizer == 2 ){

							if(!UseDistFromPlayer || (Grasses[i]!= null && UseDistFromPlayer && player!=null && (Vector3.Distance(player.transform.position,Grasses[i].gameObject.transform.position) < EnableDist)) ){
								//make appear gradually
								if(!Grasses[i].gameObject.activeInHierarchy){
									Grasses[i].Grow_tree_speed = 1000;
									Grasses[i].gameObject.SetActive(true);
									//Debug.Log("activated:"+i);
									//if(UnityEngine.Random.Range(1,3)==2){
										did_addition = true;
										break;
									//}
								}
							}
						}

						Randomizer = UnityEngine.Random.Range(1,50);
						
						if(Randomizer == 2 & !did_addition){

						if(Grasses[i]!= null && UseDistFromPlayer && player!=null && (Vector3.Distance(player.transform.position,Grasses[i].gameObject.transform.position) > (EnableDist+150)) && Grasses[i].gameObject.activeInHierarchy){


							//erase whole
							//ControlCombineChildrenINfiniDyGrass forest_holder1 = Grasses[i].Forest_holder.GetComponent<ControlCombineChildrenINfiniDyGrass> ();
							//	for(int i=0;i<forest_holder1.Added_item_count;i++){

							//	}

//							if(1==0){
//								//remove if outside bound
//								ControlCombineChildrenINfiniDyGrass forest_holder = Grasses[i].Forest_holder.GetComponent<ControlCombineChildrenINfiniDyGrass> ();
//								INfiniDyGrassField forest = Grasses[i];
//								
//								forest_holder.Restore ();
//								
//						//		DestroyImmediate (forest_holder.Added_items_handles [forest.Tree_Holder_Index].gameObject);
//								DestroyImmediate (forest_holder.Added_items [forest.Tree_Holder_Index].gameObject);
//								
//								forest_holder.Added_items_handles.RemoveAt (forest.Tree_Holder_Index);
//								forest_holder.Added_items.RemoveAt (forest.Tree_Holder_Index);
//								
//								//remove from script
//						//		Grasses.RemoveAt (forest.Grass_Holder_Index);
//						//		GrassesType.RemoveAt (forest.Grass_Holder_Index);
//
//
//								//v1.2
//								forest_holder.Full_reset();							
//								//EXTRAS
//								GameObject LEAF_POOL = new GameObject ();
//								LEAF_POOL.transform.parent = forest.transform;
//								forest.Leaf_pool = LEAF_POOL;							
//								forest.Combiner = null;
//								forest.Grow_in_Editor = false;
//								forest.growth_over = false;
//								forest.Registered_Brances.Clear ();//
//								//forest.root_tree = null;
//								forest.Branch_grew.Clear ();
//								forest.Registered_Leaves.Clear ();//
//								forest.Registered_Leaves_Rot.Clear ();//
//								forest.batching_ended = false;
//								forest.Branch_levels.Clear ();
//								forest.BranchID_per_level.Clear ();
//								//forest.Grass_Holder_Index = 0;
//								forest.Grow_level = 0;
//								forest.Grow_tree_ended = false;
//								forest.Health = forest.Max_Health;
//								forest.is_moving = false;
//								forest.Leaf_belongs_to_branch.Clear ();
//								forest.scaleVectors.Clear ();
//								forest.Leaves.Clear ();
//								forest.Tree_Holder_Index = 0;
//								forest.Grow_tree = false;
//								forest.rotation_over = false;							
//								forest.Forest_holder = null;
//
//									forest.Grow_tree_speed = 1000;
//								forest.Restart = true;
//
//								//adjust ids for items left
//								for (int j=0; j<forest_holder.Added_items.Count; j++) {
//									forest_holder.Added_items_handles [j].Tree_Holder_Index = j;
//									
//								}
//							//	for (int j=0; j<Grasses.Count; j++) {
//							//		Grasses [j].Grass_Holder_Index = j;
//							//	}								
//								
//								forest_holder.Added_item_count -= 1;
//
//								//v1.3
//								forest.gameObject.SetActive(false);
//								
//								forest_holder.MakeActive = true;
//								forest_holder.Decombine = false;
//								forest_holder.Decombined = false;
//
//								break;
//							}//END if 1==0

						}
							//if(UnityEngine.Random.Range(1,2000)==2){
								//GC.Collect();
							//}

					}//end chance

					}
				}

				//check if combiners erased
				for(int i=DynamicCombiners.Count-1;i>=0;i--){
					if(DynamicCombiners[i] == null){
						DynamicCombiners.RemoveAt(i);
					}
				}
				for(int i=StaticCombiners.Count-1;i>=0;i--){
					if(StaticCombiners[i] == null){
						StaticCombiners.RemoveAt(i);
					}
				}


				//real time grass painting
				if (Grass_painting & Enable_real_time_paint) {
			
					if (Input.GetMouseButtonDown (1) & !Input.GetKeyDown(KeyCode.LeftShift)
					    	//& (Camera.main != null | Camera.current != null)
					   		//& !Tag_based_player
					    ) {  

						Ray ray = new Ray();

						//v1.4c
						bool found_cam = false;
						if(Tag_based_player){
							if(player != null){
								Camera[] playerCams = player.GetComponentsInChildren<Camera>(false);
								//Debug.Log(playerCams.Length);
								if(playerCams != null && playerCams.Length > 0 && playerCams[0] != null){
									ray = playerCams[0].ScreenPointToRay (Input.mousePosition);
									found_cam = true;
								}else{
									if(Camera.main != null){
										ray = Camera.main.ScreenPointToRay (Input.mousePosition);
										found_cam = true;
									}
								}
							}
						}else{
							if(Camera.main != null){
								ray = Camera.main.ScreenPointToRay (Input.mousePosition);
								found_cam = true;
							}else if(Camera.current != null){
								ray = Camera.current.ScreenPointToRay (Input.mousePosition);
								found_cam = true;
							}
						}

						RaycastHit hit = new RaycastHit ();
						if (found_cam && Physics.Raycast (ray, out hit, Mathf.Infinity)) {
						
							bool is_Terrain = false;
							if ( (Terrain.activeTerrain != null && hit.collider.gameObject != null && hit.collider.gameObject == Terrain.activeTerrain.gameObject)){
								is_Terrain = true;
							}

							if ( is_Terrain |  (hit.collider.gameObject != null && hit.collider.gameObject.tag == "PPaint")) {//v1.1						
							

								//DONT PLANT if hit another grass collider
								if (hit.collider.GetComponent<GrassChopCollider> () != null) {							

								
								} else {
																
								
									GameObject TEMP = Instantiate (GrassPrefabs [Grass_selector]);
									TEMP.transform.position = hit.point;

									INfiniDyGrassField TREE = TEMP.GetComponent<INfiniDyGrassField> ();
								
									TREE.Intial_Up_Vector = hit.normal;

									TREE.Grow_tree = true;

									//v1.1 - terrain adapt
									if (AdaptOnTerrain & is_Terrain) {
										int Xpos = (int)(((hit.point.x - Tpos.x)*Tdata.alphamapWidth/Tdata.size.x));
										int Zpos = (int)(((hit.point.z - Tpos.z)*Tdata.alphamapHeight/Tdata.size.z));
										float[,,] splats = Tdata.GetAlphamaps(Xpos,Zpos,1,1);
										float[] Tarray = new float[splats.GetUpperBound(2)+1];
										for(int j =0;j<Tarray.Length;j++){
											Tarray[j] = splats[0,0,j];
											//Debug.Log(Tarray[j]); // ScalePerTexture
										}
										float Scaling = 0;
										for(int j =0;j<Tarray.Length;j++){
											if(j > ScalePerTexture.Count-1){
												Scaling = Scaling + (1*Tarray[j]);
											}else{
												Scaling = Scaling + (ScalePerTexture[j]*Tarray[j]);
											}
										}
										TREE.End_scale = Scaling*UnityEngine.Random.Range (min_scale, max_scale);
										//Debug.Log(Tarray);
									}else{
										TREE.End_scale = UnityEngine.Random.Range (min_scale, max_scale);
									}

									TREE.Max_interact_holder_items = Max_interactive_group_members;//Define max number of trees grouped in interactive batcher that opens up. 
									//Increase to lower draw calls, decrease to lower spikes when group is opened for interaction
									TREE.Max_trees_per_group = Max_static_group_members;
								
									TREE.Interactive_tree = Interactive;
									TREE.transform.localScale *= TREE.End_scale * Collider_scale;

									if(Override_spread){
										TREE.PosSpread = new Vector2(UnityEngine.Random.Range(Min_spread,Max_spread),UnityEngine.Random.Range(Min_spread,Max_spread));
									}
									if(Override_density){
										TREE.Min_Max_Branching = new Vector2(Min_density,Max_density);
									}
									TREE.PaintedOnOBJ = hit.transform.gameObject.transform;
									TREE.GridOnNormal = GridOnNormal;
									TREE.max_ray_dist = rayCastDist;
									TREE.MinAvoidDist = MinAvoidDist;
									TREE.MinScaleAvoidDist = MinScaleAvoidDist;
									TREE.InteractionSpeed = InteractionSpeed;
									TREE.InteractSpeedThres = InteractSpeedThres;

                                    //v1.9.9.7
                                    TREE.enableAbove64KMesh = enableAbove64KMesh;

									//v1.4
									TREE.Interaction_thres = Interaction_thres;
									TREE.Max_tree_dist = Max_tree_dist;//v1.4.6
									TREE.Disable_after_growth = Disable_after_growth;//v1.5
									TREE.WhenCombinerFull = WhenCombinerFull;//v1.5
									TREE.Eliminate_original_mesh = Eliminate_original_mesh;//v1.5
									TREE.Interaction_offset = Interaction_offset;

									TREE.LOD_distance = LOD_distance;
									TREE.LOD_distance1 = LOD_distance1;
									TREE.LOD_distance2 = LOD_distance2;
									TREE.Cutoff_distance = Cutoff_distance;

									TREE.Tag_based = false;
									TREE.GrassManager = this;
									TREE.Type = Grass_selector+1;
									TREE.Start_tree_scale = TREE.End_scale/4;
								
									TREE.RandomRot = RandomRot;
									TREE.RandRotMin = RandRotMin;
									TREE.RandRotMax = RandRotMax;

									TREE.GroupByObject = GroupByObject;
									TREE.ParentToObject = ParentToObject;
									TREE.MoveWithObject = MoveWithObject;
									TREE.AvoidOwnColl = AvoidOwnColl;

									TEMP.transform.parent = GrassHolder.transform;
								
									//Add to holder, in order to mass change properties
									Grasses.Add (TREE);
									GrassesType.Add (Grass_selector);
								
									TEMP.name = "GrassPatch" + Grasses.Count.ToString (); 
								
									TREE.Grass_Holder_Index = Grasses.Count - 1;//register id in grasses list								

								}
							}				
						
						}//END RAYCAST
					}//END MOUSE CLICK CHECK	

					if (Input.GetMouseButtonDown (0) 
					    //& Input.GetKeyDown(KeyCode.LeftShift)
					    & Enable_real_time_erase
					   // & (Camera.main != null | Camera.current != null)
					   // & !Tag_based_player
					    ) {

						Ray ray = new Ray();
						
						//v1.4c
						bool found_cam = false;
						if(Tag_based_player){
							if(player != null){
								Camera[] playerCams = player.GetComponentsInChildren<Camera>(false);
								//Debug.Log(playerCams.Length);
								if(playerCams != null && playerCams.Length > 0 && playerCams[0] != null){
									ray = playerCams[0].ScreenPointToRay (Input.mousePosition);
									found_cam = true;
								}else{
									if(Camera.main != null){
										ray = Camera.main.ScreenPointToRay (Input.mousePosition);
										found_cam = true;
									}
								}
							}
						}else{
							if(Camera.main != null){
								ray = Camera.main.ScreenPointToRay (Input.mousePosition);
								found_cam = true;
							}else if(Camera.current != null){
								ray = Camera.current.ScreenPointToRay (Input.mousePosition);
								found_cam = true;
							}
						}

						//Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
						RaycastHit hit = new RaycastHit ();

						if (found_cam && Physics.SphereCast (ray,20, out hit, Mathf.Infinity)) {
														
							if (hit.collider.gameObject.GetComponent<GrassChopCollider> () != null) {
								ControlCombineChildrenINfiniDyGrass forest_holder = hit.collider.gameObject.GetComponent<GrassChopCollider> ().TreeHandler.Forest_holder.GetComponent<ControlCombineChildrenINfiniDyGrass> ();
								INfiniDyGrassField forest = hit.collider.gameObject.GetComponent<GrassChopCollider> ().TreeHandler;
								
								forest_holder.Restore ();
								
								DestroyImmediate (forest_holder.Added_items_handles [forest.Tree_Holder_Index].gameObject);
								DestroyImmediate (forest_holder.Added_items [forest.Tree_Holder_Index].gameObject);
								
								forest_holder.Added_items_handles.RemoveAt (forest.Tree_Holder_Index);
								forest_holder.Added_items.RemoveAt (forest.Tree_Holder_Index);
								
								//remove from script
								Grasses.RemoveAt (forest.Grass_Holder_Index);
								GrassesType.RemoveAt (forest.Grass_Holder_Index);
								
								//adjust ids for items left
								for (int i=0; i<forest_holder.Added_items.Count; i++) {
									forest_holder.Added_items_handles [i].Tree_Holder_Index = i;
									
								}
								for (int i=0; i<Grasses.Count; i++) {
									Grasses [i].Grass_Holder_Index = i;
								}								
								
								forest_holder.Added_item_count -= 1;
								
								forest_holder.MakeActive = true;
								forest_holder.Decombine = false;
								forest_holder.Decombined = false;
								
							} else {
								//Debug.Log ("no col");
							}
								
						}//END RAYCAST
					}//END IF ERASING MOUSE CLICK CHECK

				}//END PAINTING

				///////////////////////////////////// FENCE PAINT			

				///////////////////////////////// FENCE PAINTING /////////////////////////////////
				if (Fence_painting & Enable_real_time_paint
				    //& (Camera.main != null | Camera.current != null)
				    //& !Tag_based_player
				  ) {
				
					//ERASE FENCE
					if (Input.GetKeyDown (KeyCode.LeftShift)) {

					} else {

						if (Input.GetMouseButtonDown (1) && Fence_poles_tmp.Count > 0) {

							for (int i = 0; i<Fence_poles_tmp.Count; i++) {
																		
								Fence_poles.Add (Fence_poles_tmp [i]);
								Fence_poles_midA.Add (Fence_poles_midA_tmp [i]);//
							
							}
							Fence_poles_tmp.Clear ();
							Fence_poles_midA_tmp.Clear ();//

						} else {						

							Ray ray = new Ray();
							
							//v1.4c
							bool found_cam = false;
							if(Tag_based_player){
								if(player != null){
									Camera[] playerCams = player.GetComponentsInChildren<Camera>(false);
									//Debug.Log(playerCams.Length);
									if(playerCams != null && playerCams.Length > 0 && playerCams[0] != null){
										ray = playerCams[0].ScreenPointToRay (Input.mousePosition);
										found_cam = true;
									}else{
										if(Camera.main != null){
											ray = Camera.main.ScreenPointToRay (Input.mousePosition);
											found_cam = true;
										}
									}
								}
							}else{
								if(Camera.main != null){
									ray = Camera.main.ScreenPointToRay (Input.mousePosition);
									found_cam = true;
								}else if(Camera.current != null){
									ray = Camera.current.ScreenPointToRay (Input.mousePosition);
									found_cam = true;
								}
							}

							//Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
													
							if (!Fence_start_added) {

								if (Input.GetMouseButtonDown (1)) {
									Fence_start_added = true;

								}
							} else {
							
								RaycastHit hit = new RaycastHit ();
								if (found_cam && Physics.Raycast (ray, out hit, Mathf.Infinity)) {							
									bool erasing = false;
								
									if ( (Terrain.activeTerrain != null && hit.collider.gameObject != null && hit.collider.gameObject == Terrain.activeTerrain.gameObject) 
									    | (hit.collider.gameObject != null && hit.collider.gameObject.tag == "PPaint")) {//v1.1
									
										if (erasing) {
										
										} else {																					
																				
											if (Fence_poles.Count == 0) {

												GameObject TEMP = Instantiate (FencePrefabs [Grass_selector]);
												TEMP.transform.position = hit.point;
										
												TEMP.transform.localScale = TEMP.transform.localScale * fence_scale;
												TEMP.transform.up = hit.normal;
											
												GameObject Fence = new GameObject();
												Fence.name = "Fence"+RealTimeFences.Count.ToString();
												RealTimeFences.Add(Fence.transform);
												TEMP.transform.parent = RealTimeFences[RealTimeFences.Count-1];																							
												Fence_poles.Add (TEMP.transform);
											
											} else {
											
												//create and destroy on the fly, register with mouse up
												if (1 == 1)												
												{
													///////////////////////// IF ONE BY ONE
													if (Fence_poles.Count > 0 && Fence_poles [Fence_poles.Count - 1] != null) {
														float dist = Vector3.Distance (Fence_poles [Fence_poles.Count - 1].position, hit.point); 
														//int fences = (int)(dist / min_grass_patch_dist);

														if (dist > min_grass_patch_dist) {
														
														
															float Mid_width_scale = 2;
															float Mid_height_scale = 2;
															float Fence_part_real_height = 4.75f;//the actual height of the object (not inspector scale, real height of prefab item in world space), divide to get final scale
															float Fence_part_real_width = 0.25f;
															float MidA_height_percent = 0.63f;

															float Vert_posA = Fence_part_real_height * MidA_height_percent * fence_scale;
														
															//display next fence
															if (Fence_poles_tmp.Count == 1) {

																Fence_poles_tmp [0].position = hit.point;
																Fence_poles_tmp [0].up = hit.normal;														
																Fence_poles_tmp [0].forward = hit.point - Fence_poles [Fence_poles.Count - 1].position;
															
															
																Fence_poles_midA_tmp [0].position = hit.point + Fence_poles_tmp [0].up * Vert_posA  
																	- Fence_poles_tmp [0].forward * Fence_poles_tmp [0].localScale.z * Fence_part_real_width;
															
																Fence_poles_midA_tmp [0].up = (Fence_poles [Fence_poles.Count - 1].position + Fence_poles [Fence_poles.Count - 1].up * Vert_posA) - (hit.point + Fence_poles_tmp [0].up * Vert_posA);
															
																Fence_poles_midA_tmp [0].forward = -Fence_poles_tmp [0].up;
															
																Fence_poles_midA_tmp [0].rotation = Quaternion.LookRotation (Fence_poles_tmp [0].up,
															                                                            (Fence_poles [Fence_poles.Count - 1].position + Fence_poles [Fence_poles.Count - 1].up * Vert_posA) - (Fence_poles_tmp [0].position + Fence_poles_tmp [0].up * Vert_posA));
															
																Vector3 ScaleInit = Fence_poles_midA_tmp [0].localScale;															

																float distA = Vector3.Distance (Fence_poles [Fence_poles.Count - 1].position + Vert_posA * Fence_poles [Fence_poles.Count - 1].up, 
															                                Fence_poles_tmp [0].position + Vert_posA * Fence_poles_tmp [0].up);
																Fence_poles_midA_tmp [0].localScale = new Vector3 (ScaleInit.x, distA / Fence_part_real_height, ScaleInit.z);														


															} else {

																GameObject TEMP = Instantiate (FencePrefabs [Grass_selector]);
																TEMP.transform.position = hit.point;											
																TEMP.transform.parent = RealTimeFences[RealTimeFences.Count-1];	
															
																TEMP.transform.up = hit.normal;
																TEMP.transform.localScale = TEMP.transform.localScale * fence_scale;
															
																TEMP.transform.forward = hit.point - Fence_poles [0].position;															

																string name = "Fence " + Fence_poles.Count;
																TEMP.name = name;
																Fence_poles_tmp.Add (TEMP.transform);															

																TEMP = Instantiate (FenceMidPrefabs [Grass_selector]);
																TEMP.transform.position = hit.point + new Vector3 (0, Vert_posA, 0);											
																TEMP.transform.parent = RealTimeFences[RealTimeFences.Count-1];
																//float dist1 = 1.6f;
															
																TEMP.transform.up = hit.point - Fence_poles_tmp [0].position;
																//dist1 = (hit.point - Fence_poles_tmp [0].position).magnitude / 5;
															
																TEMP.transform.localScale = new Vector3 (TEMP.transform.localScale.x / Mid_width_scale, TEMP.transform.localScale.y, TEMP.transform.localScale.z / Mid_height_scale) * fence_scale;
															
																Fence_poles_midA_tmp.Add (TEMP.transform);
															}														
														
														} else {

															for (int i = Fence_poles_tmp.Count-1; i>=0; i--) {
																if (i < Fence_poles_tmp.Count) {
																	DestroyImmediate (Fence_poles_tmp [i].gameObject);														
																	Fence_poles_tmp.RemoveAt (i);																

																	DestroyImmediate (Fence_poles_midA_tmp [i].gameObject);														
																	Fence_poles_midA_tmp.RemoveAt (i);
																}
															}
														}
													}
												
												}//END FENCE ADD ANDLE											
											
											}
										}
									}
								
									if (erasing) {								
									
									}									
								
								}
								//
							}
						}					
					}				
				}// END FENCE PAINTING

			else {
					if(Fence_start_added){
						//Destroy left over parts
						for (int i = Fence_poles_tmp.Count-1; i>=0; i--) {
							if (i < Fence_poles_tmp.Count) {
								DestroyImmediate (Fence_poles_tmp [i].gameObject);														
								Fence_poles_tmp.RemoveAt (i);
							
								//
								DestroyImmediate (Fence_poles_midA_tmp [i].gameObject);														
								Fence_poles_midA_tmp.RemoveAt (i);
							}
						}
						//clear everything
						Fence_poles.Clear ();
						Fence_poles_midA.Clear ();//
					
						Fence_poles_tmp.Clear ();
						Fence_poles_midA_tmp.Clear ();//
					
						Fence_start_added = false;

						RealTimeFences[RealTimeFences.Count-1].gameObject.AddComponent<ControlCombineChildrenINfiniDyGrass>().MakeActive = true;
						RealTimeFences[RealTimeFences.Count-1].gameObject.GetComponent<ControlCombineChildrenINfiniDyGrass>().Auto_Disable = true;
						RealTimeFences[RealTimeFences.Count-1].gameObject.GetComponent<ControlCombineChildrenINfiniDyGrass>().realtime = true;
					}
				
			}
			///////////////////////////////////// END FENCE PAINT			

			///////////////////////////////// ROCK PAINTING /////////////////////////////////
			if (Rock_painting & Enable_real_time_paint
				    //& (Camera.main != null | Camera.current != null)
				    //& !Tag_based_player
			) {			
			
				
				//ERASE GRASS
				if(Input.GetKeyDown(KeyCode.LeftShift)){
					Debug.Log("erasing");
				}
				
				if (Input.GetMouseButtonDown (0)) 
				{
					
					Ray ray = new Ray();
					
						//v1.4c
						bool found_cam = false;
						if(Tag_based_player){
							if(player != null){
								Camera[] playerCams = player.GetComponentsInChildren<Camera>(false);
								//Debug.Log(playerCams.Length);
								if(playerCams != null && playerCams.Length > 0 && playerCams[0] != null){
									ray = playerCams[0].ScreenPointToRay (Input.mousePosition);
									found_cam = true;
								}else{
									if(Camera.main != null){
										ray = Camera.main.ScreenPointToRay (Input.mousePosition);
										found_cam = true;
									}
								}
							}
						}else{
							if(Camera.main != null){
								ray = Camera.main.ScreenPointToRay (Input.mousePosition);
								found_cam = true;
							}else if(Camera.current != null){
								ray = Camera.current.ScreenPointToRay (Input.mousePosition);
								found_cam = true;
							}
						}

					//Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					RaycastHit hit1 = new RaycastHit ();
					if (found_cam && Physics.Raycast (ray, out hit1, Mathf.Infinity)) {
							if(hit1.collider.transform.parent != null){
								if(hit1.collider.transform.parent.gameObject.name == "Rocks Holder"){
									Destroy(hit1.collider.gameObject);
									RocksHolder.GetComponent<ControlCombineChildrenINfiniDyGrass>().MakeActive = true;
									RocksHolder.GetComponent<ControlCombineChildrenINfiniDyGrass>().realtime = true;
								}
							}
					}
				}
				
				if (Input.GetMouseButtonDown (1)) 
				{					
					if(1==1)
					{

						Ray ray = new Ray();
						
							//v1.4c
							bool found_cam = false;
							if(Tag_based_player){
								if(player != null){
									Camera[] playerCams = player.GetComponentsInChildren<Camera>(false);
									//Debug.Log(playerCams.Length);
									if(playerCams != null && playerCams.Length > 0 && playerCams[0] != null){
										ray = playerCams[0].ScreenPointToRay (Input.mousePosition);
										found_cam = true;
									}else{
										if(Camera.main != null){
											ray = Camera.main.ScreenPointToRay (Input.mousePosition);
											found_cam = true;
										}
									}
								}
							}else{
								if(Camera.main != null){
									ray = Camera.main.ScreenPointToRay (Input.mousePosition);
									found_cam = true;
								}else if(Camera.current != null){
									ray = Camera.current.ScreenPointToRay (Input.mousePosition);
									found_cam = true;
								}
							}

						//Keep_last_mouse_pos = cur.mousePosition;
						//Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
						RaycastHit hit = new RaycastHit ();
						if (found_cam && Physics.Raycast (ray, out hit, Mathf.Infinity)) {
								
								if((Terrain.activeTerrain != null && hit.collider.gameObject != null && hit.collider.gameObject == Terrain.activeTerrain.gameObject) 
								   | (hit.collider.gameObject != null && hit.collider.gameObject.tag == "PPaint")){//v1.1									

									GameObject TEMP = Instantiate(RockPrefabs[Grass_selector]);
									TEMP.transform.position = hit.point;	
									
									TEMP.transform.Rotate(Vector3.up,UnityEngine.Random.Range(-180,180));

									TEMP.transform.localScale = TEMP.transform.localScale * UnityEngine.Random.Range(min_scale,max_scale);
									
									TEMP.transform.parent = RocksHolder.transform;	

									RocksHolder.GetComponent<ControlCombineChildrenINfiniDyGrass>().MakeActive = true;
									RocksHolder.GetComponent<ControlCombineChildrenINfiniDyGrass>().realtime = true;
								}															
							}
						}
					}				
				}// END ROCK PAINTING



				if (player == null) {
			
					player = GameObject.FindGameObjectWithTag ("Player");
				}


			}


			//if type changes, load specular and tint
			if (prev_type != Grass_selector && Grass_selector < GrassMaterials.Count) {
				if(GrassMaterials[Grass_selector].HasProperty("_OceanCenter") & GrassMaterials[Grass_selector].HasProperty("_SpecularPower") ){
					TintPower =  GrassMaterials[Grass_selector].GetFloat("_TintPower");
					tintColor = GrassMaterials[Grass_selector].GetColor("_Color");
				}			
				if(GrassMaterials[Grass_selector].HasProperty("_TintFrequency")) {
					TintFrequency =  GrassMaterials[Grass_selector].GetFloat("_TintFrequency");
				}
				if(GrassMaterials[Grass_selector].HasProperty("_OceanCenter") & GrassMaterials[Grass_selector].HasProperty("_SpecularPower") ){
					SpecularPower = GrassMaterials[Grass_selector].GetFloat("_SpecularPower");
				}
				//v1.5
				if(GrassMaterials[Grass_selector].HasProperty("_FadeThreshold")){
					Grass_Fade_distance = GrassMaterials[Grass_selector].GetFloat("_FadeThreshold");
				}
				if(GrassMaterials[Grass_selector].HasProperty("_StopMotionThreshold")){
					Stop_Motion_distance =GrassMaterials[Grass_selector].GetFloat("_StopMotionThreshold");
				}
				prev_type = Grass_selector;
			}


			//v1.5
			if (ShaderBasedInteract && player != null) {
				Vector3 PPos = player.transform.position;
				Vector3 Direction = prev_pos - PPos;
				SpeedVec = (Direction).normalized * ((prev_pos - PPos).magnitude / Time.deltaTime);
				prev_pos = PPos;
			}

			for (int i=0; i<GrassMaterials.Count; i++) {

				//v1.4
				UpdateMaterial(GrassMaterials[i],i);

//				if(GrassMaterials[i] != null){
//				 
//					if(ShaderWind && windzone != null){
//						if(GrassMaterials[i].HasProperty("_OceanCenter") & GrassMaterials[i].HasProperty("_SpecularPower") ){
//
//							if(Preview_wind | Application.isPlaying){
//								GrassMaterials[i].SetVector("_OceanCenter",new Vector4(windzone.forward.x,windzone.forward.y,windzone.forward.z,1)*windzoneScript.windMain * AmplifyWind);
//								GrassMaterials[i].SetFloat("_BulgeScale",windzoneScript.windTurbulence * WindTurbulence);
//							}else{
//								GrassMaterials[i].SetVector("_OceanCenter",Vector4.zero);
//							}
//						}
//					}else{
//						if(GrassMaterials[i].HasProperty("_OceanCenter") & GrassMaterials[i].HasProperty("_SpecularPower") ){
//							GrassMaterials[i].SetVector("_OceanCenter",Vector4.zero);
//						}
//					}
//
//					if(i == Grass_selector){
//						if(TintGrass){
//							if(GrassMaterials[i].HasProperty("_OceanCenter") & GrassMaterials[i].HasProperty("_SpecularPower") ){
//								GrassMaterials[i].SetFloat("_TintPower",TintPower);
//								GrassMaterials[i].SetColor("_Color",tintColor);
//							}
//						}
//						if(GrassMaterials[i].HasProperty("_TintFrequency")) {
//							GrassMaterials[i].SetFloat("_TintFrequency",TintFrequency);
//						}
//						if(GrassMaterials[i].HasProperty("_OceanCenter") & GrassMaterials[i].HasProperty("_SpecularPower") ){
//							GrassMaterials[i].SetFloat("_SpecularPower",SpecularPower);
//						}
//					}
//
//					if(Application.isPlaying){
//						if(GrassMaterials[i].HasProperty("_InteractPos")){
//							GrassMaterials[i].SetVector("_InteractPos",player.transform.position);
//						}
//					}
//					if(GrassMaterials[i].HasProperty("_CameraPos")){
//						if(Camera_transf !=null){
//							GrassMaterials[i].SetVector("_CameraPos",Camera_transf.position);
//						}
//					}
//
//					//v1.4 do per brush
//					if(i == Grass_selector){
//						if(GrassMaterials[i].HasProperty("_FadeThreshold")){
//							GrassMaterials[i].SetFloat("_FadeThreshold",Grass_Fade_distance);
//						}
//						if(GrassMaterials[i].HasProperty("_StopMotionThreshold")){
//							GrassMaterials[i].SetFloat("_StopMotionThreshold",Stop_Motion_distance);
//						}
//					}
//				  
//				}
			}

			//v1.4
			for (int i=0; i<ExtraGrassMaterials.Count; i++) {
				for (int j=0; j<ExtraGrassMaterials[i].ExtraMaterials.Count; j++) {
					//UpdateMaterial (ExtraGrassMaterials[i][j], i);
					UpdateMaterial (ExtraGrassMaterials[i].ExtraMaterials[j], i);
				}
			}


	}//END UPDATE

		void UpdateMaterial(Material GrassMaterial, int i){
			//if(GrassMaterial != null){
			if(UpdateMaterials && GrassMaterial != null){ //v1.7.5
				
				if(ShaderWind && windzone != null){
					if(GrassMaterial.HasProperty("_OceanCenter") & GrassMaterial.HasProperty("_SpecularPower") ){
						
						if(Preview_wind | Application.isPlaying){
							GrassMaterial.SetVector("_OceanCenter",new Vector4(windzone.forward.x,windzone.forward.y,windzone.forward.z,1)*windzoneScript.windMain * AmplifyWind);
							GrassMaterial.SetFloat("_BulgeScale",windzoneScript.windTurbulence * WindTurbulence);
						}else{
							GrassMaterial.SetVector("_OceanCenter",Vector4.zero);
						}
					}
				}else{
					if(GrassMaterial.HasProperty("_OceanCenter") & GrassMaterial.HasProperty("_SpecularPower") ){
						GrassMaterial.SetVector("_OceanCenter",Vector4.zero);
					}
				}
				
				if(i == Grass_selector){
					if(TintGrass){
						if(GrassMaterial.HasProperty("_OceanCenter") & GrassMaterial.HasProperty("_SpecularPower") ){
							GrassMaterial.SetFloat("_TintPower",TintPower);
							GrassMaterial.SetColor("_Color",tintColor);
						}
					}
					if(GrassMaterial.HasProperty("_TintFrequency")) {
						GrassMaterial.SetFloat("_TintFrequency",TintFrequency);
					}
					if(GrassMaterial.HasProperty("_OceanCenter") & GrassMaterial.HasProperty("_SpecularPower") ){
						GrassMaterial.SetFloat("_SpecularPower",SpecularPower);
					}
				}
				
				if(Application.isPlaying && player != null){

					if (!GrassMaterial.HasProperty ("_InteractSpeed")) {
						Vector3 PPos = player.transform.position;
						if (GrassMaterial.HasProperty ("_InteractPos")) {
							GrassMaterial.SetVector ("_InteractPos", PPos);
						}
					}
					//v1.5
					if(ShaderBasedInteract){						
						//Debug.Log ("AA="+SpeedVec);
	//					for (int i = 0; i < Grassmanager.ExtraGrassMaterials.Count; i++) {
	//						for (int j = 0; j < Grassmanager.ExtraGrassMaterials [i].ExtraMaterials.Count; j++) {
	//							if (Grassmanager.ExtraGrassMaterials [i].ExtraMaterials [j].HasProperty ("_InteractPos")) {
	//								Grassmanager.ExtraGrassMaterials [i].ExtraMaterials [j].SetVector ("_InteractPos", this_tranf.position);
	//							}
	//						}
	//					}
						//for (int i = 0; i < Grassmanager.GrassMaterials.Count; i++) {
						//GrassMaterial.SetVector ("_InteractPos", PPos);
						if (GrassMaterial.HasProperty ("_InteractSpeed")) {
							Vector3 PPos = player.transform.position;
							if (GrassMaterial.HasProperty ("_InteractPos")) {
								GrassMaterial.SetVector ("_InteractPos", PPos);
							}
							//Debug.Log (SpeedVec);
							//Debug.Log (ShaderBInteractSpeed * Time.deltaTime);
							Vector3 Speedy = GrassMaterial.GetVector ("_InteractSpeed");
							if (float.IsNaN (Speedy.x) | float.IsNaN (Speedy.y) | float.IsNaN (Speedy.z)) {
								GrassMaterial.SetVector ("_InteractSpeed", Vector3.one * ShaderBInteractSpeed);
							} else {
								if (Speedy.magnitude > ShaderBInteractSpeed*5) {
									GrassMaterial.SetVector ("_InteractSpeed", Vector3.Lerp (Speedy, Vector3.zero, ShaderBInteractSpeed * Time.deltaTime));
								}else{
									GrassMaterial.SetVector ("_InteractSpeed", Vector3.Lerp (Speedy, SpeedVec, ShaderBInteractSpeed * Time.deltaTime));
									//GrassMaterial.SetVector ("_InteractSpeed", ShaderBInteractSpeed * SpeedVec);
								}
							}
						}
						//}
					}

				}
				if(GrassMaterial.HasProperty("_CameraPos")){
					if(Camera_transf !=null){
						GrassMaterial.SetVector("_CameraPos",Camera_transf.position);
					}
				}
				
				//v1.4 do per brush
				if(i == Grass_selector){
					if(GrassMaterial.HasProperty("_FadeThreshold")){
						GrassMaterial.SetFloat("_FadeThreshold",Grass_Fade_distance);
					}
					if(GrassMaterial.HasProperty("_StopMotionThreshold")){
						GrassMaterial.SetFloat("_StopMotionThreshold",Stop_Motion_distance);
					}
				}
				
			}
		}

}
}
