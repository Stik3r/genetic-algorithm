using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CarsController
{
    public static List<Transform> cars = new List<Transform>();

    
}
public class GameController : MonoBehaviour
{
    public GameObject zeroPoint;
    public GameObject lineRoad;
    public GameObject rotateRoad;
    public GameObject car;
    void Start()
    {
        for(int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                if(!MapController.Check(i, j))
                {
                    GameObject road = MapController.RoadName(i, j).Contains("Line") ? lineRoad : rotateRoad;
                    var newRoad = Instantiate(road, GetPos(i, j), GetAngels(MapController.Rotation(i, j)));
                    newRoad.transform.localScale = Vector3.one;
                }
            }
        }
        Vector3 carPos = new Vector3(0, 4, (float)67.5);
        var carObj = Instantiate(car, carPos, transform.rotation);
        carObj.transform.localScale = new Vector3(4, 4, 4);
        CarsController.cars.Add(carObj.transform);
        Driver dr = new Driver();
        dr.RandomWeight();
        using (StreamWriter writer = new StreamWriter("logWeights.log", false))
        {
            writer.WriteLine(dr.GetWeights());
        }
    }

    Vector3 GetPos(int i, int j)
    {
        double xPos = i * 15;
        double zPos = j * 15;
        return new Vector3((float)xPos, 0, (float)zPos);
    }

    Quaternion GetAngels(Vector3 angels)
    {
        return Quaternion.Euler(90, 0, angels.z);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MenuScene");
        }
    }
}
