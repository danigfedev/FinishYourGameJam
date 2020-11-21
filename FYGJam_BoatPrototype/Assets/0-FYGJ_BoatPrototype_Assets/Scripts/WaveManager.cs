using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public WaveSettings waveSettings;
    //public float waveAmplitude = 0;
    //public float waveSpeed = 1;
    //public float waveLength = 1;
    
    private bool waving = false;
    private float waveOffset = 0;

    public void ResetWaves()
    {
        waving = false;
    }

    public void InitializeWaves()
    {
        waving = true;
    }

    
    void Update()
    {
        if (!waving)
            return;

        UpdateWaves(gameObject.GetComponent<MeshFilter>().sharedMesh);

    }

    private void UpdateWaves(Mesh waterMesh)
    {
        Vector3[] newVertices = waterMesh.vertices;

        waveOffset += waveSettings.waveSpeed * Time.deltaTime;

        for (int i=0; i< newVertices.Length; i++)
        {
            //X axis wave
            float horValue = (transform.position.x + newVertices[i].x);
            float waveHeight = waveSettings.waveAmplitude * Mathf.Sin(horValue/ waveSettings.waveLenth + waveOffset);
            newVertices[i] = new Vector3(newVertices[i].x, waveHeight, newVertices[i].z);

            //TODO Z axis wave
        }

        waterMesh.vertices = newVertices;
        waterMesh.RecalculateNormals();

        //GetComponent<MeshFilter>().sharedMesh = waterMesh;
        //gameObject.GetComponent<MeshCollider>().sharedMesh = waterMesh;
    }

}
