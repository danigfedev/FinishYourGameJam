using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeometryHelper
{
    /// <summary>
    /// Calculates average center point for given array of points in space
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static Vector3 GetCenterPoint(Vector3[] points)
    {
        Vector3 centerPoint = Vector3.zero;
        foreach (Vector3 point in points)
            centerPoint += point / points.Length;

        return centerPoint;
    }

    /// <summary>
    /// Calculates normal vector of given plane (necessary at least 3 points)
    /// </summary>
    /// <param name="points">Points conforming a plane</param>
    /// <returns></returns>
    public static Vector3 GetNormalVector(Vector3[] points)
    {
        if (points.Length < 3)
        {
            Debug.LogError("Not a plane. Can't get normal vector");
            return Vector3.up;//default value
        }

        Vector3 center = GetCenterPoint(points);

        float xx = 0f, xy = 0f, xz = 0f, yy = 0f, yz = 0f, zz = 0f;

        for (int i = 0; i < points.Length; i++)
        {
            var r = points[i] - center;
            xx += r.x * r.x;
            xy += r.x * r.y;
            xz += r.x * r.z;
            yy += r.y * r.y;
            yz += r.y * r.z;
            zz += r.z * r.z;
        }

        var det_x = yy * zz - yz * yz;
        var det_y = xx * zz - xz * xz;
        var det_z = xx * yy - xy * xy;

        if (det_x > det_y && det_x > det_z)
            return new Vector3(det_x, xz * yz - xy * zz, xy * yz - xz * yy).normalized;
        if (det_y > det_z)
            return new Vector3(xz * yz - xy * zz, det_y, xy * xz - yz * xx).normalized;
        else
            return new Vector3(xy * yz - xz * yy, xy * xz - yz * xx, det_z).normalized;

    }
}
