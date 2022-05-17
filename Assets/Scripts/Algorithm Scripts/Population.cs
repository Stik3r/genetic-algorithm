
public class Population
{
    VirtualDriver[] individuals;
    static readonly System.Random r = new System.Random();


    public Population(int countPopulation)
    {
        individuals = new VirtualDriver[countPopulation];
        for(int i = 0; i < countPopulation; i++)
        {
            individuals[i] = new VirtualDriver();
        }
    }

    /// <summary>
    /// ”становка новой попул€ции
    /// </summary>
    /// <param name="newPopulation"></param>
    public void SetPopulation(VirtualDriver[] newPopulation)
    {
        individuals = newPopulation;
    }

    public VirtualDriver[] GetDrivers()
    {
        return individuals;
    }

    public void SetScore(int indx, float score)
    {
        individuals[indx].score = score;
    }

    public void RandomPopulation(float[] min, float[] max)
    {
        foreach(var induvid in individuals)
        {
            for(int i = 0; i < induvid.Solution.Length; i++)
            {
                induvid.SetRandomValue((int)min[i], (int)max[i], i);
            }
        }
    }
}
