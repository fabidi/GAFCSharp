using System;
using System.Collections.Generic;

namespace GeneticAlgorithmFramework
{
    /// <summary>
    /// Rule-based Phenotype structure. Stores known ranges for each classifications, allowing generalization.
    /// </summary>
    public class Phenotype
    {
        /// <summary>
        /// We use bool data type to reduce memory footprint. Previously we had used int.
        /// </summary>
        private Dictionary<bool, List<double[]>> _structure;

        public Dictionary<bool, List<double[]>> Structure
        {
            get { return _structure; }
            set { _structure = value; }
        }

        public Phenotype()
        {
            _structure = new Dictionary<bool, List<double[]>>();
            _structure.Add(false, new List<double[]>());
            _structure.Add(true, new List<double[]>());
        }


        /// <summary>
        /// Returns 0 or 1 as the classification, else -1 if the input cannot be classified.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public sbyte Classify(int input)
        {
            foreach (KeyValuePair<bool, List<double[]>> classRulesPair in _structure)
            {
                List<double[]> rules = classRulesPair.Value;
                foreach (double[] rule in rules)
                {
                    if (input >= rule[0] && input < rule[1])
                    {
                        bool classification = classRulesPair.Key;
                        // Classfified.
                        return Convert.ToSByte(classification);
                    }
                }
            }
            // Failed to classify. Should probably never reach here.
            return -1;
        }
    }
}
