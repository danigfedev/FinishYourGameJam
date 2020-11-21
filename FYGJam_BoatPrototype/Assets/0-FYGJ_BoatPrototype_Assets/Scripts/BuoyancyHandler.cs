using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuoyancyHandler : MonoBehaviour
{
    [Header("Physics properties")]
    public float airDrag = 1;
    public float waterDrag = 10;

    private FloatingPointHandler[] floatingPoints;
    private Vector3 floatingOffset;
    private Vector3 avgWaterLine;
    private Rigidbody rigidBody;
    private Vector3 smoothVectorRotation;


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.useGravity = false;

        floatingPoints = transform.
                        GetComponentsInChildren<FloatingPointHandler>();

        Vector3 floatingPointsAvgCenter = GeometryHelper.GetCenterPoint(GetFloatingPointPositions());
        floatingOffset = floatingPointsAvgCenter - transform.position;
    }

    
    void FixedUpdate()
    {
        //1- Get Water line and floating points' position
        Vector3[] floatingPointsPos = GetFloatingPointPositions();
        Vector3[] waterLinePoints = GetWaterHeightPoints();

        //2-Calculate avg water surface point
        avgWaterLine = GeometryHelper.GetCenterPoint(waterLinePoints);
        Vector3 newFloatingOffset = avgWaterLine - transform.position;

        //3-Update object's vertical position depending on water line avg position
        //and object's position
        
        //Calculating theoreticial waterLine based in initial offset 
        //(theoretically perpendicular to the water's surface)
        Vector3 altWaterLine = transform.position + floatingOffset;
        rigidBody.drag = airDrag;
        if (avgWaterLine.y > altWaterLine.y)
        {
            rigidBody.drag = waterDrag;

            rigidBody.position = new Vector3(
                                    rigidBody.position.x,
                                    avgWaterLine.y - floatingOffset.y,
                                    rigidBody.position.z);
        }
        
        //4-Apply gravity
        //transform.position will update at the end of the FixedUpdate loop to match
        //Rigidbody.position. It eases physics/collisions calculations.
        Vector3 deltaFloatingOffset = newFloatingOffset - floatingOffset;
        Vector3 force = Physics.gravity * Mathf.Clamp(Mathf.Abs(deltaFloatingOffset.y), 0, 1);
        rigidBody.AddForce(force);

        //5-Apply rotation
        //bool pointUnderWater = false;
        Vector3 waterLineNormal = GeometryHelper.GetNormalVector(waterLinePoints);
        if (CheckFloatingPointUnderWater(floatingPointsPos, waterLinePoints))
        {
            waterLineNormal = Vector3.SmoothDamp(transform.up, waterLineNormal, ref smoothVectorRotation, 0.2f);
            rigidBody.rotation = Quaternion.FromToRotation(transform.up, waterLineNormal) * rigidBody.rotation;
        }
    }

    /// <summary>
    /// Checks if there is any floating point under water (below corresponding water line point)
    /// </summary>
    /// <param name="floatingPointsPos"></param>
    /// <param name="waterLinePoints"></param>
    /// <returns></returns>
    private bool CheckFloatingPointUnderWater(Vector3[] floatingPointsPos, Vector3[] waterLinePoints)
    {
        for (int i = 0; i< floatingPointsPos.Length; i++)
        {
            if (floatingPointsPos[i].y < waterLinePoints[i].y)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Gets water height at floating points' position (water line)
    /// </summary>
    /// <returns></returns>
    private Vector3[] GetWaterHeightPoints(/*Vector3[] floatingPointsPos*/)
    {
        Vector3[] floatingPointsPos = new Vector3[floatingPoints.Length];
        for (int i = 0; i < floatingPoints.Length; i++)
        {
            floatingPointsPos[i] = new Vector3(
                                    floatingPoints[i].transform.position.x,
                                    floatingPoints[i].WaterHeight,
                                    floatingPoints[i].transform.position.z);
        }
        return floatingPointsPos;
    }

    /// <summary>
    /// Gets object's floating points' position
    /// </summary>
    /// <returns></returns>
    private Vector3[] GetFloatingPointPositions()
    {
        Vector3[] floatingPointsPos = new Vector3[floatingPoints.Length];
        for (int i = 0; i < floatingPoints.Length; i++)
            floatingPointsPos[i] = floatingPoints[i].transform.position;
        
        return floatingPointsPos;
    }
}
