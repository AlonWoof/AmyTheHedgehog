using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jason Haden */

[System.Serializable]
[CreateAssetMenu(fileName = "New CharacterPhysicsData", menuName = "CharacterPhysicsData", order = 51)]
public class CharacterPhysicsData : ScriptableObject
{

    public DynamicBonePreset[] mBones;
    public DynamicBoneColliderPreset[] mColliders;


}
