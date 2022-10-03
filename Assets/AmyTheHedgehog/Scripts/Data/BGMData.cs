using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New BGMData", menuName = "BGMData", order = 51)]
public class BGMData : ScriptableObject
{
    public string songName;
    public int songNameHash;


    public AudioClip introClip;
    public AudioClip mainClip;
    public AudioClip outroClip;

    public float volumeMult = 0.5f;

    private void OnValidate()
    {
        songNameHash = Animator.StringToHash(songName);
    }
}
