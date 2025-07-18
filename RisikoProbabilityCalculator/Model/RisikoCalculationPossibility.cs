namespace RisikoProbabilityCalculator.Model
{
    public class RisikoCalculationPossibility
    {
        public decimal Probability { get; set; }
        public bool Winning { get; set; }
        public int MyTanks { get; set; }
        public int EnemyTanks { get; set; }

        public override string ToString()
        {
            return $"{MyTanks}/{EnemyTanks}";
        }
    }
}
