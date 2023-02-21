using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amy;

public enum TerrainType
{
    Default,
    Grass,
    Stone,
    Dirt,
    Wood,
    Water
}


[System.Serializable]
public class FootstepFXRes
{
    public GameObject res_FootFX_Default;
    public GameObject res_FootFX_Dirt;
    public GameObject res_FootFX_Stone;
    public GameObject res_FootFX_Grass;
    public GameObject res_FootFX_Water;

    public GameObject res_SwimFX_Surface;
    public GameObject res_SwimFX_DiveStart;
    public GameObject res_SwimFX_Submerged;

    public void preloadStepFX()
    {
        GameObject.Instantiate(res_FootFX_Default);
        GameObject.Instantiate(res_FootFX_Dirt);
        GameObject.Instantiate(res_FootFX_Grass);
        GameObject.Instantiate(res_FootFX_Stone);
        GameObject.Instantiate(res_FootFX_Water);

        GameObject.Instantiate(res_SwimFX_Surface);
        GameObject.Instantiate(res_SwimFX_DiveStart);
        GameObject.Instantiate(res_SwimFX_Submerged);
    }
}


public class FootstepFX : MonoBehaviour
{

    Animator animator;
    TerrainType currentTerrain;

    public Vector3 rightFootPos;
    public Vector3 leftFootPos;

    float timeOut = 0.02f;
    float timeLeft = 0.02f;

    public float globalVolume = 1.0f;

    FootstepFXRes footFXRes;


    public bool isPlayer = false;
    public Player mPlayer;

    public const float dirtMultiplier = 0.013f;

    float heightOffset = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        footFXRes = GameManager.Instance.systemData.RES_footstepFX;

        if (gameObject.GetComponent<Player>())
        {
            isPlayer = true;
            mPlayer = gameObject.GetComponent<Player>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        getCurrentTerrain();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        rightFootPos = animator.GetIKPosition(AvatarIKGoal.RightFoot);
        leftFootPos = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
    }

    void LeftFoot(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > 0.5)
            FootFX(true);
    }

    void RightFoot(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > 0.5)
            FootFX(false);
    }

    void SwimSurface(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > 0.5)
        {
            Vector3 pos = transform.position;
            SpawnFX(pos, footFXRes.res_SwimFX_Surface);
        }
    }

    void SwimSubmerged(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > 0.5)
        {
            Vector3 pos = transform.position;
            SpawnFX(pos, footFXRes.res_SwimFX_Submerged);
        }
    }


    void FootFX(bool left)
    {

        if(timeLeft > 0.0f)
        {
            timeLeft -= Time.deltaTime;
           // return;
        }

        timeLeft = timeOut;

        Vector3 pos = transform.position;

        if (!left)
            pos = rightFootPos;

        if (left)
            pos = leftFootPos;

        

        if (currentTerrain == TerrainType.Default)
            SpawnFX(pos, footFXRes.res_FootFX_Default);

        if (currentTerrain == TerrainType.Dirt)
            SpawnFX(pos, footFXRes.res_FootFX_Dirt);

        if (currentTerrain == TerrainType.Stone)
            SpawnFX(pos, footFXRes.res_FootFX_Stone);

        if (currentTerrain == TerrainType.Grass)
            SpawnFX(pos, footFXRes.res_FootFX_Grass);


        if (currentTerrain == TerrainType.Water)
            SpawnFX(pos, footFXRes.res_FootFX_Water);


        if (isPlayer)
            playerStep();
    }

    void SpawnFX(Vector3 pos, GameObject prefab)
    {
        GameObject inst = GameObject.Instantiate(prefab);

        inst.transform.position = pos + Vector3.down * heightOffset;
        inst.transform.rotation = transform.rotation;

        if (globalVolume < 1.0f)
        {
            foreach (AudioSource a in inst.GetComponentsInChildren<AudioSource>())
            {
                a.volume *= globalVolume;
            }
        }
    }

    void getCurrentTerrain()
    {
        float length = 0.25f;

        Vector3 start = transform.position + Vector3.up;
        Vector3 end = transform.position - Vector3.up * length;

        Debug.DrawLine(start,end);
        RaycastHit hitInfo = new RaycastHit();

        LayerMask mask = LayerMask.GetMask("Collision","Water");

        if(Physics.Linecast(start,end,out hitInfo, mask))
        {
            heightOffset = transform.position.y - hitInfo.point.y;

            TerrainProperties prop = hitInfo.collider.GetComponent<TerrainProperties>();

            if (prop == null)
                return;

            currentTerrain = prop.getTerrainType();
        }

    }

    void playerStep()
    {

        //Chance of adding dirt
        if (Random.Range(0, 1) > 0.75f)
            return;

        /*
        if (currentTerrain == TerrainType.Dirt)
            PlayerManager.Instance.getCharacterStatus(mPlayer.mChara).dirtiness += (0.02f * dirtMultiplier);

        if (currentTerrain == TerrainType.Grass)
            PlayerManager.Instance.getCharacterStatus(mPlayer.mChara).dirtiness += (0.01f * dirtMultiplier);

        if (currentTerrain == TerrainType.Stone)
            PlayerManager.Instance.getCharacterStatus(mPlayer.mChara).dirtiness += (0.0001f * dirtMultiplier);

        if (currentTerrain == TerrainType.Water)
            PlayerManager.Instance.getCharacterStatus(mPlayer.mChara).dirtiness -= 0.05f;

        if (currentTerrain == TerrainType.Wood)
            PlayerManager.Instance.getCharacterStatus(mPlayer.mChara).dirtiness += (0.005f * dirtMultiplier);
        */
    }
}
