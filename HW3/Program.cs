using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace HW3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            sol3();
        }

        static void sol1()
        {
            int side = 5; // or let user choose
            Random rand = new Random(); // Shared Random instance
            Bingo playerBingo = new Bingo(side, rand);
            Bingo cpuBingo = new Bingo(side, rand);

            playerBingo.reset();
            cpuBingo.reset();

            while (true)
            {
                Console.WriteLine("Your Bingo Board:");
                playerBingo.show();

                Console.WriteLine("CPU Bingo Board:");
                cpuBingo.show();

                Console.Write($"\nInput a number (1~{side * side}, or 0 to quit): ");
                string inp = Console.ReadLine();
                if (inp == null || inp.Trim() == "0")
                {
                    Console.WriteLine("Quit Bingo game!");
                    break;
                }

                int k;
                if (!int.TryParse(inp, out k) || k < 1 || k > side * side)
                {
                    Console.WriteLine("Invalid input!");
                    continue;
                }

                playerBingo.markCell(k);
                cpuBingo.markCell(k);

                // CPU turn: pick a random unmarked number for itself
                int cpuPick;
                do
                {
                    cpuPick = rand.Next(1, side * side + 1);
                } while (cpuBingo.chkcell(cpuPick));
                Console.WriteLine($"CPU marks number: {cpuPick}");
                cpuBingo.markCell(cpuPick);
                playerBingo.markCell(cpuPick);

                // Check for win
                if (playerBingoCompleted(playerBingo, side) || playerBingoCompleted(cpuBingo, side))
                {
                    Console.WriteLine("Game over!");
                    Console.WriteLine("Your Bingo Board:");
                    playerBingo.show();
                    Console.WriteLine("CPU Bingo Board:");
                    cpuBingo.show();
                    break;
                }
            }

        }

        static bool playerBingoCompleted(Bingo bingo, int side)
        {
            return typeof(Bingo).GetField("lines", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(bingo) is int lines && lines >= side;
        }

        static void sol2() //count bulls and cows valid combination (4 digit)
        {
            int count = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    for (int m = 0; m < 10; m++)
                    {
                        for (int n = 0; n < 10; n++)
                        {
                            string s = $"{i}{j}{m}{n}";
                            if (chkvalid(s))
                            {
                                count++;
                            }
                        }
                    }
                }
            }
            Console.WriteLine($"Total valid combinations: {count}");

        }

        static bool chkvalid(string num) // check if all digits are unique
        {

            HashSet<char> digits = new HashSet<char>();
            foreach (char c in num)
            {
                if (digits.Contains(c))
                {
                    return false; // Duplicate digit found
                }
                digits.Add(c);
            }
            return true; // All digits are unique
        }

        static void sol3()
        {
            int n;
            Console.Write("Enter the number of digits for the game (1-10): ");
            n = int.Parse(Console.ReadLine());
            Game1A2B game = new Game1A2B(n);
            Console.WriteLine($"Game number 1A2B! ({n}-digits)");
            Console.WriteLine($"Debug: {game.Answer}");
            Console.Write("Your guess (or '0' to quit): ");
            string guess;
            do
            {
                guess = Console.ReadLine();
                if (guess == "0")
                {
                    Console.WriteLine("Quit the game!");
                    break;
                }
                if (!game.IsAvailable(guess) || guess.Length != n)
                {
                    Console.WriteLine($"Invalid input! Please enter a {n}-digit number with unique digits.");
                    continue;
                }
                int[] result = game.GetAB(guess);
                Console.Write($"{guess} => ");
                if (result[0] == 0 && result[1] == 0)
                {
                    Console.Write("0");
                }
                else if (result[0] == 0)
                {
                    Console.Write($"{result[1]}B");
                }
                else if (result[1] == 0)
                {
                    Console.Write($"{result[0]}A");
                }
                else
                {
                    Console.Write($"{result[0]}A{result[1]}B");
                }
                Console.WriteLine();
                if (result[0]== n)
                {
                    Console.WriteLine("You've got the answer!");
                    break;
                }
                Console.Write("Your guess (or '0' to quit): ");
            } while (true);
        }
    }
}
