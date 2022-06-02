using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class GenericAlgorithm
{
    Population p;
    double percentBest = 0.05;
    public double percentSelect = 0.35;
    public double percentMutationIndividuals = 1;
    public double percentMutation = 0.01;
    public int countPopulation = 100;
    int dontChangeBestCount = 0;
    public int generationNumber = 1;
    int geneCount = 18;
    System.Random r = new System.Random();

    static public List<VirtualDriver> bestCars = new List<VirtualDriver>();

    delegate int[] SelectionType(float[] scores);
    delegate (VirtualDriver, VirtualDriver) CrossoverType(VirtualDriver p1, VirtualDriver p2);
    SelectionType selectionType;
    CrossoverType crossoverType;

    float[] min = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 500, 3, 3, 3, 10, 100 };
    float[] max = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1500, 10, 10, 10, 180, 2000 };
    //множители угла поворота(6), множители скорости(6), масса, размеры по осям(3),
    //максимальный угол поворота, мощность двигателя

    /// <summary>
    /// Создание нового поколения
    /// </summary>
    public VirtualDriver[] RandomPopulation()
    {
        generationNumber = 1;
        p = new Population(countPopulation);
        p.RandomPopulation(min, max);
        return p.GetDrivers();
    }

    #region [Select]
    VirtualDriver[] Select(VirtualDriver[] individuals)
    {
        float[] scoreCollections = new float[individuals.Length];
        for(int i = 0; i < individuals.Length; i++)
        {
            scoreCollections[i] = individuals[i].score;
        
        }
        var drivers = (from driver in individuals        // отбор небольшого процента особей которые 
                         orderby driver.score descending// не будет изменяться
                         select driver).ToArray();
        int[] indexes = selectionType(scoreCollections);
        VirtualDriver[] bestDriver = new VirtualDriver[(int)(individuals.Length * percentSelect)];
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

    /// <summary>
    /// Турнирный отбор
    /// </summary>
    /// <param name="scores"></param>
    /// <returns></returns>
    int[] TournamentSelection(float[] scores)
    {
        int count = 0;
        List<int> result = new List<int>();
        while(count < scores.Length * percentSelect)
        {
            int playerCount = r.Next(2, 5);
            int bestIndx = -1;
            float bestScore = -1;
            for(int i = 0; i < playerCount; i++)
            {
                int indx = r.Next(0, scores.Length);
                if(bestScore < scores[indx])
                {
                    bestScore = scores[indx];
                    bestIndx = indx;
                }
            }
            result.Add(bestIndx);
            count++;
        }
        return result.ToArray();
    }

    /// <summary>
    /// Ранговый отбор
    /// </summary>
    /// <param name="scores"></param>
    /// <returns></returns>
    int[] RankingSelection(float[] scores)
    {
        (float score, int index, float rank)[] tempRanks = new (float, int, float)[scores.Length];
        for(int i = 0; i < tempRanks.Length; i++)
        {
            tempRanks[i] = (scores[i], i, 0);
        }
        var ranks = (from i in tempRanks
                     orderby i.score
                     select i).ToArray();
        float sumRanks = 0;
        for(int i = 0; i < ranks.Length; i++)
        {
            ranks[i] = (ranks[i].score, ranks[i].index, i + 1);
            sumRanks += (i + 1);
        }
        int count = 0;
        List<int> result = new List<int>();
        while(count < scores.Length * percentSelect)
        {
            int indx = r.Next(ranks.Length);
            float probability = (float)(r.Next(100) + r.NextDouble());
            if(probability <= (ranks[indx].Item3 / sumRanks) * 100f)
            {
                result.Add(ranks[indx].index);
                count++;
            }
        }
        return result.ToArray();
    }

    /// <summary>
    /// Стахостическая выборка
    /// </summary>
    /// <param name="scores"></param>
    /// <returns></returns>
    int[] StochasticUniversalSampling(float[] scores)
    {
        float sumScore = scores.Sum();
        float step = sumScore / scores.Length;
        List<int> result = new List<int>();
        float[] partSum = new float[scores.Length];
        partSum[0] = scores[0];
        for(int i = 1; i < scores.Length; i++)
        {
            partSum[i] = partSum[i - 1] + scores[i];
        }
        float begin = (float)(r.Next(0, (int)sumScore) + r.NextDouble());
        int count = 0;
        while(count < scores.Length * percentSelect)
        {
            for(int i = 0; i < scores.Length; i++)
            {
                if(partSum[i] > begin)
                {
                    result.Add(i);
                    count++;
                    break;
                }
            }
            begin += step;
            if(begin > sumScore)
            {
                begin -= sumScore;
            }
        }
        return result.ToArray();
    }

    /// <summary>
    /// Метод рулетки
    /// </summary>
    /// <param name="scores"></param>
    /// <returns></returns>
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
    #endregion

    /// <summary>
    /// Запуск работы алгоритма
    /// </summary>
    public void StartWorkAlghoritm()
    {
        string logs = "";
        dontChangeBestCount = (int)(p.GetDrivers().Length * percentBest);
        var best = Select(p.GetDrivers());
        VirtualDriver driver = new VirtualDriver();
        driver.Solution = best[0].Solution;
        driver.score = best[0].score;
        bestCars.Add(driver);
        VirtualDriver[] newIndividuals = Replication(best);
        newIndividuals = Mutation(newIndividuals);
        p.SetPopulation(newIndividuals);
        if (!Directory.Exists("Generations"))
        {
            Directory.CreateDirectory("Generations");
        }
        for(int i = 0; i < dontChangeBestCount; i++)
        {
            logs += $"car_{VirtualDriver.carNumber}\n";
            logs += best[i].SolutionLogs();
            logs += "\n";
            logs += "Счет: " + best[i].score;
            logs += "\n";
            VirtualDriver.carNumber++;
        }
        File.WriteAllText($"Generations\\Generation №{generationNumber}.txt", logs);
        VirtualDriver.carNumber = 0;
        generationNumber++;
    }

    #region [Crossover]
    /// <summary>
    /// Метод размножения
    /// </summary>
    /// <param name="bestIndividuals"></param>
    /// <returns></returns>
    public VirtualDriver[] Replication(VirtualDriver[] bestIndividuals)
    {
        VirtualDriver[] newIndividuals = new VirtualDriver[countPopulation];
        for(int i = 0; i < dontChangeBestCount; i++)
        {
            newIndividuals[i] = bestIndividuals[i];
        }
        for(int i = dontChangeBestCount; i < countPopulation; i++)
        {
            VirtualDriver parent_1 = bestIndividuals[r.Next(bestIndividuals.Length)];
            VirtualDriver parent_2 = bestIndividuals[r.Next(bestIndividuals.Length)];
            var newDrivers = crossoverType(parent_1, parent_2);
            newIndividuals[i] = newDrivers.Item1;
            i++;
            if(i < countPopulation)
            {
                newIndividuals[i] = newDrivers.Item2;
            }
        }
        return newIndividuals;
    }


    /// <summary>
    /// Одноточечное скрещиванние
    /// </summary>
    /// <param name="parent_1"></param>
    /// <param name="parent_2"></param>
    /// <returns></returns>
    (VirtualDriver, VirtualDriver) OnePointCrossover(VirtualDriver parent_1, VirtualDriver parent_2)
    {
        int point = r.Next(1, geneCount - 1);
        VirtualDriver driver_1 = new VirtualDriver();
        VirtualDriver driver_2 = new VirtualDriver();
        for (int i = 0; i < geneCount; i++)
        {
            if(i < point)
            {
                driver_1.Solution[i] = parent_1.Solution[i];
                driver_2.Solution[i] = parent_2.Solution[i];
            }
            else
            {
                driver_1.Solution[i] = parent_2.Solution[i];
                driver_2.Solution[i] = parent_1.Solution[i];
            }
        }
        return (driver_1, driver_2);
    }

    (VirtualDriver, VirtualDriver) TwoPointCrossover(VirtualDriver parent_1, VirtualDriver parent_2)
    {
        int point1, point2;
        do
        {
            point1 = r.Next(1, geneCount / 2);
            point2 = r.Next(geneCount / 2, geneCount - 1);
        } while (point1 == point2);
        VirtualDriver driver_1 = new VirtualDriver();
        VirtualDriver driver_2 = new VirtualDriver();
        for (int i = 0; i < geneCount; i++)
        {
            if(i < point1 || i > point2)
            {
                driver_1.Solution[i] = parent_1.Solution[i];
                driver_2.Solution[i] = parent_2.Solution[i];
            }
            else
            {
                driver_1.Solution[i] = parent_2.Solution[i];
                driver_2.Solution[i] = parent_1.Solution[i];
            }
        }
        return (driver_1, driver_2);
    }

    (VirtualDriver, VirtualDriver) UniformCrossover(VirtualDriver parent_1, VirtualDriver parent_2)
    {
        int geneIndx = r.Next(0, geneCount);
        VirtualDriver driver_1 = new VirtualDriver();
        VirtualDriver driver_2 = new VirtualDriver();
        for (int i = 0; i < geneCount; i++)
        {
            if(i == geneIndx)
            {
                driver_1.Solution[i] = parent_2.Solution[i];
                driver_2.Solution[i] = parent_1.Solution[i];
            }
            else
            {
                driver_1.Solution[i] = parent_1.Solution[i];
                driver_2.Solution[i] = parent_2.Solution[i];
            }
        }
        return (driver_1, driver_2);
    }

    (VirtualDriver, VirtualDriver) BlendCrossover(VirtualDriver parent_1, VirtualDriver parent_2)
    {
        VirtualDriver driver_1 = new VirtualDriver();
        VirtualDriver driver_2 = new VirtualDriver();
        for (int i = 0; i < geneCount; i++)
        {
            float min = Mathf.Min(parent_1.Solution[i], parent_2.Solution[i]);
            float max = Mathf.Max(parent_1.Solution[i], parent_2.Solution[i]);
            float begin = parent_1.Solution[i] - 0.5f * (max - min);
            float end = parent_2.Solution[i] + 0.5f * (max - min);
            driver_1.Solution[i] = Random.Range(begin, end);
            driver_2.Solution[i] = Random.Range(begin, end);
        }
        return (driver_1, driver_2);
    }

    (VirtualDriver, VirtualDriver) SimulatedBinaryCrossover(VirtualDriver parent_1, VirtualDriver parent_2)
    {
        VirtualDriver driver_1 = new VirtualDriver();
        VirtualDriver driver_2 = new VirtualDriver();
        for (int i = 0; i < geneCount; i++)
        {
            float u = Random.Range(0f, 1f);
            float beta = 0;
            if(u < 0.5)
            {
                beta = Mathf.Pow(2 * u, 1f / (10f + 1f));
            }
            else
            {
                beta = Mathf.Pow(0.5f * (1f - u), 1f / (10f + 1f));
            }
            driver_1.Solution[i] = 0.5f * ((1 + beta) * parent_1.Solution[i] + (1 - beta) * parent_2.Solution[i]);
            driver_2.Solution[i] = 0.5f * ((1 - beta) * parent_1.Solution[i] + (1 + beta) * parent_2.Solution[i]);
        }
        return (driver_1, driver_2);
    }
    #endregion

    #region [Mutation]
    /// <summary>
    /// Метод мутации
    /// </summary>
    /// <param name="individuals"></param>
    /// <returns></returns>
    VirtualDriver[] Mutation(VirtualDriver[] individuals)
    {
        for(int i = dontChangeBestCount; i < individuals.Length * percentMutationIndividuals; i++)
        {
            //int indx = r.Next(individuals.Length);
            VirtualDriver driver = individuals[i];
            for (int j = 0; j < geneCount; j++)
            {
                double mutation = r.Next(100) + r.NextDouble();
                if(mutation < percentMutation * 100)
                {
                    driver.Solution[j] = (float)RandomGene(driver.Solution[j], j);
                }
            }
            individuals[i] = driver;
        }
        return individuals;
    }


    /// <summary>
    /// Получение случайных генов, вызывается в методе мутации
    /// </summary>
    /// <returns>Случаное значение гена</returns>
    double RandomGene(float gene, int indx)
    {
        return r.Next((int)min[indx], (int)max[indx]) + r.NextDouble();
    }
    #endregion

    public VirtualDriver[] GetDrivers()
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

    public void ChangeSelectType(int number)
    {
        switch (number)
        {
            case 0: 
                selectionType = FPS;
                break;
            case 1: 
                selectionType = StochasticUniversalSampling;
                break;
            case 2: 
                selectionType = RankingSelection;
                break;
            case 3: 
                selectionType = TournamentSelection;
                break;
        }
    }

    public void ChangeCrossoverType(int number)
    {
        switch (number)
        {
            case 0:
                crossoverType = OnePointCrossover;
                break;
            case 1:
                crossoverType = TwoPointCrossover;
                break;
            case 2:
                crossoverType = UniformCrossover;
                break;
            case 3:
                crossoverType = BlendCrossover;
                break;
            case 4:
                crossoverType = SimulatedBinaryCrossover;
                break;
        }
    }

    public GenericAlgorithm()
    {
        selectionType = FPS;
        crossoverType = OnePointCrossover;
        Driver.forwardDistance = new float[6];
        for(int i = 0; i < 6; i++)
        {
            Driver.forwardDistance[i] = 15 * (i + 1);
        }
        if (Directory.Exists("Generations"))
        {
            Directory.Delete("Generations", true);
        }
    }
}