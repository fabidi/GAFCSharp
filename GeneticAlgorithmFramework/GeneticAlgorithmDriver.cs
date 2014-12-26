using System;
using System.Configuration;
using System.IO;

namespace GeneticAlgorithmFramework
{
    public class GeneticAlgorithmDriver
    {
        public static Random RandomGenerator;
        public int BestFitnessEver = 0;
        int runNumber;

        public Generation[] Generations;
        private readonly string _outputFolder;

        /// <summary>
        /// When seed=0, the seed is assigned to the current time tick. This way, the results of multiple runs will always be different.
        /// Provide a non-zero seed to make the results of experiments the same across multiple runs.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="runNumber"></param>
        /// <param name="experimentStartTime"></param>
        public GeneticAlgorithmDriver(int seed, int runNumber, String outputFolder)
        {
            this.runNumber = runNumber;
            if (seed == 0)
            {
                seed = (int)DateTime.Now.Ticks;
            }
            RandomGenerator = new Random(seed);
            Generations = new Generation[Parameters.NumGenerations];
            Generation initialGeneration = new Generation();
            initialGeneration.Population = Generation.SeedPopulation();
            Generations[0] = initialGeneration;
            DatasetManager.LoadDataToMemory(Parameters.DatasetUrl);
            Parameters.DataStructure = GeneticDataStructures.FromString(ConfigurationManager.AppSettings["datastructure"]);

            // Ensure directory exists.
            _outputFolder = outputFolder;
        }

        public double TestClassification()
        {
            string[] testingSet = DatasetManager.GetTestingSet();
            Generation lastGeneration = Generations[Generations.Length - 1];
            int fitness = GeneticOps.Evaluate_BinaryClassification(lastGeneration.BestIndividual, testingSet);
            double classificationRate = fitness / (double)testingSet.Length * 100;
            return classificationRate;
        }


        /// <summary>
        /// Runs the algorithm from start to finish.
        /// </summary>
        public void Optimize()
        {
            string resultsOfRun = "Generation,Fitness Total,Mean,Best\n";
            for (int iGen = 0; iGen < Parameters.NumGenerations; iGen++)
            {
                // Evaluation
                Generation currentGeneration = Generations[iGen];
                currentGeneration.EvaluatePopulation();
                double totalFitnessPerc = 0;
                double meanFitnessPerc = 0;
                double bestFitnessEverPerc = 0;
                if (Parameters.isExpressFitnessAsPercentage)
                {
                    int maxTotalFitness = Parameters.PopulationSize * Parameters.MaxFitness;
                    totalFitnessPerc = currentGeneration.TotalFitness / (double)maxTotalFitness * 100;
                    meanFitnessPerc = currentGeneration.MeanFitness / Parameters.MaxFitness * 100;
                    bestFitnessEverPerc = currentGeneration.BestFitness / (double)Parameters.MaxFitness * 100.0;
                    Console.WriteLine("Generation {0}\n\tFitness Total: {1:0.00}%\tMean: {2:0.00}%\tBest: {3:0.00}%",
                        iGen,
                        totalFitnessPerc, meanFitnessPerc, bestFitnessEverPerc);
                    resultsOfRun += String.Format("{0},{1:0.00},{2:0.00},{3:0.00}\n",
                        iGen, totalFitnessPerc, meanFitnessPerc, bestFitnessEverPerc);
                }
                else
                {
                    Console.WriteLine("Generation {0}\n\tFitness Total: {1}\tMean: {2}\tBest: {3}", iGen,
                        currentGeneration.TotalFitness, currentGeneration.MeanFitness, currentGeneration.BestFitness);
                }
                if (currentGeneration.BestFitness > BestFitnessEver)
                    BestFitnessEver = currentGeneration.BestFitness;

                if (iGen == Parameters.NumGenerations - 1) // Don't evolve past the last generation.
                    break;

                // Evolution (Tweaking)
                Generations[iGen + 1] = Evolve(Generations[iGen]);
            }


            // Write out parameters to file.
            string parameterFilePath = _outputFolder + "\\00parameters.txt";
            string str = "";
            str += String.Format("{0}\t{1}\n", "ExperimentId", Parameters.ExperimentId);
            str += String.Format("{0}\t{1}\n", "NumRuns", Parameters.NumRuns);
            str += String.Format("{0}\t{1}\n", "PopulationSize", Parameters.PopulationSize);
            str += String.Format("{0}\t{1}\n", "GeneomeLength", Parameters.GeneomeLength);
            str += String.Format("{0}\t{1}\n", "DatasetNumVars", Parameters.DatasetNumVars);
            str += String.Format("{0}\t{1}\n", "CrossoverRate", Parameters.CrossoverRate);
            str += String.Format("{0}\t{1}\n", "MutationRate", Parameters.MutationRate);
            str += String.Format("{0}\t{1}\n", "NumGenerations", Parameters.NumGenerations);
            str += String.Format("{0}\t{1}\n", "DatasetUrl", Parameters.DatasetUrl);
            str += String.Format("{0}\t{1}\n", "TrainingFraction", Parameters.TrainingFraction);
            str += String.Format("{0}\t{1}\n", "DataStructure", Parameters.DataStructure);
            str += String.Format("{0}\t{1}\n", "isExpressFitnessAsPercentage", Parameters.isExpressFitnessAsPercentage);
            str += String.Format("{0}\t{1}\n", "Eps", Parameters.Eps);
            File.WriteAllText(parameterFilePath, str);

            // Write Out All Generations data to single file.

            string resultFilePath = _outputFolder + "\\" + "run " + runNumber + ".csv";
            using (var file = new StreamWriter(resultFilePath))
            {
                file.Write(resultsOfRun);
            }
        }

        public Generation Evolve(Generation currentGeneration)
        {
            Generation offspring = new Generation();
            // Selection
            Generation tmpGeneration = new Generation();
            tmpGeneration.Population = GeneticOps.Selection_TournamentWithReplacement(currentGeneration.Population);
            tmpGeneration.EvaluatePopulation();

            // Crossover
            offspring.Population = GeneticOps.CrossoverPopulation_SinglePoint(tmpGeneration.Population);
            // Mutation
            GeneticOps.MutatePopulation(offspring.Population);
            return offspring;
        }

        public static bool ValidateConfigParams()
        {
            bool isValid = true;
            try
            {
                int numGenerations = Convert.ToInt32(ConfigurationManager.AppSettings["numGenerations"]);
                if (numGenerations < 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                isValid = false;
                Console.WriteLine("Error: NumGenerations in App.config must be a positve integer\n");
            }

            try
            {
                double crossoverRate = Convert.ToDouble(ConfigurationManager.AppSettings["crossoverRate"]);
                if (crossoverRate > 1 || crossoverRate < 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                isValid = false;
                Console.WriteLine("Error: CrossoverRate in App.config must be an in the range [0.0 to 1.0]\n");
            }

            try
            {
                double mutationRate = Convert.ToDouble(ConfigurationManager.AppSettings["mutationRate"]);
                if (mutationRate > 1 || mutationRate < 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                isValid = false;
                Console.WriteLine("Error: MutationRate in App.config must be an in the range [0.0 to 1.0] inclusive\n");
            }

            try
            {
                int populationSize = Convert.ToInt32(ConfigurationManager.AppSettings["populationSize"]);
                if (populationSize < 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                isValid = false;
                Console.WriteLine("Error: PopulationSize in App.config must be a positve integer\n");
            }

            try
            {
                int genomeLength = Convert.ToInt32(ConfigurationManager.AppSettings["genomeLength"]);
                if (genomeLength < 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                isValid = false;
                Console.WriteLine("Error: GenomeLength in App.config must be a positve integer\n");
            }

            try
            {
                double trainingFraction = Convert.ToDouble(ConfigurationManager.AppSettings["trainingFraction"]);
                if (trainingFraction > 1 || trainingFraction < 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                isValid = false;
                Console.WriteLine("Error: TrainingFraction in App.config must be an in the range [0.0 to 1.0] inclusive\n");
            }
            return isValid;

        }
    }
}
