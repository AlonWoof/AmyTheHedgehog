using UnityEngine;
using System.Collections;

public class AnimatedUVs : MonoBehaviour
{
    public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
    public string textureName = "_MainTex";

    bool forProjector = false;

    Vector2 uvOffset = Vector2.zero;
    void LateUpdate()
    {
        uvOffset += (uvAnimationRate * Time.deltaTime);
        if (GetComponent<Renderer>())
        {
            GetComponent<Renderer>().materials[materialIndex].SetTextureOffset(textureName, uvOffset);
        }

        if(GetComponent<Projector>())
        {
            GetComponent<Projector>().material.SetTextureOffset(textureName, uvOffset);
        }
    }



}
