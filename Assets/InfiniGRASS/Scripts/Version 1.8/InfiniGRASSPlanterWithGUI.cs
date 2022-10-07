using UnityEngine;
using System.Collections;
using Artngame.INfiniDy;
using System.Collections.Generic;

namespace Artngame.INfiniDy {

    //NEW1
    [System.Serializable]
    public class SaveGrassData
    {
        [System.Serializable]
        public struct GrassData
        {
            public Vector3 position;
            public Vector2 scaleMinMax;
            public Vector2 densityMinMax;
            public Vector2 spreadMinMax;
            public int randomRot;
            public int grassType;
        }

        //public int m_Score;
        public List<GrassData> m_GrassData = new List<GrassData>();

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void LoadFromJson(string a_Json)
        {
            JsonUtility.FromJsonOverwrite(a_Json, this);
        }
    }
    public interface ISaveable
    {
        void PopulateSaveData(SaveGrassData a_SaveData);
        void LoadFromSaveData(SaveGrassData a_SaveData);
    }


    public class InfiniGRASSPlanterWithGUI : MonoBehaviour {

        //NEW2 
        public bool useLoopDelay = false;
        public float loopDelay = 0.05f;

        //NEW1
        [SerializeField]
        SaveGrassData savedGrassStrokesInfo = new SaveGrassData();
        //to save - grass type, vector3 position of planting, density, spread, scale, random rotation on-off
        public bool HUD_ON = true;
        public bool enable_controls = false;
        public float Grass_GUI_startX = 510;
        public float Grass_GUI_startY = 21;
        //bool SnowToggle = false;
        public List<Texture2D> IconsGrass = new List<Texture2D>();
        public List<Texture2D> IconsRocks = new List<Texture2D>();
        public List<Texture2D> IconsFence = new List<Texture2D>();
        public float Wind_rotY;
        public Transform Wind;       
        public bool Toggle_GUI = true;
        float snow_growth = 0;
        float snow_glow = 0;
        public float distRayAboveGround = 10;



        public string GrassManagerObjectName = "GRASS MANAGER";
        //v1.9.9
        public bool enableErasePlants = false;
        public float SphereCastRadius = 10;
        public bool massErase = false;
        public float maxEraseDistAround = 1;

        //v2.1.3
        public bool grow_grass = false;
		public float start_size_factor = 0.3f;

	public InfiniGRASSManager GrassManager;
	public bool bulkPaint = false;
	public int Grass_selector=0;
	public float raycastHeight = 0.1f;

		public bool useCollisions = false;

	// Use this for initialization
	void Start () {

            //NEW1
            //reset snow
            Shader.SetGlobalFloat("_SnowCoverage", 0);
            snow_growth = 0;
            snow_glow = 0.75f;

            GrassManager.WindTurbulence = 0.2f;
            GrassManager.AmplifyWind = 0.7f;

            GrassManager.Min_density = 1;
            GrassManager.Max_density = 4;
            //GrassManager.SpecularPower = 4;
            GrassManager.Min_spread = 7;
            GrassManager.Max_spread = 9;
            GrassManager.min_scale = 0.3f;
            GrassManager.max_scale = 0.65f;

            GrassManager.Cutoff_distance = 530;
            GrassManager.LOD_distance = 520;
            GrassManager.LOD_distance1 = 523;
            GrassManager.LOD_distance2 = 527;

            GrassManager.RandomRot = false;
            
            //if (i == 0) {
            GrassManager.Grass_selector = 0;
            GrassManager.Min_density = 1;
            GrassManager.Max_density = 4;
            //GrassManager.SpecularPower = 4;
            GrassManager.Min_spread = 7;
            GrassManager.Max_spread = 9;
            GrassManager.min_scale = 0.4f;
            GrassManager.max_scale = 0.6f;

            GrassManager.Cutoff_distance = 530;
            GrassManager.LOD_distance = 520;
            GrassManager.LOD_distance1 = 523;
            GrassManager.LOD_distance2 = 527;

            GrassManager.RandomRot = false;
            GrassManager.AmplifyWind = 0.4f;






            if (GrassManager == null) {
				GameObject manager = GameObject.Find (GrassManagerObjectName);
				if (manager != null) {
                    GrassManager = manager.GetComponent<InfiniGRASSManager> ();
				}
			}
	}

        //NEW1
        void LateUpdate()
        {
            if (GrassManager.AmplifyWind > 0.5f)
            {
                GrassManager.AmplifyWind = 0.4f;
            }


            //// ERASE PLANTS
            if (enableErasePlants)
            {
                //MASS ERASE

                if (useMouse && GrassManager != null)
                {
                    if (Input.GetMouseButtonDown(erasePlantButton)) // & Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        Ray ray = new Ray();

                        //v1.4c
                        bool found_cam = false;
                        if (GrassManager.Tag_based_player)
                        {
                            if (GrassManager.player != null)
                            {
                                Camera[] playerCams = GrassManager.player.GetComponentsInChildren<Camera>(false);
                                //Debug.Log(playerCams.Length);
                                if (playerCams != null && playerCams.Length > 0 && playerCams[0] != null)
                                {
                                    ray = playerCams[0].ScreenPointToRay(Input.mousePosition);
                                    found_cam = true;
                                }
                                else
                                {
                                    if (Camera.main != null)
                                    {
                                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                                        found_cam = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Camera.main != null)
                            {
                                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                                found_cam = true;
                            }
                            else if (Camera.current != null)
                            {
                                ray = Camera.current.ScreenPointToRay(Input.mousePosition);
                                found_cam = true;
                            }
                        }

                        //RaycastHit hit = new RaycastHit();
                        //if (found_cam && Physics.Raycast(ray, out hit, Mathf.Infinity))
                        //{
                        //    PlantInPosition(hit.point, hit.normal, hit.collider, hit.collider.transform);
                        //}

                        if (found_cam)
                        {
                            //Ray ray = new Ray(transform.position + Vector3.up * raycastHeight + new Vector3(Random.value * maxEraseDistAround, 0, Random.value * maxEraseDistAround), -Vector3.up);
                            if (massErase)
                            {
                                //FIX ISSUE where some colliders may be deactivated
                                for (int j = 0; j < GrassManager.Grasses.Count; j++)
                                {
                                    GrassManager.Grasses[j].GetComponent<BoxCollider>().enabled = true;
                                }

                                RaycastHit[] hits = Physics.SphereCastAll(ray, SphereCastRadius, Mathf.Infinity);
                                if (hits != null & hits.Length > 0)
                                {
                                    bool one_is_outside_view = false;

                                    if (!one_is_outside_view)
                                    {
                                        for (int j = 0; j < hits.Length; j++)
                                        {
                                            RaycastHit hit1 = hits[j];

                                            if (hit1.collider != null && hit1.collider.gameObject.GetComponent<GrassChopCollider>() != null)
                                            {

                                                ControlCombineChildrenINfiniDyGrass forest_holder = hit1.collider.gameObject.GetComponent<GrassChopCollider>().TreeHandler.Forest_holder.GetComponent<ControlCombineChildrenINfiniDyGrass>();
                                                INfiniDyGrassField forest = hit1.collider.gameObject.GetComponent<GrassChopCollider>().TreeHandler;

                                                forest_holder.Restore();

                                                DestroyImmediate(forest_holder.Added_items_handles[forest.Tree_Holder_Index].gameObject);
                                                DestroyImmediate(forest_holder.Added_items[forest.Tree_Holder_Index].gameObject);

                                                forest_holder.Added_items_handles.RemoveAt(forest.Tree_Holder_Index);
                                                forest_holder.Added_items.RemoveAt(forest.Tree_Holder_Index);

                                                //remove from script
                                                GrassManager.Grasses.RemoveAt(forest.Grass_Holder_Index);
                                                GrassManager.GrassesType.RemoveAt(forest.Grass_Holder_Index);

                                                //adjust ids for items left
                                                for (int i = 0; i < forest_holder.Added_items.Count; i++)
                                                {
                                                    forest_holder.Added_items_handles[i].Tree_Holder_Index = i;

                                                }
                                                for (int i = 0; i < GrassManager.Grasses.Count; i++)
                                                {
                                                    GrassManager.Grasses[i].Grass_Holder_Index = i;
                                                }

                                                forest_holder.Added_item_count -= 1;

                                                forest_holder.MakeActive = true;
                                                forest_holder.Decombine = false;
                                                forest_holder.Decombined = false;

                                                //check if combiners erased
                                                for (int i = GrassManager.DynamicCombiners.Count - 1; i >= 0; i--)
                                                {
                                                    if (GrassManager.DynamicCombiners[i] == null)
                                                    {
                                                        GrassManager.DynamicCombiners.RemoveAt(i);
                                                    }
                                                }
                                                for (int i = GrassManager.StaticCombiners.Count - 1; i >= 0; i--)
                                                {
                                                    if (GrassManager.StaticCombiners[i] == null)
                                                    {
                                                        GrassManager.StaticCombiners.RemoveAt(i);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }//END ERASE
                            else
                            {
                                if (1 == 1)
                                {
                                    //Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                                    RaycastHit hit = new RaycastHit();
                                    //Debug.Log("no col0");
                                    if (Physics.SphereCast(ray, SphereCastRadius, out hit, Mathf.Infinity)) //if (found_cam && Physics.SphereCast(ray, 20, out hit, Mathf.Infinity))
                                    {
                                        //Debug.Log("no col1");
                                        if (hit.collider.gameObject.GetComponent<GrassChopCollider>() != null)
                                        {
                                            //Debug.Log("no col2");
                                            ControlCombineChildrenINfiniDyGrass forest_holder = hit.collider.gameObject.GetComponent<GrassChopCollider>().TreeHandler.Forest_holder.GetComponent<ControlCombineChildrenINfiniDyGrass>();
                                            INfiniDyGrassField forest = hit.collider.gameObject.GetComponent<GrassChopCollider>().TreeHandler;

                                            forest_holder.Restore();

                                            DestroyImmediate(forest_holder.Added_items_handles[forest.Tree_Holder_Index].gameObject);
                                            DestroyImmediate(forest_holder.Added_items[forest.Tree_Holder_Index].gameObject);

                                            forest_holder.Added_items_handles.RemoveAt(forest.Tree_Holder_Index);
                                            forest_holder.Added_items.RemoveAt(forest.Tree_Holder_Index);

                                            //remove from script
                                            GrassManager.Grasses.RemoveAt(forest.Grass_Holder_Index);
                                            GrassManager.GrassesType.RemoveAt(forest.Grass_Holder_Index);

                                            //adjust ids for items left
                                            for (int i = 0; i < forest_holder.Added_items.Count; i++)
                                            {
                                                forest_holder.Added_items_handles[i].Tree_Holder_Index = i;

                                            }
                                            for (int i = 0; i < GrassManager.Grasses.Count; i++)
                                            {
                                                GrassManager.Grasses[i].Grass_Holder_Index = i;
                                            }

                                            forest_holder.Added_item_count -= 1;

                                            forest_holder.MakeActive = true;
                                            forest_holder.Decombine = false;
                                            forest_holder.Decombined = false;
                                        }

                                    }//END RAYCAST
                                }//END IF ERASING MOUSE CLICK CHECK
                            }//END if not masserase
                        }
                    }
                }
            }//END if enableErasePlants
            ////END ERASE PLANTS
        }


        // Update is called once per frame
        public int plantButton = 0; //v1.9.9.5
        public int erasePlantButton = 1;
        public bool useMouse = false;
        // Update is called once per frame
        void Update()
        {
            if (useMouse && GrassManager != null)
            {
                if (Input.GetMouseButtonDown(plantButton) & !Input.GetKeyDown(KeyCode.LeftShift))
                {
                    Ray ray = new Ray();

                    //v1.4c
                    bool found_cam = false;
                    if (GrassManager.Tag_based_player)
                    {
                        if (GrassManager.player != null)
                        {
                            Camera[] playerCams = GrassManager.player.GetComponentsInChildren<Camera>(false);
                            //Debug.Log(playerCams.Length);
                            if (playerCams != null && playerCams.Length > 0 && playerCams[0] != null)
                            {
                                ray = playerCams[0].ScreenPointToRay(Input.mousePosition);
                                found_cam = true;
                            }
                            else
                            {
                                if (Camera.main != null)
                                {
                                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                                    found_cam = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Camera.main != null)
                        {
                            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            found_cam = true;
                        }
                        else if (Camera.current != null)
                        {
                            ray = Camera.current.ScreenPointToRay(Input.mousePosition);
                            found_cam = true;
                        }
                    }

                    RaycastHit hit = new RaycastHit();
                    if (found_cam && Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        PlantInPosition(hit.point, hit.normal, hit.collider, hit.collider.transform);
                    }
                }
            }
        }

        //v2.1.3
        //	void Plant (Vector2 position) {
        //			
        //		Ray ray = new Ray ();
        //		ray.direction = -Vector3.up;
        //		ray.origin = new Vector3 (position.x,raycastHeight,position.y);
        //		//v2.1
        //		Grassmanager.paintGrassPatch (ray, Grass_selector, false, true);
        //		if (bulkPaint) {
        //			Vector3 rayCenter = ray.origin;
        //			//create more around up to max batch members
        //				for (int i=0;i<Grassmanager.Max_static_group_members-1;i++){
        //					ray.origin = rayCenter + new Vector3 (UnityEngine.Random.Range(0f,1f), 0,UnityEngine.Random.Range(0f,1f)) * Grassmanager.bulkPaintRadius;
        //					Grassmanager.paintGrassPatch (ray, Grass_selector, false, true);
        //			}
        //		} 
        //	}

        void PlantInPosition (Vector3 position, Vector3 normal, Collider collider1, Transform transform1) {

//			Ray ray = new Ray ();
//			ray.direction = -Vector3.up;
//			ray.origin = new Vector3 (position.x,raycastHeight,position.y);

			RaycastHit hit = new RaycastHit ();
			hit.point = position;
			hit.normal = normal;
			//hit.collider = collider;
			//hit.transform = transform;

			//v2.1
			//Grassmanager.PlantGrass (hit, Grass_selector, false, true);
			//PlantGrass (hit, collider1, transform1, Grass_selector, false, true);

            //NEW1
            PlantGrass (hit, collider1, transform1, GrassManager.Grass_selector, false, true);
		}

	void OnCollisionEnter(Collision info){
			if (useCollisions && GrassManager != null) {
				if (info.contacts.Length > 0) {
					Vector3 point = info.contacts [0].point;
					Vector3 direction = info.contacts [0].normal;

					//Debug.DrawRay (point, direction, Color.red, 10);

					//Plant (new Vector2 (point.x, point.z));
					PlantInPosition (point, direction, info.collider, info.transform);
				}
			}
	}


		//v2.1.1
		void PlantGrass(RaycastHit hit, Collider collider1, Transform transform1,  int Grass_selector, bool growInEditor, bool registerToManager){

			bool is_Terrain = false;
			if ( (Terrain.activeTerrain != null && collider1.gameObject != null && collider1.gameObject == Terrain.activeTerrain.gameObject)){
				is_Terrain = true;
			}

			if ( is_Terrain |  (collider1.gameObject != null && collider1.gameObject.tag == "PPaint")) {//v1.1

				//DONT PLANT if hit another grass collider
				if (collider1.GetComponent<GrassChopCollider> () != null) {

				} else {

					GameObject TEMP = Instantiate (GrassManager.GrassPrefabs [Grass_selector]);
					TEMP.transform.position = hit.point;

					INfiniDyGrassField TREE = TEMP.GetComponent<INfiniDyGrassField> ();

					TREE.Intial_Up_Vector = hit.normal;

					//v2.1
					if (Application.isPlaying) {
						TREE.Grow_tree = true;
					} else {
						//TREE.Grow_tree = false;
						TREE.Grow_in_Editor = true;
					}

					//v1.1 - terrain adapt
					if (GrassManager.AdaptOnTerrain & is_Terrain) {
						int Xpos = (int)(((hit.point.x - GrassManager.Tpos.x)* GrassManager.Tdata.alphamapWidth/ GrassManager.Tdata.size.x));
						int Zpos = (int)(((hit.point.z - GrassManager.Tpos.z)* GrassManager.Tdata.alphamapHeight/ GrassManager.Tdata.size.z));
						float[,,] splats = GrassManager.Tdata.GetAlphamaps(Xpos,Zpos,1,1);
						float[] Tarray = new float[splats.GetUpperBound(2)+1];
						for(int j =0;j<Tarray.Length;j++){
							Tarray[j] = splats[0,0,j];
							//Debug.Log(Tarray[j]); // ScalePerTexture
						}
						float Scaling = 0;
						for(int j =0;j<Tarray.Length;j++){
							if(j > GrassManager.ScalePerTexture.Count-1){
								Scaling = Scaling + (1*Tarray[j]);
							}else{
								Scaling = Scaling + (GrassManager.ScalePerTexture[j]*Tarray[j]);
							}
						}
						TREE.End_scale = Scaling*UnityEngine.Random.Range (GrassManager.min_scale, GrassManager.max_scale);
						//Debug.Log(Tarray);
					}else{
						TREE.End_scale = UnityEngine.Random.Range (GrassManager.min_scale, GrassManager.max_scale);
					}

					TREE.Max_interact_holder_items = GrassManager.Max_interactive_group_members;//Define max number of trees grouped in interactive batcher that opens up. 
					//Increase to lower draw calls, decrease to lower spikes when group is opened for interaction
					TREE.Max_trees_per_group = GrassManager.Max_static_group_members;

					TREE.Interactive_tree = GrassManager.Interactive;

					//v2.1
					if (Application.isPlaying) {
						TREE.transform.localScale *= TREE.End_scale * GrassManager.Collider_scale;
					} else {
						TREE.colliderScale = Vector3.one * GrassManager.Collider_scale;
					}

					if(GrassManager.Override_spread){
						TREE.PosSpread = new Vector2(UnityEngine.Random.Range(GrassManager.Min_spread, GrassManager.Max_spread),UnityEngine.Random.Range(GrassManager.Min_spread, GrassManager.Max_spread));
					}
					if(GrassManager.Override_density){
						TREE.Min_Max_Branching = new Vector2(GrassManager.Min_density, GrassManager.Max_density);
					}
					TREE.PaintedOnOBJ = transform1.gameObject.transform;
					TREE.GridOnNormal = GrassManager.GridOnNormal;
					TREE.max_ray_dist = GrassManager.rayCastDist;
					TREE.MinAvoidDist = GrassManager.MinAvoidDist;
					TREE.MinScaleAvoidDist = GrassManager.MinScaleAvoidDist;
					TREE.InteractionSpeed = GrassManager.InteractionSpeed;
					TREE.InteractSpeedThres = GrassManager.InteractSpeedThres;

                    //v1.9.9.7
                    TREE.enableAbove64KMesh = GrassManager.enableAbove64KMesh;

                    //v1.4
                    TREE.Interaction_thres = GrassManager.Interaction_thres;
					TREE.Max_tree_dist = GrassManager.Max_tree_dist;//v1.4.6
					TREE.Disable_after_growth = GrassManager.Disable_after_growth;//v1.5
					TREE.WhenCombinerFull = GrassManager.WhenCombinerFull;//v1.5
					TREE.Eliminate_original_mesh = GrassManager.Eliminate_original_mesh;//v1.5
					TREE.Interaction_offset = GrassManager.Interaction_offset;

					TREE.LOD_distance = GrassManager.LOD_distance;
					TREE.LOD_distance1 = GrassManager.LOD_distance1;
					TREE.LOD_distance2 = GrassManager.LOD_distance2;
					TREE.Cutoff_distance = GrassManager.Cutoff_distance;

					TREE.Tag_based = false;
					TREE.GrassManager = GrassManager;////////////////////////// v2.1.1
					TREE.Type = Grass_selector+1;
					TREE.Start_tree_scale = TREE.End_scale/4;

					TREE.RandomRot = GrassManager.RandomRot;
					TREE.RandRotMin = GrassManager.RandRotMin;
					TREE.RandRotMax = GrassManager.RandRotMax;

					TREE.GroupByObject = GrassManager.GroupByObject;
					TREE.ParentToObject = GrassManager.ParentToObject;
					TREE.MoveWithObject = GrassManager.MoveWithObject;
					TREE.AvoidOwnColl = GrassManager.AvoidOwnColl;

					TEMP.transform.parent = GrassManager.GrassHolder.transform;

                    //v1.8
                    //					TREE.BatchColliders = Grassmanager.BatchColliders;
                    //					TREE.BatchInstantiation = Grassmanager.BatchInstantiation;
                    //					TREE.RandomPositions = Grassmanager.RandomPositions;
                    //					TREE.BatchCopiesCount = Grassmanager.BatchCopiesCount;
                    //					TREE.CopiedBatchSpread = Grassmanager.CopiedBatchSpread;
                    //
                    //					//v2.0.3
                    //					TREE.noOutofPlaneBlades = Grassmanager.noOutofPlaneBlades;
                    //					TREE.maxOutofPlaneSlopeHeight = Grassmanager.maxOutofPlaneSlopeHeight;

                    //Add to holder, in order to mass change properties
                    GrassManager.Grasses.Add (TREE);
                    GrassManager.GrassesType.Add (Grass_selector);

					TEMP.name = "GrassPatch" + GrassManager.Grasses.Count.ToString (); 

					TREE.Grass_Holder_Index = GrassManager.Grasses.Count - 1;//register id in grasses list



					//RECONFIG
					TREE.transform.parent = GrassManager.GrassHolder.transform;
                    GrassManager.CleanUp = false;
					INfiniDyGrassField forest = GrassManager.Grasses[GrassManager.Grasses.Count-1] ;
					//forest.gameObject.SetActive(false);//ADDED v1.8
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
					//	forest.Grow_tree = true;//v1.5 - fix issue with start scale when entering play mode from ungrown mode
				

					//v2.1.3
					if(grow_grass){
						forest.Grow_tree = true;
						forest.Start_tree_scale = forest.End_scale*start_size_factor;//v1.5
					}else{
						forest.Grow_tree = false;
						forest.Start_tree_scale = forest.End_scale;//v1.5
					}

					forest.rotation_over = false;
					forest.Forest_holder = null;
					//Grassmanager.UnGrown = true;

				}
			}
		}// END PLANT FUNCTION



        //NEW1 - GUI
        void OnGUI()
        {


          





            if (GrassManager.TintPower > 0)
            {
                GrassManager.TintGrass = true;
            }

            string Toggle_GUIs = "GUI Off";
            if (Toggle_GUI)
            {
                Toggle_GUIs = "GUI On";
            }
            if (GUI.Button(new Rect(10, 10, 110, 22), Toggle_GUIs))
            {
                if (Toggle_GUI)
                {
                    Toggle_GUI = false;
                    
                }
                else
                {
                    Toggle_GUI = true;
                    
                }
            }

                                             


            //CAMERA 
            if (Toggle_GUI)
            {
               
                if (GrassManager.windzone != null && Wind == null)
                {

                    Wind = GrassManager.windzone.transform;
                }

                
                //if (SUN != null && SUN_LIGHT != null) {
                float dispX = -100;
                float BASE_X = 20;
                float BASE1 = 20 + 100;


                //SNOW
                GUI.TextArea(new Rect(BASE_X - 10, BASE1 + (3.5f * 40) - 20 + 70 + 70 + 50, 120, 20), "Snow amount");
                snow_growth = GUI.HorizontalSlider(new Rect(BASE_X - 10, BASE1 + (3.5f * 40) + 70 + 70 + 50, 100, 30), snow_growth, 0f, 10);
                GUI.TextArea(new Rect(BASE_X - 10, BASE1 + (3.5f * 40) - 20 + 70 + 70 + 40 + 70, 120, 20), "Snow glow");
                snow_glow = GUI.HorizontalSlider(new Rect(BASE_X - 10, BASE1 + (3.5f * 40) + 70 + 70 + 40 + 70, 100, 30), snow_glow, 0f, 10);


                for (int i = 0; i < GrassManager.GrassMaterials.Count; i++)
                {
                    if (GrassManager.GrassMaterials[i].HasProperty("_TimeControl1"))
                    {
                        Vector4 prev = GrassManager.GrassMaterials[i].GetVector("_TimeControl1");
                        GrassManager.GrassMaterials[i].SetVector("_TimeControl1", new Vector4(prev.x, snow_growth, snow_glow, prev.w));
                    }
                }


                float BOX_WIDTH = 100;
                float BOX_HEIGHT = 30;
              
                if (Wind != null)
                {
                    GUI.TextArea(new Rect(1 * BOX_WIDTH + 10 + dispX, BOX_HEIGHT + 30 + 30 + 30 + 50 + 30, BOX_WIDTH + 0, 25), "Wind rot");
                    Wind_rotY = GUI.HorizontalSlider(new Rect(1 * BOX_WIDTH + 10 + dispX, BOX_HEIGHT + 30 + 30 + 30 + 50 + 30 + 30, BOX_WIDTH + 0, 30), Wind.eulerAngles.y, 0, 359);
                    Wind.eulerAngles = new Vector3(Wind.eulerAngles.x, Wind_rotY, Wind.eulerAngles.z);
                }


                //if (!OnlySunWind)
                {


                    
                    float widthS = Screen.currentResolution.width;

                    if (Camera.main != null)
                    {
                        widthS = Camera.main.pixelWidth;
                    }
                    else
                    {
                        if (Camera.current != null)
                        {
                            widthS = Camera.current.pixelWidth;
                        }
                    }

                    Grass_GUI_startX = widthS - 400;
                    /////////
                    /// 

                    //SNOW grass------------------------------------------------------------ ROCKS

                    //ICONS to choose from
                    if (GUI.Button(new Rect(0 + 130, 10 + 35 + 35, 80, 25), "Paint rocks"))
                    {
                        if (GrassManager.Rock_painting)
                        {
                            GrassManager.Rock_painting = false;
                        }
                        else
                        {
                            GrassManager.Rock_painting = true;
                            GrassManager.Fence_painting = false;
                            GrassManager.Grass_painting = false;
                        }
                        GrassManager.Grass_selector = 0;
                    }
                    if (GrassManager.Rock_painting)
                    {
                        //display icons
                        for (int i = 0; i < IconsRocks.Count; i++)
                        {
                            if (GUI.Button(new Rect(Grass_GUI_startX + 60 * i, Grass_GUI_startY + 5, 60, 60), IconsRocks[i]))
                            {
                                GrassManager.Grass_selector = i;
                            }
                        }
                        //Scale
                        GUI.TextArea(new Rect(Grass_GUI_startX, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30, 110, 22), "Min distance:" + GrassManager.min_grass_patch_dist.ToString("F1"));
                        GrassManager.min_grass_patch_dist = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30, 110, 25), GrassManager.min_grass_patch_dist, 2f, 8f);

                        //Paint dist
                        GUI.TextArea(new Rect(Grass_GUI_startX, Grass_GUI_startY + 60 + 30, 110, 40), "Rock scale (Min:" + GrassManager.min_scale.ToString("F1") + "-Max:" + GrassManager.max_scale.ToString("F1") + ")");
                        GrassManager.min_scale = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40, 110, 25), GrassManager.min_scale, 0.05f, 1.5f);
                        GrassManager.max_scale = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30, 110, 25), GrassManager.max_scale, 0.1f, 1.5f);


                    }



                    //SNOW grass------------------------------------------------------------ FENCE

                    //ICONS to choose from
                    if (GUI.Button(new Rect(0 + 130, 10 + 35, 80, 25), "Paint fence"))
                    {
                        if (GrassManager.Fence_painting)
                        {
                            GrassManager.Fence_painting = false;
                        }
                        else
                        {
                            GrassManager.Fence_painting = true;
                            GrassManager.Rock_painting = false;
                            GrassManager.Grass_painting = false;
                        }
                        GrassManager.Grass_selector = 0;
                    }
                    if (GrassManager.Fence_painting)
                    {
                        //display icons
                        for (int i = 0; i < IconsFence.Count; i++)
                        {
                            if (GUI.Button(new Rect(Grass_GUI_startX + 60 * i, Grass_GUI_startY + 5, 60, 60), IconsFence[i]))
                            {
                                if (GrassManager.Grass_selector != i)
                                {
                                    GrassManager.Grass_selector = i;
                                }
                                else
                                {
                                    //stop fence
                                    GrassManager.Fence_painting = false;
                                    //GrassManager.Fence_painting = true;
                                }
                            }
                        }
                        //Scale
                        GUI.TextArea(new Rect(Grass_GUI_startX, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30, 110, 22), "Fence Scale:" + GrassManager.fence_scale.ToString("F1"));
                        GrassManager.fence_scale = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30, 110, 25), GrassManager.fence_scale, 0.3f, 0.5f);

                        //Paint dist
                        GrassManager.min_grass_patch_dist = 0.4f;

                    }


                    //SNOW grass------------------------------------------------------------ GRASS

                    //ICONS to choose from
                    if (GUI.Button(new Rect(0 + 130, 10, 80, 22), "Paint grass"))
                    {
                        if (GrassManager.Grass_painting)
                        {
                            GrassManager.Grass_painting = false;
                        }
                        else
                        {
                            GrassManager.Grass_painting = true;
                            GrassManager.Rock_painting = false;
                            GrassManager.Fence_painting = false;
                        }
                        GrassManager.Grass_selector = 0;
                    }
                    if (GrassManager.Grass_painting)
                    {


                        GrassManager.GridOnNormal = true;

                        //display icons
                        for (int i = 0; i < IconsGrass.Count; i++)
                        {

                            //v1.5
                            int break_at = 15;
                            float grass_X = Grass_GUI_startX + 60 * i - 561;
                            float grass_Y = 0;
                            if (i > break_at)
                            {
                                grass_Y = Grass_GUI_startY + 40 - 10;
                                grass_X = Grass_GUI_startX + 60 * (i - break_at - 1) - 561;
                            }

                            float sizeing = 60;
                            if (GrassManager.Grass_selector == i)
                            {
                                sizeing = 55;
                            }


                            if (GUI.Button(new Rect(grass_X, grass_Y, 60, sizeing), IconsGrass[i]))
                            {

                                //CHANGE BRUSH
                                GrassManager.Grass_selector = i;
                                //CHANGE in MANAGER so rescale wont be activated
                                GrassManager.prev_brush = i;

                                //Grass
                                if (i == 0)
                                {
                                    GrassManager.Min_density = 1;
                                    GrassManager.Max_density = 4;
                                    //GrassManager.SpecularPower = 4;
                                    GrassManager.Min_spread = 7;
                                    GrassManager.Max_spread = 9;
                                    GrassManager.min_scale = 0.3f;
                                    GrassManager.max_scale = 0.5f;

                                    GrassManager.Cutoff_distance = 530;
                                    GrassManager.LOD_distance = 520;
                                    GrassManager.LOD_distance1 = 523;
                                    GrassManager.LOD_distance2 = 527;

                                    GrassManager.RandomRot = false;
                                }
                                //Vertex grass
                                if (i == 1)
                                {
                                    GrassManager.min_scale = 0.4f;
                                    GrassManager.max_scale = 0.8f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 7;
                                    GrassManager.Max_spread = 9;
                                    //GrassManager.SpecularPower = 4;

                                    GrassManager.Cutoff_distance = 530;
                                    GrassManager.LOD_distance = 520;
                                    GrassManager.LOD_distance1 = 523;
                                    GrassManager.LOD_distance2 = 527;

                                    GrassManager.RandomRot = false;
                                }
                                //Red flowers
                                if (i == 2)
                                {
                                    GrassManager.min_scale = 0.8f;
                                    GrassManager.max_scale = 0.9f;
                                    GrassManager.Min_density = 1.0f;
                                    GrassManager.Max_density = 1.0f;
                                    GrassManager.Min_spread = 7;
                                    GrassManager.Max_spread = 10;
                                    //GrassManager.SpecularPower = 4;

                                    GrassManager.Cutoff_distance = 530;
                                    GrassManager.LOD_distance = 520;
                                    GrassManager.LOD_distance1 = 523;
                                    GrassManager.LOD_distance2 = 527;

                                    GrassManager.RandomRot = true;
                                }
                                //Wheet
                                if (i == 3)
                                {
                                    GrassManager.min_scale = 1.0f;
                                    GrassManager.max_scale = 1.5f;
                                    GrassManager.Min_density = 1.0f;
                                    GrassManager.Max_density = 1.0f;
                                    GrassManager.Min_spread = 10;
                                    GrassManager.Max_spread = 12;
                                    //GrassManager.SpecularPower = 4;

                                    GrassManager.Cutoff_distance = 530;
                                    GrassManager.LOD_distance = 520;
                                    GrassManager.LOD_distance1 = 523;
                                    GrassManager.LOD_distance2 = 527;

                                    GrassManager.RandomRot = false;
                                }
                                //Detailed vertex
                                if (i == 4)
                                {
                                    GrassManager.min_scale = 1.0f;
                                    GrassManager.max_scale = 1.2f;
                                    GrassManager.Min_density = 1.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 7;
                                    GrassManager.Max_spread = 10;
                                    //GrassManager.SpecularPower = 4;

                                    GrassManager.Cutoff_distance = 530;
                                    GrassManager.LOD_distance = 520;
                                    GrassManager.LOD_distance1 = 523;
                                    GrassManager.LOD_distance2 = 527;

                                    GrassManager.RandomRot = false;
                                }

                                //Simple vertex
                                if (i == 5)
                                {
                                    GrassManager.min_scale = 0.5f;
                                    GrassManager.max_scale = 1.0f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 7;
                                    GrassManager.Max_spread = 10;
                                    //GrassManager.SpecularPower = 4;

                                    GrassManager.Cutoff_distance = 530;
                                    GrassManager.LOD_distance = 520;
                                    GrassManager.LOD_distance1 = 523;
                                    GrassManager.LOD_distance2 = 527;

                                    GrassManager.RandomRot = false;
                                }
                                //White flowers
                                if (i == 6)
                                {
                                    GrassManager.min_scale = 0.6f;
                                    GrassManager.max_scale = 0.9f;
                                    GrassManager.Min_density = 1.0f;
                                    GrassManager.Max_density = 1.0f;
                                    GrassManager.Min_spread = 7;
                                    GrassManager.Max_spread = 10;
                                    //GrassManager.SpecularPower = 4;

                                    GrassManager.Cutoff_distance = 530;
                                    GrassManager.LOD_distance = 520;
                                    GrassManager.LOD_distance1 = 523;
                                    GrassManager.LOD_distance2 = 527;

                                    GrassManager.RandomRot = true;
                                }
                                //Curved grass
                                if (i == 7)
                                {
                                    GrassManager.min_scale = 0.8f;
                                    GrassManager.max_scale = 2f;
                                    GrassManager.Min_density = 1.0f;
                                    GrassManager.Max_density = 4.0f;
                                    GrassManager.Min_spread = 7;
                                    GrassManager.Max_spread = 8;
                                    //GrassManager.SpecularPower = 4;

                                    GrassManager.Cutoff_distance = 530;
                                    GrassManager.LOD_distance = 520;
                                    GrassManager.LOD_distance1 = 523;
                                    GrassManager.LOD_distance2 = 527;

                                    GrassManager.RandomRot = false;
                                }
                                //Low grass - FOR LIGHT DEMO without Sky Master and real time use
                                if (i == 8)
                                {
                                    GrassManager.min_scale = 1.2f;
                                    GrassManager.max_scale = 1.3f;
                                    GrassManager.Min_density = 1.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 4;
                                    GrassManager.Max_spread = 6;
                                    //GrassManager.SpecularPower = 4;
                                    GrassManager.Collider_scale = 0.4f;

                                    GrassManager.Cutoff_distance = 530;
                                    GrassManager.LOD_distance = 520;
                                    GrassManager.LOD_distance1 = 523;
                                    GrassManager.LOD_distance2 = 527;

                                    GrassManager.RandomRot = false;
                                }
                                //Vines
                                if (i == 9)
                                {
                                    GrassManager.min_scale = 2.5f;
                                    GrassManager.max_scale = 5.5f;
                                    GrassManager.Min_density = 3.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 7;
                                    GrassManager.Max_spread = 7;
                                    //GrassManager.SpecularPower = 4;

                                    GrassManager.Cutoff_distance = 530;
                                    GrassManager.LOD_distance = 520;
                                    GrassManager.LOD_distance1 = 523;
                                    GrassManager.LOD_distance2 = 527;

                                    GrassManager.RandomRot = false;
                                }

                                //Mushrooms Brown and red
                                if (i == 10 | i == 11)
                                {
                                    GrassManager.min_scale = 0.4f;
                                    GrassManager.max_scale = 1.0f;
                                    GrassManager.Min_density = 1.0f;
                                    GrassManager.Max_density = 4.0f;
                                    GrassManager.Min_spread = 7;
                                    GrassManager.Max_spread = 9;
                                    //GrassManager.SpecularPower = 4;

                                    GrassManager.Cutoff_distance = 90;
                                    GrassManager.LOD_distance = 40;
                                    GrassManager.LOD_distance1 = 60;
                                    GrassManager.LOD_distance2 = 80;

                                    GrassManager.RandomRot = false;
                                }
                                //Ground leaves
                                if (i == 12)
                                {
                                    GrassManager.min_scale = 0.25f;
                                    GrassManager.max_scale = 0.5f;
                                    GrassManager.Min_density = 1.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 7;
                                    GrassManager.Max_spread = 11;
                                    //GrassManager.SpecularPower = 4;

                                    GrassManager.Cutoff_distance = 530;
                                    GrassManager.LOD_distance = 520;
                                    GrassManager.LOD_distance1 = 523;
                                    GrassManager.LOD_distance2 = 527;

                                    GrassManager.RandomRot = true;
                                }
                                //Noisy grass
                                if (i == 13)
                                {
                                    GrassManager.min_scale = 0.5f;
                                    GrassManager.max_scale = 1.5f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 7;
                                    GrassManager.Max_spread = 9;
                                    //GrassManager.SpecularPower = 4;

                                    GrassManager.Cutoff_distance = 530;
                                    GrassManager.LOD_distance = 520;
                                    GrassManager.LOD_distance1 = 523;
                                    GrassManager.LOD_distance2 = 527;

                                    GrassManager.RandomRot = true;
                                }
                                //Rocks
                                if (i == 14)
                                {
                                    GrassManager.min_scale = 0.6f;
                                    GrassManager.max_scale = 2.8f;
                                    GrassManager.Min_density = 1.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 7;
                                    GrassManager.Max_spread = 11;
                                 
                                    GrassManager.Cutoff_distance = 60;
                                    GrassManager.LOD_distance = 45;
                                    GrassManager.LOD_distance1 = 50;
                                    GrassManager.LOD_distance2 = 55;

                                    GrassManager.RandomRot = false;
                                }                                                               

                                //v1.5
                                if (GrassManager.Grass_selector == 15)
                                {
                                    //GROUND COVER
                                    GrassManager.min_scale = 0.8f;
                                    GrassManager.max_scale = 1.4f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 4;
                                    GrassManager.Max_spread = 5.5f;

                                    GrassManager.Cutoff_distance = 520;
                                    GrassManager.LOD_distance = 220;
                                    GrassManager.LOD_distance1 = 270;
                                    GrassManager.LOD_distance2 = 410;

                                    GrassManager.RandomRot = false;
                                }
                                if (GrassManager.Grass_selector == 16)
                                {
                                    //GROUND COVER
                                    GrassManager.min_scale = 0.3f;
                                    GrassManager.max_scale = 0.9f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 2.0f;
                                    GrassManager.Min_spread = 4;
                                    GrassManager.Max_spread = 5.5f;

                                    GrassManager.Cutoff_distance = 60;
                                    GrassManager.LOD_distance = 45;
                                    GrassManager.LOD_distance1 = 50;
                                    GrassManager.LOD_distance2 = 55;

                                    GrassManager.RandomRot = false;
                                }
                                if (GrassManager.Grass_selector == 17)
                                {
                                    //GROUND COVER
                                    GrassManager.min_scale = 0.3f;
                                    GrassManager.max_scale = 0.9f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 2.0f;
                                    GrassManager.Min_spread = 4;
                                    GrassManager.Max_spread = 5.5f;

                                    GrassManager.Cutoff_distance = 520;
                                    GrassManager.LOD_distance = 220;
                                    GrassManager.LOD_distance1 = 270;
                                    GrassManager.LOD_distance2 = 410;

                                    GrassManager.RandomRot = false;
                                }
                                if (GrassManager.Grass_selector == 18)
                                {
                                    //GROUND COVER
                                    GrassManager.min_scale = 0.2f;
                                    GrassManager.max_scale = 0.8f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 2.0f;
                                    GrassManager.Min_spread = 4;
                                    GrassManager.Max_spread = 5.5f;

                                    GrassManager.Cutoff_distance = 520;
                                    GrassManager.LOD_distance = 220;
                                    GrassManager.LOD_distance1 = 270;
                                    GrassManager.LOD_distance2 = 410;

                                    GrassManager.RandomRot = true;
                                }
                                if (GrassManager.Grass_selector == 19)
                                {
                                    //GROUND COVER
                                    GrassManager.min_scale = 0.2f;
                                    GrassManager.max_scale = 0.4f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 2.0f;
                                    GrassManager.Min_spread = 4;
                                    GrassManager.Max_spread = 5.5f;

                                    GrassManager.Cutoff_distance = 520;
                                    GrassManager.LOD_distance = 220;
                                    GrassManager.LOD_distance1 = 270;
                                    GrassManager.LOD_distance2 = 410;

                                    GrassManager.RandomRot = false;
                                }
                                if (GrassManager.Grass_selector == 20)
                                {
                                    //GROUND COVER
                                    GrassManager.min_scale = 0.15f;
                                    GrassManager.max_scale = 0.3f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 2.0f;
                                    GrassManager.Min_spread = 5;
                                    GrassManager.Max_spread = 6.5f;

                                    GrassManager.Cutoff_distance = 520;
                                    GrassManager.LOD_distance = 220;
                                    GrassManager.LOD_distance1 = 270;
                                    GrassManager.LOD_distance2 = 410;

                                    GrassManager.RandomRot = true;
                                }






                                //v1.5
                                if (GrassManager.Grass_selector == 21)
                                {
                                    //PINE TREE
                                    GrassManager.min_scale = 0.25f;
                                    GrassManager.max_scale = 0.8f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 10;
                                    GrassManager.Max_spread = 15;

                                    GrassManager.Cutoff_distance = 520;
                                    GrassManager.LOD_distance = 220;
                                    GrassManager.LOD_distance1 = 270;
                                    GrassManager.LOD_distance2 = 410;

                                    GrassManager.RandomRot = false;
                                }
                                //v1.5
                                if (GrassManager.Grass_selector == 22)
                                {
                                    //PINE TREE
                                    GrassManager.min_scale = 0.7f;
                                    GrassManager.max_scale = 1.55f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 5;
                                    GrassManager.Max_spread = 6;

                                    GrassManager.Cutoff_distance = 520;
                                    GrassManager.LOD_distance = 220;
                                    GrassManager.LOD_distance1 = 270;
                                    GrassManager.LOD_distance2 = 410;

                                    GrassManager.RandomRot = true;
                                }


                                if (GrassManager.Grass_selector == 23)
                                {
                                    //PINE TREE
                                    GrassManager.min_scale = 0.4f;
                                    GrassManager.max_scale = 0.92f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 5;
                                    GrassManager.Max_spread = 6;

                                    GrassManager.Cutoff_distance = 520;
                                    GrassManager.LOD_distance = 220;
                                    GrassManager.LOD_distance1 = 270;
                                    GrassManager.LOD_distance2 = 410;

                                    GrassManager.RandomRot = true;
                                }
                                if (GrassManager.Grass_selector == 24)
                                {
                                    //PINE TREE
                                    GrassManager.min_scale = 0.4f;
                                    GrassManager.max_scale = 1.0f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 5;
                                    GrassManager.Max_spread = 6;

                                    GrassManager.Cutoff_distance = 520;
                                    GrassManager.LOD_distance = 220;
                                    GrassManager.LOD_distance1 = 270;
                                    GrassManager.LOD_distance2 = 410;

                                    GrassManager.RandomRot = false;
                                }

                                //v1.6
                                if (GrassManager.Grass_selector == 25)
                                {
                                    //PINE TREE TWISTED
                                    GrassManager.min_scale = 1.4f;
                                    GrassManager.max_scale = 3.0f;
                                    GrassManager.Min_density = 1.0f;
                                    GrassManager.Max_density = 2.0f;
                                    GrassManager.Min_spread = 25;
                                    GrassManager.Max_spread = 36;

                                    GrassManager.Cutoff_distance = 500;
                                    GrassManager.LOD_distance = 150;
                                    GrassManager.LOD_distance1 = 190;
                                    GrassManager.LOD_distance2 = 250;

                                    GrassManager.RandomRot = true;
                                    GrassManager.RandRotMin = -20;
                                    GrassManager.RandRotMin = 20;
                                }
                                if (GrassManager.Grass_selector == 26)
                                {
                                    //STACKABLE ROCKS
                                    GrassManager.min_scale = 0.4f;
                                    GrassManager.max_scale = 2.0f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 5;
                                    GrassManager.Max_spread = 6;

                                    GrassManager.Cutoff_distance = 500;
                                    GrassManager.LOD_distance = 150;
                                    GrassManager.LOD_distance1 = 190;
                                    GrassManager.LOD_distance2 = 250;

                                    GrassManager.RandomRot = true;
                                }

                                //v1.7
                                if (GrassManager.Grass_selector == 27)
                                {
                                    //RED TREE
                                    GrassManager.min_scale = 1.2f;
                                    GrassManager.max_scale = 2.9f;
                                    GrassManager.Min_density = 1.0f;
                                    GrassManager.Max_density = 2.0f;
                                    GrassManager.Min_spread = 25;
                                    GrassManager.Max_spread = 36;

                                    GrassManager.Cutoff_distance = 500;
                                    GrassManager.LOD_distance = 150;
                                    GrassManager.LOD_distance1 = 190;
                                    GrassManager.LOD_distance2 = 250;

                                    GrassManager.RandomRot = true;
                                    GrassManager.RandRotMin = 0;
                                    GrassManager.RandRotMin = 0;
                                }
                                if (GrassManager.Grass_selector == 28 | GrassManager.Grass_selector == 29)
                                {
                                    //TOON DISTANT FOREST
                                    GrassManager.min_scale = 2f;
                                    GrassManager.max_scale = 4.0f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 5;
                                    GrassManager.Max_spread = 5;

                                    GrassManager.Cutoff_distance = 500;
                                    GrassManager.LOD_distance = 150;
                                    GrassManager.LOD_distance1 = 190;
                                    GrassManager.LOD_distance2 = 250;

                                    GrassManager.RandomRot = false;
                                    GrassManager.RandRotMin = -10;
                                    GrassManager.RandRotMin = 10;
                                }
                                if (GrassManager.Grass_selector == 30)
                                {
                                    //CIRCULAR WIND
                                    GrassManager.min_scale = 0.3f;
                                    GrassManager.max_scale = 0.7f;
                                    GrassManager.Min_density = 3.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 14;
                                    GrassManager.Max_spread = 16;

                                    GrassManager.Cutoff_distance = 500;
                                    GrassManager.LOD_distance = 150;
                                    GrassManager.LOD_distance1 = 190;
                                    GrassManager.LOD_distance2 = 250;

                                    GrassManager.RandomRot = true;
                                    GrassManager.RandRotMin = 0;
                                    GrassManager.RandRotMin = 0;
                                }
                                if (GrassManager.Grass_selector == 31)
                                {
                                    //DAISY
                                    GrassManager.min_scale = 0.8f;
                                    GrassManager.max_scale = 2.4f;
                                    GrassManager.Min_density = 2.0f;
                                    GrassManager.Max_density = 3.0f;
                                    GrassManager.Min_spread = 3;
                                    GrassManager.Max_spread = 7;

                                    GrassManager.Cutoff_distance = 500;
                                    GrassManager.LOD_distance = 150;
                                    GrassManager.LOD_distance1 = 190;
                                    GrassManager.LOD_distance2 = 250;

                                    GrassManager.RandomRot = true;
                                    GrassManager.RandRotMin = 0;
                                    GrassManager.RandRotMin = 0;
                                }



                                if (GrassManager.Grass_selector < 15)
                                {
                                    GrassManager.min_scale = GrassManager.min_scale * 0.3f;
                                    GrassManager.max_scale = GrassManager.max_scale * 0.3f;
                                    GrassManager.Min_spread = GrassManager.Min_spread * 0.6f;
                                    GrassManager.Max_spread = GrassManager.Max_spread * 0.6f;
                                }

                                if (GrassManager.AmplifyWind > 0.4f)
                                {
                                    GrassManager.AmplifyWind = 0.4f;
                                }

                            }
                        }

                        //NAMES
                        if (GrassManager.Grass_painting)
                        {
                            GUI.TextArea(new Rect(Grass_GUI_startX - 500, Grass_GUI_startY + 60 + 30, 330, 22), "Grass type(" + (GrassManager.Grass_selector + 1).ToString() + "):" + GrassManager.GrassPrefabsNames[GrassManager.Grass_selector]);
                        }                       

                        string Grass_rot = "Random rot On";
                        if (!GrassManager.RandomRot)
                        {
                            Grass_rot = "Random rot Off";
                        }
                        if (GUI.Button(new Rect(Grass_GUI_startX - 110, Grass_GUI_startY + 60 + 5 + 25 + 25, 110, 25), Grass_rot))
                        {
                            if (!GrassManager.RandomRot)
                            {
                                GrassManager.RandomRot = true;
                            }
                            else
                            {
                                GrassManager.RandomRot = false;
                            }
                        }

                        //GrassManager.Interactive = true;

                        GUI.TextArea(new Rect(Grass_GUI_startX, Grass_GUI_startY + 60 + 30, 110, 40), "Grass scale (Min:" + GrassManager.min_scale.ToString("F1") + "-Max:" + GrassManager.max_scale.ToString("F1") + ")");
                        GrassManager.min_scale = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40, 110, 25), GrassManager.min_scale, 0.05f, 2f);
                        GrassManager.max_scale = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30, 110, 25), GrassManager.max_scale, 0.05f, 3f);
                        if (GrassManager.max_scale < GrassManager.min_scale)
                        {
                            GrassManager.max_scale = GrassManager.min_scale;
                        }

                        GrassManager.Override_density = true;
                        GrassManager.Override_spread = true;
                        GUI.TextArea(new Rect(Grass_GUI_startX + 125, Grass_GUI_startY + 60 + 30, 110, 40), "Grass density (Min:" + GrassManager.Min_density.ToString("F1") + "-Max:" + GrassManager.Max_density.ToString("F1") + ")");
                        GrassManager.Min_density = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 125, Grass_GUI_startY + 60 + 5 + 30 + 40, 110, 25), GrassManager.Min_density, 1f, 3f);
                        if (GrassManager.Max_density < GrassManager.Min_density)
                        {
                            GrassManager.Max_density = GrassManager.Min_density;
                        }


                        GrassManager.Max_density = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30, 110, 25), GrassManager.Max_density, GrassManager.Min_density, 4f);
                        GUI.TextArea(new Rect(Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 30, 110, 40), "Grass spread (Min:" + GrassManager.Min_spread.ToString("F1") + "-Max:" + GrassManager.Max_spread.ToString("F1") + ")");
                        GrassManager.Min_spread = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40, 110, 25), GrassManager.Min_spread, 5f, 35f);
                        GrassManager.Max_spread = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30, 110, 25), GrassManager.Max_spread, GrassManager.Min_spread, 45f);

                        if (GrassManager.Max_spread < GrassManager.Min_spread)
                        {
                            GrassManager.Max_spread = GrassManager.Min_spread;
                        }

                        //			
                        //WIND
                        GUI.TextArea(new Rect(Grass_GUI_startX, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30, 110, 22), "Grass wind:" + GrassManager.AmplifyWind.ToString("F1"));
                        GrassManager.AmplifyWind = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30, 110, 25), GrassManager.AmplifyWind, 0f, 0.5f);

                        GUI.TextArea(new Rect(Grass_GUI_startX + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30, 110, 22), "Turbulence:" + GrassManager.WindTurbulence.ToString("F1"));
                        GrassManager.WindTurbulence = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30, 110, 25), GrassManager.WindTurbulence, 0f, 1.5f);

                        //FADE			
                        //GUI.TextArea(new Rect(Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30, 110, 22), "Grass fade:" + GrassManager.Grass_Fade_distance.ToString("F1"));
                        //GrassManager.Grass_Fade_distance = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30, 110, 25), GrassManager.Grass_Fade_distance, 25f, 400f);

                        ////Specular
                        //GUI.TextArea(new Rect(Grass_GUI_startX, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 50, 110, 22), "Grass spec:" + GrassManager.SpecularPower.ToString("F1"));
                        //GrassManager.SpecularPower = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 50, 110, 25), GrassManager.SpecularPower, -0.1f, 8.5f);

                        ////Specular
                        //GUI.TextArea(new Rect(Grass_GUI_startX, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 50 + 50, 230, 22), "Motion stop - Interaction distance:" + GrassManager.Stop_Motion_distance.ToString("F1"));
                        //GrassManager.Stop_Motion_distance = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 50 + 50, 230, 25), GrassManager.Stop_Motion_distance, 0f, 7f);

                        //GUI.TextArea(new Rect(Grass_GUI_startX, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 50 + 50 + 50, 230, 22), "Interaction stength:" + GrassManager.ShaderBInteractSpeed.ToString("F1"));
                        //GrassManager.ShaderBInteractSpeed = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 50 + 50 + 50, 230, 25), GrassManager.ShaderBInteractSpeed, 0f, 7f);

                        
                    }

                    

                    //TINT			
                    //GUI.TextArea(new Rect(Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 40 + 9 + 30 + 5, 110, 22), "Tint power:" + GrassManager.TintPower.ToString("F1"));
                    //GrassManager.TintPower = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 40 + 9 + 30 + 5, 110, 25), GrassManager.TintPower, 0, 3f);
                    //GUI.TextArea(new Rect(Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 40 + 9 + 30 + 35, 110, 25), "Tint color (RGB)");
                    //Color TMP_color = GrassManager.tintColor;
                    //TMP_color.r = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 40 + 9 + 30 + 35 + 30, 110, 25), TMP_color.r, 0f, 1f);
                    //TMP_color.g = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 40 + 9 + 30 + 35 + 35 + 30, 110, 25), TMP_color.g, 0f, 1f);
                    //TMP_color.b = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 40 + 9 + 30 + 35 + 35 + 35 + 30, 110, 25), TMP_color.b, 0f, 1f);
                    //GrassManager.tintColor = TMP_color;

                    //GUI.TextArea(new Rect(Grass_GUI_startX + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 40 + 9 - 5 + 5 + 0, 110, 22), "Tint freq:" + GrassManager.TintFrequency.ToString("F2"));
                    //GrassManager.TintFrequency = GUI.HorizontalSlider(new Rect(Grass_GUI_startX + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 40 + 9 - 5 + 5 + 0, 110, 25), GrassManager.TintFrequency, 0.01f, 0.12f);

                }

                ////////////////

            }


            if (GUI.Button(new Rect(10, 120, 110, 22), "Save Grass"))
            {
                //clear list
                savedGrassStrokesInfo.m_GrassData.Clear();

                //populate list - read grass growers
                for (int i = 0; i < GrassManager.Grasses.Count; i++)
                {
                    SaveGrassData.GrassData grasData = new SaveGrassData.GrassData();

                    //populate data
                    grasData.grassType = GrassManager.Grasses[i].Type-1;
                    grasData.position = GrassManager.Grasses[i].transform.position;
                    grasData.scaleMinMax = new Vector2(GrassManager.Grasses[i].Start_tree_scale, GrassManager.Grasses[i].End_scale);
                    grasData.spreadMinMax = GrassManager.Grasses[i].PosSpread;//grasData.spreadMinMax = GrassManager.Grasses[i].Min_max_spread;
                    grasData.densityMinMax = GrassManager.Grasses[i].Min_Max_Branching;
                    if (GrassManager.Grasses[i].RandomRot)
                    {
                        grasData.randomRot = 1;
                    }
                    else
                    {
                        grasData.randomRot = 0;
                    }

                    savedGrassStrokesInfo.m_GrassData.Add(grasData);
                }

                //save list
                if (FileManager.WriteToFile("SaveGrassData01.dat", savedGrassStrokesInfo.ToJson()))
                {
                    Debug.Log("Save successful in: " + Application.persistentDataPath);
                }
            }
            if (GUI.Button(new Rect(10, 150, 110, 22), "Load Grass"))
            {
                //clear list
                savedGrassStrokesInfo.m_GrassData.Clear();

                //load list
                if (FileManager.LoadFromFile("SaveGrassData01.dat", out var json))
                {
                    savedGrassStrokesInfo.LoadFromJson(json);
                    Debug.Log("Load completed");
                }

                if (useLoopDelay) //NEW2
                {
                    StartCoroutine(PlantGrassWithDelay());
                }
                else
                {
                    //plant grass based on loaded parameters.
                    for (int i = 0; i < savedGrassStrokesInfo.m_GrassData.Count; i++)
                    {
                        Ray ray = new Ray();

                        //make top down raycast
                        ray.origin = savedGrassStrokesInfo.m_GrassData[i].position + new Vector3(0, distRayAboveGround, 0);
                        ray.direction = -Vector3.up;

                        RaycastHit hit = new RaycastHit();
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                        {
                            //setup grass manager variables
                            GrassManager.min_scale = savedGrassStrokesInfo.m_GrassData[i].scaleMinMax.x;
                            GrassManager.max_scale = savedGrassStrokesInfo.m_GrassData[i].scaleMinMax.y;
                            GrassManager.Min_density = savedGrassStrokesInfo.m_GrassData[i].densityMinMax.x;
                            GrassManager.Max_density = savedGrassStrokesInfo.m_GrassData[i].densityMinMax.y;
                            GrassManager.Min_spread = savedGrassStrokesInfo.m_GrassData[i].spreadMinMax.x;
                            GrassManager.Max_spread = savedGrassStrokesInfo.m_GrassData[i].spreadMinMax.y;

                            GrassManager.Grass_selector = savedGrassStrokesInfo.m_GrassData[i].grassType;
                            Grass_selector = savedGrassStrokesInfo.m_GrassData[i].grassType;
                            //grass
                            if (savedGrassStrokesInfo.m_GrassData[i].randomRot == 0)
                            {
                                GrassManager.RandomRot = false;
                            }
                            else
                            {
                                GrassManager.RandomRot = true;
                            }
                            GrassManager.Override_density = true;
                            GrassManager.Override_spread = true;

                            PlantInPosition(hit.point, hit.normal, hit.collider, hit.collider.transform);
                        }

                    }
                }//if loop delay check
            }



        }// END OnGUI	

        //NEW2
        private IEnumerator PlantGrassWithDelay()
        {
            //WaitForSeconds wait = new WaitForSeconds(2f);
            WaitForSeconds wait = new WaitForSeconds(loopDelay);

            //plant grass based on loaded parameters.
            for (int i = 0; i < savedGrassStrokesInfo.m_GrassData.Count; i++)
            {
                Ray ray = new Ray();

                //make top down raycast
                ray.origin = savedGrassStrokesInfo.m_GrassData[i].position + new Vector3(0, distRayAboveGround, 0);
                ray.direction = -Vector3.up;

                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    //setup grass manager variables
                    GrassManager.min_scale = savedGrassStrokesInfo.m_GrassData[i].scaleMinMax.x;
                    GrassManager.max_scale = savedGrassStrokesInfo.m_GrassData[i].scaleMinMax.y;
                    GrassManager.Min_density = savedGrassStrokesInfo.m_GrassData[i].densityMinMax.x;
                    GrassManager.Max_density = savedGrassStrokesInfo.m_GrassData[i].densityMinMax.y;
                    GrassManager.Min_spread = savedGrassStrokesInfo.m_GrassData[i].spreadMinMax.x;
                    GrassManager.Max_spread = savedGrassStrokesInfo.m_GrassData[i].spreadMinMax.y;

                    GrassManager.Grass_selector = savedGrassStrokesInfo.m_GrassData[i].grassType;
                    Grass_selector = savedGrassStrokesInfo.m_GrassData[i].grassType;
                    //grass
                    if (savedGrassStrokesInfo.m_GrassData[i].randomRot == 0)
                    {
                        GrassManager.RandomRot = false;
                    }
                    else
                    {
                        GrassManager.RandomRot = true;
                    }
                    GrassManager.Override_density = true;
                    GrassManager.Override_spread = true;

                    PlantInPosition(hit.point, hit.normal, hit.collider, hit.collider.transform);

                    yield return wait;
                }
            }
            /////

        }


        //END NEW1 - GUI


    }
}
