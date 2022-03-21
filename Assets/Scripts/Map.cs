using System;
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
}