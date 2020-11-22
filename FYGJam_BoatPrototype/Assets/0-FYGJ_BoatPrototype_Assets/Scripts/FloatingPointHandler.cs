using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPointHandler : MonoBehaviour
{
    public bool showDebugSphere = false;

    private Water waterSurface;
    private float waterHeight = 0;
    public float WaterHeight { get { return waterHeight; } }

    private bool initialized = false;


    public void OnValidate()
    {
        transform.GetChild(0).gameObject.SetActive(showDebugSphere);
    }
    public void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);

        //Getting first by default (There should only be one water plane in this project)
        GameObject WaterGO = GameObject.FindGameObjectsWithTag(Taglist.waterTag)[0];
        if (WaterGO == null)
        {
            Debug.LogError("[FloatingPointHandler] WaterSurface object not found");
            return;
        }
        waterSurface = WaterGO.GetComponent<Water>();
        waterHeight = waterSurface.transform.position.y;
        initialized = true;
    }

    void FixedUpdate()
    {
        if(initialized)
            waterHeight = waterSurface.GetWaveHeight(transform.position);
    }


    }
