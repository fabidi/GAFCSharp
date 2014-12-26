using System.Collections.Generic;

namespace GeneticAlgorithmFramework
{
    public class Individual : IIndividual
    {
        /// <summary>
        /// Chromosome data structure (i.e. Fixed length vector as a genotype).
        /// </summary>
        public bool[] Genotype { get; set; }
        public Phenotype Phenotype { get; set; }

        public int Fitness { get; set; }

        public Individual()
        {
            Genotype = new bool[Parameters.GeneomeLength];
        }

        public Individual(bool[] genotype)
        {
            Genotype = genotype;
        }

        /// <summary>
        /// Decodes the genotype to the corresponding phenotype, and updating this Individual's Phenotype property.
        /// </summary>
        /// <returns></returns>
        public Phenotype Decode()
        {
            var phenotype = new Phenotype();
            double[] rangePair;
            int counter = 0;
            // Iterate through the genotype chromosome, and mark the boundaries where the class changes, creating a list of ranges to out in the phenotype.
            while (counter < Genotype.Length)
            {
                rangePair = new double[2];
                for (int i = 0; i < rangePair.Length; i++)
                {
                    rangePair[i] = -1;
                }
                bool latestClassification = Genotype[counter];
                List<double[]> rules = phenotype.Structure[latestClassification];
                // The counter is also the numeric value decoding of the allele at genotype[counter]. This is by design.
                int valueRepresented = counter;
                rangePair[0] = valueRepresented;
                counter++;
                if (counter == Genotype.Length)
                {
                    rangePair[1] = rangePair[0];
                }
                else
                {
                    while (counter < Genotype.Length)
                    {
                        valueRepresented = counter;
                        rangePair[1] = valueRepresented;
                        if (Genotype[counter] == latestClassification)
                            counter++;
                        else
                            break;
                    }
                }
                // Add the range to the respective classification in the phenotype.
                rules.Add(rangePair);
            }

            Phenotype = phenotype;
            return phenotype;
        }

        /// <summary>
        /// Presently, we don't need to encode. Added this just in case the need pops up later.
        /// </summary>
        /// <param name="phenotype"></param>
        /// <returns></returns>
        public int[] Encode(Phenotype phenotype)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            string str = "";
            foreach (bool gene in Genotype)
            {
                str += gene.ToString();
            }
            return str;
        }
    }
}
