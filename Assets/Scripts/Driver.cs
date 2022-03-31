using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Directional
{
    LEFT,
    RIGHT,
    FORWARD,
    BACK
}
public class Driver:MonoBehaviour
{
    float[] rayHit;
    float speed;
    int rayCount = 5;

    double[][] weightsRotate;
    double[][] weightsForward;

    public Driver()
    {
        rayHit = new float[rayCount];
        weightsRotate = new double[rayCount][];
        weightsForward = new double[rayCount][];
        RandomWeight();
    }

    public void RandomWeight()
    {
        System.Random r = new System.Random();
        for(int i = 0; i < rayCount; i++)
        {
            weightsRotate[i] = new double[2];
            weightsForward[i] = new double[2];
            for(int j = 0; j < 2; j++)
            {
                weightsRotate[i][j] = r.Next(-100, 100) + r.NextDouble();
                weightsForward[i][j] = r.Next(-100, 100) + r.NextDouble();
            }
        }
    }

    public Directional GetRotate()
    {
        double left = 0;
        double right = 0;
        for(int i = 0; i < rayCount; i++)
        {
            left += rayHit[i] * weightsRotate[i][0];
            right += rayHit[i] * weightsRotate[i][1];
        }
        Directional result = left > right ? Directional.LEFT : Directional.RIGHT;
        return result;
    }

    public Directional GetSpeed()
    {
        double gas = 0;
        double stop = 0;
        for (int i = 0; i < rayCount; i++)
        {
            gas += rayHit[i] * weightsRotate[i][0] + speed;
            stop += rayHit[i] * weightsRotate[i][1] + speed;
        }
        Directional result = stop > gas ? Directional.FORWARD : Directional.BACK;
        return result;
    }

    public void SetSpeed(float curreSpeed)
    {
        speed = curreSpeed;
    }
    public void SetRayHit(RaycastHit[] hits)
    {
        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i].collider != null)
            {
                rayHit[i] = hits[i].distance;
            }
            else
            {
                rayHit[i] = 0;
            }
        }
    }

    public string GetWeights()
    {
        string result = "Веса поворота: \n";
        for(int i = 0; i < rayCount; i++)
        {
            result += "Луч номер " + i.ToString() + ": ";
            for(int j = 0; j < 2; j++)
            {
                result += $"|{weightsRotate[i][j]}| ";
            }
            result += "\n";
        }
        result += "\n ____________________________________\n";
        result += "Веса газа/тормоза: \n";
        for (int i = 0; i < rayCount; i++)
        {
            result += "Луч номер " + i.ToString() + ": ";
            for (int j = 0; j < 2; j++)
            {
                result += $"|{weightsForward[i][j]}| ";
            }
            result += "\n";
        }
        return result;
    }
}
