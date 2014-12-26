using System;
using System.Configuration;
using System.IO;

namespace GeneticAlgorithmFramework
{
    public class Parameters
    {
        public static readonly int ExperimentId = Convert.ToInt32(ConfigurationManager.AppSettings["experimentId"]);
        public static readonly int NumRuns = Convert.ToInt32(ConfigurationManager.AppSettings["numRuns"]);
        public static readonly int PopulationSize;
        public static int GeneomeLength;
        public static readonly int DatasetNumVars;
        public static readonly double CrossoverRate;
        public static readonly double MutationRate;
        public static readonly int NumGenerations;
        public static readonly string DatasetUrl;
        public static readonly string ExperimentResultsOutputUrl;
        public static readonly double TrainingFraction;
        public static readonly bool isExpressFitnessAsPercentage;
        public static readonly double Eps;
        public static GeneticDataStructures DataStructure;

        /// <summary>
        /// MaxFitness is set by the DatasetManager.
        /// </summary>
        public static int MaxFitness;

        static Parameters()
        {
            PopulationSize = Convert.ToInt32(ConfigurationManager.AppSettings["populationSize"]);
            DatasetNumVars = Convert.ToInt32(ConfigurationManager.AppSettings["datasetnumvars"]);
            NumGenerations = Convert.ToInt32(ConfigurationManager.AppSettings["numGenerations"]);
            MutationRate = Convert.ToDouble(ConfigurationManager.AppSettings["mutationRate"]);
            CrossoverRate = Convert.ToDouble(ConfigurationManager.AppSettings["crossoverRate"]);
            TrainingFraction = Convert.ToDouble(ConfigurationManager.AppSettings["trainingFraction"]);
            isExpressFitnessAsPercentage = Convert.ToBoolean(ConfigurationManager.AppSettings["isExpressFitnessAsPercentage"]);
            Eps = Convert.ToDouble(ConfigurationManager.AppSettings["eps"]);
            String maxDataValue = "";
            int onBit = 1;
            char onBitChr = '1';
            maxDataValue = onBitChr.ToString().PadLeft(DatasetNumVars, onBitChr);
            GeneomeLength = Convert.ToInt32(maxDataValue, 2) + 1;
            Directory.SetCurrentDirectory(@"..\..\..\");
            DatasetUrl = Environment.CurrentDirectory + ConfigurationManager.AppSettings["dataseturl"];
            ExperimentResultsOutputUrl = Environment.CurrentDirectory + ConfigurationManager.AppSettings["experimentresultsoutputurl"];
        }
    }
}
