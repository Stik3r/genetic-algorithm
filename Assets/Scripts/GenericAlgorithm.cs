using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GenericAlgorithm
{
    Population p = new Population();
    double percentBest = 0.05;
    double percentSelect = 0.35;
    double percentMutationIndividuals = 1;
    double percentMutation = 0.01;
    public int countPopulation = 150;
    int dontChangeBestCount = 0;
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
        float[] scoreCollections = new float[countPopulation];
        for(int i = 0; i < countPopulation; i++)
        {
            scoreCollections[i] = individuals[i].score;
        
        }
        var drivers = (from driver in individuals        // отбор небольшого процента особей которые 
                         orderby driver.score descending// не будет изменяться
                         select driver).ToArray<Driver>();
        int[] indexes = FPS(scoreCollections.ToArray());
        Driver[] bestDriver = new Driver[(int)(individuals.Length * percentSelect)];
        for(int i = 0; i < dontChangeBestCount; i++)
        {
            bestDriver[i] = drivers[i];
        }
        for (int i = dontChangeBestCount; i < bestDriver.Length; i++)
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
        newIndividuals = Mutation(newIndividuals);
        p.SetPopulation(newIndividuals);
        if (!Directory.Exists("Generations"))
        {
            Directory.CreateDirectory("Generations");
        }
        for(int i = 0; i < dontChangeBestCount; i++)
        {
            logs += $"car_{Driver.number}\n";
            logs += best[i].GetWeightsLogs();
            logs += "\n";
            logs += "Счет: " + best[i].score;
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
        for(int i = 0; i < dontChangeBestCount; i++)
        {
            newIndividuals[i] = bestIndividuals[i];
        }
        for(int i = dontChangeBestCount; i < countPopulation; i++)
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
        int point = r.Next(Driver.rayCount / 2 + (Driver.rayCount - 1));
        double[][] weightRotate = new double[Driver.rayCount / 2][];
        double[][] weightForward = new double[Driver.rayCount - 1][];
        for(int i = 0; i < Driver.rayCount / 2 + (Driver.rayCount - 1); i++)
        {
            if(i < point)
            {
                if(i < Driver.rayCount / 2)
                {
                    weightRotate[i] = NewWeights(parent_1.WeightsRotate[i]);
                }
                else
                {
                    weightForward[i - (Driver.rayCount / 2)] =
                        NewWeights(parent_1.WeightsForward[i - (Driver.rayCount / 2)]);
                }
            }
            else
            {
                if (i < Driver.rayCount / 2)
                {
                    weightRotate[i] = NewWeights(parent_2.WeightsRotate[i]);
                }
                else
                {
                    weightForward[i - (Driver.rayCount / 2)] =
                        NewWeights(parent_2.WeightsForward[i - (Driver.rayCount / 2)]);
                }
            }
        }
        Driver newDriver = new Driver();
        newDriver.WeightsForward = weightForward;
        newDriver.WeightsRotate = weightRotate;
        return newDriver;
    }

    double[] NewWeights(double[] weights)
    {
        double[] result = new double[2];
        for(int i = 0; i < 2; i++)
        {
            result[i] = weights[i];
        }
        return result;
    }
    /// <summary>
    /// Метод мутации
    /// </summary>
    /// <param name="individuals"></param>
    /// <returns></returns>
    Driver[] Mutation(Driver[] individuals)
    {
        for(int i = dontChangeBestCount; i < individuals.Length * percentMutationIndividuals; i++)
        {
            //int indx = r.Next(individuals.Length);
            Driver driver = individuals[i];
            for(int j = 0; j < Driver.rayCount / 2; j++)
            {
                for(int k = 0; k < driver.WeightsRotate[j].Length; k++)
                {
                    double mutation = r.Next(100) + r.NextDouble();
                    if (mutation < percentMutation * 100)
                    {
                        driver.WeightsRotate[j][k] = RandomGene(driver.WeightsRotate[j][k]);
                    }
                }
            }
            for (int j = 0; j < Driver.rayCount - 1; j++)
            {
                int mutation = r.Next(100);
                if (mutation < percentMutation * 100)
                {
                    driver.WeightsForward[j][1] = RandomGene(driver.WeightsForward[j][1]);
                }
            }
            individuals[i] = driver;
        }
        return individuals;
    }


    /// <summary>
    /// Получение случайных генов, вызывается в методе мутации
    /// </summary>
    /// <returns></returns>
    double RandomGene(double gene)
    {
        double delta = r.NextDouble();
        return gene + delta > 1 || gene + delta < -1 ? gene - delta : gene + delta;
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
        dontChangeBestCount = (int)(countPopulation * percentBest);
        if (Directory.Exists("Generations"))
        {
            Directory.Delete("Generations", true);
        }
    }
}