using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// custom quoternian applied and it is tested on a default cylinder body rotation 
/// </summary>
public class CQuoternian
{
    private float x, y, z, w;
    Vector3 vec3;

    public CQuoternian()
    { 
        x = 0;
        y = 0;
        z = 0;
        w = 0;
    }
    public CQuoternian(float x, float y, float z, float w)
    {
        this.w = w;
        this.x = x;
        this.y = y;
        this.z = z;
        //vec3 = new Vector3(x, y, z);
    }

    /// <summary>
    /// This method can be used only for both conjugate and inverse of a quoternian only if the 
    /// quoternian is normalized
    /// </summary>
    /// <returns></returns>
    public static CQuoternian Conjugate(CQuoternian quot)
    {
        return new(-quot.x, -quot.y, -quot.z, quot.w);
    }

    public static CQuoternian Inverse(CQuoternian quot)
    {
        float sumOfSquaerd =1 / Mathf.Sqrt((quot.w * quot.w) + (quot.x * quot.x) + (quot.y * quot.y) + (quot.z * quot.z));
        return new(quot.w * sumOfSquaerd, -quot.x * sumOfSquaerd, -quot.y * sumOfSquaerd, -quot.z * sumOfSquaerd);
    }
    
    public static CQuoternian Normalize(CQuoternian quot)
    {
        float tempSquaredResult = ((quot.w*quot.w) + (quot.x * quot.x) + (quot.y * quot.y) + (quot.z * quot.z));
        float finalresult =1 / Mathf.Sqrt(tempSquaredResult);
        return new(quot.w * finalresult, quot.x * finalresult, quot.y * finalresult, quot.z * finalresult);
    }

    /// <summary>
    /// hamiltonian multiplication between two quoternians
    /// </summary>
    /// <param name="quo1">CQuoternian</param>
    /// <param name="quo2">CQuoternian</param>
    /// <returns>this returns a new CQuoternian type</returns>
    private static CQuoternian QuoterMultiplication(CQuoternian quo1, CQuoternian quo2)
    {
        Vector3 tempquo1 = new(quo1.x, quo1.y, quo1.z);
        Vector3 tempquo2 = new(quo2.x, quo2.y, quo2.z);
        float finalW = quo1.w * quo2.w - Vector3.Dot(tempquo1, tempquo2);
        Vector3 finalV = quo1.w * tempquo2 + quo2.w * tempquo1 + Vector3.Cross(tempquo1, tempquo2);
        return new CQuoternian(finalV.x, finalV.y, finalV.z, finalW);
    }

    public static Vector3 RotateVector(Vector3 P, CQuoternian quot)
    {
        quot = CQuoternian.Normalize(quot);
        //storing the P vector as  a quoternian
        CQuoternian tempP = new(P.x, P.y, P.z, 0);
        CQuoternian quotInv = CQuoternian.Conjugate(quot);

        CQuoternian Result = CQuoternian.QuoterMultiplication(quot, tempP);
        Result = CQuoternian.QuoterMultiplication(Result, quotInv);
        Vector3 vecResult =new Vector3(Result.x, Result.y, Result.z);

        return vecResult;
    }

    public static float Largest(float a, float b, float c)
    {
        if (a >= b)
        {
            if (a >= c) return a;
            else return c;
        }
        else
        {
            if (b >= c) return b;
            else return c;
        }
    }

    public static CQuoternian LookRotation(Vector3 forward, Vector3 up)
    {   
        float S;
        float[,] matrix = new float[3, 3];
        forward = Vector3.Normalize(forward);
        Vector3 right = Vector3.Normalize(Vector3.Cross(up, forward));
        up = Vector3.Cross(forward, right);


        // COLUMN 0: Right Vector
        matrix[0, 0] = right.x;
        matrix[1, 0] = right.y;
        matrix[2, 0] = right.z;

        // COLUMN 1: Up Vector
        matrix[0, 1] = up.x;
        matrix[1, 1] = up.y;
        matrix[2, 1] = up.z;

        // COLUMN 2: Forward Vector
        matrix[0, 2] = forward.x;
        matrix[1, 2] = forward.y;
        matrix[2, 2] = forward.z;

        float diag0 = matrix[0, 0];
        float diag1 = matrix[1, 1];
        float diag2 = matrix[2, 2];
        float Trace = diag0 + diag1 + diag2;
        CQuoternian temp = new();
        
        if (Trace > 0)
        {
            S = Mathf.Sqrt(Trace + 1f) * 2f;
            temp.w = 0.25f * S;
            temp.x = (matrix[2, 1] - matrix[1, 2]) / S;
            temp.y = (matrix[0, 2] - matrix[2, 0]) / S;
            temp.z = (matrix[1, 0] - matrix[0, 1]) / S;
        }
        else
        {
            if (Largest(diag0, diag1, diag2) == diag0)
            {
                S = Mathf.Sqrt(1f + diag0 - diag1 - diag2) * 2f;
                temp.w = (matrix[2, 1] - matrix[1, 2]) / S;
                temp.y = (matrix[0, 1] + matrix[1, 0]) / S;
                temp.z = (matrix[0, 2] + matrix[2, 0]) / S;
                temp.x = 0.25f * S;
            }
            else if (Largest(diag0, diag1, diag2) == diag1)
            {
                S = Mathf.Sqrt(1f + diag1 - diag0 - diag2) * 2f;
                temp.w = (matrix[0, 2] - matrix[2, 0]) / S;
                temp.x = (matrix[0, 1] + matrix[1, 0]) / S;
                temp.z = (matrix[1, 2] + matrix[2, 1]) / S;
                temp.y = 0.25f * S;
            }
            else
            {
                S = Mathf.Sqrt(1f + diag2 - diag1 - diag0) * 2f;
                temp.w = (matrix[1, 0] - matrix[0, 1]) / S;
                temp.x = (matrix[0, 2] + matrix[2, 0]) / S;
                temp.y = (matrix[1, 2] + matrix[2, 1]) / S;
                temp.z = 0.25f * S;
            }
        }
        return CQuoternian.Normalize(temp);
    }

    //helper method to convert to unity
    public Quaternion ToUnity()
    {
        return new Quaternion(this.x, this.y, this.z, this.w);
    }
}