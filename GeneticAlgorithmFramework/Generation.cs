using System;

namespace GeneticAlgorithmFramework
{
    public class Generation
    {
        public int TotalFitness = 0;
        public double MeanFitness = 0;
        public int BestFitness = 0;
        public Individual BestIndividual;

        public Individual[] Population;

        public Generation()
        {
            Population = new Individual[Parameters.PopulationSize];
        }

        static public Individual[] SeedPopulation()
        {
            Individual[] seedPop = new Individual[Parameters.PopulationSize];

            for (int i = 0; i < Parameters.PopulationSize; i++)
            {
                seedPop[i] = new Individual(); ;
                for (int j = 0; j < Parameters.GeneomeLength; j++)
                {
                    seedPop[i].Genotype[j] = Convert.ToBoolean(GeneticAlgorithmDriver.RandomGenerator.Next() % 2);
                }
                seedPop[i].Fitness = 0;
            }
            return seedPop;
        }

        public void EvaluatePopulation()
        {
            int tmpFitness = 0;
            foreach (Individual individual in Population)
            {
                individual.Fitness = GeneticOps.Evaluate_BinaryClassification(individual);
                tmpFitness += individual.Fitness;
                if (individual.Fitness > BestFitness)
                {
                    BestFitness = individual.Fitness;
                    BestIndividual = individual;
                }
            }
            TotalFitness = tmpFitness;
            MeanFitness = TotalFitness / (double)Parameters.PopulationSize;
        }


    }
}