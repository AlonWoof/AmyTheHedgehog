using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jason Haden */

public class CharacterPhysics : MonoBehaviour
{
    public CharacterPhysicsData mData;
    public bool unscaledTime = false;

    public Vector3 currentWind;
    public Vector3 desiredWind;

    DynamicBone[] mBones;
    DynamicBoneCollider[] mColliders;

    float weight = 1.0f;

    // Start is called before the first frame update
    // Use this for initialization
    void Start()
    {

        if (!mData)
        {
            enabled = false;
            return;
        }

        addBones();

    }

    void clearBones()
    {
        foreach (DynamicBone d in GetComponents<DynamicBone>())
        {
            Destroy(d);
        }
    }

    void addBones()
    {
        for (int i = 0; i < mData.mBones.Length; i++)
        {
            DynamicBone db = addBoneWithPreset(mData.mBones[i]);
            //db.useUnscaledTime = unscaledTime;
            //db.m_UpdateRate = 30;


            if (unscaledTime)
                db.m_UpdateMode = DynamicBone.UpdateMode.UnscaledTime;
            else
                db.m_UpdateMode = DynamicBone.UpdateMode.AnimatePhysics;

        }

        mBones = GetComponents<DynamicBone>();
    }

    DynamicBone addBoneWithPreset(DynamicBonePreset preset)
    {
        DynamicBone db = gameObject.AddComponent<DynamicBone>();

        db.m_Root = findBoneByName(preset.m_Root).transform;
        db.m_Stiffness = preset.m_Stiffness;
        db.m_StiffnessDistrib = preset.m_StiffnessDistrib;
        db.m_UpdateRate = preset.m_UpdateRate;
        //db.m_Colliders = preset.m_Colliders;
        db.m_Damping = preset.m_Damping;
        db.m_DampingDistrib = preset.m_DampingDistrib;
        db.m_DistanceToObject = preset.m_DistanceToObject;
        db.m_DistantDisable = preset.m_DistantDisable;
        db.m_Elasticity = preset.m_Elasticity;
        db.m_ElasticityDistrib = preset.m_ElasticityDistrib;
        db.m_EndLength = preset.m_EndLength;
        db.m_EndOffset = preset.m_EndOffset;
        db.m_Force = preset.m_Force;
        //m_FreezeAxis = db.m_FreezeAxis;
        db.m_Gravity = preset.m_Gravity;
        db.m_Inert = preset.m_Inert;
        db.m_InertDistrib = preset.m_InertDistrib;
        db.m_Radius = preset.m_Radius;
        db.m_RadiusDistrib = preset.m_RadiusDistrib;

        db.SetWeight(0.75f);

        return db;
    }

    public GameObject findBoneByName(string name)
    {
        foreach (Transform bone in GetComponentInChildren<SkinnedMeshRenderer>().bones)
        {
            if (bone.name.ToLower() == name.ToLower())
            {
                return bone.gameObject;
            }
        }

        return null;
    }

    public void ResetPhysics()
    {
        if (!GetComponent<DynamicBone>())
            return;

        if (mBones == null)
            mBones = GetComponents<DynamicBone>();

        foreach (DynamicBone d in mBones)
        {
            
        }
    }
}

