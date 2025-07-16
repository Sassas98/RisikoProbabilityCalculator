using RisikoProbabilityCalculator;
using System.Text.Json;

namespace RisikoProbabilityConsoleTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var calculator = RisikoCalculatorFactory.BuildCalculator();
            var result = calculator.GetResults(5, 3);
            foreach ( var item in result.OrderByDescending(x => x.MyTanks).ThenBy(x => x.EnemyTanks) )
            {
                Console.WriteLine(JsonSerializer.Serialize(item));
            }
            Console.WriteLine($"Probabilità di vincere: {result.Where(x => x.Winning).Sum(x => x.Probability)}");
        }
    }
}
