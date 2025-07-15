using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RisikoProbabilityCalculator
{
    public static class RisikoCalculatorFactory
    {
        public static IRisikoProbabilityCalculator BuildCalculator() => new RisikoProbabilityCalculator();
    }
}
