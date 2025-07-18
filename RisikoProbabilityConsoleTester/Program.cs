using RisikoProbabilityCalculator.Service;
using System.Text.Json;

namespace RisikoProbabilityConsoleTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var calculator = RisikoCalculatorFactory.BuildCalculator();
            int m = 50, e = 35;
            var result = calculator.GetResults(m, e);
            Console.WriteLine($"Stato attuale: {result.MyActualTanks}/{result.EnemyActualTanks}");
            Console.WriteLine($"Probabilità di vincere: {result.WinningRate.GetPercentage()}");
            while(Math.Min(e, m) > 0)
            {
                result = calculator.GetResults(GetRandomDices(Math.Min(e, m)));
                Console.WriteLine($"Stato attuale: {result.MyActualTanks}/{result.EnemyActualTanks}");
                Console.WriteLine($"Probabilità di vincere: {result.WinningRate.GetPercentage()}");
                e = result.EnemyActualTanks;
                m = result.MyActualTanks;
            }
        }

        private static int[][] GetRandomDices(int min)
        {
            return Enumerable.Range(0, 2).Select(x => Enumerable.Range(0, Math.Min(min, 3)).Select(x => GetRandomDice()).ToArray()).ToArray();
        }

        private static int GetRandomDice()
        {
            var random = new Random();
            return random.Next(1, 7);
        }
    }
}
