using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveSettings", menuName = "Water/WaveSettings")]
public class WaveSettings : ScriptableObject
{
    public float waveAmplitude = 1;
    public float waveSpeed = 1;
    public float waveLenth = 1;
}
