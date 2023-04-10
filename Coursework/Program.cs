using System;
using System.Collections.Generic;

namespace Coursework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int populationSize = 50,
                maxIterations = 500,
                maxIterWithoutChanges = 50,
                toNextPopulation = 40;
            float a = -1,
                b = 2,
                B = a;
            double eps = 0.000001f;
            int power = GetPower(a, b, eps);
            double k = (b - B) / (Math.Pow(2, power) - 1);
            float pm = 0.2f,
                pc = 0.8f;
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
                int[] best = BestV(newPop, k, B);
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

                Console.WriteLine("Max iterations reached, res x = " + FromBinaty(BestV(newPop, k, B), power, k, B));

            }
            else
            {
                Console.WriteLine("New populations don't have better individes, res x = " + FromBinaty(BestV(newPop, k, B), power, k, B));
            }

            Console.ReadLine();
        }
        public static List<int[]> NewPopulation(List<int[]> oldPop, float pm, float pc, int toNextPopulation, double k, float B)
        {
            int populationSize = oldPop.Count;
            int power = oldPop[0].Length;
            List<int[]> newPop = RandomPopulation(populationSize, power);
            List<int[]> smallPop = Evaluate(oldPop, pm, pc, toNextPopulation, k, B);
            for(int i = 0; i < toNextPopulation; i++)
            {
                newPop[i] = smallPop[i];
            }
            return newPop;
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
        public static int[] Mutation(int[] v)
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
            return v;
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
                    list2[i] = Mutation(list2[i]);
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
        public static int[] BestV(List<int[]> population, double k, float B)
        {
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
            }
            return best;
        }

    }
}
