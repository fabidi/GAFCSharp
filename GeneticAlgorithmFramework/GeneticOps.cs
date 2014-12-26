using System;

namespace GeneticAlgorithmFramework
{
    public class GeneticOps
    {
        /// <summary>
        /// Tournament selection with T=2
        /// </summary>
        /// <param name="population"></param>
        /// <returns></returns>
        public static Individual[] Selection_TournamentWithReplacement(Individual[] population)
        {
            int popSize = population.Length;
            Individual[] offspring = new Individual[popSize];
            for (int iTournamentMatch = 0; iTournamentMatch < population.Length; iTournamentMatch++)
            {
                Individual ind1 = null;
                Individual ind2 = null;
                while (ind1 == ind2) // Avoid double picking the same individual in the same tournament match.
                {
                    int randomIndividualIndex = GeneticAlgorithmDriver.RandomGenerator.Next() % popSize;
                    ind1 = population[randomIndividualIndex];
                    randomIndividualIndex = GeneticAlgorithmDriver.RandomGenerator.Next() % popSize;
                    ind2 = population[randomIndividualIndex];
                }
                // Fight!
                if (ind1.Fitness >= ind2.Fitness)
                    offspring[iTournamentMatch] = new Individual(CopyGenes(ind1.Genotype));
                else
                    offspring[iTournamentMatch] = new Individual(CopyGenes(ind2.Genotype));
            }
            return offspring;
        }

        public static bool[] CopyGenes(bool[] genes)
        {
            bool[] copiedGenes = new bool[genes.Length];
            for (int i = 0; i < genes.Length; i++)
            {
                copiedGenes[i] = genes[i];
            }
            return copiedGenes;
        }

        /// <summary>
        /// Tournament selection with T=2
        /// Performs a single tournament selection match on the population and returns the indexes of 2 individuals
        /// who will be used as crossover parents.
        /// </summary>
        /// <param name="population"></param>
        /// <returns></returns>
        public static int[] SelectionForCrossover_TournamentWithReplacement(Individual[] population)
        {
            int[] parentsSelectedForCrossoverIndexes = new int[2];
            for (int iParent = 0; iParent < 2; iParent++)
            {
                int parentAIndex = GeneticAlgorithmDriver.RandomGenerator.Next() % population.Length;
                int parentBIndex = GeneticAlgorithmDriver.RandomGenerator.Next() % population.Length;
                Individual parentA = population[parentAIndex];
                Individual parentB = population[parentBIndex];
                if (parentA.Fitness >= parentB.Fitness)
                    parentsSelectedForCrossoverIndexes[iParent] = parentAIndex;
                else
                    parentsSelectedForCrossoverIndexes[iParent] = parentBIndex;
            }
            return parentsSelectedForCrossoverIndexes;
        }

        public static int Evaluate_CountingOnes(int[] genes)
        {
            int fitness = 0;
            for (int i = 0; i < genes.Length; i++)
            {
                fitness += genes[i];
            }
            return fitness;
        }

        /// <summary>
        /// Performs the evaluation by calling the appropriate Fitness function.
        /// </summary>
        /// <param name="genes"></param>
        /// <param name="testingSet">Pass in a testing set to evaluate against. Pass null to automatically use the preloaded training set.</param>
        /// <returns></returns>
        public static int Evaluate_BinaryClassification(Individual ind, string[] testingSet = null)
        {
            int fitness = Fitness_Classify(ind, testingSet);
            return fitness;

        }

        /// <summary>
        /// The fitness function.
        /// </summary>
        /// <param name="genes"></param>
        /// <param name="dataset">Pass in a testing set to evaluate against. Pass null to automatically use the preloaded training set.</param>
        /// <returns></returns>
        private static int Fitness_Classify(Individual ind, string[] dataset = null)
        {
            int fitness = 0;
            if (dataset == null)
                dataset = DatasetManager.GetTrainingSet();

            foreach (string input in dataset)
            {
                string[] parts = input.Split(' ');
                string dataValue = parts[0];
                sbyte actualClassification = Convert.ToSByte(parts[1]);
                // Binary to Decimal.
                int value = Convert.ToInt32(dataValue, 2);
                sbyte learnedClassification = -1;
                // NOTE: We can select between Lookup datastructure and Rule-Based datastructure.
                if (Parameters.DataStructure == GeneticDataStructures.LOOKUP)
                {
                    learnedClassification = Convert.ToSByte(Fitness_ClassifyLookup(ind.Genotype, value));
                }
                else if (Parameters.DataStructure == GeneticDataStructures.RULEBASED)
                {
                    learnedClassification = Fitness_ClassifyRanged(ind, value);
                }

                if (learnedClassification == actualClassification)
                    fitness++;
            }
            return fitness;
        }

        private static sbyte Fitness_ClassifyRanged(Individual ind, int value)
        {
            Phenotype phenotype = ind.Decode();
            sbyte learnedClassification = phenotype.Classify(value);
            return learnedClassification;
        }

        private static bool Fitness_ClassifyLookup(bool[] genes, int value)
        {
            return genes[value];
        }

        /// <summary>
        /// Performs crossover on the entire population and returns the offspring population.
        /// </summary>
        /// <param name="population"></param>
        /// <returns></returns>
        public static Individual[] CrossoverPopulation_SinglePoint(Individual[] population)
        {
            int popSize = population.Length;
            Individual[] offsprings = new Individual[popSize];
            for (int i = 0; i < popSize; i++)
            {
                double randomFraction = getRandomFraction();
                // Select a pair of parents using Tournament (W/ Replacement).
                int[] parentPairIndexes = SelectionForCrossover_TournamentWithReplacement(population);
                Individual[] parentPair = { population[parentPairIndexes[0]], population[parentPairIndexes[1]] };
                int crossOverIndex;
                if (randomFraction <= Parameters.CrossoverRate)
                {
                    crossOverIndex = GeneticAlgorithmDriver.RandomGenerator.Next(Parameters.GeneomeLength);
                    Individual[] offspringPair = CrossoverPair_SinglePoint(parentPair, crossOverIndex);
                    offsprings[i] = new Individual(CopyGenes(offspringPair[0].Genotype));
                    offsprings[i + 1] = new Individual(CopyGenes(offspringPair[1].Genotype));
                }
                else
                {
                    // No Crossover. Copy as is.
                    offsprings[i] = new Individual(CopyGenes(parentPair[0].Genotype));
                    offsprings[i + 1] = new Individual(CopyGenes(parentPair[1].Genotype));
                }
                i++;
            }
            return offsprings;
        }

        /// <summary>
        /// Performs SinglePoint crossover on a pair of parent and returns a pair of offsprings.
        /// </summary>
        /// <param name="parents"></param>
        /// <param name="crossOverIndex"></param>
        /// <returns></returns>
        public static Individual[] CrossoverPair_SinglePoint(Individual[] parents, int crossOverIndex = -1)
        {
            Individual parent1 = parents[0];
            Individual parent2 = parents[1];
            int genomeLength = parent1.Genotype.Length;
            Individual[] offspringPair = new Individual[2];
            offspringPair[0] = new Individual();
            offspringPair[1] = new Individual();
            // Pick random cross over point within the genome bits.
            if (crossOverIndex == -1)
                crossOverIndex = GeneticAlgorithmDriver.RandomGenerator.Next(genomeLength);

            //* 1st offspring
            // Copy genes upto crossOverIndex from parent1 to offspring[0].
            for (int iGene = 0; iGene <= crossOverIndex; iGene++)
                offspringPair[0].Genotype[iGene] = parent1.Genotype[iGene];

            // Copy remaining genes after crossOverIndex from parent2 to offspring[0].
            if (crossOverIndex < genomeLength)
                for (int iGene = crossOverIndex + 1; iGene < genomeLength; iGene++)
                    offspringPair[0].Genotype[iGene] = parent2.Genotype[iGene];

            //* 2nd offpsring.
            // Copy genes upto crossOverIndex from parent2 to offspring[1].
            for (int iGene = 0; iGene <= crossOverIndex; iGene++)
                offspringPair[1].Genotype[iGene] = parent2.Genotype[iGene];

            // Copy remaining genes after crossOverIndex from parent1  to offspring[1].
            if (crossOverIndex < genomeLength)
                for (int iGene = crossOverIndex + 1; iGene < genomeLength; iGene++)
                    offspringPair[1].Genotype[iGene] = parent1.Genotype[iGene];

            return offspringPair;
        }

        /// <summary>
        /// Performs Mutation operation on entire popluation (array elements modified in-place).
        /// </summary>
        /// <param name="population"></param>
        public static void MutatePopulation(Individual[] population)
        {
            foreach (Individual individual in population)
            {
                Mutate_Bitwise(individual);
            }
        }

        /// <summary>
        /// Mutates the genes of the supplied Individual.
        /// </summary>
        /// <param name="ind1"></param>
        public static void Mutate_Bitwise(Individual ind1)
        {
            for (int iGene = 0; iGene < ind1.Genotype.Length; iGene++)
            {
                double randomFraction = getRandomFraction();
                if (randomFraction <= Parameters.MutationRate)
                {
                    ind1.Genotype[iGene] = !ind1.Genotype[iGene];
                }
            }
        }

        public static double getRandomFraction()
        {
            return GeneticAlgorithmDriver.RandomGenerator.Next(101) / 100.0;
        }


    }

    public sealed class GeneticDataStructures
    {
        private readonly String _name;
        private readonly int _value;

        public static readonly GeneticDataStructures LOOKUP = new GeneticDataStructures(1, "LOOKUP");
        public static readonly GeneticDataStructures RULEBASED = new GeneticDataStructures(2, "RULEBASED");
        private static readonly GeneticDataStructures[] List = new GeneticDataStructures[] { LOOKUP, RULEBASED };
        private GeneticDataStructures(int value, String name)
        {
            this._name = name;
            this._value = value;
        }



        public override String ToString()
        {
            return _name;
        }

        public static GeneticDataStructures FromString(string s)
        {
            foreach (var dataStructure in List)
                if (s.ToUpper().Equals(dataStructure.ToString()))
                    return dataStructure;

            return null;
        }


    }
}
