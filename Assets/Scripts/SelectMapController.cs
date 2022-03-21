using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectMapController : MonoBehaviour
{
    public GameObject mapPrefab;
    public GameObject lineRoad;
    public GameObject rotateRoad;
    GameObject previousMapSelect;
    int miniMapIndexX = 0;
    int miniMapIndexY = 0;

    void Start()
    {
        MapController.DeserializationMaps();
        if (MapController.maps != null)
        {
            LoadMaps();
        }
        miniMapIndexX = 0;
        miniMapIndexY = 0;
        MapController.map = new Map();
    }

    Vector3 MiniMapPosition()
    {
        float posX = miniMapIndexX * (float)3.9;
        float posY = -miniMapIndexY * (float)4.5;
        miniMapIndexX++;
        if(miniMapIndexX == 3)
        {
            miniMapIndexX = 0;
            miniMapIndexY++;
        }
        return new Vector3(posX, posY, 0);
    }
    Vector3 RoadPos(int i, int j)
    {
        float xPos = i * (float)0.45;
        float yPos = j * (float)0.45;
        return new Vector3(xPos, yPos, 0);
    }

    void LoadMaps()
    {
        foreach (var map in MapController.maps)
        {
            GameObject miniMap = Instantiate(mapPrefab, transform);
            miniMap.transform.GetComponentInChildren<TextMesh>().text = map.Name;
            miniMap.transform.localPosition = MiniMapPosition();
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (map.GetCell(i, j) != Cell.EpmtyCell())
                    {
                        GameObject road = map.GetCell(i, j).roadName.Contains("Line") ? lineRoad : rotateRoad;
                        var newRoad = Instantiate(road, miniMap.transform);
                        newRoad.transform.rotation = Quaternion.Euler(map.GetCell(i, j).rotation);
                        newRoad.transform.localScale = new Vector3((float)0.03, (float)0.03, 0);
                        newRoad.transform.localPosition = RoadPos(i, j);
                    }
                }
            }
        }
    }
    public void DeleteBtn()
    {
        if(SelectMapBorder.selected != null)
        {
            
            string mapName = SelectMapBorder.selected.GetChild(3).GetComponent<TextMesh>().text;
            Map removeMap = MapController.maps.Find(m => m.Name == mapName);
            MapController.maps.Remove(removeMap);
            for(int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            LoadMaps();
            miniMapIndexX = 0;
            miniMapIndexY = 0;
        }
    }
    public void MainMenuBtn()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void CreateMapBtn()
    {
        SceneManager.LoadScene("CreateMapScene");
    }

    public void StartMapBtn()
    {
        if (SelectMapBorder.selected != null)
        {
            string mapName = SelectMapBorder.selected.GetChild(3).GetComponent<TextMesh>().text;
            MapController.map = MapController.maps.Find(m => m.Name == mapName);
            MapController.SerializationMaps();
            SceneManager.LoadScene("MainGame");
        }
    }
}
