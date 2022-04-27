using System;
using System.IO;
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
    static readonly System.Random r = new System.Random();
    static public int number = 0;

    float[] rayHit;
    static public int rayCount = 6;

    public float score;
    float livePoint, triggerPoint;
    float timer, duration, checkCars, timerCars;
    float distanse;

    double[][] weightsRotate;
    double[][] weightsForward;
    //double[][] weightsStop;
    Move move;

    Transform previousTrigger;

    Vector3 oldPos;

    public bool Alive { get; set; }

    public void RandomWeight()
    {
        for(int i = 0; i < rayCount / 2; i++)
        {
            weightsRotate[i] = new double[2];
            for(int j = 0; j < 2; j++)
            {
                weightsRotate[i][j] = r.Next(-1, 1) + r.NextDouble();
            }
        }
        for(int i = 0; i < rayCount - 1; i++)
        {
            weightsForward[i] = new double[2];
            weightsForward[i][0] = 90 / 6 * (i + 1);
            weightsForward[i][1] = r.Next(-1, 1) + r.NextDouble();
        }
    }

    public Directional GetRotate(out float scaleRotate)
    {
        double left = 0;
        double right = 0;
        for(int i = 0; i < rayCount; i++)
        {
            if(i <= 2)
            {
                right += rayHit[i];
            }
            else if( i > 2)
            {
                left += rayHit[i];
            }
        }
        Directional result = left > right ? Directional.LEFT : Directional.RIGHT;
        int start;
        scaleRotate = 0;
        if(result == Directional.LEFT)
        {
            start = 3;
            for (int i = 0; i < rayCount / 2; i++)
            {
                if (rayHit[i + start] < 15f)
                {
                    scaleRotate += (float)weightsRotate[i][0];
                }
            }
        }
        else
        {
            start = 0;
            for (int i = 0; i < rayCount / 2; i++)
            {
                if (rayHit[i + start] < 15f)
                {
                    scaleRotate += (float)weightsRotate[i][1];
                }
            }
        }
        return result;
    }

    public float GetSpeed()
    {
        double gas = 0;
        float result = 0;
        for (int i = 0; i < rayCount; i++)
        {
            gas += rayHit[i];
        }
        for(int i = 0; i < rayCount - 1; i++)
        {
            if (weightsForward[i][0] >= (int)gas)
            {
                result = (float)weightsForward[i][1];
            }
        }
        return result;
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
                rayHit[i] = 15f;
            }
        }
    }

    public string GetWeightsLogs()
    {
        string result = "Веса поворота: \n";
        for(int i = 0; i < rayCount / 2; i++)
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
        for (int i = 0; i < rayCount - 1; i++)
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
    public void Logs()
    {
        using (StreamWriter writer = new StreamWriter("weight_cars.log", true))
        {
            writer.Write($"car_{number}\n");
            writer.Write(GetWeightsLogs());
            writer.Write("\n");
            number++;
        }
        number = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Alive = false;
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(previousTrigger == null)
        {
            previousTrigger = other.transform.parent;
            return;
        }
        int indx = Array.IndexOf(MapController.triggers, previousTrigger.gameObject) + 1;
        if(indx == MapController.triggers.Length)
        {
            previousTrigger = other.transform.parent;
            score += triggerPoint;
        }
        else
        {
            if(MapController.triggers[indx].name == other.transform.parent.name)
            {
                score += triggerPoint;
                previousTrigger = other.transform.parent;
            }
            else
            {
                triggerPoint = 0;
                livePoint = 0;
                Alive = false;
            }
        }
        //Debug.Log(score);
    }
    private void Raycast()
    {
        RaycastHit[] hits = new RaycastHit[rayCount];
        int carLayer = LayerMask.GetMask("Car", "Trigger");
        int layerMask = (1 << carLayer);
        for (int i = 0; i < rayCount; i++)
        {
            var ray = transform.GetChild(2 + i);
            Vector3 forward = ray.TransformDirection(Vector3.forward) * 15;
            Physics.Raycast(new Ray(ray.position, forward), out RaycastHit raycastHit, 15f, layerMask);
            hits[i] = raycastHit;
        }
        SetRayHit(hits);
    }

    public double[][] WeightsRotate 
    {
        get
        {
            return weightsRotate;
        }
        set
        {
            weightsRotate = value;
        }
    }

    public double[][] WeightsForward
    {
        get
        {
            return weightsForward;
        }
        set
        {
            weightsForward = value;
        }
    }

    private void Awake()
    {
        score = 0;
        distanse = 0;
        duration = 0.05f;
        livePoint = 0.002f;
        triggerPoint = 5f;
        timerCars = 3f;
        checkCars = Time.time + timerCars;
        move = transform.GetComponent<Move>();
        Alive = true;
        rayHit = new float[rayCount];
        weightsRotate = new double[rayCount / 2][];
        weightsForward = new double[rayCount - 1][];
        oldPos = transform.position;
    }
    private void Update()
    {
        if (Alive)
        {
            if (checkCars < Time.time)
            {
                Alive = distanse < 8 ? false : Alive;
                checkCars = Time.time + timerCars;
                distanse = 0;
            }
            if (timer < Time.time)
            {
                Raycast();
                distanse += Vector3.Distance(oldPos, transform.position);
                oldPos = transform.position;
                float scaleRotate;
                move.SetMoveValue(GetSpeed(), GetRotate(out scaleRotate), scaleRotate);
                move.MoveCar();
                timer = Time.time + duration;
            }
            score += livePoint;
        }
    }
}
