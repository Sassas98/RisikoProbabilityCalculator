namespace RisikoProbabilityCalculator
{
    public class RisikoCalculationResult
    {
        public decimal Probability { get; set; }
        public bool Winning { get; set; }
        public int MyTanks { get; set; }
        public int EnemyTanks { get; set; }
    }
}
