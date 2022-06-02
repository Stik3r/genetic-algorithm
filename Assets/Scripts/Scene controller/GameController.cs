using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class CarsController
{
    public static List<GameObject> cars = new List<GameObject>();

    public static int BestCar()
    {
        float max = 0;
        int indx = -1;
        for(int i = 0; i < cars.Count; i++)
        {
            Driver driver = cars[i].GetComponent<Driver>();
            if (driver.Alive)
            {
                if(max < driver.score)
                {
                    max = driver.score;
                    indx = i;
                }
            }
        }
        return indx;
    }
}
public class GameController : MonoBehaviour
{
    public GameObject zeroPoint;
    public GameObject lineRoad;
    public GameObject rotateRoad;
    public GameObject car;

    public GameObject menuPanel;
    public GameObject menuBotton;
    public GameObject statsContent;
    public GameObject statsPanel;

    public GameObject tamplatePanel;
    public GameObject tamplateText;
    public GameObject tamplateImage;

    Vector3 startPos = Vector3.zero;

    GenericAlgorithm algorithm = new GenericAlgorithm();

    float timer, duration = 3;

    bool open = false;
    
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
        SpawnCars(true);
        timer = Time.time + duration;
        int q = 0;
        foreach(var trigger in MapController.triggers)
        {
            trigger.name += q.ToString();
            q++;
        }
        SetDefaultValues();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CameraSettings.selectedCar = null;
            CarsController.cars.Clear();
            GenericAlgorithm.bestCars.Clear();
            SceneManager.LoadScene("MenuScene");
        }
        if(timer < Time.time)
        {
            AliveCars();
            timer = Time.time + duration;
        }
    }

    /// <summary>
    /// Устанавливает стандартные значения в меню настройки
    /// </summary>
    void SetDefaultValues()
    {
        menuPanel.transform.Find("TypeSelect").GetComponent<Dropdown>().onValueChanged.AddListener(TypeSelectChange);
        menuPanel.transform.Find("TypeCrossover").GetComponent<Dropdown>().onValueChanged.AddListener(TypeCrossoverChange);
        var countPopulation = menuPanel.transform.Find("CountPopulation").GetComponent<InputField>();
        countPopulation.text = algorithm.countPopulation.ToString();
        countPopulation.onEndEdit.AddListener(ChangeCount);
        var chanceMutation = menuPanel.transform.Find("ChanceMutation").GetComponent<InputField>();
        chanceMutation.text = (algorithm.percentMutation * 100).ToString();
        chanceMutation.onEndEdit.AddListener(ChanceMutationChange);
        var countMutation = menuPanel.transform.Find("CountMutation").GetComponent<InputField>();
        countMutation.text = (algorithm.percentMutationIndividuals * 100).ToString();
        countMutation.onEndEdit.AddListener(CountMutationChange);
        var countSelect = menuPanel.transform.Find("CountSelect").GetComponent<InputField>();
        countSelect.text = (algorithm.percentSelect * 100).ToString();
        countSelect.onEndEdit.AddListener(CountSelectChange);
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

    void SpawnCars(bool random)
    {
        VirtualDriver[] virtualDrivers = random ? algorithm.RandomPopulation() : algorithm.GetDrivers();
        menuPanel.transform.Find("GenerationNumber").GetComponent<Text>().text = algorithm.generationNumber.ToString();
        for (int i = 0; i < algorithm.countPopulation; i++)
        {
            Vector3 carPos = new Vector3(startPos.x, 4, startPos.z);
            var carObj = Instantiate(car, carPos, transform.rotation);
            carObj.transform.localScale =
                new Vector3(virtualDrivers[i].Solution[13], virtualDrivers[i].Solution[14],
                virtualDrivers[i].Solution[15]);
            CarsController.cars.Add(carObj);
            carObj.GetComponent<Driver>().Solution = virtualDrivers[i].Solution;
            carObj.GetComponent<Move>().maxSteeringAngle = virtualDrivers[i].Solution[16];
            carObj.GetComponent<Move>().maxMotorTorque = virtualDrivers[i].Solution[17];
            carObj.GetComponent<Rigidbody>().mass = virtualDrivers[i].Solution[12];
        }
    }

    public void MenuStatus()
    {
        if (open)
        {
            menuBotton.SetActive(true);
            menuPanel.SetActive(false);
            open = false;
        }
        else
        {
            menuBotton.SetActive(false);
            menuPanel.SetActive(true);
            open = true;
        }
    }

    public void StatsBtn()
    {
        statsPanel.SetActive(true);
        Camera.main.orthographic = true;
        var scores = from car in GenericAlgorithm.bestCars
                     select car.score;
        float max = Mathf.Max(scores.ToArray());
        float maxHeight = 405f;
        int indx = 0;
        foreach (var score in scores)
        {
            float heightMultiply = score / max;
            float sizeY = maxHeight * heightMultiply + 20;

            GameObject panel = Instantiate(tamplatePanel, statsContent.transform, false);
            panel.SetActive(true);
            panel.name = indx.ToString();
            panel.GetComponent<RectTransform>().sizeDelta = new Vector2(33, sizeY);
            panel.AddComponent<StatsBestCar>();

            BoxCollider collider = panel.AddComponent<BoxCollider>();
            collider.center = new Vector3(1, 0, 0);
            collider.size = new Vector3(30, sizeY, 0);

            GameObject text = Instantiate(tamplateText, panel.transform, false);
            text.SetActive(true);
            text.GetComponent<RectTransform>().localPosition =
                new Vector3(0, (-sizeY / 2f) - 10f, 0);
            text.GetComponent<Text>().text = Math.Round(score, 1).ToString();

            GameObject image = Instantiate(tamplateImage, panel.transform, false);
            image.SetActive(true);
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(33, sizeY - 15);
            image.GetComponent<Image>().color = Color.blue;

            indx++;
        }
    }
    public void CloseBtn()
    {
        Camera.main.orthographic = false;
        for(int i = 0; i < statsContent.transform.childCount; i++)
        {
            Destroy(statsContent.transform.GetChild(i).gameObject);
        }
        statsPanel.SetActive(false);
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
            Destroy(car.gameObject);
        }
        CarsController.cars.Clear();
        SpawnCars(false);
        Camera.main.GetComponent<CameraSettings>().enabled = true;
    }

    public void Restart()
    {
        Camera.main.GetComponent<CameraSettings>().enabled = false;
        GenericAlgorithm.bestCars.Clear();
        foreach (var car in CarsController.cars)
        {
            Destroy(car.gameObject);
        }
        CarsController.cars.Clear();
        SpawnCars(true);
        Camera.main.GetComponent<CameraSettings>().enabled = true;
    }

    public void ChangeCount(string value)
    {
        if(int.Parse(value) < 10)
        {
            menuPanel.transform.Find("CountPopulation").GetComponent<InputField>().text =
                algorithm.countPopulation.ToString();
            return;
        }
        algorithm.countPopulation = int.Parse(value);
    }

    public void ChanceMutationChange(string value)
    {
        float percent = float.Parse(value) / 100f;
        if(percent < 0 || percent > 1)
        {
            menuPanel.transform.Find("ChanceMutation").GetComponent<InputField>().text =
                (algorithm.percentMutation * 100).ToString();
            return;
        }
        algorithm.percentMutation = percent;
    }
    
    public void CountMutationChange(string value)
    {
        float percent = float.Parse(value) / 100f;
        if (percent < 0 || percent > 1)
        {
            menuPanel.transform.Find("CountMutation").GetComponent<InputField>().text =
                (algorithm.percentMutationIndividuals * 100).ToString();
            return;
        }
        algorithm.percentMutationIndividuals = percent;
    }

    public void CountSelectChange(string value)
    {
        float percent = float.Parse(value) / 100f;
        if (percent < 0 || percent > 1)
        {
            menuPanel.transform.Find("CountSelect").GetComponent<InputField>().text =
                (algorithm.percentSelect * 100).ToString();
            return;
        }
        algorithm.percentSelect = percent;
    }

    public void TypeSelectChange(int value)
    {
        algorithm.ChangeSelectType(value);
    }

    public void TypeCrossoverChange(int value)
    {
        algorithm.ChangeCrossoverType(value);
    }
}
