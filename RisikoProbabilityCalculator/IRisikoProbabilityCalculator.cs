namespace RisikoProbabilityCalculator
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
        public List<RisikoCalculationResult> GetResults(int myT, int enemyT);

        /// <summary>
        /// Method that reset the calculation
        /// </summary>
        public void ClearCalculation();

        /// <summary>
        /// Method for update the path of probability
        /// </summary>
        /// <param name="dices"></param>
        /// <returns>the result of a successive fight</returns>
        public List<RisikoCalculationResult> GetResults(int[][] dices);

        /// <summary>
        /// Method to get all the path of the current fight
        /// </summary>
        /// <returns>the path of the fight</returns>
        public List<RisikoRecord> GetRecords();
    }
}
