using RisikoProbabilityCalculator.Model;

namespace RisikoProbabilityCalculator.Service
{
    public interface IRisikoProbabilityCalculator
    {
        /// <summary>
        /// Method for the authomatic calculation of this fight.
        /// The calculation presumes you continue until the end.
        /// </summary>
        /// <param name="myT"></param>
        /// <param name="enemyT"></param>
        /// <returns>the first result of the calculation</returns>
        public RisikoCalculationResult GetResults(int myT, int enemyT);

        /// <summary>
        /// Method that reset the calculation
        /// </summary>
        public void ClearCalculation();

        /// <summary>
        /// Method for update the path of probability
        /// </summary>
        /// <param name="dices"></param>
        /// <returns>the result of a successive fight</returns>
        public RisikoCalculationResult GetResults(int[][] dices);
    }
}
