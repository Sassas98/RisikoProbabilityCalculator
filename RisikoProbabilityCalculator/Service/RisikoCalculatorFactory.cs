using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RisikoProbabilityCalculator.Service
{
    public static class RisikoCalculatorFactory
    {
        public static IRisikoProbabilityCalculator BuildCalculator() => new RisikoProbabilityCalculator();
        public static IRisikoProbabilityCalculator BuildCalculator(int m, int e) => new RisikoProbabilityCalculator(m, e);

        public static string GetPercentage(this decimal num) => decimal.Ceiling(num * 100) / 100 + "%";
    }
}
