using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WaterSurfaceSettings", menuName = "Water/WaterSurfaceSettings")]
public class WaterSurfaceSettings : ScriptableObject
{ 
    [Range(2, 256)]
    public int waterResolution = 10;//number of vertex in one side of the plane
    [Range(1, 1000)]
    public float waterSurfaceSide = 10;//meters
    public Material waterMaterial;

}
