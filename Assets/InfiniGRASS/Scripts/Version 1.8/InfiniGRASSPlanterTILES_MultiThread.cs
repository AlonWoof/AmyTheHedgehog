using UnityEngine;
using System.Collections;
using Artngame.INfiniDy;
using System.Collections.Generic;
using System.Threading;

namespace Artngame.INfiniDy {
public class InfiniGRASSPlanterTILES_MultiThread : MonoBehaviour {

        public GameObject Tile;
        public List<GameObject> Tiles;
        public List<Vector3> TilesPositions;

        public string GrassManagerObjectName = "GRASS MANAGER";

		//v2.1.3
		public bool grow_grass = false;
		public float start_size_factor = 0.3f;

	public InfiniGRASSManager Grassmanager;
	public bool bulkPaint = false;
	public int Grass_selector=0;
	public float raycastHeight = 0.1f;

		public bool useCollisions = false;

	// Use this for initialization
	void Start () {
			if (Grassmanager == null) {
				GameObject manager = GameObject.Find (GrassManagerObjectName);
				if (manager != null) {
					Grassmanager = manager.GetComponent<InfiniGRASSManager> ();
				}
			}
	}

        public Transform player;
        public int gridSize = 50;
        public float tileSize = 2;
        public float distThreshold = 10;

        //v1.9.9.6e
        public bool selectRandomGrasses = false;
        public List<int> randomGrassesIDs = new List<int>();

        // Update is called once per frame
        public int plantButton = 0; //v1.9.9.5
        public bool useMouse = false;

        //v0.1
        public bool signalPlanting = false;
        public bool updateOnPlayerMove = false;
        Vector3 tilePos = new Vector3(-1,-1,-1);
        Vector3 player_pos;
        public bool playerMoved = false;
        public float updatePlayerThreshold = 0.1f;

        //https://www.jacksondunstan.com/articles/3930
        //MainThreadQueue mainThreadQueue;
        Vector3 prevPlayerPos;
        public bool endlessGrid = false;

        // Update is called once per frame
        void Update()
        {
            //v0.1
            player_pos = player.position;
            //mainThreadQueue.Execute(5);

            if(!playerMoved && (player_pos - prevPlayerPos).magnitude > updatePlayerThreshold)
            {
                playerMoved = true;
                prevPlayerPos = player.position;
            }

            if (Tile != null && !signalPlanting && (playerMoved || UnityEngine.Random.Range(1,10)==5 || Time.fixedTime < 5 || !updateOnPlayerMove) )
            {
                playerMoved = false;                

                LoomINfiniDyGRASS.RunAsync(() =>
                {
                    //if (Tile != null && !signalPlanting)
                    {
                        //if player near tile position and is not in tiles list, add it
                        //int tileID = -1;
                        float tileX = 0;
                        float tileY = 0;

                        int tileStartX = (int)(player_pos.x / tileSize);
                        int tileStartZ = (int)(player_pos.z / tileSize);

                        bool insideGrid = false;
                        if (endlessGrid || (!endlessGrid && tileStartX >=0 && tileStartX <= gridSize && tileStartZ >= 0 && tileStartZ <= gridSize))
                        {
                            insideGrid = true;
                        }
                        if (insideGrid)
                        {
                            for (int i = tileStartX - 5; i < tileStartX + 5; i++)//for (int i = 0; i < gridSize; i++)
                            {
                                for (int j = tileStartZ - 5; j < tileStartZ + 5; j++)//for (int j = 0; j < gridSize; j++)
                                {
                                    tileX = i * tileSize;
                                    tileY = j * tileSize;
                                    Vector3 tilePosIN = new Vector3(tileX, 0, tileY);
                                    Vector3 playerPos = new Vector3(player_pos.x, 0, player_pos.z);
                                    float distance = (playerPos - tilePosIN).magnitude;
                                    bool notYetCreated = true;

                                    //search for created
                                    for (int k = 0; k < TilesPositions.Count; k++)
                                    {
                                        if (new Vector3(TilesPositions[k].x, 0, TilesPositions[k].z) == new Vector3(tileX, 0, tileY))
                                        {
                                            notYetCreated = false;
                                            break;
                                        }
                                    }
                                    //Debug.Log(distance);
                                    // Debug.Log(new Vector3(tileX, 0, tileY));
                                    if (distance < distThreshold && notYetCreated)
                                    {
                                        signalPlanting = true;
                                        tilePos = new Vector3(tilePosIN.x, 0, tilePosIN.z);
                                        //break; 
                                    }
                                }
                                if (signalPlanting)
                                {
                                    break;
                                }
                            }
                        }
                    }
                });
            }

            //v0.1
            if (signalPlanting && tilePos != new Vector3(-1, -1, -1))
            {
                //v0.1a
                //LoomINfiniDyGRASS.QueueOnMainThread(() =>
                //{
                //    //instantiate tiles around player
                //    //Debug.Log(new Vector3(tileX, 0, tileY));
                //    GameObject instancedTile = Instantiate(Tile, new Vector3(tilePos.x, 0, tilePos.z), Quaternion.identity);
                //    Tiles.Add(instancedTile);
                //    TilesPositions.Add(new Vector3(tilePos.x, 0, tilePos.z));

                //    //plant on it
                //    Ray ray = new Ray();
                //    ray.origin = new Vector3(tilePos.x, raycastHeight, tilePos.z);
                //    ray.direction = -Vector3.up;
                //    RaycastHit hit = new RaycastHit();

                //    //randomize grass
                //    int[] integers = new int[] { 0, 1, 2, 3, 21, 10, 11 };
                //    if (selectRandomGrasses && randomGrassesIDs.Count > 0)//v1.9.9.6e
                //    {
                //        //pass new list
                //        integers = randomGrassesIDs.ToArray();
                //    }
                //    int randValue = Random.Range(0, integers.Length);
                //    int value = integers[randValue];
                //    Grass_selector = value;
                //    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                //    {
                //        PlantInPosition(hit.point, hit.normal, hit.collider, hit.collider.transform);
                //    }
                //    tilePos = new Vector3(-1, -1, -1);
                //    signalPlanting = false;
                //});

                //instantiate tiles around player
                //Debug.Log(new Vector3(tileX, 0, tileY));
                GameObject instancedTile = Instantiate(Tile, new Vector3(tilePos.x, 0, tilePos.z), Quaternion.identity);
                Tiles.Add(instancedTile);
                TilesPositions.Add(new Vector3(tilePos.x, 0, tilePos.z));

                //plant on it
                Ray ray = new Ray();
                ray.origin = new Vector3(tilePos.x, raycastHeight, tilePos.z);
                ray.direction = -Vector3.up;
                RaycastHit hit = new RaycastHit();

                //randomize grass
                int[] integers = new int[] { 0, 1, 2, 3, 21, 10, 11 };
                if (selectRandomGrasses && randomGrassesIDs.Count > 0)//v1.9.9.6e
                {
                    //pass new list
                    integers = randomGrassesIDs.ToArray();
                }
                int randValue = Random.Range(0, integers.Length);
                int value = integers[randValue];
                Grass_selector = value;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    PlantInPosition(hit.point, hit.normal, hit.collider, hit.collider.transform);
                }
                tilePos = new Vector3(-1, -1, -1);
                signalPlanting = false;
            }

            if (player == null)
            {
                player = Camera.main.transform;
            }

            




            if (useMouse && Grassmanager != null)
            {
                if (Input.GetMouseButtonDown(plantButton) & !Input.GetKeyDown(KeyCode.LeftShift))
                {
                    Ray ray = new Ray();

                    //v1.4c
                    bool found_cam = false;
                    if (Grassmanager.Tag_based_player)
                    {
                        if (Grassmanager.player != null)
                        {
                            Camera[] playerCams = Grassmanager.player.GetComponentsInChildren<Camera>(false);
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
			PlantGrass (hit, collider1, transform1, Grass_selector, false, true);
		}

	void OnCollisionEnter(Collision info){
			if (useCollisions && Grassmanager != null) {
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

					GameObject TEMP = Instantiate (Grassmanager.GrassPrefabs [Grass_selector]);
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
					if (Grassmanager.AdaptOnTerrain & is_Terrain) {
						int Xpos = (int)(((hit.point.x - Grassmanager.Tpos.x)*Grassmanager.Tdata.alphamapWidth/Grassmanager.Tdata.size.x));
						int Zpos = (int)(((hit.point.z - Grassmanager.Tpos.z)*Grassmanager.Tdata.alphamapHeight/Grassmanager.Tdata.size.z));
						float[,,] splats = Grassmanager.Tdata.GetAlphamaps(Xpos,Zpos,1,1);
						float[] Tarray = new float[splats.GetUpperBound(2)+1];
						for(int j =0;j<Tarray.Length;j++){
							Tarray[j] = splats[0,0,j];
							//Debug.Log(Tarray[j]); // ScalePerTexture
						}
						float Scaling = 0;
						for(int j =0;j<Tarray.Length;j++){
							if(j > Grassmanager.ScalePerTexture.Count-1){
								Scaling = Scaling + (1*Tarray[j]);
							}else{
								Scaling = Scaling + (Grassmanager.ScalePerTexture[j]*Tarray[j]);
							}
						}
						TREE.End_scale = Scaling*UnityEngine.Random.Range (Grassmanager.min_scale, Grassmanager.max_scale);
						//Debug.Log(Tarray);
					}else{
						TREE.End_scale = UnityEngine.Random.Range (Grassmanager.min_scale, Grassmanager.max_scale);
					}

					TREE.Max_interact_holder_items = Grassmanager.Max_interactive_group_members;//Define max number of trees grouped in interactive batcher that opens up. 
					//Increase to lower draw calls, decrease to lower spikes when group is opened for interaction
					TREE.Max_trees_per_group = Grassmanager.Max_static_group_members;

					TREE.Interactive_tree = Grassmanager.Interactive;

					//v2.1
					if (Application.isPlaying) {
						TREE.transform.localScale *= TREE.End_scale * Grassmanager.Collider_scale;
					} else {
						TREE.colliderScale = Vector3.one *Grassmanager.Collider_scale;
					}

					if(Grassmanager.Override_spread){
						TREE.PosSpread = new Vector2(UnityEngine.Random.Range(Grassmanager.Min_spread,Grassmanager.Max_spread),UnityEngine.Random.Range(Grassmanager.Min_spread,Grassmanager.Max_spread));
					}
					if(Grassmanager.Override_density){
						TREE.Min_Max_Branching = new Vector2(Grassmanager.Min_density,Grassmanager.Max_density);
					}
					TREE.PaintedOnOBJ = transform1.gameObject.transform;
					TREE.GridOnNormal = Grassmanager.GridOnNormal;
					TREE.max_ray_dist = Grassmanager.rayCastDist;
					TREE.MinAvoidDist = Grassmanager.MinAvoidDist;
					TREE.MinScaleAvoidDist = Grassmanager.MinScaleAvoidDist;
					TREE.InteractionSpeed = Grassmanager.InteractionSpeed;
					TREE.InteractSpeedThres = Grassmanager.InteractSpeedThres;

                    //v1.9.9.7
                    TREE.enableAbove64KMesh = Grassmanager.enableAbove64KMesh;

                    //v1.4
                    TREE.Interaction_thres = Grassmanager.Interaction_thres;
					TREE.Max_tree_dist = Grassmanager.Max_tree_dist;//v1.4.6
					TREE.Disable_after_growth = Grassmanager.Disable_after_growth;//v1.5
					TREE.WhenCombinerFull = Grassmanager.WhenCombinerFull;//v1.5
					TREE.Eliminate_original_mesh = Grassmanager.Eliminate_original_mesh;//v1.5
					TREE.Interaction_offset = Grassmanager.Interaction_offset;

					TREE.LOD_distance = Grassmanager.LOD_distance;
					TREE.LOD_distance1 = Grassmanager.LOD_distance1;
					TREE.LOD_distance2 = Grassmanager.LOD_distance2;
					TREE.Cutoff_distance = Grassmanager.Cutoff_distance;

					TREE.Tag_based = false;
					TREE.GrassManager = Grassmanager;////////////////////////// v2.1.1
					TREE.Type = Grass_selector+1;
					TREE.Start_tree_scale = TREE.End_scale/4;

					TREE.RandomRot = Grassmanager.RandomRot;
					TREE.RandRotMin = Grassmanager.RandRotMin;
					TREE.RandRotMax = Grassmanager.RandRotMax;

					TREE.GroupByObject = Grassmanager.GroupByObject;
					TREE.ParentToObject = Grassmanager.ParentToObject;
					TREE.MoveWithObject = Grassmanager.MoveWithObject;
					TREE.AvoidOwnColl = Grassmanager.AvoidOwnColl;

					TEMP.transform.parent = Grassmanager.GrassHolder.transform;

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
					Grassmanager.Grasses.Add (TREE);
					Grassmanager.GrassesType.Add (Grass_selector);

					TEMP.name = "GrassPatch" + Grassmanager.Grasses.Count.ToString (); 

					TREE.Grass_Holder_Index = Grassmanager.Grasses.Count - 1;//register id in grasses list



					//RECONFIG
					TREE.transform.parent = Grassmanager.GrassHolder.transform;
					Grassmanager.CleanUp = false;
					INfiniDyGrassField forest = Grassmanager.Grasses[Grassmanager.Grasses.Count-1] ;
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
		}


}
}
