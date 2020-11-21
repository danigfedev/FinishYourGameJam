using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Water : MonoBehaviour
{
    //public WaterSurfaceSettings waterSettings;
    [Range(2, 256)]
    public int waterResolution = 5;
    public float waterSurfaceSide = 10;
    public Material waterMaterial;
    
    private WaterPlane waterPlane;
    private float waveOffset;

    private void OnValidate()
    {
        InitializeWater();
    }

    public void Update()
    {
        UpdateWaveOffset();
    }

    private void InitializeWater()
    {
        //1-Generate water's mesh
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter.sharedMesh == null)
            meshFilter.sharedMesh = new Mesh();
        waterPlane = new WaterPlane(meshFilter.sharedMesh, waterResolution, waterSurfaceSide);
        waterPlane.GenerateWaterMesh();

        //2-Assign water material
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = waterMaterial;
    }

    public float GetWaveHeight(Vector3 point)
    {
        Vector3 localPoint = point - transform.position;
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Vector3 originCorner = meshFilter.sharedMesh.vertices[0];
        localPoint = localPoint - originCorner;

        float horLength = localPoint.x;
        float vertLength = localPoint.z;

        float quadSide = waterSurfaceSide / (waterResolution-1); //width / Numer of quads

        int Nvx = (int)Mathf.Floor(horLength / quadSide);
        int Nvz = (int)Mathf.Floor(vertLength / quadSide);

        int vertexIndex = (Nvz) + (Nvx) * waterResolution;

        Vector3 quadVertex1 = meshFilter.sharedMesh.vertices[vertexIndex];
        Vector3 quadVertex2 = meshFilter.sharedMesh.vertices[vertexIndex + waterResolution];
        Vector3 quadVertex3 = meshFilter.sharedMesh.vertices[vertexIndex + waterResolution + 1];
        Vector3 quadVertex4 = meshFilter.sharedMesh.vertices[vertexIndex + 1];

        //DebugQuad(quadVertex1, quadVertex2, quadVertex3, quadVertex4);

        //Get tri by barycentric technique
        Vector3 center;
        if (CheckpointPointInTriangle(localPoint, quadVertex1 - originCorner, quadVertex2-originCorner, quadVertex3-originCorner))
        {
            //first tri
            center = GetTriangleCenter(quadVertex1, quadVertex2, quadVertex3);
            float waterHeight = CalculateWaveValue(center);
            return waterHeight + transform.position.y;
        }
        else if(CheckpointPointInTriangle(localPoint, quadVertex1 - originCorner, quadVertex3 - originCorner, quadVertex4 - originCorner))
        {
            //second tri
            center = GetTriangleCenter(quadVertex1, quadVertex2, quadVertex3);
            float waterHeight = CalculateWaveValue(center);
            return waterHeight + transform.position.y;
        }

        return float.NaN;
    }

    List<GameObject> debugSphereList;
    private void DebugQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        if (debugSphereList == null)
            debugSphereList = new List<GameObject>();

        foreach (GameObject sphere in debugSphereList)
            Destroy(sphere);
        debugSphereList.Clear();

        //Update height:
        p1.y = CalculateWaveValue(p1);
        p2.y = CalculateWaveValue(p2);
        p3.y = CalculateWaveValue(p3);
        p4.y = CalculateWaveValue(p4);

        List<Vector3> positions = new List<Vector3>();
        positions.Add(p1);
        positions.Add(p2);
        positions.Add(p3);
        positions.Add(p4);

        for(int i=0; i< 4; i++)
        {
            GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debugSphere.transform.position = positions[i];
            debugSphere.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            Destroy(debugSphere.GetComponent<Collider>());
            debugSphereList.Add(debugSphere);
        }

    }

    private void DebugSphere(Vector3 p1)
    {
        GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugSphere.transform.position = p1;
        debugSphere.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        Destroy(debugSphere.GetComponent<Collider>());
    }

    private void UpdateWaveOffset()
    {
        float speed = waterMaterial.GetFloat("_WaveSpeed");
        waveOffset += speed * Time.deltaTime;
    }

    private float CalculateWaveValue(Vector3 point)
    {
        float amplitude = waterMaterial.GetFloat("_WaveAmplitude");
        float length = waterMaterial.GetFloat("_WaveLength");
        //float speed = waterMaterial.GetFloat("_WaveSpeed");
        //float offset = speed * Time.deltaTime;
        
        float waveHeight = amplitude * Mathf.Sin(point.x / length + waveOffset);
        //Debug.Log("Wave height: " + waveHeight);
        return waveHeight;
    }

    private bool CheckpointPointInTriangle(Vector3 point, Vector3 A, Vector3 B, Vector3 C)
    {
        //Remove height:
        point = Vector3.Scale(point, new Vector3(1, 0, 1));
        A = Vector3.Scale(A, new Vector3(1, 0, 1));
        B = Vector3.Scale(B, new Vector3(1, 0, 1));
        C = Vector3.Scale(C, new Vector3(1, 0, 1));


        // Compute vectors        
        Vector3 v0 = C - A;
        Vector3 v1 = B - A;
        Vector3 v2 = point - A;

        // Compute dot products
        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        // Compute barycentric coordinates
        float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        //Debug.Log(invDenom);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        // Check if point is in triangle
        return (u >= 0) && (v >= 0) && (u + v < 1);
    }

    private Vector3 GetTriangleCenter(Vector3 A, Vector3 B, Vector3 C)
    {
        return (A + B + C) / 3;
    }
}
