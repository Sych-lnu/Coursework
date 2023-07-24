using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Coursework
{
    internal class Program
    {

            static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int populationSize = 50,
                maxIterations = 600,
                maxIterWithoutChanges = 300,
                toNextPopulation = 12,
                countOfVars = 3;
            float a = -1,
                b = 1,
                B = a;
            double eps = 0.0001f;
            int power = GetPower(a, b, eps);
            double k = (b - B) / (Math.Pow(2, power) - 1);
            float pm = 0.1f,
            pc = 0.7f;

            List<int[]> oldPop = new List<int[]>();
            List<int[]> newPop = new List<int[]>();
            oldPop = RandomPopulation(populationSize, power, countOfVars);
            SortPopulation(oldPop, power, countOfVars, k, B);

            double oldRes = Evaluate(FromBinary(oldPop[0], power, countOfVars, k, B));
            double newRes;
            double error = 5;
            int n = 0, iterWithoutChanges = 0, iterWithSolution = -1;
            while (n < maxIterations && iterWithoutChanges < maxIterWithoutChanges)
            {
                n++;
                newPop = NewPopulation(oldPop, countOfVars, pm, pc, toNextPopulation, k, B);
                int[] best = BestV(newPop, power, countOfVars, k, B);
                newRes = Evaluate(FromBinary(best, power, countOfVars, k, B));
                error = Math.Abs(newRes - oldRes);
                Console.WriteLine("|best_prev-best_new| = " + error);
                if (error < eps)
                {
                    iterWithoutChanges++;
                }
                else
                {
                    iterWithoutChanges = 0;
                    iterWithSolution = n;
                }
                oldPop = newPop;
                oldRes = newRes;
            }
            sw.Stop();
            TimeSpan elapsed = sw.Elapsed;

            Console.WriteLine("Count of iterations = " + n);
            Console.WriteLine("|best_prev-best_new| = " + error);
            var sln = FromBinary(BestV(newPop, power, countOfVars, k, B), power, countOfVars, k, B);
            if (iterWithoutChanges < maxIterWithoutChanges)
            {

                Console.WriteLine("Max iterations reached, res x = " + sln[0] + "; " + sln[1] + "; " + sln[2]);

            }
            else
            {
                Console.WriteLine("New populations don't have better individes, res x = " + sln[0]+"; "+ sln[1]+"; "+ sln[2]);
            }
             Console.WriteLine($"|exact-approximate|={Evaluate(sln)}");
            Console.WriteLine($"Solution found on {iterWithSolution} iteration");
            Console.WriteLine("Час виконання: " + elapsed.TotalMilliseconds + " мс");
            Console.ReadLine();
        }
        public static double Evaluate(List<double> vars)
        {
            //return Math.Abs(f21(vars)) + Math.Abs(f22(vars));
            return Math.Abs(f31(vars)) + Math.Abs(f32(vars))+Math.Abs(f33(vars));
        } 


        public static List<int[]> SortPopulation(List<int[]> pop, int power, int countOfVars, double k, float B)
        {
            for (int i = 0; i < pop.Count; i++)
            {
                for (int j = i + 1; j < pop.Count; j++)
                {
                    var res1 = FromBinary(pop[i], power, countOfVars, k, B);
                    var res2 = FromBinary(pop[j], power, countOfVars, k, B);
                    var ev1 = Evaluate(res1);
                    var ev2 = Evaluate(res2);
                    if (ev1 > ev2)
                    {
                        var temp = pop[i];
                        pop[i] = pop[j];
                        pop[j] = temp;
                    }
                }
            }
            return pop;
        }


        public static List<int[]> NewPopulation(List<int[]> oldPop, int countOfVars, float pm, float pc, int toNextPopulation, double k, float B)
        {
            int power = oldPop[0].Length / countOfVars;
            List<int[]> pop = new List<int[]>();
            List<int[]> sortedpop = oldPop;
            SortPopulation(sortedpop, power, countOfVars, k, B);
            for (int i = 0; i < toNextPopulation; i++)
            {
                pop.Add(sortedpop[i]);
            }
            List<int[]> rouletteRes = Roulette(sortedpop, countOfVars, pm, pc, toNextPopulation, k, B);
            for (int i = 0; i < rouletteRes.Count; i++)
            {
                pop.Add(rouletteRes[i]);
            }
            SortPopulation(pop, power, countOfVars, k, B);
            return pop;
        }

        private static List<int[]> Roulette(List<int[]> sortedpop, int countOfVars, float pm, float pc, int toNextPopulation, double k, float B)
        {
            Random rnd = new Random();
            int power = sortedpop[0].Length/countOfVars;
            List<int[]> res = new List<int[]>();
            double worst = Evaluate(FromBinary(sortedpop[sortedpop.Count - 1], power, countOfVars, k, B));
            double sum = 0;
            List<double> r = new List<double>();
            for (int i = 0; i < sortedpop.Count; i++)
            {
                double temp = worst - (Evaluate(FromBinary(sortedpop[i], power, countOfVars, k, B))) + 1;

                r.Add(temp);
                sum += temp;
            }

            List<double> sectors = new List<double>();
            for (int i = 0; i < sortedpop.Count; i++)
            {
                r[i] /= sum;
                if (i != 0)
                {
                    sectors.Add(r[i] + sectors[i - 1]);
                }
                else
                {
                    sectors.Add(r[i]);
                }

            }

            for (int i = 0; i < sortedpop.Count - toNextPopulation; i += 2)
            {
                double sec1 = rnd.NextDouble();
                double sec2 = rnd.NextDouble();
                int index1 = 0, index2 = 0;
                for (int j = sectors.Count - 1; j >= 0; j--)
                {
                    if (sectors[j] < sec1)
                    {
                        index1 = j + 1;
                        break;
                    }
                }
                for (int j = sectors.Count - 1; j >= 0; j--)
                {
                    if (sectors[j] < sec2)
                    {
                        index2 = j + 1;
                        break;
                    }
                }
                var v1 = sortedpop[index1].Clone() as int[];
                var v2 = sortedpop[index2].Clone() as int[];
                double PM = rnd.NextDouble();
                if (PM < pm)
                {
                    Mutation(v1);
                    Mutation(v2);
                }
                double PC = rnd.NextDouble();
                if (PC < pc)
                {
                    Cross(v1, v2);
                }
                res.Add(v1);
                res.Add(v2);

            }
            return res;
        }
        public static double f21(List<double> vars)
        {
            return vars[0] + vars[1] - Math.Sin(vars[2]);
        }
        public static double f22(List<double> vars)
        {
            return Math.Abs(vars[1]) + Math.Cos(vars[2]) - 2;
        }
        public static double f31(List<double> vars)
        {
            return Math.Pow(vars[0], 2) + Math.Pow(vars[1], 2) + Math.Pow(vars[2], 2) - 1;
        }
        public static double f32(List<double> vars)
        {
            return vars[0] + Math.Sin(vars[1]) - Math.Pow(vars[2], 2) + 0.4;
        }
        public static double f33(List<double> vars)
        {
            return vars[0] + 2 * vars[1] - Math.Exp(vars[2]) + 3;
        }

        public static int GetPower(float a, float b, double eps)
        {
            float lenght = b - a;
            int power = Convert.ToInt32(Math.Log(lenght / eps, 2) + 0.5);
            return power;
        }
        public static List<double> FromBinary(int[] bin, int power, int countOfvars, double k, float B)
        {
            List<double> list = new List<double>();
            for (int i = 0; i < countOfvars; i++)
            {
                double temp = 0;
                for (int j = power - 1; j >= 0; j--)
                {
                    temp += bin[i * power + j] * Math.Pow(2, j);
                }
                list.Add(temp * k + B);
            }
            return list;
        }

        public static List<int[]> RandomPopulation(int populationSize, int power, int countOfVars)
        {
            Random random = new Random();
            List<int[]> list = new List<int[]>();
            for (int i = 0; i < populationSize; i++)
            {
                int[] temp = new int[power*countOfVars];
                for (int j = 0; j < power*countOfVars; j++)
                {
                    temp[j] = random.Next(2);
                }
                list.Add(temp);
            }
            return list;
        }
        public static void Mutation(int[] v)
        {
            Random rnd = new Random();
            int power = v.Length;
            int index = rnd.Next(power);
            if (v[index] == 1)
            {
                v[index] = 0;
            }
            else
            {
                v[index] = 1;
            }
        }
        public static List<int[]> Cross(int[] v1, int[] v2)
        {
            List<int[]> list = new List<int[]>();
            int power = v1.Length;
            for (int i = (int)(power / 2); i < power; i++)
            {
                int temp = v1[i];
                v1[i] = v2[i];
                v2[i] = temp;
            }
            list.Add(v1);
            list.Add(v2);
            return list;
        }
        public static int[] BestV(List<int[]> population, int power, int countOfVars, double k, float B)
        {
            var best = SortPopulation(population, power, countOfVars, k, B)[0];
            return best;
        }


    }
}
