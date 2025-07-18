namespace RisikoProbabilityCalculator.Model
{
    public class RisikoCalculationResult
    {
        public List<RisikoCalculationPossibility> possibilities { get; set; }
        public decimal WinningRate { get; set; }
        public int MyActualTanks { get; set; }
        public int EnemyActualTanks { get; set; }

        public RisikoCalculationResult(List<RisikoCalculationPossibility> possibilities, int myTanks, int enemyTanks)
        {
            this.possibilities = possibilities.ToList();
            WinningRate = possibilities.Where(x => x.Winning).Sum(x => x.Probability);
            MyActualTanks = myTanks;
            EnemyActualTanks = enemyTanks;
        }

        public RisikoCalculationResult() { }
    }
}
