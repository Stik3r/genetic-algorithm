using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CarsController
{
    public static List<GameObject> cars = new List<GameObject>();

}
public class GameController : MonoBehaviour
{
    public GameObject zeroPoint;
    public GameObject lineRoad;
    public GameObject rotateRoad;
    public GameObject car;

    Vector3 startPos = Vector3.zero;

    GenericAlgorithm algorithm = new GenericAlgorithm();

    float timer, duration = 3;
    
    /// <summary>
    /// Создание карты при загрузке сцены
    /// </summary>
    void Start()
    {
        (int, int) indxStartPos = MapController.map.FindStartPosition();
        var way = MapController.CreateWay();
        MapController.triggers = new GameObject[way.Count];
        for(int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                if(!MapController.Check(i, j))
                {
                    if(i == indxStartPos.Item1 && j == indxStartPos.Item2)
                    {
                        startPos = MapController.GetPos(i, j);
                    }
                    GameObject road = MapController.RoadName(i, j).Contains("Line") ? lineRoad : rotateRoad;
                    var newRoad = Instantiate(road, MapController.GetPos(i, j), 
                        MapController.GetAngels(MapController.Rotation(i, j)));
                    newRoad.transform.localScale = Vector3.one;
                    if (way.Contains((i, j)))
                    {
                        MapController.triggers[way.IndexOf((i, j))] = newRoad;
                    }
                }
            }
        }
        for(int i = 0; i < algorithm.countPopulation; i++)
        {
            Vector3 carPos = new Vector3(startPos.x, 4, startPos.z);
            var carObj = Instantiate(car, carPos, transform.rotation);
            carObj.transform.localScale = new Vector3(4, 4, 4);
            CarsController.cars.Add(carObj);
            carObj.GetComponent<Driver>().RandomWeight();
            //carObj.GetComponent<Driver>().Logs();
        }
        algorithm.NewPopulation(CarsController.cars.ToArray());
        timer = Time.time + duration;
        int q = 0;
        foreach(var trigger in MapController.triggers)
        {
            trigger.name += q.ToString();
            q++;
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MenuScene");
        }
        if(timer < Time.time)
        {
            AliveCars();
            timer = Time.time + duration;
        }
    }

    /// <summary>
    /// Проверка на наличие не врезавшихся автомобилей
    /// </summary>
    void AliveCars()
    {
        bool alive = false;
        foreach(var car in CarsController.cars)
        {
            if (car.GetComponent<Driver>().Alive)
            {
                alive = true;
                break;
            }
        }
        if (!alive)
        {
            Newgeneration();
        }
    }

    /// <summary>
    /// Создание нового поколения
    /// </summary>
    public void Newgeneration()
    {
        Camera.main.GetComponent<CameraSettings>().enabled = false;
        algorithm.SetScore(CarsController.cars.ToArray());
        algorithm.StartWorkAlghoritm();
        foreach(var car in CarsController.cars)
        {
            GameObject.Destroy(car.gameObject);
        }
        CarsController.cars.Clear();
        Driver[] drivers = algorithm.GetDrivers();
        for(int i = 0; i < algorithm.countPopulation; i++)
        {
            Vector3 carPos = new Vector3(startPos.x, 4, startPos.z);
            var carObj = Instantiate(car, carPos, transform.rotation);
            carObj.transform.localScale = new Vector3(4, 4, 4);
            CarsController.cars.Add(carObj);
            carObj.GetComponent<Driver>().WeightsForward = drivers[i].WeightsForward;
            carObj.GetComponent<Driver>().WeightsRotate = drivers[i].WeightsRotate;
        }
        Camera.main.GetComponent<CameraSettings>().enabled = true;
    }
}
