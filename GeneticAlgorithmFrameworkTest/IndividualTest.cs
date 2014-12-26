using System.Collections.Generic;
using System.Linq;
using GeneticAlgorithmFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneticAlgorithmFrameworkTest
{
    [TestClass]
    public class IndividualTest
    {
        [TestMethod]
        public void TestDecode()
        {
            bool[] genotypeA = new bool[] {true, true,false, false, true, false, true, false};
            Parameters.GeneomeLength = genotypeA.Length;
            var ind = new Individual(genotypeA);
            Phenotype phenotype = ind.Decode();
            List<double[]> actualRules0 = phenotype.Structure[false];
            double[] expectedRule1 = new double[]{2, 4};
            double[] expectedRule2 = new double[] { 5, 6 };
            double[] expectedRule3 = new double[] { 7, 7 };
            List<double[]> expectedRuleList0 = new List<double[]>(){expectedRule1, expectedRule2, expectedRule3};
            Decode_TestHelper(expectedRuleList0, actualRules0);

            List<double[]> actualRules1 = phenotype.Structure[true];
            expectedRule1 = new double[] { 0, 2 };
            expectedRule2 = new double[] { 4, 5 };
            expectedRule3 = new double[] { 6, 7 };
            List<double[]> expectedRuleList1 = new List<double[]>() { expectedRule1, expectedRule2, expectedRule3 };
            Decode_TestHelper(expectedRuleList1, actualRules1);
        }

        private static void Decode_TestHelper(List<double[]> expectedRuleList0, List<double[]> rules0)
        {
            for (int i = 0; i < expectedRuleList0.Count; i++)
            {
                double[] expRule = expectedRuleList0[i];
                double[] actualRule = rules0.ElementAt(i);
                for (int j = 0; j < expRule.Length; j++)
                {
                    double expectedVal = expRule[j];
                    double actualVal = actualRule[j];
                    Assert.IsTrue(expectedVal.Equals(actualVal));
                }
            }
        }
    }
}
