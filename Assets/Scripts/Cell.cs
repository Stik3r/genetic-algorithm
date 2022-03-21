using System;
using UnityEngine;


[Serializable]
public class Cell
{
    [SerializeField]
    public Vector3 rotation;
    public string roadName;

    public Cell()
    {
        rotation = Vector3.one;
        roadName = "";
    }
    public Cell(Vector3 _rotation, string _roadName)
    {
        rotation = _rotation;
        roadName = _roadName;
    }

    /// <summary>
    /// Метод получения пустой клетки
    /// </summary>
    /// <returns>Возвращает клетку с соответствующими значениями </returns>
    static public Cell EpmtyCell()
    {
        return new Cell(Vector3.one, "");
    }

    static public bool operator ==(Cell cell1, Cell cell2)
    {
        return (cell1.rotation == cell2.rotation && cell1.roadName == cell2.roadName);
    }

    static public bool operator !=(Cell cell1, Cell cell2)
    {
        return (cell1.rotation != cell2.rotation && cell1.roadName != cell2.roadName);
    }
}

