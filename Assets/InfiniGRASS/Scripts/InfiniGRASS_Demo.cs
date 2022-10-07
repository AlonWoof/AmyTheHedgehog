using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Artngame.INfiniDy {

	public class InfiniGRASS_Demo : MonoBehaviour {

	#pragma warning disable 414 

		public GameObject Water1;

	void Start () {

			//AUDIO
			Audio.enabled = true;
			Audio.volume = 0;

		
	}


	//v1.7


		public GameObject Freeze_POOL;
		public GameObject Freezer;

		public GameObject Rain1;
		public GameObject Rain2;

		public GameObject Bats;
		public GameObject Leaves;

		public GameObject Floor;
		public GameObject Floor_stripes;
		public GameObject Floor_collider;


	public float Sun_time_start = 14.43f;	//at this time, rotation of sunsystem must be 62 (14, -1.525879e-05, -1.525879e-05 WORKS !!)

	
	public GameObject TREES;

	public bool HUD_ON=true;

	

	public bool enable_controls=false;
	//public bool enable_hud=true;

	
		public GameObject TerrainObj;
		public GameObject HorizonObj;
		public GameObject MeshTerrainObj;//enable in all cases besides Terrain
		public GameObject Water;
		public GameObject CloudHanlder;

		float Camera_up;





		public float Grass_GUI_startX = 510;
		public float Grass_GUI_startY = 21;
		bool SnowToggle = false;
		public List<Texture2D> IconsGrass = new List<Texture2D>();
		public List<Texture2D> IconsRocks = new List<Texture2D>();
		public List<Texture2D> IconsFence = new List<Texture2D>();
		public InfiniGRASSManager GrassManager;
		public ObjectThrowerInfiniGRASS Thrower;

		//public Material terrainMat;
		public AudioSource Audio;

	void OnGUI() {


			//CAMERA 

			//AUDIO
			if (GrassManager.AmplifyWind > 0.1f) {
			
				Audio.volume = Mathf.Lerp(Audio.volume, GrassManager.AmplifyWind/8, Time.deltaTime);//up to 0.5
			
			}
			






			float widthS = Screen.currentResolution.width;
			
			widthS = Camera.main.pixelWidth;

			Grass_GUI_startX = widthS - 400;
			/////////
			/// 

			//SNOW grass------------------------------------------------------------ ROCKS
			
			//ICONS to choose from
			if (GUI.Button (new Rect (Grass_GUI_startX + 90 +90+90, 0, 80, 25), "Paint rocks")) {	
				if(GrassManager.Rock_painting){
					GrassManager.Rock_painting = false;
				}else{
					GrassManager.Rock_painting = true;
					GrassManager.Fence_painting = false;
					GrassManager.Grass_painting = false;
				}
				GrassManager.Grass_selector = 0;
			}
			if (GrassManager.Rock_painting) {
				//display icons
				for (int i=0; i<IconsRocks.Count; i++) {
					if (GUI.Button (new Rect (Grass_GUI_startX + 60 * i, Grass_GUI_startY + 5, 60, 60), IconsRocks [i])) {	
						GrassManager.Grass_selector = i;
					}
				}
				//Scale
				GUI.TextArea( new Rect (Grass_GUI_startX , Grass_GUI_startY + 60 +5 +30+40+30 +30, 110, 22),"Min distance:" + GrassManager.min_grass_patch_dist.ToString("F1"));
				GrassManager.min_grass_patch_dist = GUI.HorizontalSlider(new Rect (Grass_GUI_startX +0, Grass_GUI_startY + 60 +5 +30+40+30 +30+30, 110, 25),GrassManager.min_grass_patch_dist,2f,8f);
				
				//Paint dist
				GUI.TextArea( new Rect (Grass_GUI_startX , Grass_GUI_startY + 60 +30, 110, 40),"Rock scale (Min:" + GrassManager.min_scale.ToString("F1")+"-Max:" + GrassManager.max_scale.ToString("F1")+")");				
				GrassManager.min_scale = GUI.HorizontalSlider(new Rect (Grass_GUI_startX + 0, Grass_GUI_startY + 60 +5 +30+40, 110, 25),GrassManager.min_scale,0.3f,1.5f);
				GrassManager.max_scale = GUI.HorizontalSlider(new Rect (Grass_GUI_startX + 0, Grass_GUI_startY + 60 +5 +30+40+30, 110, 25),GrassManager.max_scale,0.3f,1.5f);

				
			}



			//SNOW grass------------------------------------------------------------ FENCE
			
			//ICONS to choose from
			if (GUI.Button (new Rect (Grass_GUI_startX + 90 +90, 0, 80, 25), "Paint fence")) {	
				if(GrassManager.Fence_painting){
					GrassManager.Fence_painting = false;
				}else{
					GrassManager.Fence_painting = true;
					GrassManager.Rock_painting = false;
					GrassManager.Grass_painting = false;
				}
				GrassManager.Grass_selector = 0;
			}
			if (GrassManager.Fence_painting) {
				//display icons
				for (int i=0; i<IconsFence.Count; i++) {
					if (GUI.Button (new Rect (Grass_GUI_startX + 60 * i, Grass_GUI_startY + 5, 60, 60), IconsFence [i])) {	
						GrassManager.Grass_selector = i;
					}
				}
				//Scale
				GUI.TextArea( new Rect (Grass_GUI_startX , Grass_GUI_startY + 60 +5 +30+40+30 +30, 110, 22),"Fence Scale:" + GrassManager.fence_scale.ToString("F1"));
				GrassManager.fence_scale = GUI.HorizontalSlider(new Rect (Grass_GUI_startX +0, Grass_GUI_startY + 60 +5 +30+40+30 +30+30, 110, 25),GrassManager.fence_scale,1f,4f);

				//Paint dist
				GrassManager.min_grass_patch_dist = 2;

			}


			//SNOW grass------------------------------------------------------------ GRASS

			//ICONS to choose from
			if (GUI.Button (new Rect (Grass_GUI_startX + 90, 0, 80, 25), "Paint grass")) {	
				if(GrassManager.Grass_painting){
					GrassManager.Grass_painting = false;
				}else{
					GrassManager.Grass_painting = true;
					GrassManager.Rock_painting = false;
					GrassManager.Fence_painting = false;
				}
				GrassManager.Grass_selector = 0;
			}
			if(GrassManager.Grass_painting){


				GrassManager.GridOnNormal = true;

				//display icons
				for(int i=0;i<IconsGrass.Count;i++){
					if (GUI.Button (new Rect (Grass_GUI_startX + 60*i - 230, Grass_GUI_startY+5, 60, 60), IconsGrass[i])) {	
						GrassManager.Grass_selector = i;

						//Grass
						if(i==0){
							GrassManager.Min_density = 1;
							GrassManager.Max_density = 5;
							GrassManager.SpecularPower = 4;
							GrassManager.Min_spread = 7;
							GrassManager.Max_spread = 9;
							GrassManager.min_scale = 0.4f;
							GrassManager.max_scale = 0.6f;
						}
						//Vertex grass
						if(i==1){
							GrassManager.min_scale = 0.4f;
							GrassManager.max_scale = 0.8f;
							GrassManager.Min_density = 2.0f;
							GrassManager.Max_density = 3.0f;
							GrassManager.Min_spread = 7;
							GrassManager.Max_spread = 9;
							GrassManager.SpecularPower = 4;
						}
						//Red flowers
						if(i==2){
							GrassManager.min_scale = 0.8f;
							GrassManager.max_scale = 0.9f;
							GrassManager.Min_density = 1.0f;
							GrassManager.Max_density = 1.0f;
							GrassManager.Min_spread = 7;
							GrassManager.Max_spread = 10;
							GrassManager.SpecularPower = 4;
						}
						//Wheet
						if(i==3){
							GrassManager.min_scale = 1.0f;
							GrassManager.max_scale = 1.5f;
							GrassManager.Min_density = 1.0f;
							GrassManager.Max_density = 1.0f;
							GrassManager.Min_spread = 15;
							GrassManager.Max_spread = 20;
							GrassManager.SpecularPower = 4;
						}
						//Detailed vertex
						if(i==4){
							GrassManager.min_scale = 1.0f;
							GrassManager.max_scale = 1.2f;
							GrassManager.Min_density = 1.0f;
							GrassManager.Max_density = 3.0f;
							GrassManager.Min_spread = 7;
							GrassManager.Max_spread = 10;
							GrassManager.SpecularPower = 4;
						}

						//Simple vertex
						if(i==5){
							GrassManager.min_scale = 0.5f;
							GrassManager.max_scale = 1.0f;
							GrassManager.Min_density = 2.0f;
							GrassManager.Max_density = 3.0f;
							GrassManager.Min_spread = 7;
							GrassManager.Max_spread = 10;
							GrassManager.SpecularPower = 4;
						}
						//White flowers
						if(i==6){
							GrassManager.min_scale = 0.6f;
							GrassManager.max_scale = 0.9f;
							GrassManager.Min_density = 1.0f;
							GrassManager.Max_density = 1.0f;
							GrassManager.Min_spread = 7;
							GrassManager.Max_spread = 10;
							GrassManager.SpecularPower = 4;
						}
						//Curved grass
						if(i==7){
							GrassManager.min_scale = 0.5f;
							GrassManager.max_scale = 1.5f;
							GrassManager.Min_density = 1.0f;
							GrassManager.Max_density = 5.0f;
							GrassManager.Min_spread = 7;
							GrassManager.Max_spread = 8;
							GrassManager.SpecularPower = 4;
						}
						//Low grass - FOR LIGHT DEMO without Sky Master and real time use
						if(i==8){
							GrassManager.min_scale = 0.8f;
							GrassManager.max_scale = 1.0f;
							GrassManager.Min_density = 2.0f;
							GrassManager.Max_density = 3.0f;
							GrassManager.Min_spread = 7;
							GrassManager.Max_spread = 10;
							GrassManager.SpecularPower = 4;
						}
						//Vines
						if(i==9){
							GrassManager.min_scale = 1.5f;
							GrassManager.max_scale = 1.5f;
							GrassManager.Min_density = 4.0f;
							GrassManager.Max_density = 4.0f;
							GrassManager.Min_spread = 7;
							GrassManager.Max_spread = 7;
							GrassManager.SpecularPower = 4;
						}






					}
				} 

				string Grass_inter = "Interactive On";
				if(!GrassManager.Interactive){
					Grass_inter = "Interactive Off";
				}
				if (GUI.Button (new Rect (Grass_GUI_startX , Grass_GUI_startY + 60 +5, 110, 25), Grass_inter)) {	
					if(!GrassManager.Interactive){
						GrassManager.Interactive = true;
					}else{
						GrassManager.Interactive = false;
					}
				}

				GUI.TextArea( new Rect (Grass_GUI_startX , Grass_GUI_startY + 60 +30, 110, 40),"Grass scale (Min:" + GrassManager.min_scale.ToString("F1")+"-Max:" + GrassManager.max_scale.ToString("F1")+")");				
				GrassManager.min_scale = GUI.HorizontalSlider(new Rect (Grass_GUI_startX + 0, Grass_GUI_startY + 60 +5 +30+40, 110, 25),GrassManager.min_scale,0.5f,1.5f);
				GrassManager.max_scale = GUI.HorizontalSlider(new Rect (Grass_GUI_startX + 0, Grass_GUI_startY + 60 +5 +30+40+30, 110, 25),GrassManager.max_scale,0.5f,1.5f);

				GrassManager.Override_density = true;
				GrassManager.Override_spread = true;
				GUI.TextArea( 								  new Rect (Grass_GUI_startX + 125, Grass_GUI_startY + 60 +30, 110, 40),"Grass density (Min:" + GrassManager.Min_density.ToString("F1")+"-Max:" + GrassManager.Max_density.ToString("F1")+")");				
				GrassManager.Min_density = GUI.HorizontalSlider(new Rect (Grass_GUI_startX + 125, Grass_GUI_startY + 60 +5 +30+40, 110, 25),GrassManager.Min_density,1f,4f);
				GrassManager.Max_density = GUI.HorizontalSlider(new Rect (Grass_GUI_startX + 125, Grass_GUI_startY + 60 +5 +30+40+30, 110, 25),GrassManager.Max_density,GrassManager.Min_density,5f);
				GUI.TextArea( 								  new Rect (Grass_GUI_startX + 125+ 125, Grass_GUI_startY + 60 +30, 110, 40),"Grass spread (Min:" + GrassManager.Min_spread.ToString("F1")+"-Max:" + GrassManager.Max_spread.ToString("F1")+")");				
				GrassManager.Min_spread = GUI.HorizontalSlider(new Rect (Grass_GUI_startX + 125+ 125, Grass_GUI_startY + 60 +5 +30+40, 110, 25),GrassManager.Min_spread,7f,49f);
				GrassManager.Max_spread = GUI.HorizontalSlider(new Rect (Grass_GUI_startX + 125+ 125, Grass_GUI_startY + 60 +5 +30+40+30, 110, 25),GrassManager.Max_spread,GrassManager.Min_spread,50f);
			
				//WIND
				GUI.TextArea( new Rect (Grass_GUI_startX , Grass_GUI_startY + 60 +5 +30+40+30 +30, 110, 22),"Grass wind:" + GrassManager.AmplifyWind.ToString("F1"));
				GrassManager.AmplifyWind = GUI.HorizontalSlider(new Rect (Grass_GUI_startX +0, Grass_GUI_startY + 60 +5 +30+40+30 +30+30, 110, 25),GrassManager.AmplifyWind,0f,4f);

				GUI.TextArea( new Rect (Grass_GUI_startX+125 , Grass_GUI_startY + 60 +5 +30+40+30 +30, 110, 22),"Turbulence" + GrassManager.WindTurbulence.ToString("F1"));
				GrassManager.WindTurbulence = GUI.HorizontalSlider(new Rect (Grass_GUI_startX +125, Grass_GUI_startY + 60 +5 +30+40+30 +30+30, 110, 25),GrassManager.WindTurbulence,0f,2.2f);

				//FADE			
				GUI.TextArea( new Rect (Grass_GUI_startX+125+125 , Grass_GUI_startY + 60 +5 +30+40+30 +30, 110, 22),"Grass fade:" + GrassManager.Grass_Fade_distance.ToString("F1"));
				GrassManager.Grass_Fade_distance = GUI.HorizontalSlider(new Rect (Grass_GUI_startX +125+125, Grass_GUI_startY + 60 +5 +30+40+30 +30+30, 110, 25),GrassManager.Grass_Fade_distance,120f,2000f);


				//Specular
				GUI.TextArea( new Rect (Grass_GUI_startX , Grass_GUI_startY + 60 +5 +30+40+30 +30+50, 110, 22),"Grass spec:" + GrassManager.SpecularPower.ToString("F1"));
				GrassManager.SpecularPower = GUI.HorizontalSlider(new Rect (Grass_GUI_startX +0, Grass_GUI_startY + 60 +5 +30+40+30 +30+30+50, 110, 25),GrassManager.SpecularPower,0f,4f);

				//_Shininess
				//if(terrainMat.HasProperty("_Shininess")){
					//GUI.TextArea( new Rect (Grass_GUI_startX+125 , Grass_GUI_startY + 60 +5 +30+40+30 +30+50, 110, 22),"Terrain wet:" + terrainMat.GetFloat("_Shininess").ToString("F1"));
					//float shine = GUI.HorizontalSlider(new Rect (Grass_GUI_startX +125, Grass_GUI_startY + 60 +5 +30+40+30 +30+30+50, 110, 25),terrainMat.GetFloat("_Shininess"),0f,2.2f);
					//terrainMat.SetFloat("_Shininess",shine);
				//}
				//FADE			
				//GUI.TextArea( new Rect (Grass_GUI_startX+125+125 , Grass_GUI_startY + 60 +5 +30+40+30 +30+50, 110, 22),"Grass fade:" + GrassManager.Grass_Fade_distance.ToString("F1"));
				//GrassManager.Grass_Fade_distance = GUI.HorizontalSlider(new Rect (Grass_GUI_startX +125+125, Grass_GUI_startY + 60 +5 +30+40+30 +30+30+50, 110, 25),GrassManager.Grass_Fade_distance,120f,2000f);

			}

				string Toggle_action = "Cast Balls On";
				if(!Thrower.enabled){
					Toggle_action = "Cast Balls Off";
				}
				if (GUI.Button (new Rect (Grass_GUI_startX+125+125 , Grass_GUI_startY + 60 +5 +30+40+30 +30+40+9, 110, 22), Toggle_action)) {					
					if(Thrower.enabled){
						Thrower.enabled = false;
						GrassManager.Grass_painting = true;
					}else{
						Thrower.enabled = true;
						GrassManager.Grass_painting = false;
					}					
				}




		

			////////////////








  }// END OnGUI	
}
}