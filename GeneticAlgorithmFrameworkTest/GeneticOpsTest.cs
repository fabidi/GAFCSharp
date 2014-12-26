using GeneticAlgorithmFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneticAlgorithmFrameworkTest
{
    [TestClass]
    public class GeneticOpsTest
    {
        [TestMethod]
        public void TestCrossoverPair_SinglePoint()
        {
            bool[] genotypeA = new bool[] {false, false, false, false, true, false, true, false};
            bool[] genotypeB = new bool[] {true, true, true, true, false, true, false, true};
            Parameters.GeneomeLength = genotypeA.Length;
            var parent1 = new Individual(genotypeA);
            var parent2 = new Individual(genotypeB);
            Individual[] parents = {parent1, parent2};
            Individual[] offspring;
            int crossOverIndex = 3;
            offspring = GeneticOps.CrossoverPair_SinglePoint(parents, crossOverIndex);
            Assert.IsTrue("FalseFalseFalseFalseFalseTrueFalseTrue".Equals(offspring[0].ToString()));
            Assert.IsTrue("TrueTrueTrueTrueTrueFalseTrueFalse".Equals(offspring[1].ToString()));
        }


        [TestMethod]
        public void TestDecode()
        {


            
        }
        
    }
}
