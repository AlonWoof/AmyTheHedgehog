using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Artngame.INfiniDy {

	public class InfiniGRASS_DemoRealTime : MonoBehaviour {

	#pragma warning disable 414 

		public GameObject Water1;
		public GameObject GUI_text;

		public bool OnlySunWind = false;
	

	void Start () {

			//AUDIO
			Audio.enabled = true;
			Audio.volume = 0;
			if (SUN != null) {
				SUN_LIGHT = SUN.GetComponent<Light> ();
			}
			//v1.1 - set LODs explicitly per brush
			//GrassManager.Cutoff_distance = 530;
			//GrassManager.LOD_distance = 520;
			//GrassManager.LOD_distance1 = 523;
			///GrassManager.LOD_distance2 = 527;

			GrassManager.WindTurbulence = 0.2f;
			GrassManager.AmplifyWind = 0.7f;

			GrassManager.Min_density = 1;
			GrassManager.Max_density = 4;
			//GrassManager.SpecularPower = 4;
			GrassManager.Min_spread = 7;
			GrassManager.Max_spread = 9;
			GrassManager.min_scale = 0.4f;
			GrassManager.max_scale = 0.8f;
			
			GrassManager.Cutoff_distance = 530;
			GrassManager.LOD_distance = 520;
			GrassManager.LOD_distance1 = 523;
			GrassManager.LOD_distance2 = 527;
			
			GrassManager.RandomRot = false;

			QualitySettings.shadowCascades = 2;
			

			
		}


	//v1.7
		public Transform SUN;
		public Light SUN_LIGHT;

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

		public float Sun_rotX;
		public float Sun_rotY;
		public Color Sun_col;

		public float Wind_rotY;
		public Transform Wind;

		bool ImageFX_active = false;
		bool Toggle_GUI = true;

		int quality=1;

	void OnGUI() {


			string Toggle_GUIs= "GUI Off";
			if(Toggle_GUI){
				Toggle_GUIs = "GUI On";
			}
			if (GUI.Button (new Rect (10 , 10, 110, 22), Toggle_GUIs)) {	
				if(Toggle_GUI){
					Toggle_GUI = false;
					if(GUI_text != null && GUI_text.activeInHierarchy){
						GUI_text.SetActive(false);
					}
				}else{
					Toggle_GUI = true;
					if(GUI_text != null && !GUI_text.activeInHierarchy){
						GUI_text.SetActive(true);
					}
				}
			}

			//CAMERA 
		if (Toggle_GUI) {
				//AUDIO
				if (GrassManager.AmplifyWind > 0.1f) {
			
					Audio.volume = Mathf.Lerp (Audio.volume, GrassManager.AmplifyWind / 8, Time.deltaTime);//up to 0.5
			
				}

				if (GrassManager.windzone != null && Wind == null) {
			
					Wind = GrassManager.windzone.transform;
				}

				//if (SUN != null && SUN_LIGHT == null) {
				//	SUN_LIGHT = SUN.GetComponent<Light>();
				//}

				//if (SUN != null && SUN_LIGHT != null) {
				float dispX = -100;
				float BASE_X = 20;
				float BASE1 = 20 + 100;

				if (SUN_LIGHT != null) {
					GUI.TextArea (new Rect (BASE_X + 10, BASE1 + (3.5f * 40) - 20 + 70, 180, 20), "Sun Tint (RGB)");
					Sun_col.r = GUI.HorizontalSlider (new Rect (BASE_X + 10, BASE1 + (3.5f * 40) + 70, 100, 30), SUN_LIGHT.color.r, 0.5f, 1);
					//TintColor.x = SUNMASTER.m_TintColor.r;
					Sun_col.g = GUI.HorizontalSlider (new Rect (BASE_X + 10, BASE1 + (4.3f * 40) + 70, 100, 30), SUN_LIGHT.color.g, 0.5f, 1);
					//TintColor.y = SUNMASTER.m_TintColor.g;
					Sun_col.b = GUI.HorizontalSlider (new Rect (BASE_X + 10, BASE1 + (5.1f * 40) + 70, 100, 30), SUN_LIGHT.color.b, 0.5f, 1);
					//TintColor.z = SUNMASTER.m_TintColor.b;
					SUN_LIGHT.color = Sun_col;
				}

				float BOX_WIDTH = 100;
				float BOX_HEIGHT = 30;
				if (SUN_LIGHT != null) {
					GUI.TextArea (new Rect (1 * BOX_WIDTH + 10 + dispX, BOX_HEIGHT + 30, BOX_WIDTH + 0, 25), "SkyDome rot");
					Sun_rotX = GUI.HorizontalSlider (new Rect (1 * BOX_WIDTH + 10 + dispX, BOX_HEIGHT + 30 + 30, BOX_WIDTH + 0, 30), SUN.eulerAngles.x, 5, 80);
					SUN.eulerAngles = new Vector3 (Sun_rotX, SUN.eulerAngles.y, SUN.eulerAngles.z);
					Sun_rotY = GUI.HorizontalSlider (new Rect (1 * BOX_WIDTH + 10 + dispX, BOX_HEIGHT + 30 + 30 + 30, BOX_WIDTH + 0, 30), SUN.eulerAngles.y, -179, 179);
					SUN.eulerAngles = new Vector3 (SUN.eulerAngles.x, Sun_rotY, SUN.eulerAngles.z);


					GUI.TextArea (new Rect (1 * BOX_WIDTH + 10 + dispX, BOX_HEIGHT + 30 + 30 + 30 + 30, BOX_WIDTH + 0, 25), "Sun intensity");
					SUN_LIGHT.intensity = GUI.HorizontalSlider (new Rect (1 * BOX_WIDTH + 10 + dispX, BOX_HEIGHT + 30 + 30 + 30 + 30 + 30, BOX_WIDTH + 0, 30), SUN_LIGHT.intensity, 0.1f, 5);
				}
				
				if (Wind != null) {
					GUI.TextArea (new Rect (1 * BOX_WIDTH + 10 + dispX, BOX_HEIGHT + 30 + 30 + 30 + 50 + 30, BOX_WIDTH + 0, 25), "Wind rot");
					Wind_rotY = GUI.HorizontalSlider (new Rect (1 * BOX_WIDTH + 10 + dispX, BOX_HEIGHT + 30 + 30 + 30 + 50 + 30 + 30, BOX_WIDTH + 0, 30), Wind.eulerAngles.y, -360, 360);
					Wind.eulerAngles = new Vector3 (Wind.eulerAngles.x, Wind_rotY, Wind.eulerAngles.z);
				}
			

				if (!OnlySunWind) {


					if (GUI.Button (new Rect (1 * BOX_WIDTH + 10 + dispX, BOX_HEIGHT + 30 + 30 + 30 + 50 + 30 + 30 + 30, BOX_WIDTH + 50, 30), "Quality:" + quality)) {	
						if (quality == 2) {
							//Camera.main.GetComponent<ScreenSpaceAmbientOcclusion>().enabled = true;

						
							QualitySettings.shadowCascades = 0;
						

							quality = 0;

						} else if (quality == 1) {
							//Camera.main.GetComponent<ScreenSpaceAmbientOcclusion>().enabled = false;

						
							QualitySettings.shadowCascades = 4;					

							quality = 2;

						} else if (quality == 0) {
							//Camera.main.GetComponent<ScreenSpaceAmbientOcclusion>().enabled = false;

						
							QualitySettings.shadowCascades = 2;
						


							quality = 1;
						}
					}


					//}



					float widthS = Screen.currentResolution.width;
			
					if(Camera.main != null){
						widthS = Camera.main.pixelWidth;
					}else{
						if(Camera.current != null){
							widthS = Camera.current.pixelWidth;
						}
					}

					Grass_GUI_startX = widthS - 400;
					/////////
					/// 

					//SNOW grass------------------------------------------------------------ ROCKS
			
					//ICONS to choose from
					if (GUI.Button (new Rect (Grass_GUI_startX + 90 + 90 + 90, 0, 80, 25), "Paint rocks")) {	
						if (GrassManager.Rock_painting) {
							GrassManager.Rock_painting = false;
						} else {
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
						GUI.TextArea (new Rect (Grass_GUI_startX, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30, 110, 22), "Min distance:" + GrassManager.min_grass_patch_dist.ToString ("F1"));
						GrassManager.min_grass_patch_dist = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30, 110, 25), GrassManager.min_grass_patch_dist, 2f, 8f);
				
						//Paint dist
						GUI.TextArea (new Rect (Grass_GUI_startX, Grass_GUI_startY + 60 + 30, 110, 40), "Rock scale (Min:" + GrassManager.min_scale.ToString ("F1") + "-Max:" + GrassManager.max_scale.ToString ("F1") + ")");				
						GrassManager.min_scale = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40, 110, 25), GrassManager.min_scale, 0.05f, 1.5f);
						GrassManager.max_scale = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30, 110, 25), GrassManager.max_scale, 0.1f, 1.5f);

				
					}



					//SNOW grass------------------------------------------------------------ FENCE
			
					//ICONS to choose from
					if (GUI.Button (new Rect (Grass_GUI_startX + 90 + 90, 0, 80, 25), "Paint fence")) {	
						if (GrassManager.Fence_painting) {
							GrassManager.Fence_painting = false;
						} else {
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
						GUI.TextArea (new Rect (Grass_GUI_startX, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30, 110, 22), "Fence Scale:" + GrassManager.fence_scale.ToString ("F1"));
						GrassManager.fence_scale = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30, 110, 25), GrassManager.fence_scale, 1f, 4f);

						//Paint dist
						GrassManager.min_grass_patch_dist = 2;

					}


					//SNOW grass------------------------------------------------------------ GRASS

					//ICONS to choose from
					if (GUI.Button (new Rect (Grass_GUI_startX + 90, 0, 80, 25), "Paint grass")) {	
						if (GrassManager.Grass_painting) {
							GrassManager.Grass_painting = false;
						} else {
							GrassManager.Grass_painting = true;
							GrassManager.Rock_painting = false;
							GrassManager.Fence_painting = false;
						}
						GrassManager.Grass_selector = 0;
					}
					if (GrassManager.Grass_painting) {


						GrassManager.GridOnNormal = true;

						//display icons
						for (int i=0; i<IconsGrass.Count; i++) {
							if (GUI.Button (new Rect (Grass_GUI_startX + 60 * i - 550, Grass_GUI_startY + 5, 60, 60), IconsGrass [i])) {	
								GrassManager.Grass_selector = i;

								//Grass
								if (i == 0) {
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
								}
								//Vertex grass
								if (i == 1) {
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
								if (i == 2) {
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
								if (i == 3) {
									GrassManager.min_scale = 1.0f;
									GrassManager.max_scale = 1.5f;
									GrassManager.Min_density = 1.0f;
									GrassManager.Max_density = 1.0f;
									GrassManager.Min_spread = 15;
									GrassManager.Max_spread = 20;
									//GrassManager.SpecularPower = 4;

									GrassManager.Cutoff_distance = 530;
									GrassManager.LOD_distance = 520;
									GrassManager.LOD_distance1 = 523;
									GrassManager.LOD_distance2 = 527;
								
									GrassManager.RandomRot = false;
								}
								//Detailed vertex
								if (i == 4) {
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
								if (i == 5) {
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
								if (i == 6) {
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
								if (i == 7) {
									GrassManager.min_scale = 0.5f;
									GrassManager.max_scale = 1.5f;
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
								if (i == 8) {
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
								if (i == 9) {
									GrassManager.min_scale = 1.5f;
									GrassManager.max_scale = 1.5f;
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
								if (i == 10 | i == 11) {
									GrassManager.min_scale = 0.4f;
									GrassManager.max_scale = 1.0f;
									GrassManager.Min_density = 1.0f;
									GrassManager.Max_density = 4.0f;
									GrassManager.Min_spread = 7;
									GrassManager.Max_spread = 9;
									//GrassManager.SpecularPower = 4;
								
									GrassManager.Cutoff_distance = 530;
									GrassManager.LOD_distance = 80;
									GrassManager.LOD_distance1 = 120;
									GrassManager.LOD_distance2 = 520;
								
									GrassManager.RandomRot = false;
								}
								//Ground leaves
								if (i == 12) {
									GrassManager.min_scale = 0.5f;
									GrassManager.max_scale = 0.8f;
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
								if (i == 13) {
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
								if (i == 14) {
									GrassManager.min_scale = 0.7f;
									GrassManager.max_scale = 1.2f;
									GrassManager.Min_density = 1.0f;
									GrassManager.Max_density = 3.0f;
									GrassManager.Min_spread = 7;
									GrassManager.Max_spread = 11;
									//GrassManager.SpecularPower = 4;
								
//								GrassManager.Cutoff_distance = 520;
//								GrassManager.LOD_distance = 110;
//								GrassManager.LOD_distance1 = 150;
//								GrassManager.LOD_distance2 = 510;

									GrassManager.Cutoff_distance = 520;
									GrassManager.LOD_distance = 220;
									GrassManager.LOD_distance1 = 270;
									GrassManager.LOD_distance2 = 410;
								
									GrassManager.RandomRot = false;
								}


							}
						} 

						string Grass_inter = "Interactive On";
						if (!GrassManager.Interactive) {
							Grass_inter = "Interactive Off";
						}
						if (GUI.Button (new Rect (Grass_GUI_startX - 110, Grass_GUI_startY + 60 + 5 + 25, 110, 25), Grass_inter)) {	
							if (!GrassManager.Interactive) {
								GrassManager.Interactive = true;
							} else {
								GrassManager.Interactive = false;
							}
						}

						string Grass_rot = "Random rot On";
						if (!GrassManager.RandomRot) {
							Grass_rot = "Random rot Off";
						}
						if (GUI.Button (new Rect (Grass_GUI_startX - 110, Grass_GUI_startY + 60 + 5 + 25 + 25, 110, 25), Grass_rot)) {	
							if (!GrassManager.RandomRot) {
								GrassManager.RandomRot = true;
							} else {
								GrassManager.RandomRot = false;
							}
						}

						//GrassManager.Interactive = true;

						GUI.TextArea (new Rect (Grass_GUI_startX, Grass_GUI_startY + 60 + 30, 110, 40), "Grass scale (Min:" + GrassManager.min_scale.ToString ("F1") + "-Max:" + GrassManager.max_scale.ToString ("F1") + ")");				
						GrassManager.min_scale = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40, 110, 25), GrassManager.min_scale, 0.1f, 2f);
						GrassManager.max_scale = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30, 110, 25), GrassManager.max_scale, 0.5f, 3f);

						GrassManager.Override_density = true;
						GrassManager.Override_spread = true;
						GUI.TextArea (new Rect (Grass_GUI_startX + 125, Grass_GUI_startY + 60 + 30, 110, 40), "Grass density (Min:" + GrassManager.Min_density.ToString ("F1") + "-Max:" + GrassManager.Max_density.ToString ("F1") + ")");				
						GrassManager.Min_density = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 125, Grass_GUI_startY + 60 + 5 + 30 + 40, 110, 25), GrassManager.Min_density, 1f, 3f);
						GrassManager.Max_density = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30, 110, 25), GrassManager.Max_density, GrassManager.Min_density, 3f);
						GUI.TextArea (new Rect (Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 30, 110, 40), "Grass spread (Min:" + GrassManager.Min_spread.ToString ("F1") + "-Max:" + GrassManager.Max_spread.ToString ("F1") + ")");				
						GrassManager.Min_spread = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40, 110, 25), GrassManager.Min_spread, 5f, 35f);
						GrassManager.Max_spread = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30, 110, 25), GrassManager.Max_spread, GrassManager.Min_spread, 45f);
//			
						//WIND
						GUI.TextArea (new Rect (Grass_GUI_startX, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30, 110, 22), "Grass wind:" + GrassManager.AmplifyWind.ToString ("F1"));
						GrassManager.AmplifyWind = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30, 110, 25), GrassManager.AmplifyWind, 0f, 2f);

						GUI.TextArea (new Rect (Grass_GUI_startX + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30, 110, 22), "Turbulence:" + GrassManager.WindTurbulence.ToString ("F1"));
						GrassManager.WindTurbulence = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30, 110, 25), GrassManager.WindTurbulence, 0f, 1.5f);

						//FADE			
						GUI.TextArea (new Rect (Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30, 110, 22), "Grass fade:" + GrassManager.Grass_Fade_distance.ToString ("F1"));
						GrassManager.Grass_Fade_distance = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30, 110, 25), GrassManager.Grass_Fade_distance, 80f, 2000f);

						//Specular
						GUI.TextArea (new Rect (Grass_GUI_startX, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 50, 110, 22), "Grass spec:" + GrassManager.SpecularPower.ToString ("F1"));
						GrassManager.SpecularPower = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 0, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 50, 110, 25), GrassManager.SpecularPower, -0.1f, 5.5f);

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
					if (!Thrower.enabled) {
						Toggle_action = "Cast Balls Off";
					}
					if (GUI.Button (new Rect (Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 40 + 9, 110, 22), Toggle_action)) {					
						if (Thrower.enabled) {
							Thrower.enabled = false;
							GrassManager.Grass_painting = true;
							GrassManager.Enable_real_time_erase = true;
						} else {
							Thrower.enabled = true;
							GrassManager.Grass_painting = false;
							GrassManager.Enable_real_time_erase = false;
						}					
					}

					//TINT			
					GUI.TextArea (new Rect (Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 40 + 9 + 30 + 5, 110, 22), "Tint power:" + GrassManager.TintPower.ToString ("F1"));
					GrassManager.TintPower = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 40 + 9 + 30 + 5, 110, 25), GrassManager.TintPower, 0, 3f);
					GUI.TextArea (new Rect (Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 40 + 9 + 30 + 35, 110, 25), "Tint color (RGB)");
					Color TMP_color = GrassManager.tintColor;
					TMP_color.r = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 40 + 9 + 30 + 35 + 30, 110, 25), TMP_color.r, 0f, 1f);
					TMP_color.g = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 40 + 9 + 30 + 35 + 35 + 30, 110, 25), TMP_color.g, 0f, 1f);
					TMP_color.b = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 125 + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 40 + 9 + 30 + 35 + 35 + 35 + 30, 110, 25), TMP_color.b, 0f, 1f);
					GrassManager.tintColor = TMP_color;

					GUI.TextArea (new Rect (Grass_GUI_startX + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 40 + 9 - 5 + 5 + 0, 110, 22), "Tint freq:" + GrassManager.TintFrequency.ToString ("F2"));
					GrassManager.TintFrequency = GUI.HorizontalSlider (new Rect (Grass_GUI_startX + 125, Grass_GUI_startY + 60 + 5 + 30 + 40 + 30 + 30 + 30 + 40 + 9 - 5 + 5 + 0, 110, 25), GrassManager.TintFrequency, 0.01f, 0.12f);

				}

		

				////////////////




			}



  }// END OnGUI	
}
}