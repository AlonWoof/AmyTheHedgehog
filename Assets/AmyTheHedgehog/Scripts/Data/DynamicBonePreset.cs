using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jason Haden */

[System.Serializable]
public class DynamicBonePreset
{

    public string m_Root = null;
    public float m_UpdateRate = 60.0f;
    [Range(0, 1)]
    public float m_Damping = 0.1f;
    public AnimationCurve m_DampingDistrib = null;
    [Range(0, 1)]
    public float m_Elasticity = 0.1f;
    public AnimationCurve m_ElasticityDistrib = null;
    [Range(0, 1)]
    public float m_Stiffness = 0.1f;
    public AnimationCurve m_StiffnessDistrib = null;
    [Range(0, 1)]
    public float m_Inert = 0;
    public AnimationCurve m_InertDistrib = null;
    public float m_Radius = 0;
    public AnimationCurve m_RadiusDistrib = null;

    public float m_EndLength = 0;
    public Vector3 m_EndOffset = Vector3.zero;
    public Vector3 m_Gravity = Vector3.zero;
    public Vector3 m_Force = Vector3.zero;
    public List<DynamicBoneCollider> m_Colliders = null;
    public List<Transform> m_Exclusions = null;
    public enum FreezeAxis
    {
        None, X, Y, Z
    }
    public FreezeAxis m_FreezeAxis = FreezeAxis.None;
    public bool m_DistantDisable = false;
    public string m_ReferenceObject = null;
    public float m_DistanceToObject = 20;


}

[System.Serializable]

public class DynamicBoneColliderPreset
{
    public string m_Root = null;
    public Vector3 m_Center = Vector3.zero;
    public float m_Radius = 0.5f;
    public float m_Height = 0;
    public DynamicBoneCollider.Direction m_Direction = DynamicBoneCollider.Direction.X;
    public DynamicBoneCollider.Bound m_Bound = DynamicBoneCollider.Bound.Outside;

}

