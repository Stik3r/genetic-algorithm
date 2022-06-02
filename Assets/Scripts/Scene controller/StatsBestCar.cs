using UnityEngine;
using UnityEngine.UI;

public class StatsBestCar : MonoBehaviour
{
    static GameObject score;
    static GameObject right;
    static GameObject left;
    static GameObject speed;
    static GameObject mass;
    static GameObject scale;
    static GameObject maxRotate;
    static GameObject maxSpeed;

    private void OnMouseDown()
    {
        Debug.Log(1);
        if(score == null)
        {
            FindObjects();
        }
        VirtualDriver virtualDriver = GenericAlgorithm.bestCars[int.Parse(transform.name)];
        string[] stats = virtualDriver.CarStats();
        score.GetComponent<Text>().text = stats[0];
        right.GetComponent<Text>().text = stats[1];
        left.GetComponent<Text>().text = stats[2];
        speed.GetComponent<Text>().text = stats[3];
        mass.GetComponent<Text>().text = stats[4];
        scale.GetComponent<Text>().text = stats[5];
        maxRotate.GetComponent<Text>().text = stats[6];
        maxSpeed.GetComponent<Text>().text = stats[7];
    }
    
    private void FindObjects()
    {
        score = GameObject.Find("ScoreOutput");
        right = GameObject.Find("RightOutput");
        left = GameObject.Find("LeftOutput");
        speed = GameObject.Find("SpeedOutput");
        mass = GameObject.Find("MassOutput");
        scale = GameObject.Find("ScaleOutput");
        maxRotate = GameObject.Find("MaxRotateOutput");
        maxSpeed = GameObject.Find("MaxSpeedOutput");
    }
}
