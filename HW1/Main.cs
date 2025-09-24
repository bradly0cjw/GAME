using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW1
{
    internal class Hw1
    {
        static void Main(string[] args)
        {
            sol1(args);
        }
            public class Score
            {
                public string Name { get; set; }
                public int[] score;

                public Score(string name, int numberOfScores)
                {
                    Name = name;
                    score = new int[numberOfScores];
                }

                public void show()
                {
                    Console.Write($"{Name}\t");
                    foreach (int s in score)
                    {
                        Console.Write($"{s}\t");
                    }
                    Console.WriteLine();
                }

                public override string ToString()
                {
                    return $"{Name}\t{string.Join("\t", score)}";
                }
            }
            static void sol1(string[] args)
            {
                string data =
                    "成翔財A 27 91 21 33 13\n" +
                    "詹雅婷B 96 90 40 55 69\n" +
                    "袁維茹C 38 85 72 13 34\n" +
                    "黃士哲D 81 40 24 93 79\n" +
                    "郭珮珊E 72 33 32 83 73\n" +
                    "陳儀琬F 78 55 22 41 62\n" +
                    "李碧彥G 30 48 13 93 70\n" +
                    "梁健玉H 23 89 10 44 24\n" +
                    "許雅淑I 90 11 33 27 67\n" +
                    "蕭宛新J 29 64 64 90 43\n";

                string[] items = data.Trim().Split('\n');
                List<Score> studentScores = new List<Score>();

                foreach (string itemLine in items)
                {
                    string[] fields = itemLine.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                    if (fields.Length < 2) continue;

                    string studentName = fields[0];
                    Score s = new Score(studentName, fields.Length - 1);

                    for (int j = 1; j < fields.Length; ++j)
                    {
                        int num;
                        if (int.TryParse(fields[j], out num))
                        {
                            s.score[j - 1] = num;
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Unable to parse '{fields[j]}' as a number. This score will be set to 0.");
                            s.score[j - 1] = 0;
                        }
                    }
                    studentScores.Add(s);
                }

                int sortSubjectIndex = 0;

                Console.WriteLine($"Original Data (Sorted by the {sortSubjectIndex + 1}th Score):");
                foreach (Score student in studentScores)
                {
                    student.show();
                }
                Console.WriteLine("\n------------------------------------------------\n");

                List<Score> sortedStudentScores = new List<Score>(studentScores);

                sortedStudentScores.Sort((s1, s2) =>
                {
                    if (sortSubjectIndex >= s1.score.Length || sortSubjectIndex >= s2.score.Length)
                    {
                        return 0;
                    }
                    return s1.score[sortSubjectIndex].CompareTo(s2.score[sortSubjectIndex]);
                });

                Console.WriteLine($"Sorted Data (by the {sortSubjectIndex + 1}th score in ascending order):");
                foreach (Score student in sortedStudentScores)
                {
                    student.show();
                }
            }

        static void sol2(string[] args)
        {
            int n = 9;
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (j == 1)
                        Console.Write($"{i}x{j}={i * j}");
                    else
                        Console.Write($" {i}x{j}={i * j,2}");
                }
                Console.WriteLine();
            }
        }
        static void sol3(string[] args)
        {
            int n = 9;
            int fixed_column_width = 7;
            int leadingSpacesCount;

            for (int i = 1; i <= n; i++)
            {
                int numberOfTermsInCurrentRow = i;

                if (n - numberOfTermsInCurrentRow == 1)
                    leadingSpacesCount = 6;
                else if (n - numberOfTermsInCurrentRow == 0)
                    leadingSpacesCount = 0;
                else
                    leadingSpacesCount = (n - (numberOfTermsInCurrentRow + 1)) * fixed_column_width + 6;

                Console.Write(new string(' ', leadingSpacesCount));

                for (int j = (n + 1) - i; j <= n; j++)
                {
                    string term;
                    if (j == 1)
                    {
                        term = string.Format($"{i}x{j}={i * j} ");
                    }
                    else
                    {
                        term = string.Format($"{i}x{j}={i * j,2} ");
                    }
                    Console.Write(term);
                }
                Console.WriteLine();
            }
        }
        public static void sol4(string[] args)
        {


            int n = 9;
            bool[,] printMask = new bool[n, n];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    printMask[i, j] = false;
            for (int j = 0; j < n; j++) printMask[j, 0] = true;
            for (int j = 0; j < n; j++) printMask[0, j] = true;
            printMask[1, 1] = true;
            for (int j = 8; j < n; j++) printMask[1, j] = true;
            for (int j = 7; j < n; j++) printMask[2, j] = true;
            printMask[2, 2] = true;
            for (int j = 7; j < n; j++) printMask[3, j] = true;
            printMask[3, 3] = true;
            for (int j = 6; j < n; j++) printMask[3, j] = true;
            printMask[5, 3] = true;
            printMask[6, 2] = true;
            printMask[7, 1] = true;
            for (int j = 0; j < n; j++) printMask[4, j] = true;
            for (int j = 6; j < n; j++) printMask[5, j] = true;
            for (int j = 7; j < n; j++) printMask[6, j] = true;
            for (int j = 8; j < n; j++) printMask[7, j] = true;
            for (int j = 0; j < n; j++) printMask[8, j] = true;

            int termWidth = 6;
            string emptySpace = new string(' ', termWidth);

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (printMask[i - 1, j - 1])
                    {
                        if (j == 1)
                            Console.Write(string.Format("{0}x{1}={2}", i, j, i * j));
                        else
                            Console.Write(string.Format("{0}x{1}={2,2}", i, j, i * j));
                    }
                    else
                    {
                        Console.Write(emptySpace);
                    }
                    if (j < n)
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }

        }
        static void sol5(string[] args)
        {
            Random rand = new Random();
            int answer = rand.Next(2, 99);

            int minRange = 1;
            int maxRange = 99;
            int guess;

            Console.WriteLine($"Debug: {answer}");
            Console.WriteLine("Pw Guessing Game");
            while (true)
            {
                Console.WriteLine($"Plz enter number between ({minRange}, {maxRange}):\n");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out guess))
                {
                    continue;
                }

                if (guess <= minRange || guess >= maxRange)
                {
                    continue;
                }


                if (guess == answer)
                {
                    Console.WriteLine("Correct!\n");
                    break;
                }
                else if (guess < answer)
                {
                    minRange = guess;
                }
                else
                {
                    maxRange = guess;
                }
                Console.WriteLine($"Plz enter number between ({minRange}, {maxRange}):\n");
            }
        }
    }
}

