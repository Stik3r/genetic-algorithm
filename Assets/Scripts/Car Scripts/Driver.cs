using System;
using UnityEngine;


public enum Directional
{
    LEFT,
    RIGHT,
    FORWARD,
    BACK
}

public class VirtualDriver
{
    static readonly System.Random r = new System.Random();
    static public int carNumber = 0;
    static public int rayCount = 6;

    public float score = 0;
    public float[] Solution { get; set; }

    public VirtualDriver()
    {
        Solution = new float[18];
    }
    public void SetRandomValue(int min, int max, int indx)
    {
        float result;
        do
        {
            result = (float)(r.Next(min, max) + r.NextDouble());
        } while (result < min || result > max);
        Solution[indx] = result;
    }
    public string SolutionLogs()
    {
        string log = "Множители поворота:\n";
        for(int i = 0; i < 6; i++)
        {
            log += $"{i} - {Solution[i]}\n";
        }
        log += "Множители скорости:\n";
        for (int i = 6; i < 12; i++)
        {
            log += $"{i - 3} - {Solution[i]} \n";
        }
        log += $"Масса: {Solution[12]}\n";
        log += "Множители размера:\n";
        for (int i = 13; i < 16; i++)
        {
            log += $"{i - 13} - {Solution[i]}\n";
        }
        log += $"Максимальный угол поворота: {Solution[16]}\n";
        log += $"Максимальные обороты двигателя: {Solution[17]}\n";
        return log;
    }
}

public class Driver : MonoBehaviour
{
    static readonly System.Random r = new System.Random();
    static public int rayCount = 6;
    static public float[] forwardDistance;

    float[] rays;
    public float score = 0;
    float livePoint = 0.01f;
    float triggerPoint = 5f;

    float timerDistance, timerRay;
    float rateDistance, rateRays;

    float distance;

    Move move;
    Transform lastTrigger;
    Vector3 oldPos;

    bool alive;

    int gasIndx = 6;
    
    public bool Alive 
    {
        get
        {
            return alive;
        }
        set
        {
            if(value == false)
            {
                transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
            alive = value;
        } 
    }

    public float[] Solution { get; set; }

    void Awake()
    {
        Alive = true;
        rateRays = 0.1f;
        rateDistance = 3f;
        timerDistance = Time.time + rateDistance;
        timerRay = Time.time + rateRays;
        move = transform.GetComponent<Move>();
        oldPos = transform.position;
        rays = new float[rayCount];
    }

    void Update()
    {
        if (Alive)
        {
            if (timerDistance < Time.time)
            {
                Alive = distance < 8 ? false : Alive;
                timerDistance = Time.time + rateDistance;
                distance = 0;
            }
            if (timerRay < Time.time)
            {
                Raycast();
                distance += Vector3.Distance(oldPos, transform.position); 
                score += Vector3.Distance(oldPos, transform.position);
                oldPos = transform.position;
                move.SetMoveValue(GetSpeed(), GetRotate());
                move.MoveCar();
                timerRay = Time.time + rateRays;
            }
        }
    }

    public float GetRotate()
    {
        float left = 0;
        float right = 0;
        int hitRaysL = 0;
        int hitRaysR = 0;
        for (int i = 0; i < rayCount; i++)
        {
            if(i < 2)
            {
                if(rays[i] < 15f)
                {
                    hitRaysR++;
                }
                right += rays[i];
            }
            else
            {
                if (rays[i] < 15f)
                {
                    hitRaysL++;
                }
                left += rays[i];
            }
        }
        Directional result = left > right ? Directional.LEFT : Directional.RIGHT;
        if (result == Directional.LEFT)
        {
            switch (hitRaysL)
            {
                case 1: return Solution[hitRaysL - 1];
                case 2: return Solution[hitRaysL - 1];
                case 3: return Solution[hitRaysL - 1];
                default: return -1000;
            }
        }
        else
        {
            switch (hitRaysR)
            {
                case 1: return Solution[hitRaysL + 2];
                case 2: return Solution[hitRaysL + 2];
                case 3: return Solution[hitRaysL + 2];
                default: return -1000;
            }
        }
    }

    public float GetSpeed()
    {
        float gas = 0;
        float result = 0;
        for(int i = 0; i < rayCount; i++)
        {
            gas += rays[i];
        }
        for(int i = gasIndx; i < 12; i++)
        {
            if(gas < forwardDistance[i - gasIndx])
            {
                result = Solution[i];
                break;
            }
        }
        return result;
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
        SetRays(hits);
    }

    public void SetRays(RaycastHit[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null)
            {
                rays[i] = hits[i].distance;
            }
            else
            {
                rays[i] = 15f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Alive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (lastTrigger == null)
        {
            lastTrigger = other.transform.parent;
            return;
        }
        int indx = Array.IndexOf(MapController.triggers, lastTrigger.gameObject) + 1;
        if (indx == MapController.triggers.Length)
        {
            lastTrigger = other.transform.parent;
            score += triggerPoint;
        }
        else
        {
            if (MapController.triggers[indx].name == other.transform.parent.name)
            {
                score += triggerPoint;
                lastTrigger = other.transform.parent;
            }
            else
            {
                triggerPoint = 0;
                livePoint = 0;
                if(score > 10)
                {
                    score -= 10;
                }
                Alive = false;
            }
        }
    }
}

