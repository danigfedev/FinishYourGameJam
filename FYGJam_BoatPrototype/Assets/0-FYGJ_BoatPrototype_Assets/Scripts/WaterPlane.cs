using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlane
{
    Mesh waterMesh;
    int waterPlaneResolution;
    float waterPlaneSide;
    Vector3 horizontalAxis = Vector3.right;
    Vector3 verticalAxis = Vector3.forward;

    public WaterPlane(Mesh waterMesh, int waterPlaneResolution, float sideSize)
    {
        this.waterMesh = waterMesh;
        this.waterPlaneResolution = waterPlaneResolution;
        this.waterPlaneSide = sideSize;
    }

    public void GenerateWaterMesh()
    {
        Vector3[] vertices = new Vector3[waterPlaneResolution * waterPlaneResolution];
        int[] triangles = new int[(int)Mathf.Pow(waterPlaneResolution - 1, 2) * 2 * 3]; //(r-1)^2 quads; 2 tris per quad; 3 vertices per tri

        int triangleIndex = 0;
        //TODO Loop the matrix generating 
        for(int x=0; x < waterPlaneResolution; x++)
        {
            for(int y=0; y< waterPlaneResolution; y++)
            {
                //1-calculate value of next vertex
                float offset = waterPlaneSide / (waterPlaneResolution - 1);
                int vertexIndex = y + x * waterPlaneResolution;
                Vector3 horComponent = (x * offset - waterPlaneSide / 2) * horizontalAxis;
                Vector3 vertComponent = (y * offset - waterPlaneSide / 2) * verticalAxis;
                vertices[vertexIndex] = horComponent + vertComponent;


                //2-Calculate triangle indices
                if(x!=waterPlaneResolution-1 && y!= waterPlaneResolution - 1)
                {
                    //First tri in quad
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + waterPlaneResolution + 1;
                    triangles[triangleIndex + 2] = vertexIndex + waterPlaneResolution;

                    //Second tri in quad
                    triangles[triangleIndex + 3] = vertexIndex;
                    triangles[triangleIndex + 4] = vertexIndex + 1;
                    triangles[triangleIndex + 5] = vertexIndex + waterPlaneResolution + 1;

                    triangleIndex += 6;
                }

            }
        }

        waterMesh.Clear();
        waterMesh.vertices = vertices;
        waterMesh.triangles = triangles;
        waterMesh.RecalculateNormals();

    }
}
