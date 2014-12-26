namespace GeneticAlgorithmFramework
{
    public interface IIndividual
    {
        bool[] Genotype { get; set; }
        Phenotype Phenotype { get; set; }
        int Fitness { get; set; }

        Phenotype Decode();
        int[] Encode(Phenotype phenotype);
        string ToString();
    }
}