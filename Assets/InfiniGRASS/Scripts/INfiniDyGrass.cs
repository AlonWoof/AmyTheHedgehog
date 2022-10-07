using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Artngame.INfiniDy {
	public class INfiniDyGrass
	{
		public int Level;
		public List<INfiniDyGrass> Leaves = new List<INfiniDyGrass>();
		GameObject TreeHolder;
		public GameObject INfiniDyForestOBJ;
		GameObject Instances;	
		public INfiniDyGrassField INfiniDyForestC;
		public bool is_root = false;
		List<INfiniDyGrass> Branch_List;
		
		public float current_time;
		public float interval = 0.5f;
		public int Height_Level;
		
		void makeBranches()
		{
			float lenA  = INfiniDyForestC.lower_length_by * Instances.transform.localScale.y;
			float radiA = INfiniDyForestC.lower_radi_by   * Instances.transform.localScale.x;
			
			int count_to=1;
			if(Level == 0){
				count_to = INfiniDyForestC.Height_levels;
			}
			Transform previous = TreeHolder.transform;
			for(int k=0;k<count_to;k++){
				
				//reduce radius by height
				if(!INfiniDyForestC.Use_height){
					Height_Level = k;
				}else{
					if(Level == 0 & INfiniDyForestC.Height_levels > 1){
						Height_Level = k;
					}
				}
				
				
				float Radi_min = INfiniDyForestC.Min_Max_Radi.x;
				if(Height_Level>0){
					Radi_min = Radi_min *(0.99f-(Height_Level*INfiniDyForestC.Height_reduce));				
				}
				
				if(radiA < Radi_min){ 
					
				}else{	
					
					if(Height_Level>0){
						lenA  = lenA *(0.99f-(Height_Level*INfiniDyForestC.Height_reduce));
						radiA = radiA*(0.99f-(Height_Level*INfiniDyForestC.Height_reduce));
					}
					
					Branch_List = new List<INfiniDyGrass>();
					
					GameObject Instance = new GameObject();	
					
					//parent to height segments if height > 0 
					if(!INfiniDyForestC.Decouple_from_bark){
						if(Height_Level == 0){
							Instance.transform.parent = previous;  
							previous = Instance.transform;
						}else{
							Instance.transform.parent = previous;
							previous = Instance.transform;
						}
					}else{
						Instance.transform.parent = previous;
					}
					
					Instance.transform.localPosition = Vector3.zero;
					Instance.transform.localEulerAngles= Vector3.zero;
					Instance.name = "Node";
					
					if(Height_Level > 0 & Level < 2){
					}
					
					Instance.transform.Translate(0,Instances.transform.localPosition.y*2,0);
					
					//CHANGES - Added the below if
					if(k > 0 & Level < 1){ 
						Instance.transform.position = Instance.transform.position + (INfiniDyForestC.Height_offset*1) + new Vector3(0,k*INfiniDyForestC.Height_separation,0);
					}
					
					if(Height_Level > 0 & Level < 1){
						Instance.transform.Rotate(INfiniDyForestC.Level1Decline*(Height_Level+1),0,INfiniDyForestC.Level1Decline*(Height_Level+1));
					}
					
					int child_count = (int)(Random.Range(INfiniDyForestC.Min_Max_Branching.x,INfiniDyForestC.Min_Max_Branching.y)/(1)); 
					
					//v1.2 - max branches per level
					if(Level > 0 & INfiniDyForestC.Max_branches_per_level.Count >= Level){
						child_count = (int)(Random.Range(INfiniDyForestC.Min_Max_Branching.x,INfiniDyForestC.Max_branches_per_level[Level-1])/(1)); 
					}
					
					for(int i = 0; i < child_count; i++)
					{
						INfiniDyGrass childTree = new INfiniDyGrass();
						childTree.INfiniDyForestC = INfiniDyForestC;
						Branch_List.Add(childTree);
						
						if(INfiniDyForestOBJ != null){
							childTree.INfiniDyForestC = INfiniDyForestOBJ.GetComponent(typeof(INfiniDyGrassField)) as INfiniDyGrassField;
						}
						childTree.Level = Level+1;
						childTree.Height_Level = Height_Level;
						childTree.generateTree(radiA,lenA,Color.blue,Vector3.zero+new Vector3(0,k*INfiniDyForestC.Height_separation,0), Instance);
					}
					
					
					if(Level > INfiniDyForestC.Leaf_level_min_max.x & Level < INfiniDyForestC.Leaf_level_min_max.y &  (!INfiniDyForestC.Use_height | (INfiniDyForestC.Use_height & Height_Level > INfiniDyForestC.Leaf_height_min_max.x & Height_Level < INfiniDyForestC.Leaf_height_min_max.y))  ){
						for (int j=0;j<INfiniDyForestC.Leaves_per_branch;j++){
							
							INfiniDyForestC.Registered_Leaves.Add (Instance.transform.position + Instance.transform.up.normalized*INfiniDyForestC.Leaf_dist_factor*(j+1)*Instance.transform.localScale.y); 
							
							INfiniDyForestC.Registered_Leaves_Rot.Add(Quaternion.Slerp(Instance.transform.rotation, Quaternion.AngleAxis(Random.Range(-270,270),Instance.transform.right.normalized+new Vector3(Random.Range(-270,270),Random.Range(-270,270),Random.Range(-270,270))),0.5f));
							INfiniDyForestC.Leaf_belongs_to_branch.Add(INfiniDyForestC.Registered_Brances.Count-1);
						}
					}
					if(Level > 1){
						
					}				
					
					//CHANGES 1 (1 to 0 !!!!)
					if(Level > 0){
						Instances.transform.localScale = Instances.transform.localScale/INfiniDyForestC.Growth_start_div;					
					}
				}
			}
		}	
		
		public void RotTree(float angle)
		{	if( Level >1){
				TreeHolder.transform.Rotate(0,0,angle);
			}
			if(Branch_List==null | Level <= 1){
			}else{ 
				for(int i=0;i<Branch_List.Count;i++)
				{
					Branch_List[i].RotTree(angle);
				}
			}
		}
		
		public void generateTree(float radi, float len, Color Branch_Color, Vector3 Position, GameObject INfiniDyGrassParent)
		{		
			if (INfiniDyForestOBJ != null) {
				
			}

			if (Level < 1) {
				//Debug.Log (INfiniDyForestC.gameObject.name);
			}
			//INFINIGRASS - avoid scene objects
			float find_x = 0;
			float find_z = 0;
			float scale_mod = 1;
			if (INfiniDyForestC != null && INfiniDyForestC.root_tree != null) {


				//find_x = Random.Range (-7, 7 + Random.Range (-4, 4));
				//find_z = Random.Range (-7, 7 + Random.Range (-4, 4));

				find_x = Random.Range (-INfiniDyForestC.PosSpread.x, INfiniDyForestC.PosSpread.x + Random.Range (-INfiniDyForestC.PosSpread.x/2, INfiniDyForestC.PosSpread.x/2));
				find_z = Random.Range (-INfiniDyForestC.PosSpread.y, INfiniDyForestC.PosSpread.y + Random.Range (-INfiniDyForestC.PosSpread.y/2, INfiniDyForestC.PosSpread.y/2));


				int sign = 1;
				if(INfiniDyForestC.Intial_Up_Vector.y < 0){
					sign = -1;
				}

				Vector3 Start_pos = new Vector3 (find_x, 0, find_z);
				Ray ray = new Ray (INfiniDyForestC.root_tree.transform.position+Quaternion.FromToRotation(Vector3.up,-INfiniDyForestC.Intial_Up_Vector) * Start_pos 
				               + (INfiniDyForestC.max_ray_dist*INfiniDyForestC.Intial_Up_Vector), -INfiniDyForestC.Intial_Up_Vector);
				if(!INfiniDyForestC.GridOnNormal){
					Start_pos = new Vector3 (INfiniDyForestC.root_tree.transform.position.x + find_x, INfiniDyForestC.root_tree.transform.position.y+(sign*INfiniDyForestC.max_ray_dist), INfiniDyForestC.root_tree.transform.position.z + find_z);
					ray = new Ray (Start_pos, -sign*Vector3.up);
				}
				RaycastHit hit = new RaycastHit ();
				if (Physics.Raycast (ray, out hit, INfiniDyForestC.max_ray_dist)) {
					//if (hit.collider.gameObject != Terrain.activeTerrain.gameObject & hit.collider.gameObject.tag != "PPaint") {
					if (INfiniDyForestC.PaintedOnOBJ != null && hit.collider.gameObject != INfiniDyForestC.PaintedOnOBJ.gameObject) {
						return;
					}
				}
				float MOD1 = INfiniDyForestC.MinAvoidDist;
				Start_pos = new Vector3 (find_x, 0, find_z)+ new Vector3(MOD1,0,MOD1);
				ray = new Ray (INfiniDyForestC.root_tree.transform.position+Quaternion.FromToRotation(Vector3.up,-INfiniDyForestC.Intial_Up_Vector) * Start_pos 
				                   + (INfiniDyForestC.max_ray_dist*INfiniDyForestC.Intial_Up_Vector), -INfiniDyForestC.Intial_Up_Vector);
				if(!INfiniDyForestC.GridOnNormal){
					ray = new Ray (Start_pos + new Vector3(MOD1,0,MOD1), -sign*Vector3.up);
				}
				if (Physics.Raycast (ray, out hit, INfiniDyForestC.max_ray_dist*2)) {
					//if (hit.collider.gameObject != Terrain.activeTerrain.gameObject & hit.collider.gameObject.tag != "PPaint") {
					if (INfiniDyForestC.PaintedOnOBJ != null && hit.collider.gameObject != INfiniDyForestC.PaintedOnOBJ.gameObject) {
						return;
					}
				}

				Start_pos = new Vector3 (find_x, 0, find_z)+ new Vector3(-MOD1,0,-MOD1);
				ray = new Ray (INfiniDyForestC.root_tree.transform.position+Quaternion.FromToRotation(Vector3.up,-INfiniDyForestC.Intial_Up_Vector) * Start_pos 
				               + (INfiniDyForestC.max_ray_dist*INfiniDyForestC.Intial_Up_Vector), -INfiniDyForestC.Intial_Up_Vector);
					if(!INfiniDyForestC.GridOnNormal){
						ray = new Ray (Start_pos + new Vector3(-MOD1,0,-MOD1), -sign*Vector3.up);
					}
				if (Physics.Raycast (ray, out hit,INfiniDyForestC.max_ray_dist*2)) {
					//if (hit.collider.gameObject != Terrain.activeTerrain.gameObject & hit.collider.gameObject.tag != "PPaint") {
					if (INfiniDyForestC.PaintedOnOBJ != null && hit.collider.gameObject != INfiniDyForestC.PaintedOnOBJ.gameObject) {
						return;
					}
				}

				Start_pos = new Vector3 (find_x, 0, find_z)+ new Vector3(MOD1,0,-MOD1);
				ray = new Ray (INfiniDyForestC.root_tree.transform.position+Quaternion.FromToRotation(Vector3.up,-INfiniDyForestC.Intial_Up_Vector) * Start_pos 
				               + (INfiniDyForestC.max_ray_dist*INfiniDyForestC.Intial_Up_Vector), -INfiniDyForestC.Intial_Up_Vector);
						if(!INfiniDyForestC.GridOnNormal){
							ray = new Ray (Start_pos + new Vector3(MOD1,0,-MOD1), -sign*Vector3.up);
						}
				if (Physics.Raycast (ray, out hit, INfiniDyForestC.max_ray_dist*2)) {
					//if (hit.collider.gameObject != Terrain.activeTerrain.gameObject & hit.collider.gameObject.tag != "PPaint") {
					if (INfiniDyForestC.PaintedOnOBJ != null && hit.collider.gameObject != INfiniDyForestC.PaintedOnOBJ.gameObject) {
						return;
					}
				}

				Start_pos = new Vector3 (find_x, 0, find_z)+ new Vector3(-MOD1,0,MOD1);
				ray = new Ray (INfiniDyForestC.root_tree.transform.position+Quaternion.FromToRotation(Vector3.up,-INfiniDyForestC.Intial_Up_Vector) * Start_pos 
				               + (INfiniDyForestC.max_ray_dist*INfiniDyForestC.Intial_Up_Vector), -INfiniDyForestC.Intial_Up_Vector);
				if(!INfiniDyForestC.GridOnNormal){
					ray = new Ray (Start_pos + new Vector3(-MOD1,0,MOD1), -sign*Vector3.up);
				}
				if (Physics.Raycast (ray, out hit, INfiniDyForestC.max_ray_dist*2)) {
					//if (hit.collider.gameObject != Terrain.activeTerrain.gameObject & hit.collider.gameObject.tag != "PPaint") {
					if (INfiniDyForestC.PaintedOnOBJ != null && hit.collider.gameObject != INfiniDyForestC.PaintedOnOBJ.gameObject) {
						return;
					}
				}

				MOD1 = INfiniDyForestC.MinScaleAvoidDist;
				Start_pos = new Vector3 (find_x, 0, find_z)+ new Vector3(MOD1,0,MOD1);
				ray = new Ray (INfiniDyForestC.root_tree.transform.position+Quaternion.FromToRotation(Vector3.up,-INfiniDyForestC.Intial_Up_Vector) * Start_pos 
				               + (INfiniDyForestC.max_ray_dist*INfiniDyForestC.Intial_Up_Vector), -INfiniDyForestC.Intial_Up_Vector);
				if(!INfiniDyForestC.GridOnNormal){
					ray = new Ray (Start_pos + new Vector3(MOD1,0,MOD1), -sign*Vector3.up);
				}
				if (Physics.Raycast (ray, out hit, INfiniDyForestC.max_ray_dist*2)) {
					//if (hit.collider.gameObject != Terrain.activeTerrain.gameObject & hit.collider.gameObject.tag != "PPaint") {
					if (INfiniDyForestC.PaintedOnOBJ != null && hit.collider.gameObject != INfiniDyForestC.PaintedOnOBJ.gameObject) {
						//return;
						scale_mod = 0.2f;
					}
				}

				Start_pos = new Vector3 (find_x, 0, find_z)+ new Vector3(-MOD1,0,-MOD1);
				ray = new Ray (INfiniDyForestC.root_tree.transform.position+Quaternion.FromToRotation(Vector3.up,-INfiniDyForestC.Intial_Up_Vector) * Start_pos 
				               + (INfiniDyForestC.max_ray_dist*INfiniDyForestC.Intial_Up_Vector), -INfiniDyForestC.Intial_Up_Vector);
				if(!INfiniDyForestC.GridOnNormal){
				ray = new Ray (Start_pos + new Vector3(-MOD1,0,-MOD1), -sign*Vector3.up);
				}
				if (Physics.Raycast (ray, out hit, INfiniDyForestC.max_ray_dist*2)) {
					//if (hit.collider.gameObject != Terrain.activeTerrain.gameObject & hit.collider.gameObject.tag != "PPaint") {
					if (INfiniDyForestC.PaintedOnOBJ != null && hit.collider.gameObject != INfiniDyForestC.PaintedOnOBJ.gameObject) {
						//return;
						scale_mod = 0.2f;
					}
				}

				Start_pos = new Vector3 (find_x, 0, find_z)+ new Vector3(MOD1,0,-MOD1);
				ray = new Ray (INfiniDyForestC.root_tree.transform.position+Quaternion.FromToRotation(Vector3.up,-INfiniDyForestC.Intial_Up_Vector) * Start_pos 
				               + (INfiniDyForestC.max_ray_dist*INfiniDyForestC.Intial_Up_Vector), -INfiniDyForestC.Intial_Up_Vector);
				if(!INfiniDyForestC.GridOnNormal){
					ray = new Ray (Start_pos + new Vector3(MOD1,0,-MOD1), -sign*Vector3.up);
				}
				if (Physics.Raycast (ray, out hit, INfiniDyForestC.max_ray_dist*2)) {
					//if (hit.collider.gameObject != Terrain.activeTerrain.gameObject & hit.collider.gameObject.tag != "PPaint") {
					if (INfiniDyForestC.PaintedOnOBJ != null && hit.collider.gameObject != INfiniDyForestC.PaintedOnOBJ.gameObject) {
						//return;
						scale_mod = 0.2f;
					}
				}

				Start_pos = new Vector3 (find_x, 0, find_z)+ new Vector3(-MOD1,0,MOD1);
				ray = new Ray (INfiniDyForestC.root_tree.transform.position+Quaternion.FromToRotation(Vector3.up,-INfiniDyForestC.Intial_Up_Vector) * Start_pos 
				               + (INfiniDyForestC.max_ray_dist*INfiniDyForestC.Intial_Up_Vector), -INfiniDyForestC.Intial_Up_Vector);
				if(!INfiniDyForestC.GridOnNormal){
					ray = new Ray (Start_pos + new Vector3(-MOD1,0,MOD1), -sign*Vector3.up);
				}
				if (Physics.Raycast (ray, out hit, INfiniDyForestC.max_ray_dist*2)) {
					//if (hit.collider.gameObject != Terrain.activeTerrain.gameObject & hit.collider.gameObject.tag != "PPaint") {
					if (INfiniDyForestC.PaintedOnOBJ != null && hit.collider.gameObject != INfiniDyForestC.PaintedOnOBJ.gameObject) {
						//return;
						scale_mod = 0.2f;
					}
				}
			
				//CHECK IF HITS - if not leave (or if terain stay)
				Start_pos = new Vector3 (find_x, 0, find_z);
				ray = new Ray (INfiniDyForestC.root_tree.transform.position+Quaternion.FromToRotation(Vector3.up,-INfiniDyForestC.Intial_Up_Vector) * Start_pos 
				                   + (INfiniDyForestC.max_ray_dist*INfiniDyForestC.Intial_Up_Vector), -INfiniDyForestC.Intial_Up_Vector);
				
				if(!INfiniDyForestC.GridOnNormal){
//					int sign = 1;
//					if(INfiniDyForestC.Intial_Up_Vector.y < 0){
//						sign = -1;
//					}
					Start_pos = new Vector3 (INfiniDyForestC.root_tree.transform.position.x + find_x, INfiniDyForestC.root_tree.transform.position.y+(sign*INfiniDyForestC.max_ray_dist), INfiniDyForestC.root_tree.transform.position.z + find_z);
					ray = new Ray (Start_pos, -sign*Vector3.up);
				}
				hit = new RaycastHit ();
				if (Physics.Raycast (ray, out hit, INfiniDyForestC.max_ray_dist*2)) {

					if(!INfiniDyForestC.GridOnNormal){
						if (INfiniDyForestC.PaintedOnOBJ != null && hit.collider.gameObject == INfiniDyForestC.PaintedOnOBJ.gameObject) {

						}else{
							return;
						}
					}else{
						if (INfiniDyForestC.PaintedOnOBJ != null && hit.collider.gameObject == INfiniDyForestC.PaintedOnOBJ.gameObject) {							

						}else{
							return;
						}
					}

				}else{
					if(Terrain.activeTerrain == null || (Terrain.activeTerrain != null && INfiniDyForestC.PaintedOnOBJ != Terrain.activeTerrain.gameObject) ){//v1.1
						return;
					}
				}
			
			}



			TreeHolder = new GameObject();
			
			//parent custom leaves there
			if(Level < 1){
				INfiniDyForestC.Leaf_pool.transform.parent = TreeHolder.transform;
			}		
			
			if(INfiniDyForestC !=null){
				
			}
			
			///////////// COLOR ///////
			if(is_root){		

				//v1.5
				if(INfiniDyForestC.BarkPrefab == ""){
					GameObject Branch = (GameObject)Object.Instantiate(INfiniDyForestC.BarkPrefabOBJ);
					Instances = Branch;	
				}else{
					GameObject Branch = (GameObject)Object.Instantiate(Resources.Load (INfiniDyForestC.BarkPrefab));
					Instances = Branch;	
				}
			}
			else{			
				//if(INfiniDyForestC.Brance_prefab_per_level){ //v1.2 - removed bool, check by list size
				if(INfiniDyForestC.BranchPrefabs.Count > 0){
					if(INfiniDyForestC.BranchPrefabs.Count > 1){
						if(INfiniDyForestC.BranchID_per_level != null){
							//if(INfiniDyForestC.BranchID_per_level.Count > 0 & INfiniDyForestC.BranchID_per_level.Count >= (Level+1)){
							if((INfiniDyForestC.BranchID_per_level.Count > 0) & (INfiniDyForestC.BranchID_per_level.Count >= (Level+1)) ){
								
								if(INfiniDyForestC.BranchID_per_level[Level] < INfiniDyForestC.BranchPrefabs.Count){
									GameObject Branch = (GameObject)Object.Instantiate(Resources.Load (INfiniDyForestC.BranchPrefabs[INfiniDyForestC.BranchID_per_level[Level]]));
									Instances = Branch;	
								}else{
									GameObject Branch = (GameObject)Object.Instantiate(Resources.Load (INfiniDyForestC.BranchPrefabs[0]));
									Instances = Branch;	
								}
								
							}else{
								GameObject Branch = (GameObject)Object.Instantiate(Resources.Load (INfiniDyForestC.BranchPrefabs[0]));
								Instances = Branch;	
							}
						}else{
							GameObject Branch = (GameObject)Object.Instantiate(Resources.Load (INfiniDyForestC.BranchPrefabs[0]));
							Instances = Branch;	
						}
					}else{
						GameObject Branch = (GameObject)Object.Instantiate(Resources.Load (INfiniDyForestC.BranchPrefabs[0]));
						Instances = Branch;	
					}
				}else{
					if(INfiniDyForestC.BranchPrefabsOBJ.Count > 1){
						if(INfiniDyForestC.BranchID_per_level != null){
							//if(INfiniDyForestC.BranchID_per_level.Count > 0 & INfiniDyForestC.BranchID_per_level.Count >= (Level+1)){
							if((INfiniDyForestC.BranchID_per_level.Count > 0) & (INfiniDyForestC.BranchID_per_level.Count >= (Level+1)) ){
								
								if(INfiniDyForestC.BranchID_per_level[Level] < INfiniDyForestC.BranchPrefabsOBJ.Count){
									GameObject Branch = (GameObject)Object.Instantiate(INfiniDyForestC.BranchPrefabsOBJ[INfiniDyForestC.BranchID_per_level[Level]]);
									Instances = Branch;	
								}else{
									GameObject Branch = (GameObject)Object.Instantiate(INfiniDyForestC.BranchPrefabsOBJ[0]);
									Instances = Branch;	
								}
								
							}else{
								GameObject Branch = (GameObject)Object.Instantiate(INfiniDyForestC.BranchPrefabsOBJ[0]);
								Instances = Branch;	
							}
						}else{
							GameObject Branch = (GameObject)Object.Instantiate(INfiniDyForestC.BranchPrefabsOBJ[0]);
							Instances = Branch;	
						}
					}else{
						GameObject Branch = (GameObject)Object.Instantiate(INfiniDyForestC.BranchPrefabsOBJ[0]);
						Instances = Branch;	
					}
				}
			}
			
			if(Level == 2){
				//Instances.renderer.material.color = Color.cyan;
			}
			if(Level == 3){
				//Instances.renderer.material.color = Color.magenta;
			}
			if(Level == 4){
				//Instances.renderer.material.color = Color.grey;
			}
			if(INfiniDyGrassParent == null){ 
				//TreeHolder.tag = "INfiniDyTreeRoot";
				INfiniDyForestC.root_tree = TreeHolder;
			}
			
			if(!INfiniDyForestC.is_grass | Level == 0){
				if(INfiniDyGrassParent != null){  
					TreeHolder.transform.parent = INfiniDyGrassParent.transform;
				}
				TreeHolder.transform.localEulerAngles = Vector3.zero;
				TreeHolder.transform.localPosition = Position;




			}else{
				if(INfiniDyGrassParent != null){  
					if(Level ==0){//parent everything to the first treeholder
						TreeHolder.transform.parent = INfiniDyGrassParent.transform;
					}else{
						TreeHolder.transform.parent = INfiniDyForestC.root_tree.transform;
					}
				}
				TreeHolder.transform.localEulerAngles = Vector3.zero;
				
//				float find_x = Random.Range(-7,7+Random.Range(-4,4));
//				float find_z = Random.Range(-7,7+Random.Range(-4,4));
				float Dist_Above_Terrain = 0.0f;
				float find_y = 0;
				if(Terrain.activeTerrain != null){
					find_y = -INfiniDyForestC.root_tree.transform.position.y+Terrain.activeTerrain.SampleHeight(INfiniDyForestC.root_tree.transform.position+new Vector3(find_x,0,find_z))+Dist_Above_Terrain+ Terrain.activeTerrain.transform.position.y;
				}

				//FIND WITHOUT TERRAIN

				Vector3 Start_pos = new Vector3 (find_x, 0, find_z);
				Ray ray = new Ray (INfiniDyForestC.root_tree.transform.position+Quaternion.FromToRotation(Vector3.up,-INfiniDyForestC.Intial_Up_Vector) * Start_pos 
				                   + (INfiniDyForestC.max_ray_dist*INfiniDyForestC.Intial_Up_Vector), -INfiniDyForestC.Intial_Up_Vector);

				if(!INfiniDyForestC.GridOnNormal){
					int sign = 1;
					if(INfiniDyForestC.Intial_Up_Vector.y < 0){
						sign = -1;
					}
					Start_pos = new Vector3 (INfiniDyForestC.root_tree.transform.position.x + find_x, INfiniDyForestC.root_tree.transform.position.y+(sign*INfiniDyForestC.max_ray_dist), INfiniDyForestC.root_tree.transform.position.z + find_z);
					ray = new Ray (Start_pos, -sign*Vector3.up);
				}

				RaycastHit hit = new RaycastHit ();
				if (Physics.Raycast (ray, out hit, INfiniDyForestC.max_ray_dist*2)) {

					//Debug.DrawRay(INfiniDyForestC.root_tree.transform.position+Quaternion.FromToRotation(Vector3.up,-INfiniDyForestC.Intial_Up_Vector) * Start_pos 
					//              + (INfiniDyForestC.max_ray_dist*INfiniDyForestC.Intial_Up_Vector), -INfiniDyForestC.Intial_Up_Vector);

					if(!INfiniDyForestC.GridOnNormal){
						if (INfiniDyForestC.PaintedOnOBJ != null && hit.collider.gameObject == INfiniDyForestC.PaintedOnOBJ.gameObject) {
							find_y = -INfiniDyForestC.root_tree.transform.position.y + hit.point.y;
							TreeHolder.transform.localPosition = new Vector3(0+find_x,find_y,0+find_z);
						}
					}else{
						if (INfiniDyForestC.PaintedOnOBJ != null && hit.collider.gameObject == INfiniDyForestC.PaintedOnOBJ.gameObject) {

							TreeHolder.transform.position = hit.point;
						}
					}
				}else{


				
				TreeHolder.transform.localPosition = new Vector3(0+find_x,find_y,0+find_z);//infinigrass 1+find_z -> 0+find_z
				}


			}
			
			Instances.transform.parent = TreeHolder.transform;
			Instances.transform.localPosition = Vector3.zero;
			Instances.transform.localEulerAngles= Vector3.zero;
			
			float RANGE_X=Random.Range (-INfiniDyForestC.Min_max_spread.x,INfiniDyForestC.Min_max_spread.y);
			float RANGE_Y=Random.Range (-INfiniDyForestC.Min_max_spread.x,INfiniDyForestC.Min_max_spread.y);
			float RANGE_YY=0;
			if(INfiniDyForestC.Spread_Z_separate){
				RANGE_Y=Random.Range (-INfiniDyForestC.Min_max_spread_Z.x,INfiniDyForestC.Min_max_spread_Z.y);
			}
			if(INfiniDyForestC.Spread_Y_separate){
				RANGE_YY=Random.Range (-INfiniDyForestC.Min_max_spread_Y.x,INfiniDyForestC.Min_max_spread_Y.y);
			}
			
			if(Level > 1){
				if(!INfiniDyForestC.Reduce_angle_with_height){
					if(!INfiniDyForestC.Grow_angles){
						TreeHolder.transform.Rotate(RANGE_X,0,RANGE_Y);
					}else{
						TreeHolder.transform.Rotate(RANGE_X/INfiniDyForestC.Growth_angle_div,RANGE_YY,RANGE_Y/INfiniDyForestC.Growth_angle_div);
					}
				}else{
					if(!INfiniDyForestC.Grow_angles){
						TreeHolder.transform.Rotate(RANGE_X*(1-INfiniDyForestC.Height_reduce*Height_Level),RANGE_YY,RANGE_Y*(1-INfiniDyForestC.Height_reduce*Height_Level));
					}else{
						TreeHolder.transform.Rotate(RANGE_X*(1-INfiniDyForestC.Height_reduce*Height_Level)/INfiniDyForestC.Growth_angle_div,RANGE_YY,RANGE_Y*(1-INfiniDyForestC.Height_reduce*Height_Level)/INfiniDyForestC.Growth_angle_div);
					}
				}
			}
			TreeHolder.name = "TreeHolder";
			Instances.name = "Instances";
			Instances.transform.Translate(0,INfiniDyForestC.Length_scale*0.5f*len,0);
			Vector3 scaler = new Vector3(radi,len,radi);
			Instances.transform.localScale = (scaler/1f)*scale_mod; //INFINIGRASS
			
			if(Level > 4){
				
			}
			
			//v1.1 - changed 1 to 0 in check
			if(Level >= 0){//InfiniGRASS
				INfiniDyForestC.Registered_Brances.Add(Instances.transform);
				INfiniDyForestC.Branch_grew.Add(0);
				INfiniDyForestC.Start_Scales.Add(Instances.transform.localScale);
				INfiniDyForestC.scaleVectors.Add(scaler);
				
				INfiniDyForestC.Branch_levels.Add(Level);
			}
			
			if(Height_Level > 0){
			}
			
			makeBranches();
			current_time = Time.fixedTime;//mark time to start scale growth
			Leaves.Add(this);
			Instances.transform.Translate(0,-INfiniDyForestC.Length_scale*0.5f*len,0);//v1.1


		}	
		
		
	}
	
	
}

