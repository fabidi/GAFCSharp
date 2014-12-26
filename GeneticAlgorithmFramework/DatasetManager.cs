using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeneticAlgorithmFramework
{
    /// <summary>
    /// Singleton class that exposes a few static methods to:
    /// a) load the data;
    /// b) partition data into training and testing sets; and
    /// c) retrieve training and testing sets.
    /// </summary>
    class DatasetManager
    {
        private static DatasetManager _instance;
        private string[] _dataset;
        private string[] _trainingSet;
        private string[] _testingSet;

        private DatasetManager(){}

        private static DatasetManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DatasetManager();
                }
                return _instance;
            }
        }


        public static string[] GetData()
        {
            return Instance._dataset;
        }

        public static string[] GetTrainingSet()
        {
            return Instance._trainingSet;
        }

        public static string[] GetTestingSet()
        {
            return Instance._testingSet;
        }

        public static void LoadDataToMemory(string url)
        {
            Instance._dataset = File.ReadAllLines(Parameters.DatasetUrl);
            SplitDataset();
            
        }

        /// <summary>
        /// Splits the dataset into training and testing sets according to the ratio
        /// specified in Parameter.TrainingFraction
        /// </summary>
        private static void SplitDataset()
        {
            string[] dataset = Instance._dataset;
            double trainingFraction = Parameters.TrainingFraction;
            int trainingSetSize = (int) Math.Round(dataset.Length*trainingFraction);
            var datasetList = new List<string>(dataset);
            var trainingSetList = new List<string>(trainingSetSize);
            for (int i = 0; i < trainingSetSize; i++)
            {
                int randomIndex = GeneticAlgorithmDriver.RandomGenerator.Next(datasetList.Count);
                trainingSetList.Add(datasetList.ElementAt(randomIndex));
                datasetList.RemoveAt(randomIndex);
            }

            Instance._testingSet = datasetList.ToArray();
            Instance._trainingSet = trainingSetList.ToArray();
            Parameters.MaxFitness = trainingSetList.Count;
        }
    }
}
