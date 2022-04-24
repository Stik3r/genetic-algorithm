using System.Collections;
using System.Linq;
using UnityEngine;

public class Population
{
    Driver[] individuals;


    public Population()
    {

    }

    /// <summary>
    /// ”становка новой попул€ции
    /// </summary>
    /// <param name="newPopulation"></param>
    public void SetPopulation(Driver[] newPopulation)
    {
        individuals = newPopulation;
    }

    public Driver[] GetDrivers()
    {
        return individuals;
    }

    public void SetScore(int indx, float score)
    {
        individuals[indx].score = score;
    }
}
