using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class CreateMapController : MonoBehaviour
{
    public InputField field;
    public GameObject emptyNamePanel;
    public GameObject sameNamePanel;
    double zeroPointX = 0;
    double zeroPointY = 0;
    double roadSize = 15.75;
    Vector3 startPos = Vector3.one;
    Vector3 outConvasScale = new Vector3((float)1.041667, (float)1.041667, 1);

    /// <summary>
    /// Создание нового дороги или начало перемещения старой
    /// </summary>
    public void OnMouseDown()
    {
        if(transform.parent.name == "Box")
        {
            GameObject road = Instantiate(transform.gameObject, transform.parent);
            transform.parent = null;
        }
        else
        {
            startPos = transform.localPosition;
            float x = GetRoundX(transform.localPosition.x, out int indexX);
            float y = GetRoundY(transform.localPosition.y, out int indexY);
            MapController.RemoveRoad(indexX, indexY);
            transform.parent = null;
        }
    }

    /// <summary>
    /// Перемещение дороги за курсором мыши
    /// </summary>
    public void OnMouseDrag()
    {
        var mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
        transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        if (Input.GetMouseButtonDown(1))
        {
            transform.Rotate(0, 0, 90);
        }
    }

    /// <summary>
    /// Отжатие клавиши мыши и выполнение необходимых проверок
    /// </summary>
    public void OnMouseUpAsButton()
    {
        var roadPos = transform.position;
        if (roadPos.x > -27)
        {
            transform.parent = GameObject.Find("Map").transform;
            float x = GetRoundX(transform.localPosition.x, out int indexX);
            float y = GetRoundY(transform.localPosition.y, out int indexY);
            transform.parent = null;
            if (MapController.Check(indexX, indexY))
            {
                MapController.SetRoad(transform.gameObject, indexX, indexY);
                transform.parent = GameObject.Find("Map").transform;
                transform.localPosition = new Vector3(x, y, 1);
            }
            else
            {
                if(startPos == Vector3.one)
                {
                    Destroy(transform.gameObject);
                }
                else
                {
                    transform.parent = transform.parent = GameObject.Find("Map").transform;
                    transform.localPosition = startPos;
                    startPos = Vector3.one;
                }
            }
        }
        else
        {
            Destroy(transform.gameObject);
        }
    }


    /// <summary>
    /// Получение округленного значения координаты Х
    /// </summary>
    /// <param name="x">Х-координата дороги</param>
    /// <param name="_indx">Индекс данной дороги в массиве</param>
    /// <returns></returns>
    float GetRoundX(float x, out int _indx)
    {
        if(x <= zeroPointX)
        {
            _indx = 0;
            return (float)zeroPointX;
        }
        if(x >= 94.5)
        {
            _indx = 6;
            return (float)94.5;
        }
        int index = (int)Mathf.Round((Mathf.Abs((float)zeroPointX) + x)/(float)roadSize);
        _indx = index;
        return (float)(zeroPointX + roadSize * index);
    }

    /// <summary>
    /// Получение округленного значения координаты У
    /// </summary>
    /// <param name="x">У-координата дороги</param>
    /// <param name="_indx">Индекс данной дороги в массиве</param>
    /// <returns></returns>
    float GetRoundY(float y, out int _indx)
    {
        if (y >= 78.75)
        {
            _indx = 5;
            return (float)78.75;
        }
        if (y <= zeroPointY)
        {
            _indx = 0;
            return (float)zeroPointY;
        }
        int index = (int)Mathf.Round(((float)zeroPointY + y) / (float)roadSize);
        _indx = index;
        return (float)(zeroPointY + roadSize * index);
    }


    /// <summary>
    /// Кнопка старта
    /// </summary>
    public void StartBtn()
    {
        if(field.text == "")
        {
            emptyNamePanel.SetActive(true);
            return;
        }
        if (MapController.FindSameMap(field.text))
        {
            sameNamePanel.SetActive(true);
            return;
        }
        MapController.map.Name = field.text;
        MapController.AddMap();
        MapController.SerializationMaps();
        SceneManager.LoadScene("MainGame");
    }

    public void OKBtn()
    {
        emptyNamePanel.SetActive(false);
    }
    public void OKBtn_2()
    {
        sameNamePanel.SetActive(false);
    }

    /// <summary>
    /// Кнопка выхода
    /// </summary>
    public void ExitBtnn()
    {
        SceneManager.LoadScene("SelectMap");
    }
}
