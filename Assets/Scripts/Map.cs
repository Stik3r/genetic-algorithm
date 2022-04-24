using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Map
{
    [SerializeField]
    public Cell[][] map = new Cell[7][];
    [SerializeField]
    string nameMap = "";

    public Map()
    {
        for (int i = 0; i < 7; i++)
        {
            map[i] = new Cell[6];
            for (int j = 0; j < 6; j++)
            {
                map[i][j] = Cell.EpmtyCell();
            }
        }
    }

    public string Name 
    { 
        get { return nameMap; } 
        set { nameMap = value; }
    }

    public bool Check(int x, int y)
    {
        if (map[x][y] == Cell.EpmtyCell())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetRoad(GameObject road, int x, int y)
    {

        map[x][y] = new Cell(road.transform.eulerAngles, road.name);
    }
    public void RemoveRoad(int x, int y)
    {
        map[x][y] = Cell.EpmtyCell();
    }

    public string NameRoad(int x, int y)
    {
        return map[x][y].roadName;
    }

    public Vector3 Rotation(int x, int y)
    {
        return map[x][y].rotation;
    }
    
    public Cell GetCell(int x, int y)
    {
        return map[x][y];
    }

    /// <summary>
    /// Нахождение стартовой позиции
    /// </summary>
    /// <returns></returns>
    public (int, int) FindStartPosition()
    {
        for(int i = 0; i < map.Length; i++)
        {
            for(int j = 0; j < map[i].Length; j++)
            {
                if(map[i][j] != Cell.EpmtyCell() && map[i][j].roadName.Contains("Line"))
                {
                    return (i, j);
                }
            }
        }
        return (0, 0);
    }

    public (int, int) LineNextSection((int, int) previous, (int, int) current, bool firstSection = false)
    {
        if (firstSection)
        {
            return (map[current.Item1][current.Item2].rotation.z == 0 ||
            map[current.Item1][current.Item2].rotation.z == 180) ?
            (current.Item1, current.Item2 + 1) : (current.Item1 + 1, current.Item2);
        }
        if(map[current.Item1][current.Item2].rotation.z == 0 ||
            map[current.Item1][current.Item2].rotation.z == 180)
        {
            return current.Item2 - 1 == previous.Item2 ? (current.Item1, current.Item2 + 1)
                : (current.Item1, current.Item2 - 1);
        }
        else
        {
            return current.Item1 - 1 == previous.Item1 ? (current.Item1 + 1, current.Item2)
                : (current.Item1 - 1, current.Item2);
        }
    }

    public (int, int) RotateNextSection((int, int) previous, (int, int) current)
    {
        switch (map[current.Item1][current.Item2].rotation.z)
        {
            case 0:
                return current.Item1 + 1 == previous.Item1 ? (current.Item1, current.Item2 - 1)
                    : (current.Item1 + 1, current.Item2);
            case 90:
                return current.Item1 + 1 == previous.Item1 ? (current.Item1, current.Item2 + 1)
                    : (current.Item1 + 1, current.Item2);
            case 180:
                return current.Item1 - 1 == previous.Item1 ? (current.Item1, current.Item2 + 1)
                    : (current.Item1 - 1, current.Item2);
            default:
                return current.Item1 - 1 == previous.Item1 ? (current.Item1, current.Item2 - 1)
                    : (current.Item1 - 1, current.Item2);

        }
    }
}