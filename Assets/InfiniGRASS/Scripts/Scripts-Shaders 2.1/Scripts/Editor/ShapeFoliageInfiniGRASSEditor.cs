using UnityEditor;
using UnityEditor.Macros;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Artngame.INfiniDy
{
    [CustomEditor(typeof(ShapeFoliageInfiniGRASS))]
    public class ShapeFoliageInfiniGRASSEditor : Editor
    {

        SerializedProperty textureScale;

        SerializedProperty runtimePlant;
        SerializedProperty GrassManager;

        //v2.1.16
        SerializedProperty useBrushFade;
        SerializedProperty fadeAmount;

        //v2.1.12
        SerializedProperty TexturePaintAlpha;
        SerializedProperty PaintAlphaID;

        //v2.1.10
        SerializedProperty useInternalInteractTexture;
        SerializedProperty InternalInteractTextureDims;

        //v2.1.8
        SerializedProperty SphereCastRadius;

        //v2.1.5
        Vector2 scroll_pos;

        //v2.1.1
        SerializedProperty Interactors;

        //v2.0.8
        SerializedProperty TextureInteract;
        SerializedProperty TextureErase;
        SerializedProperty TextureComb;
        SerializedProperty EraseBrushSize;
        SerializedProperty EraseDepth;//can also raise grass !!
        SerializedProperty TexCombBend;
        SerializedProperty InteractTexture;
        SerializedProperty InteractTexturePos;

        //Global Sky master control script
        private ShapeFoliageInfiniGRASS script;
        void Awake()
        {
            script = (ShapeFoliageInfiniGRASS)target;
        }

        public void OnEnable()
        {
            textureScale = serializedObject.FindProperty("textureScale");

            runtimePlant = serializedObject.FindProperty("runtimePlant");
            GrassManager = serializedObject.FindProperty("GrassManager");

            //v2.1.16
            useBrushFade = serializedObject.FindProperty("useBrushFade");
            fadeAmount = serializedObject.FindProperty("fadeAmount");

            //v2.1.12
            TexturePaintAlpha = serializedObject.FindProperty("TexturePaintAlpha");
            PaintAlphaID = serializedObject.FindProperty("PaintAlphaID");

            //v2.1.10
            useInternalInteractTexture = serializedObject.FindProperty("useInternalInteractTexture");
            InternalInteractTextureDims = serializedObject.FindProperty("InternalInteractTextureDims");

            //v2.1.8
            SphereCastRadius = serializedObject.FindProperty("SphereCastRadius");

            //v2.1.1
            Interactors = serializedObject.FindProperty("Interactors");

            //v2.0.8
            TextureInteract = serializedObject.FindProperty("TextureInteract");
            TextureErase = serializedObject.FindProperty("TextureErase");
            TextureComb = serializedObject.FindProperty("TextureComb");
            EraseBrushSize = serializedObject.FindProperty("EraseBrushSize");
            EraseDepth = serializedObject.FindProperty("EraseDepth");
            TexCombBend = serializedObject.FindProperty("TexCombBend");
            InteractTexture = serializedObject.FindProperty("InteractTexture");
            InteractTexturePos = serializedObject.FindProperty("InteractTexturePos");

        }



        //v2.1.8
        void OnSceneGUI()
        {
            Event cur = Event.current;

            //DRAW MOUSE
            if (script.Grass_painting)
            {
                //v1.7.7
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                Ray ray1 = HandleUtility.GUIPointToWorldRay(cur.mousePosition);
                RaycastHit hit2 = new RaycastHit();
                if (Physics.Raycast(ray1, out hit2, Mathf.Infinity))
                {

                    if (script.Erasing)
                    {
                        if (script.MassErase)
                        {

                            Handles.color = Color.red;
                            Handles.DrawWireDisc(hit2.point, hit2.normal, script.SphereCastRadius * 1.2f);
                        }
                        else
                        {
                            Handles.color = Color.red;

                            Handles.DrawWireDisc(hit2.point, hit2.normal, 15);
                        }
                    }
                    else
                    {
                        if (script.Looking)
                        {

                        }
                        else
                        {
                            Handles.color = Color.white;
                            Handles.DrawWireDisc(hit2.point, hit2.normal, 15 * (script.Max_spread / 12));
                        }
                    }
                }
                SceneView.RepaintAll();


                //int ManagerID = (int)script.ManagerIDforPosition(hit2).y; //script.ManagerIDforPosition (hit2); //v2.1.30
                //if (ManagerID >= 0 && ManagerID < script.ManagerGroups[script.currentManagerGroupId].GrassManagersH.Count)
                //{ //v2.1.18
                //PaintOnManager(script.ManagerGroups[script.currentManagerGroupId].GrassManagersH[ManagerID], script, cur);
                PaintOnManager(script, cur);
                //}
            }



        }//END ON SCENE GUI


        Vector3 Keep_last_mouse_pos;
        Vector3 Camera_last_pos;
        Quaternion Camera_last_rot;//keep just before mouse drag starts
        Vector3 prevMousePos;

        void PaintOnManager(ShapeFoliageInfiniGRASS globalManager, Event cur)//void PaintOnManager(InfiniGRASSManager script, GlobalGrassManager globalManager, Event cur)
        {




            if (script.Grass_painting & !Application.isPlaying)
            {




                //ERASE GRASS
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {


                    //} else if (cur.type == EventType.keyDown & cur.keyCode == (KeyCode.LeftControl)) { //v1.7.7
                }
                else if (cur.type == EventType.KeyDown & (cur.keyCode == (KeyCode.LeftControl) || cur.keyCode == (KeyCode.LeftCommand) || cur.keyCode == (KeyCode.LeftAlt))
                  || (cur.type == EventType.MouseDrag || cur.type == EventType.MouseDown) && (cur.alt && (cur.control || cur.command))
              )
                { //v1.7.7
                  //rotate camera
                  //Debug.Log("Lock camera");
                }
                else
                {

                    bool erasing = false;
                    globalManager.Erasing = false;
                    if (cur.modifiers > 0)
                    {
                        if ((cur.modifiers) == EventModifiers.Shift)
                        {

                            erasing = true;
                            globalManager.Erasing = true;
                        }
                    }

                    bool looking = false;
                    globalManager.Looking = false;
                    if (cur.modifiers > 0)
                    {
                        //if ((cur.modifiers) == EventModifiers.Control) {
                        //if ((cur.modifiers) == EventModifiers.Alt) { //v1.7.7
                        if ((cur.modifiers) == EventModifiers.Alt || (cur.isKey && cur.type == EventType.KeyDown) || (cur.modifiers) == EventModifiers.Command
                            || (cur.modifiers) == EventModifiers.Control || (cur.modifiers) == EventModifiers.Numeric || (cur.modifiers) == EventModifiers.FunctionKey)
                        { //v1.7.7
                          //Debug.Log("look");

                            looking = true;
                            globalManager.Looking = true;
                        }
                    }

                    //					if (!looking & cur.keyCode != (KeyCode.LeftControl) & cur.type != EventType.keyDown &
                    //						((cur.type == EventType.MouseDrag && cur.button == 1 & Vector3.Distance (Keep_last_mouse_pos, cur.mousePosition) > script.min_grass_patch_dist)  
                    //							| (cur.type == EventType.MouseDown && cur.button == 1)) 
                    bool buttonPressed = (!globalManager.leftMousePaint && cur.button == 1) || (globalManager.leftMousePaint && cur.button == 0);
                    if (!looking && cur.keyCode != (KeyCode.LeftControl) && cur.keyCode != (KeyCode.LeftAlt) && cur.type != EventType.KeyDown &&
                        ((cur.type == EventType.MouseDrag && buttonPressed && Vector3.Distance(Keep_last_mouse_pos, cur.mousePosition) > script.GrassManager.min_grass_patch_dist)//script.min_grass_patch_dist)
                            || (cur.type == EventType.MouseDown && buttonPressed))
                    )
                    {
                        Keep_last_mouse_pos = cur.mousePosition;
                        Ray ray = HandleUtility.GUIPointToWorldRay(cur.mousePosition);
                        RaycastHit hit = new RaycastHit();

                        //fix camera
                        if (Camera.current != null)
                        {
                            Camera.current.transform.position = Camera_last_pos;
                            Camera.current.transform.rotation = Camera_last_rot;
                        }

                        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                        {
                            //v1.2 - dont paint if out of editor view
                            //Debug.Log(Vector3.Distance (hit.point, Camera.current.transform.position));
                            if (Vector3.Distance(hit.point, Camera.current.transform.position) < script.Editor_view_dist)
                            {
                                //plantOnRaycast(script, globalManager, hit, ray, erasing); //v2.1.28
                                plantOnRaycast(script, hit, ray, erasing); //v2.1.28

                            }
                        }
                    }
                    else
                    {
                        if (Camera.current != null)
                        {
                            Camera_last_pos = Camera.current.transform.position;
                            Camera_last_rot = Camera.current.transform.rotation;
                        }
                    }
                }
            }// END GRASS PAINTING
        }//END PAINTING ON MANAGER FUNCTION

        //v2.1.28
        void plantOnRaycast(ShapeFoliageInfiniGRASS globalManager, RaycastHit hit, Ray ray, bool erasing)
        {

            bool is_Terrain = false;
            //if ( (Terrain.activeTerrain != null && hit.collider.gameObject != null && hit.collider.gameObject == Terrain.activeTerrain.gameObject)){
            if ((hit.collider.gameObject != null && hit.collider.gameObject.GetComponent<Terrain>() != null))
            { //v2.1.18
                is_Terrain = true;
            }

            if (is_Terrain)// || (script.PaintonTag && hit.collider.gameObject != null && hit.collider.gameObject.tag == "PPaint"))
            {//v1.1	

                if (globalManager.TextureInteract)
                {
                    //v2.1.10
                    if (globalManager.useInternalInteractTexture)
                    {//v2.1.10
                        if (globalManager.InternalInteractTexture == null)
                        {
                            globalManager.InternalInteractTexture = new Texture2D((int)globalManager.InternalInteractTextureDims.x, (int)globalManager.InternalInteractTextureDims.y);
                            globalManager.InternalInteractTexture.name = "InternalTex";

                            //v2.1.10 - set to black
                            Color[] colors = globalManager.InternalInteractTexture.GetPixels();
                            for (int i = 0; i < colors.Length; i++)
                            {
                                colors[i] = Color.black;
                            }
                            globalManager.InternalInteractTexture.SetPixels(colors);
                            //globalManager.InteractTexture [0] = globalManager.InternalInteractTexture; // for debug purposes
                        }
                    }

                    Vector3 hit_point = hit.point;
                    //v2.0.8
                    //bool TextureComb = true;
                    //bool TextureErase = true;
                    //float EraseBrushSize = 10;
                    //float TexCombBend = 1000;
                    //float EraseDepth = 10;
                    if (globalManager.TextureInteract)
                    {   //TextureInteract) {
                        //PAINT ON TETXURE - InteractTexture
                        float IntTextScale = globalManager.textureScale;//0.1f;
                        int IntTextWidth = Mathf.Max(1, (int)(globalManager.EraseBrushSize * IntTextScale)); // Mathf.Max (1, (int)(10 * IntTextScale));
                                                                                                             //Color[] pixels = InteractTexture [0].GetPixels();
                                                                                                             //Color[] pixels = InteractTexture [0].GetPixels((int)(hit_point.x*IntTextScale)-IntTextWidth, (int)(hit_point.z*IntTextScale)-IntTextWidth, 2*IntTextWidth, 2*IntTextWidth);

                        int originX = (int)(hit_point.x * IntTextScale) - IntTextWidth;
                        int originY = (int)(hit_point.z * IntTextScale) - IntTextWidth;

                        Color[] pixels = new Color[2 * IntTextWidth * 2 * IntTextWidth];

                        bool lerpInteraction = true;

                        //							if (TextureErase && Input.GetMouseButton(1) ) { //TexCombBend
                        //								for (int i = 0; i < pixels.Length; i++) {
                        //									pixels [i] = Color.white * EraseDepth;
                        //								}
                        //							}

                        //Debug.Log ("prev mouse pos =" + prevMousePos);

                        if (globalManager.TextureComb)
                        {// && Input.GetMouseButton(1)) {
                            Vector3 motionDir = hit_point - prevMousePos;
                            float posDiff = (motionDir).magnitude;
                            if (lerpInteraction)
                            {
                                if (globalManager.useInternalInteractTexture)
                                {//v2.1.10

                                    //pixels = globalManager.InternalInteractTexture.GetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth);
                                    globalManager.getPixelsSanitized(globalManager.InternalInteractTexture, ref pixels, originX, originY, IntTextWidth);
                                }
                                else
                                {
                                    //v2.1.33
                                    //Debug.Log("originX, originY, 2 * IntTextWidth, 2 * IntTextWidth = "+ originX +", "+ originY + ", " 
                                    //    +  IntTextWidth + ", " + IntTextWidth + ", "+ globalManager.InteractTexture[0].width);
                                    //pixels = script.GlobalManager.InteractTexture[0].GetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth);//v2.1.33

                                    //pixels = globalManager.InteractTexture[0].GetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth);//v2.1.33
                                    globalManager.getPixelsSanitized(globalManager.InteractTexture[0], ref pixels, originX, originY, IntTextWidth);


                                }
                            }
                            //only affect green-blue channels (grass direction and bend amount)
                            float angle1 = Vector3.Angle(new Vector3(motionDir.x, 0, motionDir.z), Vector3.forward) / 180;
                            //float sign = 1;
                            //Debug.Log("angle="+Mathf.Sign(motionDir.x)*angle1);
                            for (int i = 0; i < pixels.Length; i++)
                            {
                                //if (angle1 <= 0) {
                                if (Mathf.Sign(motionDir.x) < 0)
                                {
                                    if (lerpInteraction)
                                    {
                                        pixels[i].g = Mathf.Lerp(pixels[i].g, 0.5f + angle1 / 2, 0.5f);
                                    }
                                    else
                                    {
                                        pixels[i].g = 0.5f + angle1 / 2;
                                    }
                                }
                                else
                                {
                                    if (lerpInteraction)
                                    {
                                        pixels[i].g = Mathf.Lerp(pixels[i].g, angle1 / 2, 0.5f);
                                    }
                                    else
                                    {
                                        pixels[i].g = angle1 / 2;
                                    }
                                }
                                //pixels [i].g = Mathf.Abs(Vector3.Angle(new Vector3(motionDir.x,0,motionDir.z),Vector3.forward)/360);
                            }
                            for (int i = 0; i < pixels.Length; i++)
                            {
                                pixels[i].b = globalManager.TexCombBend / 255;
                            }
                        }

                        if (globalManager.TextureErase)
                        {// && Input.GetMouseButton(1) ) { //TexCombBend
                         //v2.1.16
                            if (globalManager.useBrushFade)
                            {
                                //pixels = globalManager.InteractTexture.GetPixels (originX, originY, 2 * IntTextWidth, 2 * IntTextWidth);
                                if (globalManager.useInternalInteractTexture)
                                {//v2.1.10

                                    //pixels = globalManager.InternalInteractTexture.GetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth);
                                    globalManager.getPixelsSanitized(globalManager.InternalInteractTexture, ref pixels, originX, originY, IntTextWidth);
                                }
                                else
                                {
                                    //v2.1.33
                                    //pixels = script.InteractTexture[0].GetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth);

                                    //pixels = globalManager.InteractTexture[0].GetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth);
                                    globalManager.getPixelsSanitized(globalManager.InteractTexture[0], ref pixels, originX, originY, IntTextWidth);
                                }
                                for (int i = 0; i < 2 * IntTextWidth; i++)
                                {
                                    for (int j = 0; j < 2 * IntTextWidth; j++)
                                    {
                                        //Find and normalize distance
                                        float distFromCenter = Mathf.Sqrt(Mathf.Pow(i - IntTextWidth, 2) + Mathf.Pow(j - IntTextWidth, 2)) / Mathf.Sqrt(Mathf.Pow(IntTextWidth, 2) + Mathf.Pow(IntTextWidth, 2));
                                        //Debug.Log("pixels [i * 2 * IntTextWidth + j].r="+pixels [i * 2 * IntTextWidth + j].r+" "+"TO: "+EraseDepth * Mathf.Pow ((1 - distFromCenter), fadeAmount));
                                        if (globalManager.EraseDepth >= 0)
                                        {
                                            if (pixels[i * 2 * IntTextWidth + j].r < globalManager.EraseDepth * Mathf.Pow((1 - distFromCenter), globalManager.fadeAmount))
                                            {
                                                pixels[i * 2 * IntTextWidth + j].r = globalManager.EraseDepth * Mathf.Pow((1 - distFromCenter), globalManager.fadeAmount);
                                            }
                                        }
                                        else
                                        {
                                            if (pixels[i * 2 * IntTextWidth + j].r > Mathf.Abs(globalManager.EraseDepth) * Mathf.Pow((distFromCenter), 1 / globalManager.fadeAmount))
                                            {
                                                pixels[i * 2 * IntTextWidth + j].r = Mathf.Abs(globalManager.EraseDepth) * Mathf.Pow((distFromCenter), 1 / globalManager.fadeAmount);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < pixels.Length; i++)
                                {
                                    pixels[i].r = globalManager.EraseDepth;
                                }
                            }
                            //												for (int i = 0; i < pixels.Length; i++) {
                            //													pixels [i].r = globalManager.EraseDepth;
                            //												}
                        }

                        //v2.1.12
                        if (globalManager.TexturePaintAlpha)
                        {
                            for (int i = 0; i < pixels.Length; i++)
                            {
                                pixels[i].a = ((float)globalManager.PaintAlphaID) / 255f;
                            }
                        }

                        //Debug.Log (pixels.Length);
                        //						for (int i = 0; i < 512; i++) {
                        //							for (int j = 0; j < 512; j++) {
                        //								if(hit_point.x*IntTextScale < i+IntTextWidth && hit_point.x*IntTextScale > i-IntTextWidth){
                        //									if (hit_point.z*IntTextScale < j + IntTextWidth && hit_point.z*IntTextScale > j - IntTextWidth) {
                        //										pixels [j*512+ i] = Color.white;
                        //									}
                        //								}
                        //							}
                        //						}
                        //InteractTexture [0].SetPixels (pixels);
                        if ((globalManager.TextureErase || globalManager.TextureComb || globalManager.TexturePaintAlpha))
                        { //&& Input.GetMouseButton (1)) { //v2.1.12
                            if (globalManager.useInternalInteractTexture)
                            {//v2.1.10

                                //globalManager.InternalInteractTexture.SetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth, pixels);
                                globalManager.setPixelsSanitized(globalManager.InternalInteractTexture, pixels, originX, originY, IntTextWidth);
                                globalManager.InternalInteractTexture.Apply();
                                
                            }
                            else
                            {
                                //v2.1.33
                                //script.InteractTexture[0].SetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth, pixels);
                                //script.InteractTexture[0].Apply();

                                //globalManager.InteractTexture[0].SetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth, pixels);
                                globalManager.setPixelsSanitized(globalManager.InteractTexture[0], pixels, originX, originY, IntTextWidth);
                                globalManager.InteractTexture[0].Apply();
                                
                            }
                        }
                    }
                    //END PAINT ON TEXTURE

                    prevMousePos = hit_point;

                }

            }//end v2.1.28

        }



        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            if (script != null)
            {
                //Undo.RecordObject (script.SkyManager, "Sky Variabe Change");
                Undo.RecordObject(script, "Parameters change");
            }

            //if (GUILayout.Button("Grass painting - shaping"))
            //{//if (GUILayout.Button ("Mass Erase parameters")) {
            //    script.currentTab = 2;
            //}

            //v2.1.1
            EditorGUILayout.PropertyField(Interactors, true);

            //v3.4
            float sliderWidth = 295.0f;

            //v2.1.8
            if (1 == 1)//if ((script.UseTabs && script.currentTab == 2) | !script.UseTabs)
            {

                //EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
                //GUILayout.Space(10);
                //script.massEraseFolder = EditorGUILayout.Foldout(script.massEraseFolder, "Paint & Mass Erase Folder");
                //EditorGUILayout.EndHorizontal();

                //if (script.massEraseFolder)
                {


                    //v2.1.10
                    EditorGUILayout.BeginVertical("box", GUILayout.Width(410));
                    EditorGUILayout.PropertyField(TextureInteract);
                    EditorGUILayout.PropertyField(TextureErase);
                    EditorGUILayout.PropertyField(TexturePaintAlpha); //v2.1.12
                    EditorGUILayout.PropertyField(TextureComb);
                    EditorGUILayout.PropertyField(EraseBrushSize);
                    EditorGUILayout.PropertyField(EraseDepth);
                    EditorGUILayout.PropertyField(TexCombBend);

                    //v2.1.16
                    EditorGUILayout.PropertyField(useBrushFade);
                    EditorGUILayout.PropertyField(fadeAmount);

                    EditorGUILayout.PropertyField(textureScale);

                    EditorGUILayout.PropertyField(runtimePlant);
                    EditorGUILayout.PropertyField(GrassManager);
                    //script.GrassManager = (InfiniGRASSManager)GrassManager;


                    EditorGUILayout.BeginHorizontal(GUILayout.Width(400));
                    GUILayout.Space(10); EditorGUILayout.PropertyField(InteractTexture, true);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(400));
                    GUILayout.Space(10); EditorGUILayout.PropertyField(InteractTexturePos, true);
                    EditorGUILayout.EndHorizontal();
                    // EditorGUILayout.PropertyField(PoolPrefill);
                    // EditorGUILayout.PropertyField(PrefillAmount);
                    EditorGUILayout.EndVertical();
                    GUILayout.Box("", GUILayout.Height(2), GUILayout.Width(410));


                    EditorGUILayout.HelpBox("Paint grass on layer", MessageType.None);
                    EditorGUILayout.BeginHorizontal();
                    script.Grass_painting = EditorGUILayout.Toggle(script.Grass_painting, GUILayout.MaxWidth(sliderWidth));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.HelpBox("Paint with left Mouse Button", MessageType.None);
                    EditorGUILayout.BeginHorizontal();
                    script.leftMousePaint = EditorGUILayout.Toggle(script.leftMousePaint, GUILayout.MaxWidth(sliderWidth));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.HelpBox("Erase distance", MessageType.None);
                    EditorGUILayout.BeginHorizontal();
                    //script.SphereCastRadius = EditorGUILayout.Slider (script.SphereCastRadius, 0.1f, 1000, GUILayout.MaxWidth (sliderWidth));
                    script.EraseRadious = EditorGUILayout.Slider(script.EraseRadious, 0.1f, 1000, GUILayout.MaxWidth(sliderWidth)); //v2.1.16
                    EditorGUILayout.EndHorizontal();

                    //v2.1.16
                    EditorGUILayout.HelpBox("Use Per Brush Erase", MessageType.None);
                    EditorGUILayout.BeginHorizontal();
                    script.erasePerBrush = EditorGUILayout.Toggle(script.erasePerBrush, GUILayout.MaxWidth(sliderWidth));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.HelpBox("Current brush", MessageType.None);
                    //EditorGUILayout.BeginHorizontal();
                    //script.GrassManager.Grass_selector = EditorGUILayout.IntSlider(script.GrassManager.Grass_selector, 0, script.GrassManager.GrassPrefabs.Count - 1, GUILayout.MaxWidth(sliderWidth)); //v2.1.16
                    //EditorGUILayout.EndHorizontal();

                    //v2.1.12
                    EditorGUILayout.HelpBox("Paint Alpha ID", MessageType.None);
                    EditorGUILayout.BeginHorizontal();
                    script.PaintAlphaID = EditorGUILayout.IntSlider(script.PaintAlphaID, 0, 255, GUILayout.MaxWidth(sliderWidth));
                    EditorGUILayout.EndHorizontal();


                    //v2.1.10
                    EditorGUILayout.HelpBox("Use Internal Shaping Texture", MessageType.None);
                    EditorGUILayout.BeginHorizontal();
                    script.useInternalInteractTexture = EditorGUILayout.Toggle(script.useInternalInteractTexture, GUILayout.MaxWidth(sliderWidth));
                    EditorGUILayout.EndHorizontal();

                    //external texture
                    if (script.InteractTexture.Count > 0 && script.InteractTexture[0] != null)
                    {
                        if (GUILayout.Button(new GUIContent("Reset External Texture to Black"), GUILayout.Width(210)))
                        {
                            //Color[] colors = script.InteractTexture[0].GetPixels();
                            //for (int i = 0; i < colors.Length; i++)
                            //{
                            //    colors[i] = Color.black;
                            //    colors[i].a = ((float)script.PaintAlphaID)/255f;//  0; //v2.1.12
                            //}
                            //script.InteractTexture[0].SetPixels(colors);
                            //script.InteractTexture[0].Apply();
                            script.resetExternalTextureToColor(Color.black);
                        }
                        if (GUILayout.Button(new GUIContent("Reset External Texture to White"), GUILayout.Width(210)))
                        {
                            //Color[] colors = script.InteractTexture[0].GetPixels();
                            //for (int i = 0; i < colors.Length; i++)
                            //{
                            //    colors[i] = Color.white;
                            //    colors[i].a = ((float)script.PaintAlphaID) / 255f;//0; //v2.1.12
                            //}
                            //script.InteractTexture[0].SetPixels(colors);
                            //script.InteractTexture[0].Apply();
                            script.resetExternalTextureToColor(Color.white);
                        }
                    }


                    //v2.1.10
                    if (script.useInternalInteractTexture)
                    {//v2.1.10
                        if (script.InternalInteractTexture == null)
                        {
                            if (GUILayout.Button(new GUIContent("Create Internal Texture"), GUILayout.Width(210)))
                            {
                                script.InternalInteractTexture = new Texture2D((int)script.InternalInteractTextureDims.x, (int)script.InternalInteractTextureDims.y);
                                script.InternalInteractTexture.name = "InternalTex";
                                Color[] colors = script.InternalInteractTexture.GetPixels();
                                for (int i = 0; i < colors.Length; i++)
                                {
                                    colors[i] = Color.black;
                                }
                                script.InternalInteractTexture.SetPixels(colors);
                                script.InternalInteractTexture.Apply();
                            }
                        }
                        if (script.InternalInteractTexture != null)
                        {
                            if (GUILayout.Button(new GUIContent("Reset Internal Texture to Black"), GUILayout.Width(210)))
                            {
                                Color[] colors = script.InternalInteractTexture.GetPixels();
                                for (int i = 0; i < colors.Length; i++)
                                {
                                    colors[i] = Color.black;
                                    colors[i].a = 0; //v2.1.12
                                }
                                script.InternalInteractTexture.SetPixels(colors);
                                script.InternalInteractTexture.Apply();
                            }
                            if (GUILayout.Button(new GUIContent("Reset Internal Texture to White"), GUILayout.Width(210)))
                            {
                                Color[] colors = script.InternalInteractTexture.GetPixels();
                                for (int i = 0; i < colors.Length; i++)
                                {
                                    colors[i] = Color.white;
                                    colors[i].a = 0; //v2.1.12
                                }
                                script.InternalInteractTexture.SetPixels(colors);
                                script.InternalInteractTexture.Apply();
                            }
                        }
                    }

                    //v2.1.12
                    //if (GUILayout.Button(new GUIContent("Assign Depth Texture to Materials"), GUILayout.Width(260)))
                    //{

                    //    script.AssignDepthTextureToMaterials();

                    //}

                    if (GUILayout.Button(new GUIContent("Assign Internal Texture to Materials"), GUILayout.Width(260)))
                    {

                        //v2.1.10
                        //if (globalManager.useInternalInteractTexture) {//v2.1.10
                        if (script.InternalInteractTexture == null)
                        {
                            script.InternalInteractTexture = new Texture2D((int)script.InternalInteractTextureDims.x, (int)script.InternalInteractTextureDims.y);
                            script.InternalInteractTexture.name = "InternalTex";
                            //globalManager.InteractTexture [0] = globalManager.InternalInteractTexture; // for debug purposes
                        }
                        //}

                        script.AssignShapeTextureToMaterials(script.InternalInteractTexture);

                        //						if (script.InternalInteractTexture != null) {
                        //							for (int i = 0; i < script.GrassManager.GrassMaterials.Count; i++) {
                        //								if (script.GrassManager.GrassMaterials [i].HasProperty ("_InteractTexture")) {
                        //									script.GrassManager.GrassMaterials [i].SetTexture ("_InteractTexture", script.InternalInteractTexture);
                        //								}
                        //							}
                        //							for (int i = 0; i < script.GrassManager.ExtraGrassMaterials.Count; i++) {
                        //								for (int i1 = 0; i1 < script.GrassManager.ExtraGrassMaterials[i].ExtraMaterials.Count; i1++) {
                        //									if (script.GrassManager.ExtraGrassMaterials[i].ExtraMaterials [i1].HasProperty ("_InteractTexture")) {
                        //										script.GrassManager.ExtraGrassMaterials[i].ExtraMaterials [i1].SetTexture ("_InteractTexture", script.InternalInteractTexture);
                        //									}
                        //								}
                        //							}
                        //						}
                    }
                    if (GUILayout.Button(new GUIContent("Assign External Texture to Materials"), GUILayout.Width(260)))
                    {

                        script.AssignShapeTextureToMaterials(script.InteractTexture[0]);

                        //						if (script.InteractTexture[0] != null) {
                        //							for (int i = 0; i < script.GrassManager.GrassMaterials.Count; i++) {
                        //								if (script.GrassManager.GrassMaterials [i].HasProperty ("_InteractTexture")) {
                        //									script.GrassManager.GrassMaterials [i].SetTexture ("_InteractTexture", script.InteractTexture[0]);
                        //								}
                        //							}
                        //							for (int i = 0; i < script.GrassManager.ExtraGrassMaterials.Count; i++) {
                        //								for (int i1 = 0; i1 < script.GrassManager.ExtraGrassMaterials[i].ExtraMaterials.Count; i1++) {
                        //									if (script.GrassManager.ExtraGrassMaterials[i].ExtraMaterials [i1].HasProperty ("_InteractTexture")) {
                        //										script.GrassManager.ExtraGrassMaterials[i].ExtraMaterials [i1].SetTexture ("_InteractTexture", script.InteractTexture[0]);
                        //									}
                        //								}
                        //							}
                        //						}
                    }
                    if (script.useInternalInteractTexture && script.InternalInteractTexture != null)
                    {
                        GUILayout.Box("Internal Interact Texture", GUILayout.Height(23), GUILayout.Width(410));
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(script.InternalInteractTexture, GUILayout.MaxWidth(410.0f), GUILayout.MaxHeight(410.0f));
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(410));
                    }

                    //v2.1.33
                    //GUILayout.Box("External Interact Texture 0", GUILayout.Height(3), GUILayout.Width(410));
                    if (script.InteractTexture.Count > 0 &&  script.InteractTexture[0] != null)
                    {
                        GUILayout.Box("External Interact Texture 0", GUILayout.Height(23), GUILayout.Width(410));
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(script.InteractTexture[0], GUILayout.MaxWidth(410.0f), GUILayout.MaxHeight(410.0f));
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(410));
                    }

                    //v2.1.16
                    //if (script.GrassManager.DepthCam != null)
                    //{
                    //    TerrainDepthInfiniGRASS depthScript = script.GrassManager.DepthCam.GetComponent<TerrainDepthInfiniGRASS>();
                    //    if (script.GrassManager.DepthCam.GetComponent<Camera>() != null && script.GrassManager.DepthCam.GetComponent<Camera>().targetTexture != null)
                    //    {
                    //        GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(410));
                    //        EditorGUILayout.BeginHorizontal();
                    //        GUILayout.Label(script.GrassManager.DepthCam.GetComponent<Camera>().targetTexture, GUILayout.MaxWidth(410.0f), GUILayout.MaxHeight(410.0f));
                    //        EditorGUILayout.EndHorizontal();
                    //        GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(410));
                    //    }
                    //    if (depthScript != null)
                    //    {
                    //        EditorGUILayout.HelpBox("Depth height factor", MessageType.None);
                    //        EditorGUILayout.BeginHorizontal();
                    //        depthScript.heightFactor = EditorGUILayout.Slider(depthScript.heightFactor, 0, 15, GUILayout.MaxWidth(sliderWidth));
                    //        EditorGUILayout.EndHorizontal();
                    //        EditorGUILayout.HelpBox("Depth contrast factor", MessageType.None);
                    //        EditorGUILayout.BeginHorizontal();
                    //        depthScript.contrast = EditorGUILayout.Slider(depthScript.contrast, 0, 2, GUILayout.MaxWidth(sliderWidth));
                    //        EditorGUILayout.EndHorizontal();
                    //    }
                    //}
                }//END massEraseFolder FOLDER
            }//END TAB2

            serializedObject.ApplyModifiedProperties();
        }



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}