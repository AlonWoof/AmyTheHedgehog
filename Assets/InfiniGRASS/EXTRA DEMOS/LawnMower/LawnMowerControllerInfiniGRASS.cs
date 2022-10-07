using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Artngame.INfiniDy
{
    public class LawnMowerControllerInfiniGRASS : MonoBehaviour
    {
        public Transform lawnMower;
        public Transform lawnMowerBlades;
        public float distFromGround = 1;

        public List<Material> grassMats = new List<Material>();

        public ShapeFoliageInfiniGRASS grassShaper;

        // Start is called before the first frame update
        void Start()
        {
            grassShaper.resetExternalTextureToColor(Color.black);
        }

        public float heliWindAmplitude = 0;
        public float heliWindFrequency = 0;
        public float heliWindRadious = 0;

        public GameObject grassParticle;
        int pixelsFound = 0;
        public float stopParticleDelay = 1;

        // Update is called once per frame
        void Update()
        {


            //GET STATE
            float EraseBrushSizeINT = grassShaper.EraseBrushSize;
            float IntTextScale = grassShaper.textureScale;
            int IntTextWidth = Mathf.Max(1, (int)(EraseBrushSizeINT * IntTextScale));                                                                                     
            int originX = (int)(lawnMower.position.x * IntTextScale) - IntTextWidth;
            int originY = (int)(lawnMower.position.z * IntTextScale) - IntTextWidth;
            Color[] pixels = new Color[2 * IntTextWidth * 2 * IntTextWidth];

            //v2.1.10
            if (grassShaper.useInternalInteractTexture && grassShaper.InternalInteractTexture != null)
            { //NEW
                //InteractorsUpdate(InternalInteractTexture);
                //MassEraseUpdate(InternalInteractTexture);
            }
            else
            {
                //v2.1.8
                if (grassShaper.InteractTexture.Count > 0 && grassShaper.InteractTexture[0] != null)
                {
                    //InteractorsUpdate(InteractTexture[0]);
                    //MassEraseUpdate(InteractTexture[0]);
                    grassShaper.getPixelsSanitized(grassShaper.InteractTexture[0], ref pixels, originX, originY, IntTextWidth);
                    // int pixelsFound = 0;
                    
                    for (int i = 0; i < pixels.Length; i++)
                    {
                        if (pixels[i].r < 0.5f)
                        {
                            pixelsFound = 0;
                            //Debug.Log(pixels[i]);
                            if (!grassParticle.activeInHierarchy)
                            {
                                grassParticle.SetActive(true);
                            }
                            break;
                        }
                        else
                        {
                            if (pixelsFound > 63*100*stopParticleDelay)
                            {
                                if (grassParticle.activeInHierarchy)
                                {
                                    grassParticle.SetActive(false);
                                }
                                pixelsFound = 0;
                            }
                            else
                            {
                                pixelsFound++;
                            }
                        }
                        //if (!grassParticle.activeInHierarchy)
                        //{
                        //    grassParticle.SetActive(true);
                        //}
                        //if (pixels[i].r > 0.7f)
                        //{
                        //    //Debug.Log(pixelsFound);
                        //    if (pixelsFound == 63)
                        //    {

                        //        //Debug.Log(pixels[i]);
                        //        if (grassParticle.activeInHierarchy)
                        //        {
                        //            grassParticle.SetActive(false);
                        //        }
                        //        break;
                        //    }
                        //    else
                        //    {
                        //        pixelsFound++;
                        //    }
                        //}
                    }
                }
            }
           





            for (int i = 0; i < grassMats.Count; i++)
            {
                //_InteractAmpFreqRad("_InteractAmpFreqRadial", Vector) = (1, 1, 1)
                //_InteractPos2
                grassMats[i].SetVector("_InteractPos2", lawnMowerBlades.position);

                grassMats[i].SetVector("_InteractAmpFreqRad", new Vector4(heliWindAmplitude, heliWindFrequency, heliWindRadious, 0));
            }

            if (lawnMower != null)
            {
                Ray ray = new Ray(lawnMower.position + new Vector3(0, 10, 0), -Vector3.up);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit, 100))
                {
                    lawnMower.position = new Vector3(lawnMower.position.x, hit.point.y + distFromGround, lawnMower.position.z);
                    //lawnMower.eulerAngles = new Vector3(lawnMower.eulerAngles.x, lawnMower.eulerAngles.y + 20 * Time.deltaTime, lawnMower.eulerAngles.z);
                    lawnMower.up = Vector3.Lerp(lawnMower.up, hit.normal, Time.deltaTime*100);

                }
            }
        }
        public float rotateSpeed =1;

        public int updateUpRate = 5;
        int currentUpdate = 0;
        void LateUpdate()
        {
            currentUpdate++;
            if(currentUpdate > updateUpRate)
            {
                Ray ray = new Ray(lawnMowerBlades.position + new Vector3(0, 10, 0), -Vector3.up);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit, 100))
                {
                    lawnMowerBlades.up = Vector3.Lerp(lawnMowerBlades.up, hit.normal, Time.deltaTime * 100);
                }
                currentUpdate = 0;

            }


            lawnMowerBlades.eulerAngles = new Vector3(lawnMowerBlades.eulerAngles.x, lawnMowerBlades.eulerAngles.y + rotateSpeed* 200 * Time.deltaTime, lawnMowerBlades.eulerAngles.z);
            //Ray ray = new Ray(lawnMower.position + new Vector3(0, 10, 0), -Vector3.up);
            //RaycastHit hit = new RaycastHit();
            //if (Physics.Raycast(ray, out hit, 100))
            //{
            //    //lawnMower.up = Vector3.Lerp(lawnMower.up, hit.normal, Time.deltaTime);
            //}
        }
    }
}