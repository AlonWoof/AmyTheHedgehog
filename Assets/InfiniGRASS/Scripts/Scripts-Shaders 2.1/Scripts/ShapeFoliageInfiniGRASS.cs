using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artngame.INfiniDy {


    //v2.1.1 - Interactors
    [System.Serializable]
    public class InfiniGRASS_Interactors
    {
        public Transform Interactor;
        public Vector3 prevPosition;
        public int InteractTextureLayer = 0;
        public float Radius = 1;
        public float Weight = 1;
    }

    [System.Serializable]
    [ExecuteInEditMode]//v2.0.9
    public class ShapeFoliageInfiniGRASS : MonoBehaviour
    {
        public float Editor_view_dist = 1000;
        public float textureScale = 1.0f;

        //v2.1.16 - erase per brush
        public bool erasePerBrush = false;
        public bool useBrushFade = false;
        public float fadeAmount = 1;

        //v2.1.12
        public bool TexturePaintAlpha = false;
        public int PaintAlphaID = 0;

        //v2.1.10
        public bool useInternalInteractTexture = false;
        public Texture2D InternalInteractTexture;
        public Vector2 InternalInteractTextureDims = new Vector2(512, 512);
        public bool parentManagersToTerrains = false;
        public bool groupManagersPerLayer = true;

        //v2.1.8
        public bool Grass_painting = false;
        public float SphereCastRadius = 15;//radius when mass erasing
                                           //public bool MassErase = false;
        public bool Erasing = false;
        public bool Looking = false;
        public bool ActivateHelp = true;
        public float Max_spread = 5;
        public bool leftMousePaint = false;



        //v2.1.1
        public int MainGrassType = 0;
        public List<InfiniGRASS_Interactors> Interactors = new List<InfiniGRASS_Interactors>();

        //v2.0.8
        public bool TextureInteract = false;
        public bool TextureErase = false;
        public bool TextureComb = false;
        public float EraseBrushSize = 10;
        public float EraseDepth = 0;//can also raise grass !!
        public float TexCombBend = 45;
        public List<Texture2D> InteractTexture = new List<Texture2D>();
        public List<Vector3> InteractTexturePos = new List<Vector3>();


        //Vector3 prevMousePos;//v2.0.8


        public float EraseRadious = 100;
        [Range(0, 1)]
        public int MouseButton = 0;
        public GameObject EraseDefiner;
        public List<Transform> Definers = new List<Transform>();
        public int EraseCircleSegments = 8;
        Vector3 prevMousePos;//v2.0.8

        public void setPixelsSanitized(Texture2D texture, Color[] pixels, int originX, int originY, int IntTextWidth)
        {
            if (originX > 0 && originY > 0 && originX < texture.width && originY < texture.height)
            {
                int correctWidth = 2 * IntTextWidth;
                int correctHeight = 2 * IntTextWidth;
                if (originX + 2 * IntTextWidth > texture.width)
                {
                    correctWidth = texture.width - originX;
                }
                if (originY + 2 * IntTextWidth > texture.height)
                {
                    correctHeight = texture.height - originY;
                }
                texture.SetPixels(originX, originY, correctWidth, correctHeight, pixels);
            }
        }
        public void getPixelsSanitized(Texture2D texture, ref Color[] colorsOut, int originX, int originY, int IntTextWidth)
        {
            if (originX > 0 && originY > 0 && originX < texture.width && originY < texture.height) {
                int correctWidth = 2 * IntTextWidth;
                int correctHeight = 2 * IntTextWidth;
                if (originX + 2 * IntTextWidth > texture.width)
                {
                    correctWidth = texture.width - originX;
                }
                if (originY + 2 * IntTextWidth > texture.height)
                {
                    correctHeight = texture.height - originY;
                }
                colorsOut = texture.GetPixels(originX, originY, correctWidth, correctHeight);
            }            
        }

        // Update is called once per frame
        void Update()
        {
            //v2.1.10
            if (useInternalInteractTexture && InternalInteractTexture != null)
            { //NEW
                InteractorsUpdate(InternalInteractTexture);
                MassEraseUpdate(InternalInteractTexture);
            }
            else
            {
                //v2.1.8
                if (InteractTexture.Count > 0 && InteractTexture[0] != null)
                {
                    InteractorsUpdate(InteractTexture[0]);
                    MassEraseUpdate(InteractTexture[0]);
                }
            }
        }//END UPDATE

        void InteractorsUpdate(Texture2D InteractTexture)
        { //v2.1.8 //v2.1.10

            //v2.1.1 - Interactors
            //v2.1.1
            if (TextureInteract && Interactors.Count > 0)
            {
                for (int j = 0; j < Interactors.Count; j++)
                {
                    Vector3 hit_point = Interactors[j].Interactor.position;

                    float EraseBrushSizeINT = EraseBrushSize;
                    bool TextureCombINT = TextureComb;
                    float TexCombBendINT = TexCombBend;
                    bool useBrushFadeINT = useBrushFade;
                    float EraseDepthINT = EraseDepth;
                    bool TextureEraseINT = TextureErase;

                    //Check if script exist, read params
                    InfiniGRASSInteractor interactorProps = Interactors[j].Interactor.GetComponent<InfiniGRASSInteractor>();
                    if(interactorProps != null)
                    {
                        EraseBrushSizeINT = interactorProps.EraseBrushSize;
                        TextureCombINT = interactorProps.TextureComb;
                        TexCombBendINT = interactorProps.TexCombBend;
                        useBrushFadeINT = interactorProps.useBrushFade;
                        EraseDepthINT = interactorProps.EraseDepth;
                        TextureEraseINT = interactorProps.TextureErase;
                    }

                    //PAINT ON TETXURE - InteractTexture
                    float IntTextScale = textureScale;// 0.1f;
                    int IntTextWidth = Mathf.Max(1, (int)(EraseBrushSizeINT * IntTextScale)); // Mathf.Max (1, (int)(10 * IntTextScale));
                                                                                           //Color[] pixels = InteractTexture [0].GetPixels();
                                                                                           //Color[] pixels = InteractTexture [0].GetPixels((int)(hit_point.x*IntTextScale)-IntTextWidth, (int)(hit_point.z*IntTextScale)-IntTextWidth, 2*IntTextWidth, 2*IntTextWidth);

                    int originX = (int)(hit_point.x * IntTextScale) - IntTextWidth;
                    int originY = (int)(hit_point.z * IntTextScale) - IntTextWidth;
                    Color[] pixels = new Color[2 * IntTextWidth * 2 * IntTextWidth];
                    bool lerpInteraction = true;

                    //v2.0.9
                    bool inBounds = true;
                    if (originX < 0 || originY < 0 || (originX + 2 * IntTextWidth) > InteractTexture.width || (originY + 2 * IntTextWidth) > InteractTexture.height)
                    { //v2.1.1
                        inBounds = false;
                    }

                    //v2.1.1
                    Ray ray = new Ray();
                    ray.origin = hit_point;
                    ray.direction = -Vector3.up;
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        if (hit.distance > 5.5f)
                        { // put object volume here
                            inBounds = false;
                        }
                    }

                    if (inBounds)
                    { //v2.0.9					
                        if (TextureCombINT)
                        {
                            Vector3 motionDir = hit_point - Interactors[j].prevPosition; /////////////////////////////prevMousePos;
                            float posDiff = (motionDir).magnitude;
                            if (lerpInteraction)
                            {

                                //pixels = InteractTexture.GetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth);
                                getPixelsSanitized(InteractTexture, ref pixels, originX, originY, IntTextWidth);
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
                                pixels[i].b = TexCombBendINT / 255;
                            }
                        }

                        if (TextureEraseINT)
                        { //TexCombBendINT
                          //v2.1.16
                            if (useBrushFadeINT)
                            {
                                
                                //pixels = InteractTexture.GetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth);
                                getPixelsSanitized(InteractTexture, ref pixels, originX, originY, IntTextWidth);

                                for (int i = 0; i < 2 * IntTextWidth; i++)
                                {
                                    for (int j1 = 0; j1 < 2 * IntTextWidth; j1++)
                                    {
                                        //Find and normalize distance
                                        float distFromCenter = Mathf.Sqrt(Mathf.Pow(i - IntTextWidth, 2) + Mathf.Pow(j1 - IntTextWidth, 2)) / Mathf.Sqrt(Mathf.Pow(IntTextWidth, 2) + Mathf.Pow(IntTextWidth, 2));
                                        //Debug.Log("pixels [i * 2 * IntTextWidth + j].r="+pixels [i * 2 * IntTextWidth + j].r+" "+"TO: "+EraseDepthINT * Mathf.Pow ((1 - distFromCenter), fadeAmount));
                                        if (EraseDepthINT >= 0)
                                        {
                                            if (pixels[i * 2 * IntTextWidth + j1].r < EraseDepthINT * Mathf.Pow((1 - distFromCenter), fadeAmount))
                                            {
                                                pixels[i * 2 * IntTextWidth + j1].r = EraseDepthINT * Mathf.Pow((1 - distFromCenter), fadeAmount);
                                            }
                                        }
                                        else
                                        {
                                            if (pixels[i * 2 * IntTextWidth + j1].r > Mathf.Abs(EraseDepthINT) * Mathf.Pow((distFromCenter), 1 / fadeAmount))
                                            {
                                                pixels[i * 2 * IntTextWidth + j1].r = Mathf.Abs(EraseDepthINT) * Mathf.Pow((distFromCenter), 1 / fadeAmount);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < pixels.Length; i++)
                                {
                                    pixels[i].r = EraseDepthINT;
                                }
                            }
                        }

                        if ((TextureEraseINT || TextureCombINT))
                        {

                            //InteractTexture.SetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth, pixels);
                            setPixelsSanitized(InteractTexture, pixels, originX, originY, IntTextWidth);

                            InteractTexture.Apply();
                        }
                    }//END BOUNDS CHECK

                    Interactors[j].prevPosition = hit_point;
                }

                //put outside loop
                //				if ((TextureErase || TextureCombINT)) {
                //					InteractTexture [0].SetPixels (originX, originY, 2 * IntTextWidth, 2 * IntTextWidth, pixels);
                //					InteractTexture [0].Apply ();
                //				}
            }

        }//END INTERACTORS UPDATE


        public bool MassErase = false;//enable mass erase in play mode



        void MassEraseUpdate(Texture2D InteractTexture)
        { //v2.1.8 //v2.1.10

            //MASS ERASE
            if (MassErase)
            {
                if (Camera.main != null)
                {
                    Vector3 hit_point = Vector3.zero;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        hit_point = hit.point;

                        //v2.0.8
                        if (TextureInteract)
                        {
                            //PAINT ON TETXURE - InteractTexture
                            float IntTextScale = textureScale;// 0.1f;
                            int IntTextWidth = Mathf.Max(1, (int)(EraseBrushSize * IntTextScale)); // Mathf.Max (1, (int)(10 * IntTextScale));
                                                                                                   //Color[] pixels = InteractTexture [0].GetPixels();
                                                                                                   //Color[] pixels = InteractTexture [0].GetPixels((int)(hit_point.x*IntTextScale)-IntTextWidth, (int)(hit_point.z*IntTextScale)-IntTextWidth, 2*IntTextWidth, 2*IntTextWidth);

                            int originX = (int)(hit_point.x * IntTextScale) - IntTextWidth;
                            int originY = (int)(hit_point.z * IntTextScale) - IntTextWidth;

                            Color[] pixels = new Color[2 * IntTextWidth * 2 * IntTextWidth];

                            bool lerpInteraction = true;

                            //v2.0.9
                            bool inBounds = true;
                            //if(originX < 0 || originY < 0 || 2*IntTextWidth > InteractTexture[0].width || 2*IntTextWidth > InteractTexture[0].height ){
                            if (originX < 0 || originY < 0 || (originX + 2 * IntTextWidth) > InteractTexture.width || (originY + 2 * IntTextWidth) > InteractTexture.height)
                            { //v2.1.1
                                inBounds = false;
                            }

                            if (inBounds)
                            { //v2.0.9
                              //							if (TextureErase && Input.GetMouseButton(1) ) { //TexCombBend
                              //								for (int i = 0; i < pixels.Length; i++) {
                              //									pixels [i] = Color.white * EraseDepth;
                              //								}
                              //							}
                                if (TextureComb && Input.GetMouseButton(1))
                                {
                                    Vector3 motionDir = hit_point - prevMousePos;
                                    float posDiff = (motionDir).magnitude;
                                    if (lerpInteraction)
                                    {

                                        //pixels = InteractTexture.GetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth);
                                        getPixelsSanitized(InteractTexture, ref pixels, originX, originY, IntTextWidth);

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
                                        pixels[i].b = TexCombBend / 255;
                                    }
                                }

                                if (TextureErase && Input.GetMouseButton(1))
                                { //TexCombBend

                                    //v2.1.16
                                    if (useBrushFade)
                                    {

                                        //pixels = InteractTexture.GetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth);
                                        getPixelsSanitized(InteractTexture, ref pixels, originX, originY, IntTextWidth);

                                        for (int i = 0; i < 2 * IntTextWidth; i++)
                                        {
                                            for (int j = 0; j < 2 * IntTextWidth; j++)
                                            {
                                                //Find and normalize distance
                                                float distFromCenter = Mathf.Sqrt(Mathf.Pow(i - IntTextWidth, 2) + Mathf.Pow(j - IntTextWidth, 2)) / Mathf.Sqrt(Mathf.Pow(IntTextWidth, 2) + Mathf.Pow(IntTextWidth, 2));
                                                //Debug.Log("pixels [i * 2 * IntTextWidth + j].r="+pixels [i * 2 * IntTextWidth + j].r+" "+"TO: "+EraseDepth * Mathf.Pow ((1 - distFromCenter), fadeAmount));
                                                if (EraseDepth >= 0)
                                                {
                                                    if (pixels[i * 2 * IntTextWidth + j].r < EraseDepth * Mathf.Pow((1 - distFromCenter), fadeAmount))
                                                    {
                                                        pixels[i * 2 * IntTextWidth + j].r = EraseDepth * Mathf.Pow((1 - distFromCenter), fadeAmount);
                                                    }
                                                }
                                                else
                                                {
                                                    if (pixels[i * 2 * IntTextWidth + j].r > Mathf.Abs(EraseDepth) * Mathf.Pow((distFromCenter), 1 / fadeAmount))
                                                    {
                                                        pixels[i * 2 * IntTextWidth + j].r = Mathf.Abs(EraseDepth) * Mathf.Pow((distFromCenter), 1 / fadeAmount);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < pixels.Length; i++)
                                        {
                                            pixels[i].r = EraseDepth;
                                        }
                                    }
                                }

                                //v2.1.12
                                if (TexturePaintAlpha)
                                {
                                    for (int i = 0; i < pixels.Length; i++)
                                    {
                                        pixels[i].a = ((float)PaintAlphaID) / 255f;
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
                                if ((TextureErase || TextureComb || TexturePaintAlpha) && Input.GetMouseButton(1))
                                { //v2.1.12

                                    //InteractTexture.SetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth, pixels);
                                    setPixelsSanitized(InteractTexture, pixels, originX, originY, IntTextWidth);
                                    InteractTexture.Apply();
                                    
                                }
                            }//END BOUNDS CHECK
                        }
                        //END PAINT ON TEXTURE

                        prevMousePos = hit_point;

                    }
                    if (EraseDefiner != null)
                    {
                        if (Definers.Count == 0)
                        {
                            if (EraseDefiner.GetComponent<Collider>() != null)
                            {
                                EraseDefiner.GetComponent<Collider>().enabled = false;
                            }
                            for (int i = 0; i < EraseCircleSegments; i++)
                            {
                                GameObject TMP = (GameObject)Instantiate(EraseDefiner, Vector3.zero, Quaternion.identity);
                                TMP.SetActive(true);
                                Definers.Add(TMP.transform);
                            }
                        }
                        for (int i = 0; i < EraseCircleSegments; i++)
                        {
                            Vector3 target = Quaternion.AngleAxis(i * (360 / EraseCircleSegments), Vector3.up) * Vector3.forward;
                            Ray ray1 = new Ray();
                            ray1.origin = hit_point + (target * EraseRadious) + new Vector3(0, 100, 0);//also move up 100
                            ray1.direction = -Vector3.up;
                            RaycastHit hit1 = new RaycastHit();
                            if (Physics.Raycast(ray1, out hit1, Mathf.Infinity))
                            {
                                //hit_point = hit.point;
                                Definers[i].position = hit1.point;
                            }
                        }
                    }
                }
            }
            else
            {
                if (Definers.Count > 0)
                {
                    for (int i = Definers.Count - 1; i >= 0; i--)
                    {
                        Destroy(Definers[i].gameObject);


                    }
                    Definers.Clear();
                }
            }
            //END MASS ERASE

            //			if (ConvertGrass) {
            //				GrabGrass ();
            //				ConvertGrass = false;
            //			}

        }//END mass erase UPDATE


        //v2.1.28
        public bool runtimePlant = true;
        void LateUpdate()
        {
            //v2.1.28
            if (runtimePlant && Input.GetMouseButton(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    // int ManagerID = (int)ManagerIDforPosition(hit).y;   //v2.1.30
                    // currentManagerGroupId = (int)ManagerIDforPosition(hit).x;   //v2.1.30
                    //if (ManagerID >= 0 && ManagerID < ManagerGroups[currentManagerGroupId].GrassManagersH.Count)
                    //{
                        //v2.1.18
                        //PlantInPosition(hit,0);//(RaycastHit hit, int layer);
                        //plantOnRaycast(ManagerGroups[currentManagerGroupId].GrassManagersH[ManagerID], this, hit, ray, false); //v2.1.28     
                        plantOnRaycast(this, hit, ray, false); //v2.1.28  
                    //}
                }
            }

            //v2.0.8
            //if (PoolPrefill)
            //{
            //    refillPool(PrefillAmount, 100);//v2.1.23
            //}
            //run late update for managers here

            //v1.8.1
            //for (int m = 0; m < ManagerGroups.Count; m++)
            //{
            //    ManagerGroups[m].LateUpdate();                
            //}

            //v2.0.3
            //if (OneMaterialUpdate)
            //{
            //    if (GrassManager != null)
            //    {
            //        if (GrassManager.gameObject.activeInHierarchy && GrassManager.UpdateMaterials)
            //        {
            //        }
            //        else
            //        {
            //            GrassManager.UpdateAllMaterials();
            //        }
            //    }
            //}
        }



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

                //bool GrassShaping = true;
                if (erasing)
                {

                }
                else if (globalManager.TextureInteract)
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
                        float IntTextScale = textureScale;// 1f; //1.0f;// 0.1f;
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
                                    getPixelsSanitized(globalManager.InternalInteractTexture, ref pixels, originX, originY, IntTextWidth);
                                }
                                else
                                {

                                    //pixels = script.InteractTexture[0].GetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth);
                                    //pixels = globalManager.InteractTexture[0].GetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth);//v2.1.33
                                    getPixelsSanitized(globalManager.InteractTexture[0], ref pixels, originX, originY, IntTextWidth);

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
                                    getPixelsSanitized(globalManager.InternalInteractTexture, ref pixels, originX, originY, IntTextWidth);
                                }
                                else
                                {

                                    //pixels = script.InteractTexture[0].GetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth);
                                    //pixels = globalManager.InteractTexture[0].GetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth);//v2.1.33
                                    getPixelsSanitized(globalManager.InteractTexture[0], ref pixels, originX, originY, IntTextWidth);
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
                                setPixelsSanitized(globalManager.InternalInteractTexture, pixels, originX, originY, IntTextWidth);
                                globalManager.InternalInteractTexture.Apply();
                                
                            }
                            else
                            {
                                //script.InteractTexture[0].SetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth, pixels);
                                //script.InteractTexture[0].Apply();

                                // globalManager.InteractTexture[0].SetPixels(originX, originY, 2 * IntTextWidth, 2 * IntTextWidth, pixels);
                                setPixelsSanitized(globalManager.InteractTexture[0], pixels, originX, originY, IntTextWidth);
                                globalManager.InteractTexture[0].Apply();
                                
                            }
                        }
                    }
                    //END PAINT ON TEXTURE

                    prevMousePos = hit_point;

                }
            }//end v2.1.28
        }



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        //void Update()
        //{

        //}

        public InfiniGRASSManager GrassManager;

        //v2.1.10
        public void AssignShapeTextureToMaterials(Texture2D InternalInteractTexture)
        {
            if (InternalInteractTexture != null)
            {
                for (int i = 0; i < GrassManager.GrassMaterials.Count; i++)
                {
                    if (GrassManager.GrassMaterials[i].HasProperty("_InteractTexture"))
                    {
                        GrassManager.GrassMaterials[i].SetTexture("_InteractTexture", InternalInteractTexture);

                        //Assign tex sizing
                        GrassManager.GrassMaterials[i].SetVector("_InteractTexturePos", new Vector3(0,InternalInteractTexture.width / textureScale, 0));
                    }
                }
                for (int i = 0; i < GrassManager.ExtraGrassMaterials.Count; i++)
                {
                    for (int i1 = 0; i1 < GrassManager.ExtraGrassMaterials[i].ExtraMaterials.Count; i1++)
                    {
                        if (GrassManager.ExtraGrassMaterials[i].ExtraMaterials[i1].HasProperty("_InteractTexture"))
                        {
                            GrassManager.ExtraGrassMaterials[i].ExtraMaterials[i1].SetTexture("_InteractTexture", InternalInteractTexture);

                            //Assign tex sizing
                            GrassManager.ExtraGrassMaterials[i].ExtraMaterials[i1].SetVector("_InteractTexturePos", new Vector3(0, InternalInteractTexture.width / textureScale, 0));
                        }
                    }
                }
            }

            

        }

        ////v2.1.15
        //public void AssignBaseHeightOffsetToMaterials(float offset)
        //{
        //    if (GrassManager.DepthCam != null)
        //    {
        //        for (int i = 0; i < GrassManager.DepthCam.GetComponent<TerrainDepthInfiniGRASS>().DepthToMats.Count; i++)
        //        {
        //            if (GrassManager.DepthCam.GetComponent<TerrainDepthInfiniGRASS>().DepthToMats[i].HasProperty("_BaseHeight"))
        //            {
        //                float startHeight = GrassManager.DepthCam.GetComponent<TerrainDepthInfiniGRASS>().DepthToMats[i].GetFloat("_BaseHeight");
        //                GrassManager.DepthCam.GetComponent<TerrainDepthInfiniGRASS>().DepthToMats[i].SetFloat("_BaseHeight", startHeight + offset);
        //            }
        //        }
        //        for (int i = 0; i < GrassManager.GrassMaterials.Count; i++)
        //        {
        //            if (GrassManager.GrassMaterials[i].HasProperty("_BaseHeight"))
        //            {
        //                float startHeight = GrassManager.GrassMaterials[i].GetFloat("_BaseHeight");
        //                GrassManager.GrassMaterials[i].SetFloat("_BaseHeight", startHeight + offset);
        //            }
        //        }
        //        for (int i = 0; i < GrassManager.ExtraGrassMaterials.Count; i++)
        //        {
        //            for (int i1 = 0; i1 < GrassManager.ExtraGrassMaterials[i].ExtraMaterials.Count; i1++)
        //            {
        //                if (GrassManager.ExtraGrassMaterials[i].ExtraMaterials[i1].HasProperty("_BaseHeight"))
        //                {
        //                    float startHeight = GrassManager.ExtraGrassMaterials[i].ExtraMaterials[i1].GetFloat("_BaseHeight");
        //                    GrassManager.ExtraGrassMaterials[i].ExtraMaterials[i1].SetFloat("_BaseHeight", startHeight + offset);
        //                }
        //            }
        //        }
        //    }
        //}

        ////v2.1.12
        //public void AssignDepthTextureToMaterials()
        //{
        //    if (GrassManager.DepthCam != null)
        //    {
        //        for (int i = 0; i < GrassManager.DepthCam.GetComponent<TerrainDepthInfiniGRASS>().DepthToMats.Count; i++)
        //        {
        //            if (GrassManager.DepthCam.GetComponent<TerrainDepthInfiniGRASS>().DepthToMats[i].HasProperty("_ShoreContourTex"))
        //            {
        //                GrassManager.DepthCam.GetComponent<TerrainDepthInfiniGRASS>().DepthToMats[i].SetTexture("_ShoreContourTex", GrassManager.DepthCam.GetComponent<Camera>().targetTexture);
        //            }
        //        }
        //        for (int i = 0; i < GrassManager.GrassMaterials.Count; i++)
        //        {
        //            if (GrassManager.GrassMaterials[i].HasProperty("_ShoreContourTex"))
        //            {
        //                GrassManager.GrassMaterials[i].SetTexture("_ShoreContourTex", GrassManager.DepthCam.GetComponent<Camera>().targetTexture);
        //            }
        //        }
        //        for (int i = 0; i < GrassManager.ExtraGrassMaterials.Count; i++)
        //        {
        //            for (int i1 = 0; i1 < GrassManager.ExtraGrassMaterials[i].ExtraMaterials.Count; i1++)
        //            {
        //                if (GrassManager.ExtraGrassMaterials[i].ExtraMaterials[i1].HasProperty("_ShoreContourTex"))
        //                {
        //                    GrassManager.ExtraGrassMaterials[i].ExtraMaterials[i1].SetTexture("_ShoreContourTex", GrassManager.DepthCam.GetComponent<Camera>().targetTexture);
        //                }
        //            }
        //        }
        //    }
        //}

        public void resetExternalTextureToColor(Color color)
        {
            Color[] colors = InteractTexture[0].GetPixels();
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = color;
                colors[i].a = ((float)PaintAlphaID) / 255f;//  0; //v2.1.12
            }
            InteractTexture[0].SetPixels(colors);
            InteractTexture[0].Apply();
        }

    }
}