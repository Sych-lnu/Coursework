using System;
using System.Collections.Generic;

namespace Coursework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int populationSize = 50,
                maxIterations = 1000,
                maxIterWithoutChanges = 500,
                toNextPopulation = 16,
                countOfvars = 2;
            float a = -1,
                b = 2,
                B = a;
            double eps = 0.000001f;
            int power = GetPower(a, b, eps);
            double k = (b - B) / (Math.Pow(2, power) - 1);
            float pm = 1f,
                pc = 0.9f;
            List<int[]> oldPop = new List<int[]>();
            List<int[]> newPop = new List<int[]>();
            oldPop = RandomPopulation(populationSize, power);
            double oldRes = func(FromBinaty(oldPop[0], power, k, B));
            double newRes;
            double error = 5;
            int n = 0, iterWithoutChanges = 0;
            
            while(n<maxIterations && iterWithoutChanges < maxIterWithoutChanges)
            {
                n++;
                newPop = NewPopulation(oldPop, pm, pc, toNextPopulation, k, B);
                int[] best = BestV(newPop, power, k, B);
                newRes = func(FromBinaty(best, power, k, B));
                error = Math.Abs(newRes - oldRes);
                Console.WriteLine("Error = " + error);
                if (error<eps)
                {
                    iterWithoutChanges++;
                }
                else
                {
                    iterWithoutChanges = 0;
                }
                oldPop = newPop;
                oldRes = newRes;
            }
        
            
            Console.WriteLine("Count of iterations = " + n);
            Console.WriteLine("Error = " + error);
            if(iterWithoutChanges < maxIterWithoutChanges)
            {

                Console.WriteLine("Max iterations reached, res x = " + FromBinaty(BestV(newPop, power, k, B), power, k, B));

            }
            else
            {
                Console.WriteLine("New populations don't have better individes, res x = " + FromBinaty(BestV(newPop, power, k, B), power, k, B));
            }

            Console.ReadLine();
        }
        public static List<int[]> SortPopulation(List<int[]> pop,int power, double k, float B)
        {
            for(int i =  0; i < pop.Count; i++)
            {
                for(int j = i +1; j < pop.Count; j++)
                {
                    var res1 = FromBinaty(pop[i], power, k, B);
                    var res2 = FromBinaty(pop[j], power, k, B);
                    if (func(res1) > func(res2))
                    {
                        var temp = pop[i];
                        pop[i] = pop[j];
                        pop[j] = temp;
                    }
                }
            }
            return pop;
        }
        public static List<int[]> SortPopulation(List<int[]> pop, int power, int countOfvars, double k, float B)
        {
            for (int i = 0; i < pop.Count; i++)
            {
                for (int j = i + 1; j < pop.Count; j++)
                {
                    var res1 = FromBinaty(pop[i], power, countOfvars, k, B);
                    var res2 = FromBinaty(pop[j], power, countOfvars, k, B);
                    if (func2(res1) > func2(res2))
                    {
                        var temp = pop[i];
                        pop[i] = pop[j];
                        pop[j] = temp;
                    }
                }
            }
            return pop;
        }

        private static int func2(List<double> res1)
        {
            throw new NotImplementedException();
        }

        public static List<int[]> NewPopulation(List<int[]> oldPop, float pm, float pc, int toNextPopulation, double k, float B)
        {
            int populationSize = oldPop.Count;
            int power = oldPop[0].Length;
            List<int[]> pop = new List<int[]>();
            List<int[]> sortedpop = oldPop;
            SortPopulation(sortedpop, power, k, B);
            for (int i = 0; i < toNextPopulation; i++)
            {
                pop.Add(sortedpop[i]);
            }
            List<int[]> rouletteRes = Roulette(sortedpop, pm, pc, toNextPopulation, k, B);
            for(int i = 0; i < rouletteRes.Count; i++)
            {
                pop.Add(rouletteRes[i]);
            }
            return pop;
        }

        private static List<int[]> Roulette(List<int[]> sortedpop, float pm, float pc, int toNextPopulation, double k, float B)
        {
            Random rnd = new Random();
            int power = sortedpop[0].Length;
            List<int[]> res = new List<int[]>();
            double worst = func(FromBinaty(sortedpop[sortedpop.Count - 1], power, k, B));
            double sum = 0;
            List<double> r = new List<double>();
            for(int i = 0; i < sortedpop.Count; i++)
            {
                double temp = worst - func(FromBinaty(sortedpop[i], power, k, B)) + 1;
                
                r.Add(temp);
                sum += temp;
            }

            List<double> sectors = new List<double>();
            for (int i=0;i<sortedpop.Count;i++)
            {
                r[i] /= sum;
                if (i != 0)
                {
                    sectors.Add(r[i] + sectors[i-1]);
                }
                else
                {
                    sectors.Add(r[i]);
                }

            }
            /*
            for(int i = 0; i < sortedpop.Count ; i++)
            {
                Console.WriteLine(func(FromBinaty(sortedpop[i], power,k,B)) + " - " + sectors[i]);
            }
            */

            for(int i = 0; i <sortedpop.Count-toNextPopulation; i += 2)
            {
                double sec1 = rnd.NextDouble();
                double sec2 = rnd.NextDouble();
                int index1 = 0, index2 = 0;
                for(int j = sectors.Count - 1; j>=0; j--)
                {
                    if (sectors[j] < sec1)
                    {
                        index1 = j + 1;
                        break;
                    }
                }
                for(int j = sectors.Count - 1; j>=0; j--)
                {
                    if (sectors[j] < sec2)
                    {
                        index2 = j + 1;
                        break;
                    }
                }
                var v1 = sortedpop[index1];
                var v2 = sortedpop[index2];
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

        public static double func(double x)
        {
            return (x * Math.Sin(10 * x * Math.PI) + 1);
        }
        public static int GetPower(float a, float b, double eps)
        {
            float lenght = b - a;
            int power = Convert.ToInt32(Math.Log(lenght / eps, 2) + 0.5);
            return power;
        }
        public static double FromBinaty(int[] bin, int power, double k, float B)
        {
            double temp = 0;
            for(int i = power-1; i>=0; i--)
            {
                temp += bin[i]*Math.Pow(2, i);
            }
            return (temp * k + B);
        }
        public static List<double> FromBinaty(int[] bin, int power, int countOfvars, double k, float B)
        {
            List<double> list = new List<double>();
            for(int i = 0; i<countOfvars; i++)
            {
                double temp = 0;
                for (int j = power-1; j>=0; j--)
                {
                    temp += bin[i*power+j] * Math.Pow(2, i);
                }
                list.Add(temp);
            }
            return list;
        }

        public static List<int[]> RandomPopulation(int populationSize, int power)
        {
            Random random = new Random();
            List<int[]> list = new List<int[]>();
            for (int i = 0; i < populationSize; i++)
            {
                int[] temp = new int[power];
                for (int j = 0; j < power; j++)
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
        public static List<int[]> Cross(int[]v1, int[]v2)
        {
            List<int[]> list = new List<int[]>();
            int power = v1.Length;
            for(int i = (int)(power/2); i < power; i++)
            {
                int temp = v1[i];
                v1[i] = v2[i];
                v2[i] = temp;
            }
            list.Add(v1);
            list.Add(v2);
            return list;
        }
        public static List<int[]> Evaluate(List<int[]> population, float pm, float pc, int toNextPopulation, double k, float B)
        {
            Random rnd = new Random();
            List<int[]> list2 = new List<int[]>();
            int populationSize = population.Count;
            int power = population[0].Length;
            for(int i = 0; i < toNextPopulation; i++)
            {
                int i1 = rnd.Next(populationSize);
                int i2 = rnd.Next(populationSize);
                double x1 = FromBinaty(population[i1], power, k, B);
                double x2 = FromBinaty(population[i2], power, k, B);
                double res1 = func(x1);
                double res2 = func(x2);
                if(res1 < res2)
                {
                    list2.Add(population[i1]);
                }
                else
                {
                    list2.Add(population[i2]);
                }
            }
            for(int i = 0; i< toNextPopulation; i++)
            {
                double currPm = rnd.NextDouble();
                if (currPm < pm)
                {
                    //Console.WriteLine("Mutation detected");
                    Mutation(list2[i]);
                }
            }
            for(int i = 0; i < toNextPopulation; i += 2)
            {
                double currPc = rnd.NextDouble();
                if (currPc < pc)
                {
                    //Console.WriteLine("Cross detected");
                    List<int[]> smallList = Cross(list2[i], list2[i + 1]);
                    list2[i] = smallList[0];
                    list2[i+1] = smallList[1];
                }
            }
            return list2;
        }
        public static int[] BestV(List<int[]> population, int power, double k, float B)
        {
            /*
            int power = population[0].Length;
            int populationSize = population.Count;
            int[] best = population[0];
            double bestRes = func(FromBinaty(best, power, k, B));
            for(int i = 1; i < populationSize; i++)
            {
                double res2 = func(FromBinaty(population[i], power, k, B));
                if(res2<bestRes)
                {
                    best = population[i];
                    bestRes = res2;
                }
            }*/
            var best = SortPopulation(population, power, k, B)[0];
            return best;
        }
        public static int[] BestV(List<int[]> population, int power, int countOfvars, double k, float B)
        {
            var best = SortPopulation(population, power, countOfvars, k, B)[0];
            return best;
        }


    }
}
