using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GenericAlgorithm
{
    Population p = new Population();
    double percentSelect = 0.2;
    double percentMutationIndividuals = 1;
    double percentMutation = 0.05;
    public int countPopulation = 100;
    int generationNumber = 1;
    System.Random r = new System.Random();

    /// <summary>
    /// Создание нового поколения
    /// </summary>
    /// <param name="individuals"></param>
    public void NewPopulation(GameObject[] individuals)
    {
        List<Driver> drivers = new List<Driver>();
        foreach(var cars in individuals)
        {
            drivers.Add(cars.GetComponent<Driver>());
        }
        p.SetPopulation(drivers.ToArray());
    }

    Driver[] Select(Driver[] individuals)
    {
        var scoreCollections = from driver in individuals
                               select driver.score;
        int[] indexes = FPS(scoreCollections.ToArray());
        Driver[] bestDriver = new Driver[(int)(individuals.Length * percentSelect)];
        for(int i = 0; i < bestDriver.Length; i++)
        {
            bestDriver[i] = individuals[indexes[i]];
        }
        return bestDriver;
    }

    int[] FPS(float[] scores)
    {
        float allScore = scores.Sum();
        int count = 0;
        List<int> result = new List<int>();
        while (count < scores.Count() * percentSelect)
        {
            int indx = r.Next(scores.Length);
            Debug.Log("1");
            float probability = (float)(r.Next(100) + r.NextDouble());
            if(probability <= scores[indx] / allScore * 100)
            {
                result.Add(indx);
                count++;
            }
        }
        return result.ToArray();
    }

    /// <summary>
    /// Запуск работы алгоритма
    /// </summary>
    public void StartWorkAlghoritm()
    {
        string logs = "";

        var best = Select(p.GetDrivers());
        Driver[] newIndividuals = Replication(best);
        File.WriteAllText("Log.txt", logs);
        newIndividuals = Mutation(newIndividuals);
        p.SetPopulation(newIndividuals);
        if (!Directory.Exists("Generations"))
        {
            Directory.CreateDirectory("Generations");
        }
        foreach(var drive in newIndividuals)
        {
            logs += $"car_{Driver.number}\n";
            logs += drive.GetWeightsLogs();
            logs += "\n";
            Driver.number++;
        }
        File.WriteAllText($"Generations\\Generation №{generationNumber}.txt", logs);
        Driver.number = 0;
        generationNumber++;
    }

    /// <summary>
    /// Метод размножения
    /// </summary>
    /// <param name="bestIndividuals"></param>
    /// <returns></returns>
    public Driver[] Replication(Driver[] bestIndividuals)
    {
        Driver[] newIndividuals = new Driver[countPopulation];
        for(int i = 0; i < countPopulation; i++)
        {
            Driver parent_1 = bestIndividuals[r.Next(bestIndividuals.Length)];
            Driver parent_2 = bestIndividuals[r.Next(bestIndividuals.Length)];
            newIndividuals[i] = OnePointCrossover(parent_1, parent_2);
        }
        return newIndividuals;
    }


    /// <summary>
    /// Одноточечное скрещиванние
    /// </summary>
    /// <param name="parent_1"></param>
    /// <param name="parent_2"></param>
    /// <returns></returns>
    Driver OnePointCrossover(Driver parent_1, Driver parent_2)
    {
        int point = r.Next(Driver.rayCount);
        double[][] weightRotate = new double[Driver.rayCount][];
        double[][] weightForward = new double[Driver.rayCount][];
        for(int i = 0; i < Driver.rayCount; i++)
        {
            if(i < point)
            {
                weightRotate[i] = parent_1.WeightsRotate[i];
                weightForward[i] = parent_1.WeightsForward[i];
            }
            else
            {
                weightRotate[i] = parent_2.WeightsRotate[i];
                weightForward[i] = parent_2.WeightsForward[i];
            }
        }
        Driver newDriver = new Driver();
        newDriver.WeightsForward = weightForward;
        newDriver.WeightsRotate = weightRotate;
        return newDriver;
    }

    /// <summary>
    /// Метод мутации
    /// </summary>
    /// <param name="individuals"></param>
    /// <returns></returns>
    Driver[] Mutation(Driver[] individuals)
    {
        for(int i = 0; i < individuals.Length * percentMutationIndividuals; i++)
        {
            int indx = r.Next(individuals.Length);
            Driver driver = individuals[indx];
            for(int j = 0; j < Driver.rayCount; j++)
            {
                int mutation = r.Next(100);
                if(mutation < percentMutation * 100)
                {
                    driver.WeightsRotate[j] = RandomGenes();
                }
            }
            for (int j = 0; j < Driver.rayCount; j++)
            {
                int mutation = r.Next(100);
                if (mutation < percentMutation * 100)
                {
                    driver.WeightsForward[j] = RandomGenes();
                }
            }
            individuals[indx] = driver;
        }
        return individuals;
    }


    /// <summary>
    /// Получение случайных генов, вызывается в методе мутации
    /// </summary>
    /// <returns></returns>
    double[] RandomGenes()
    {
        double[] genes = new double[2];
        genes[0] = r.Next(-10, 10) + r.NextDouble();
        genes[1] = r.Next(-10, 10) + r.NextDouble();
        return genes;
    }

    public Driver[] GetDrivers()
    {
        return p.GetDrivers();
    }

    public void SetScore(GameObject[] cars)
    {
        for(int i = 0; i < cars.Length; i++)
        {
            p.SetScore(i, cars[i].GetComponent<Driver>().score);
        }
    }

    public GenericAlgorithm()
    {
        if (Directory.Exists("Generations"))
        {
            Directory.Delete("Generations", true);
        }
    }
}