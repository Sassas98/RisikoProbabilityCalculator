using RisikoProbabilityCalculator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RisikoProbabilityCalculator.Service
{
    internal class RisikoProbabilityCalculator : IRisikoProbabilityCalculator
    {
        private int[][][] diceResults;
        private List<(decimal count, int winA, int winD)>[,] results;
        private (int m, int e)? state = null;
        private readonly Dictionary<(int myT, int enemyT), List<RisikoCalculationPossibility>> endFightCache = new();

        public RisikoProbabilityCalculator()
        {
            diceResults = CalculateDiceResults();
            results = CalculateResults();
        }

        public RisikoProbabilityCalculator(int m, int e) : base()
        {
            state = (m, e);
        }

        #region preparation

        private List<(decimal count, int winA, int winD)>[,] CalculateResults()
        {
            var list = new List<(decimal count, int winA, int winD)>[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    list[i, j] = SimulateFight(i + 1, j + 1);
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
            state = null;
        }

        public RisikoCalculationResult GetResults(int myT, int enemyT)
        {
            if (state.HasValue && state.Value.m + state.Value.e != myT + enemyT + Math.Min(3, Math.Min(state.Value.m, state.Value.e)))
                throw new Exception("Fight not consecutive with the last result.");
            if (myT < 1 || enemyT < 1)
                return BuildFinalRisikoCalculationResult(myT, enemyT);

            state = (myT, enemyT);
            var possibilities = GetOrCalculateEndFightPossibilities(myT, enemyT);
            return new RisikoCalculationResult(possibilities, myT, enemyT);
        }

        public RisikoCalculationResult GetResults(int[][] dices)
        {
            if (state == null) throw new Exception("Fight not already started.");
            var result = SimulateFight(dices[0], dices[1]);
            return GetResults(state.Value.m - result.Item2, state.Value.e - result.Item1);
        }

        #region fight calculation

        private List<RisikoRecord> SimulateFight(int attack, int defend, decimal startProbability)
        {
            int attkDices = Math.Min(attack, 3);
            int defDices = Math.Min(defend, 3);
            List<(decimal count, int winA, int winD)> results = this.results[attkDices - 1, defDices - 1];
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
            for (int i = 0; i < attk.Length; i++)
            {
                if (attk[i] > def[i]) countA++;
                else countD++;
            }
            return (countA, countD);
        }

        private List<(int[], int[])> DaiCombinazioni(int[][] dicesA, int[][] dicesD)
        {
            List<(int[], int[])> r = new List<(int[], int[])>();
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

        #region model building

        private RisikoCalculationResult BuildFinalRisikoCalculationResult(int myT, int enemyT)
        {
            return new RisikoCalculationResult(new List<RisikoCalculationPossibility>
            {
                new RisikoCalculationPossibility
                {
                    MyTanks = myT,
                    EnemyTanks = enemyT,
                    Probability = 100,
                    Winning = myT > 0
                }
            }, myT, enemyT);
        }

        private List<RisikoCalculationPossibility> GetOrCalculateEndFightPossibilities(int myT, int enemyT)
        {
            if (endFightCache.TryGetValue((myT, enemyT), out var cached))
            {
                return cached.Select(x => new RisikoCalculationPossibility
                {
                    Winning = x.Winning,
                    MyTanks = x.MyTanks,
                    EnemyTanks = x.EnemyTanks,
                    Probability = x.Probability
                }).ToList();
            }

            List<RisikoCalculationPossibility> calculated;

            if (myT < 1 || enemyT < 1)
            {
                calculated = new List<RisikoCalculationPossibility>
                {
                    new RisikoCalculationPossibility
                    {
                        MyTanks = myT,
                        EnemyTanks = enemyT,
                        Probability = 100,
                        Winning = myT > 0
                    }
                };
            }
            else
            {
                var grouped = new Dictionary<(int myT, int enemyT), decimal>();
                foreach (var step in SimulateFight(myT, enemyT, 100))
                {
                    var children = GetOrCalculateEndFightPossibilities(step.MyActualTanks, step.EnemyActualTanks);
                    foreach (var child in children)
                    {
                        var key = (child.MyTanks, child.EnemyTanks);
                        var probability = step.Probability * child.Probability / 100m;

                        if (grouped.ContainsKey(key))
                            grouped[key] += probability;
                        else
                            grouped[key] = probability;
                    }
                }

                calculated = grouped.Select(x => new RisikoCalculationPossibility
                {
                    Winning = x.Key.enemyT == 0,
                    MyTanks = x.Key.myT,
                    EnemyTanks = x.Key.enemyT,
                    Probability = x.Value
                }).ToList();
            }

            endFightCache[(myT, enemyT)] = calculated.Select(x => new RisikoCalculationPossibility
            {
                Winning = x.Winning,
                MyTanks = x.MyTanks,
                EnemyTanks = x.EnemyTanks,
                Probability = x.Probability
            }).ToList();

            return calculated;
        }

        #endregion
    }
}