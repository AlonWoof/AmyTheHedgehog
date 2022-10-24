using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    [ExecuteInEditMode]
    public class Mirror : MonoBehaviour
    {
        Camera mainCam;
        Camera mirrorCam;

        public RenderTexture mRenderTexture;
        bool renderInProgress = false;

        Renderer mirrorRenderer;
        MaterialPropertyBlock propertyBlock;

        int refTexParam;

        // Start is called before the first frame update
        void Start()
        {
            refTexParam = Shader.PropertyToID("_ReflectionTex");
            mirrorRenderer = GetComponent<Renderer>();
            propertyBlock = new MaterialPropertyBlock();
        }


        private static Matrix4x4 calcReflectionMatrix(Vector4 plane)
        {
            Matrix4x4 ret = Matrix4x4.identity;
            ret.m00 = 1f - 2f * plane[0] * plane[0];
            ret.m01 = -2f * plane[0] * plane[1];
            ret.m02 = -2f * plane[0] * plane[2];
            ret.m03 = -2f * plane[3] * plane[0];
            ret.m10 = -2f * plane[1] * plane[0];
            ret.m11 = 1f - 2f * plane[1] * plane[1];
            ret.m12 = -2f * plane[1] * plane[2];
            ret.m13 = -2f * plane[3] * plane[1];
            ret.m20 = -2f * plane[2] * plane[0];
            ret.m21 = -2f * plane[2] * plane[1];
            ret.m22 = 1f - 2f * plane[2] * plane[2];
            ret.m23 = -2f * plane[3] * plane[2];
            ret.m30 = 0f;
            ret.m31 = 0f;
            ret.m32 = 0f;
            ret.m33 = 1f;
            return ret;
        }

        Vector4 getCameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal)
        {
            Matrix4x4 matrix4x4 = cam.worldToCameraMatrix;
            Vector3 vector3 = matrix4x4.MultiplyPoint(pos);
            Vector3 vector31 = matrix4x4.MultiplyVector(normal);
            return Helper.Plane(vector3, vector31.normalized);
        }

        private static Vector3 getPositionFromMatrix(Matrix4x4 matrix)
        {
            return new Vector3(matrix.m03, matrix.m13, matrix.m23);
        }

        private static float copySign(float sizeValue, float signValue)
        {
            return Mathf.Sign(signValue) * Mathf.Abs(sizeValue);
        }

        private static Quaternion getRotationFromMatrix(Matrix4x4 matrix)
        {
            Quaternion q1 = new Quaternion()
            {
                w = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 + matrix.m11 + matrix.m22)) / 2f,
                x = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 - matrix.m11 - matrix.m22)) / 2f,
                y = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 + matrix.m11 - matrix.m22)) / 2f,
                z = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 - matrix.m11 + matrix.m22)) / 2f
            };
            Quaternion q2 = q1;
            q2.x = copySign(q2.x, matrix.m21 - matrix.m12);
            q2.y = copySign(q2.y, matrix.m02 - matrix.m20);
            q2.z = copySign(q2.z, matrix.m10 - matrix.m01);
            return q2;
        }


        void updateMirrorCamera()
        {

            mirrorCam.ResetWorldToCameraMatrix();
            mirrorCam.transform.position = mainCam.transform.position;
            mirrorCam.transform.rotation = mainCam.transform.rotation;
            mirrorCam.projectionMatrix = mainCam.projectionMatrix;
            mirrorCam.fieldOfView = mainCam.fieldOfView;
            mirrorCam.clearFlags = mainCam.clearFlags;
            mirrorCam.cullingMask = mainCam.cullingMask;

            mirrorCam.targetTexture = mRenderTexture;

            Vector4 mirrorPlane = Helper.Plane(transform.position, -transform.forward);
            mirrorCam.worldToCameraMatrix = mirrorCam.worldToCameraMatrix * calcReflectionMatrix(mirrorPlane);

            Vector4 camSpacePlane = getCameraSpacePlane(mirrorCam, transform.position, -transform.forward);

            mirrorCam.projectionMatrix = (mirrorCam.CalculateObliqueMatrix(camSpacePlane));
            mirrorCam.transform.position = getPositionFromMatrix(mirrorCam.cameraToWorldMatrix);
            mirrorCam.transform.rotation = getRotationFromMatrix(mirrorCam.cameraToWorldMatrix);

            bool flag = GL.invertCulling;
            GL.invertCulling = !flag;
            mirrorCam.Render();
            GL.invertCulling = flag;

        }

        private void OnDestroy()
        {
            if (mirrorCam == null)
                return;

            if (Application.isEditor)
                Object.DestroyImmediate(mirrorCam.gameObject);
            else
                Object.Destroy(mirrorCam.gameObject);

            mirrorCam = null;
        }


        private void OnWillRenderObject()
        {
            if (renderInProgress)
                return;

            if (!mirrorCam)
            {
                GameObject oldCam = GameObject.Find("MirrorCam");

                if (oldCam)
                {
                    if (Application.isEditor)
                    {
                        DestroyImmediate(oldCam);
                    }
                    else
                    {
                        Destroy(oldCam);
                    }
                }

                mirrorCam = new GameObject("MirrorCam", typeof(Camera)).GetComponent<Camera>();
            }

            if(!mirrorRenderer)
                mirrorRenderer = GetComponent<Renderer>();

            if(propertyBlock == null)
                propertyBlock = new MaterialPropertyBlock();

            mainCam = Camera.current;

            if (!mainCam || mainCam == mirrorCam)
                return;

            if (!mRenderTexture)
            {
                mRenderTexture = new RenderTexture(mainCam.pixelWidth * 2, mainCam.pixelHeight * 2, 24, RenderTextureFormat.ARGBHalf, 4);
            }



            renderInProgress = true;

            updateMirrorCamera();

            propertyBlock.SetTexture("_refTex", mRenderTexture);

            mirrorRenderer.SetPropertyBlock(propertyBlock);

            renderInProgress = false;
        }

    }
}
