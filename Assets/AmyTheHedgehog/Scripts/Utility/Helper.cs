using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{
    public static float Remap(this float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }


}


public static class Helper
{

    public struct ClipPlanePoints
    {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
    }

    public static float horizontalDistance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(zeroAltitude(a), zeroAltitude(b));
    }

    public static float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }


    public static Vector3 zeroAltitude(Vector3 a)
    {
        a.y = 0;
        return a;
    }

    public static Vector3 GetMeanVector(Vector3[] positions)
    {
        if (positions.Length == 0)
            return Vector3.zero;
        float x = 0f;
        float y = 0f;
        float z = 0f;
        foreach (Vector3 pos in positions)
        {
            x += pos.x;
            y += pos.y;
            z += pos.z;
        }
        return new Vector3(x / positions.Length, y / positions.Length, z / positions.Length);
    }


    public static Vector4 Plane(Vector3 pos, Vector3 normal)
    {
        return new Vector4(normal.x, normal.y, normal.z, -Vector3.Dot(pos, normal));
    }


    public static Transform findInChildrenByName(GameObject obj, string name)
    {
        Transform[] trans = obj.GetComponentsInChildren<Transform>();

        foreach (Transform tr in trans)
        {
            if (tr.gameObject.name == name)
                return tr;

        }

        return null;
    }



    public static float LerpWithThreshold(float a,float b,float fac,float threshold)
    {

        float dif = (a - Mathf.Abs(b));

        if (dif < threshold)
            return b;
        else
            return Mathf.Lerp(a, b, fac);

    }

    public static float getAngleToDirection(Vector3 referenceForward, Vector3 newDirection)
    {
        Vector3 referenceRight = Vector3.Cross(Vector3.up, referenceForward);
        float angle = Vector3.Angle(newDirection, referenceForward);
        // Determine if the degree value should be negative.  Here, a positive value
        // from the dot product means that our vector is on the right of the reference vector   
        // whereas a negative value means we're on the left.
        float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));
        float finalAngle = sign * angle;

        return finalAngle;
    }

    public static bool isCollidable(Collider col)
    {
        bool collidable = true;

        if (col.isTrigger)
            collidable = false;

        if (col.gameObject.layer == LayerMask.NameToLayer("Actor"))
            collidable = false;

        if (col.gameObject.layer == LayerMask.NameToLayer("IgnoreCamera"))
            collidable = false;

        if (col.gameObject.layer == LayerMask.NameToLayer("IgnoreRaycast"))
            collidable = false;

        if (col.gameObject.layer == LayerMask.NameToLayer("Water"))
            collidable = false;

        return collidable;
    }

    public static float getDistance(float a, float b)
    {
        return Mathf.Abs(a - b);
    }

    public static Vector3 getDirectionTo(Vector3 a, Vector3 b)
    {
        Vector3 heading = a - b;

        float distance = heading.magnitude;
        Vector3 direction = heading / distance; // This is now the normalized direction.

        return -direction;
    }



    public static float ClampAngle(float angle, float min, float max)
    {
        do
        {
            if (angle < -360)
                angle += 360;

            if (angle > 360)
                angle -= 360;

        } while (angle < -360 || angle > 360);

        return Mathf.Clamp(angle, min, max);
    }

    public static float remapRange(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

        public static ClipPlanePoints ClipPlaneAtNear(Vector3 pos)
    {
        var clipPlanePoints = new ClipPlanePoints();

        if (Camera.main == null)
            return clipPlanePoints;

        var transform = Camera.main.transform;
        var halfFOV = (Camera.main.fieldOfView / 2) * Mathf.Deg2Rad;
        var aspect = Camera.main.aspect;
        var distance = Camera.main.nearClipPlane;
        var height = distance * Mathf.Tan(halfFOV);
        var width = height * aspect;

        clipPlanePoints.LowerRight = pos + transform.right * width;
        clipPlanePoints.LowerRight -= transform.up * height;
        clipPlanePoints.LowerRight += transform.forward * distance;

        clipPlanePoints.LowerLeft = pos - transform.right * width;
        clipPlanePoints.LowerLeft -= transform.up * height;
        clipPlanePoints.LowerLeft += transform.forward * distance;

        clipPlanePoints.UpperRight = pos + transform.right * width;
        clipPlanePoints.UpperRight += transform.up * height;
        clipPlanePoints.UpperRight += transform.forward * distance;

        clipPlanePoints.UpperLeft = pos - transform.right * width;
        clipPlanePoints.UpperLeft += transform.up * height;
        clipPlanePoints.UpperLeft += transform.forward * distance;

        return clipPlanePoints;
    }
}
