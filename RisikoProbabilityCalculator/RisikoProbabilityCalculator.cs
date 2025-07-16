using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RisikoProbabilityCalculator
{
    internal class RisikoProbabilityCalculator : IRisikoProbabilityCalculator
    {
        private int[][][] diceResults;
        private List<(decimal count, int winA, int winD)>[,] results;

        public RisikoProbabilityCalculator()
        {
            diceResults = CalculateDiceResults();
            results = CalculateResults();
        }

        #region preparation

        private List<(decimal count, int winA, int winD)>[,] CalculateResults()
        {
            var list = new List<(decimal count, int winA, int winD)>[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    list[i, j] = SimulateFight(i+1, j+1);
                }
            }
            return list;
        }

        private int[][][] CalculateDiceResults()
        {
            return Enumerable.Range(1, 3).Select(n =>
            {
                if (n == 1) return [[1], [2], [3], [4], [5], [6]];
                else if (n == 2)
                {
                    var list = new List<int[]>();
                    for (int i = 1; i < 7; i++)
                    {
                        for (int j = 1; j < 7; j++)
                        {
                            list.Add([i, j]);
                        }
                    }
                    return list.OrderBy(x => x[0]).ThenBy(x => x[1]).ToArray();
                }
                else
                {
                    var list = new List<int[]>();
                    for (int i = 1; i < 7; i++)
                    {
                        for (int j = 1; j < 7; j++)
                        {
                            for (int k = 1; k < 7; k++)
                            {
                                list.Add([i, j, k]);
                            }
                        }
                    }
                    return list.OrderBy(x => x[0]).ThenBy(x => x[1]).ThenBy(x => x[2]).ToArray();
                }
            }).ToArray();
        }

        #endregion

        public void ClearCalculation()
        {
            throw new NotImplementedException();
        }

        public List<RisikoRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        public List<RisikoCalculationResult> GetResults(int myT, int enemyT)
        {
            var result = SimulateFight(myT, enemyT, 100);
            var list = result.Where(x => x.EnemyActualTanks == 0 || x.MyActualTanks == 0).ToList();
            result.RemoveAll(list.Contains);
            while(result.Count > 0)
            {
                result = result.SelectMany(x => SimulateFight(x.MyActualTanks, x.EnemyActualTanks, x.Probability)).ToList();
                list.AddRange(result.Where(x => x.EnemyActualTanks == 0 || x.MyActualTanks == 0));
                result = result.Where(x => x.EnemyActualTanks > 0 && x.MyActualTanks > 0).ToList();
            }
            return list.GroupBy(x => new { m = x.MyActualTanks, e = x.EnemyActualTanks })
                       .Select(x => new RisikoCalculationResult
            {
                Winning = x.Key.e == 0,
                MyTanks = x.Key.m,
                EnemyTanks = x.Key.e,
                Probability = x.Sum(x => x.Probability)

            }).ToList();
        }

        public List<RisikoCalculationResult> GetResults(int[][] dices)
        {
            throw new NotImplementedException();
        }

        #region fight calculation

        private List<RisikoRecord> SimulateFight(int attack, int defend, decimal startProbability)
        {
            int attkDices = Math.Min(attack, 3);
            int defDices = Math.Min(defend, 3);
            List<(decimal count, int winA, int winD)> results = this.results[attkDices-1, defDices-1];
            decimal tot = results.Select(x => x.count).Sum();
            return results.Select(e => new RisikoRecord
            {
                MyInnerTanks = attack,
                EnemyInnerTanks = defend,
                MyActualTanks = attack - e.winD,
                EnemyActualTanks = defend - e.winA,
                Probability = startProbability * (e.count / tot)
            }).ToList();
        }
        private List<(decimal count, int winA, int winD)> SimulateFight(int attack, int defend)
        {
            int attkDices = Math.Min(attack, 3);
            int defDices = Math.Min(defend, 3);
            bool? attkPlus = attkDices > defDices ? true : attkDices < defDices ? false : null;
            var dicesA = attkPlus == true ? ReduceDices(diceResults[attkDices - 1], defDices) :
               attkPlus == false ? AddDices(diceResults[attkDices - 1], defDices - attkDices) : diceResults[attkDices - 1];
            var dicesD = attkPlus == false ? ReduceDices(diceResults[defDices - 1], attkDices) :
                attkPlus == true ? AddDices(diceResults[defDices - 1], attkDices - defDices) : diceResults[defDices - 1];
            List<(int[] attk, int[] def)> combinazioni = DaiCombinazioni(dicesA, dicesD);
            return combinazioni.Select(c => SimulateFight(c.attk, c.def))
                               .GroupBy(x => x).Select(x => ((decimal)x.Count(), x.First().Item1, x.First().Item2)).ToList();
        }

        private (int, int) SimulateFight(int[] attk, int[] def)
        {
            attk = attk.OrderByDescending(x => x).ToArray();
            def = def.OrderByDescending(x => x).ToArray();
            int countA = 0;
            int countD = 0;
            for(int i = 0; i <  attk.Length; i++)
            {
                if(attk[i] > def[i]) countA++;
                else countD++;
            }
            return (countA, countD);
        }

        private List<(int[], int[])> DaiCombinazioni(int[][] dicesA, int[][] dicesD)
        {
            List < (int[], int[]) > r = new List<(int[], int[])> ();
            foreach (int[] d1 in dicesA)
                foreach (int[] d2 in dicesD)
                    r.Add((d1, d2));
            return r;
        }

        private int[][] ReduceDices(int[][] dices, int newDim)
        {
            return dices.Select(a =>
            {
                if (newDim == 1) return [a.Max()];
                else
                {
                    return a.Order().Skip(1).ToArray();
                }
            }).ToArray();
        }

        private int[][] AddDices(int[][] ints, int v)
        {
            var list = new List<int[]>();
            for (int i = 0; i < v; i++)
            {
                for (int j = 0; j < 6; j++)
                    list.AddRange(ints);
                ints = list.ToArray();
            }
            return ints;
        }

        #endregion
    }
}
