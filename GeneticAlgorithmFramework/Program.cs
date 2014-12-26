using System;
using System.Configuration;
using System.IO;

namespace GeneticAlgorithmFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            int numRuns = Parameters.NumRuns;
            int experimentId = Parameters.ExperimentId;
            int seed = Convert.ToInt32(ConfigurationManager.AppSettings["seed"]);
            bool isConfigValid = GeneticAlgorithmDriver.ValidateConfigParams();
            int totalBestFitness = 0;
            double totalClassificationRate = 0;
            double averageBestFitness = 0;
            double averageClassificationRate = 0;
            string experimentStartTime = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            // Ensure directory exists.
            if (!Directory.Exists(Parameters.ExperimentResultsOutputUrl))
                Directory.CreateDirectory(Parameters.ExperimentResultsOutputUrl);

            string experimentsMainOutputFolder = Parameters.ExperimentResultsOutputUrl;
            string experimentCurrentOutputFolder = experimentsMainOutputFolder + "\\Experiment " + experimentId;
            string outputFolder = experimentCurrentOutputFolder + "\\" + experimentStartTime;

            string averageOfRunsStr = "";

            // Ensure directories exist.
            if (!Directory.Exists(experimentsMainOutputFolder))
                Directory.CreateDirectory(experimentsMainOutputFolder);
            if (!Directory.Exists(experimentCurrentOutputFolder))
                Directory.CreateDirectory(experimentCurrentOutputFolder);
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            // Check if file exists, if not then create it and append header info.
            string experimentResultFilePath = experimentCurrentOutputFolder + "\\" + "experiment " + experimentId + ".csv";
            if (!File.Exists(experimentResultFilePath))
            {
                averageOfRunsStr = "NumRuns,AverageBestFitness(%%),AverageTestingSetClassificationRate(%%),NumGenerations,PopulationSize,GeneomeLength,"+
                        "CrossoverRate,MutationRate,TrainingFraction\n";
            }
            GeneticAlgorithmDriver geneticAlgorithmDriver = null;
            if (isConfigValid)
            {
                for (int iRun = 0; iRun < numRuns; iRun++)
                {
                    geneticAlgorithmDriver = new GeneticAlgorithmDriver(seed + iRun, iRun + 1, outputFolder);
                    geneticAlgorithmDriver.Optimize();
                    totalBestFitness += geneticAlgorithmDriver.BestFitnessEver;
                    totalClassificationRate += geneticAlgorithmDriver.TestClassification(); ;
                }
                averageBestFitness = totalBestFitness / (double)numRuns;
                averageClassificationRate = totalClassificationRate / (double)numRuns;
                if (Parameters.isExpressFitnessAsPercentage)
                {
                    double averageBestFitnessPerc = averageBestFitness / (double)Parameters.MaxFitness * 100;
                    Console.WriteLine("\nAverage Best Fitness over {0:0.00} runs: {1}\n", numRuns, averageBestFitnessPerc);
                    Console.WriteLine("Average Testing Set Classification Rate: {0:0.00}", averageClassificationRate);

                    // Collect the averaging and parameter information for all the runs in this batch for writing to file.
                    averageOfRunsStr += String.Format("{0},{1:0.00},{2:0.00},{3},{4},{5},{6:0.00},{7:0.00},{8:0.00}\n",
                        numRuns, averageBestFitnessPerc,
                        averageClassificationRate, Parameters.NumGenerations, Parameters.PopulationSize, Parameters.GeneomeLength,
                        Parameters.CrossoverRate, Parameters.MutationRate, Parameters.TrainingFraction);
                }
                else
                    Console.WriteLine("\nAverage Best Fitness over {0} runs: {1}\n", numRuns, averageBestFitness);

                // Append Write average of all runs to file under the current ExperimentId to build our collection of runs with different parameters.
                using (var file = File.AppendText(experimentResultFilePath))
                {
                    file.Write(averageOfRunsStr);
                }
                Console.WriteLine("Press Any Key To Terminate...\n");
                Console.ReadKey(true);
            }

        }
    }
}
