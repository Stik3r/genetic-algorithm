using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public static class MapController
{
    public static Map map = new Map();
    [SerializeField]
    public static List<Map> maps;
    public static GameObject[] triggers;

    static MapController()
    {

    }

    public static void RemoveRoad(int indxX, int indxY)
    {
        map.RemoveRoad(indxX, indxY);
    }

    public static bool Check(int indxX, int indxY)
    {
        return map.Check(indxX, indxY);
    }

    public static void SetRoad(GameObject road, int indxX, int indxY)
    {
        map.SetRoad(road, indxX, indxY);
    }
    public static string RoadName(int indxX, int indxY)
    {
        return map.NameRoad(indxX, indxY);
    }

    public static Vector3 Rotation(int indxX, int indxY)
    {
        return map.Rotation(indxX, indxY);
    }

    public static void DeserializationMaps()
    {
        if (File.Exists("Maps.xml"))
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<Map>));
            using (FileStream fs = new FileStream("Maps.xml", FileMode.OpenOrCreate))
            {
                maps = (List<Map>)formatter.Deserialize(fs);
            }
        }
        else
        {
            maps = new List<Map>();
        }
    }

    public static void SerializationMaps()
    {
        XmlSerializer formatter = new XmlSerializer(typeof(List<Map>));
        if (File.Exists("Maps.xml"))
        {
            File.Delete("Maps.xml");
        }
        using (FileStream fs = new FileStream("Maps.xml", FileMode.OpenOrCreate))
        {
            formatter.Serialize(fs, maps);
        }
    }

    public static bool FindSameMap(string mapName)
    {
        if(maps.Count > 0)
        {
            return maps.Exists(x => x.Name == mapName);
        }
        return false;
    }

    public static void AddMap()
    {
        maps.Add(map);
    }

    /// <summary>
    /// Получение позиции в мире относительно позиции в массиве
    /// </summary>
    /// <param name="i">Индекс</param>
    /// <param name="j">Индекс</param>
    /// <returns></returns>
    public static Vector3 GetPos(int i, int j)
    {
        double xPos = i * 15;
        double zPos = j * 15;
        return new Vector3((float)xPos, 0, (float)zPos);
    }

    /// <summary>
    /// Получение угла вращения относительно угла вращения в массиве
    /// </summary>
    /// <param name="angels">Вектор поворота</param>
    /// <returns></returns>
    public static Quaternion GetAngels(Vector3 angels)
    {
        return Quaternion.Euler(90, 0, angels.z);
    }

    public static List<(int, int)> CreateWay()
    {
        List<(int, int)> way = new List<(int, int)>();
        var prevPos = map.FindStartPosition();
        var nextPos = map.LineNextSection((0, 0), prevPos, true);
        var startPos = prevPos;
        way.Add(prevPos);
        while (nextPos != way[0])
        {
            way.Add(nextPos);
            var buf = nextPos;
            if (map.GetCell(nextPos.Item1, nextPos.Item2).roadName.Contains("Line"))
            {
                nextPos = map.LineNextSection(prevPos, nextPos);
            }
            else
            {
                nextPos = map.RotateNextSection(prevPos, nextPos);
            }
            prevPos = buf;
        }
        return way;
    }

}

