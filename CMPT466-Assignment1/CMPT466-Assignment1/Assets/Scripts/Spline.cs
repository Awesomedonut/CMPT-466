using System.Collections.Generic;
using UnityEngine;

public static class Spline
{
    private static int substeps = 100;

    public static List<Vector2> GetSpline(List<Vector2> controlPoints, Vector2 startTangent, Vector2 endTangent)
    {
        List<Vector2> interpolated = new List<Vector2>();

        /*** Please explain your choice of spline and approach for the first and last segments here ***/

        // For the first and last segments, I chose the Catmull-Rom Interpolation method
        // this is because in lecture, Prof Yin told us it is the most commonly used method in animation.


        /*** Please write your code here ***/

        List<Vector2> points = new List<Vector2>();
        
        points.Add(startTangent);
        points.AddRange(controlPoints);
        points.Add(endTangent);



        for(int i = 0; i < points.Count - 3; i++){

            for(int k = 0; k < substeps; k++){

                Vector2 point = Evaluate(k / (float)(substeps - 1), points[i], points[i + 1], points[i + 2], points[i + 3]);
                interpolated.Add(point);



            }
        }

        return interpolated;
    }

    private static Vector2 Evaluate(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        /*** Please write your code for Catmull-Rom spline here ***/

        // p prime (ti) = ((p_i+1) - (p_i-i)) / 2

        // float 

        // return Vector3.zero;

    Vector2 m1 = (p2 - p0) * 0.5f;
    Vector2 m2 = (p3 - p1) * 0.5f;

    float t2 = t * t;
    float t3 = t2 * t;

    // Hermite basis functions
    float h00 = 2f * t3 - 3f * t2 + 1f;
    float h10 = t3 - 2f * t2 + t;
    float h01 = -2f * t3 + 3f * t2;
    float h11 = t3 - t2;

    // Interpolate the position using the Hermite spline formula.
    return h00 * p1 + h10 * m1 + h01 * p2 + h11 * m2;
    }
}
