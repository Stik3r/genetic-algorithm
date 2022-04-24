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
    float speed;
    static public int rayCount = 5;

    public float score = 0;
    float timer, duration = 0.5f;

    double[][] weightsRotate;
    double[][] weightsForward;
    Move move;

    Transform previousTrigger;

    public bool Alive { get; set; }

    public void RandomWeight()
    {
        for(int i = 0; i < rayCount; i++)
        {
            weightsRotate[i] = new double[2];
            weightsForward[i] = new double[2];
            for(int j = 0; j < 2; j++)
            {
                weightsRotate[i][j] = 0;
                weightsForward[i][j] = 0;
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
            gas += rayHit[i] * weightsForward[i][0] + speed;
            stop += rayHit[i] * weightsForward[i][1] + speed;
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

    public string GetWeightsLogs()
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
        UnityEngine.Object.Destroy(transform.GetComponent<Rigidbody>());
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
        }
        else
        {
            if(MapController.triggers[indx].name == other.transform.parent.name)
            {
                score += 3;
                previousTrigger = other.transform.parent;
            }
            else
            {
                
            }
        }
        //Debug.Log(score);
    }
    private void Raycast()
    {
        RaycastHit[] hits = new RaycastHit[5];
        int carLayer = LayerMask.GetMask("Car", "Trigger");
        int layerMask = (1 << carLayer);
        for (int i = 0; i < 5; i++)
        {
            var ray = transform.GetChild(2 + i);
            Vector3 forward = ray.TransformDirection(Vector3.forward) * 15;
            Physics.Raycast(new Ray(ray.position, forward), out RaycastHit raycastHit, 15f, layerMask);
            hits[i] = raycastHit;
        }
        SetRayHit(hits);
        SetSpeed(move.motor);
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
        move = transform.GetComponent<Move>();
        Alive = true;
        rayHit = new float[rayCount];
        weightsRotate = new double[rayCount][];
        weightsForward = new double[rayCount][];
    }
    private void FixedUpdate()
    {
        if (Alive)
        {
            if (timer < Time.time)
            {
                score += 0.1f;
                Raycast();
                move.SetMoveValue(GetSpeed(), GetRotate());
                move.MoveCar();
                timer = Time.time + duration;
            }
            /*Raycast();
            move.SetMoveValue(GetSpeed(), GetRotate());
            move.MoveCar();*/
        }
    }
}
