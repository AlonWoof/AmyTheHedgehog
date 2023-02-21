using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainProperties : MonoBehaviour
{

    [SerializeField] TerrainType defaultType;

    public TerrainType getTerrainType()
    {
        return defaultType;
    }
}
