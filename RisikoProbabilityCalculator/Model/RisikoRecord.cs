namespace RisikoProbabilityCalculator.Model
{
    internal class RisikoRecord
    {
        public decimal Probability { get; set; }
        public int MyInnerTanks { get; set; }
        public int EnemyInnerTanks { get; set; }
        public int MyActualTanks { get; set; }
        public int EnemyActualTanks { get; set; }
    }
}
