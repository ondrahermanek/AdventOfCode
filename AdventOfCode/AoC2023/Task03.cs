using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using FuncSharp;
using Library;

namespace AoC2023
{
    internal class Task03 : IAocTask
    {
        public async Task<List<string>> Run()
        {
            var lines = await Utilities.ReadFileAsync("Input3.txt");
            var (numbers, symbols) = GetSymbolsAndSymbols(lines);

            var numbersByRows = numbers.GroupBy(n => n.Row).ToDictionary(g => g.Key, g => g.AsEnumerable());

            var gears = symbols.Where(s => s.Value == "*");
            var gearRatios = gears.Select(g =>
            {
                var adjacentNumbers = GetAdjacentNumbers(g, numbersByRows);

                Console.WriteLine($"Symbol {g} neighbours with {adjacentNumbers.Length} numbers: {adjacentNumbers.Select(n => n.ToString()).MkString(", ")}.");

                if (adjacentNumbers.Length == 2)
                {
                    return adjacentNumbers[0].Value * adjacentNumbers[1].Value;
                }

                return 0;

            }).ToArray();

            var results = gearRatios.Select(n => n.ToString());
            var result = gearRatios.Sum().ToString();

            return results.Concat(result).ToList();
        }

        public async Task<List<string>> Run1()
        {
            var lines = await Utilities.ReadFileAsync("Input3.txt");
            var (numbers, symbols) = GetSymbolsAndSymbols(lines);

            /*
            Console.WriteLine("Numbers:");
            Console.WriteLine(numbers.Select(n => n.ToString()).MkString("\n"));

            Console.WriteLine("Symbols:");
            Console.WriteLine(symbols.Select(s => s.ToString()).MkString("\n"));
            */

            var numbersByRows = numbers.GroupBy(n => n.Row).ToDictionary(g => g.Key, g => g.AsEnumerable());

            var numbersAdjacentToSymbols = symbols.SelectMany(s =>
            {
                var adjacentNumbers = GetAdjacentNumbers(s, numbersByRows);

                Console.WriteLine($"Symbol {s} neighbours with {adjacentNumbers.Select(n => n.ToString()).MkString(", ")}.");

                return adjacentNumbers;

            }).ToArray();

            var results = numbersAdjacentToSymbols.Select(n => n.ToString());
            var result = numbersAdjacentToSymbols.Sum(n => n.Value).ToString();

            return results.Concat(result).ToList();
        }

        private static (List<Number> numbers, List<Symbol> symbols) GetSymbolsAndSymbols(List<string> lines)
        {
            var numbers = new List<Number>(lines.Count * 2);
            var symbols = new List<Symbol>(lines.Count);

            var rowIndex = 0;
            foreach (var line in lines)
            {
                var numberIndex = -1;
                var number = 0;
                for (int index = 0; index < line.Length; index++)
                {
                    var character = line[index];
                    if (char.IsNumber(character))
                    {
                        int value = character - '0';
                        number = number * 10 + value;
                        if (numberIndex < 0)
                        {
                            numberIndex = index;
                        }
                    }
                    else
                    {
                        if (number > 0)
                        {
                            numbers.Add(new Number(number, rowIndex, numberIndex, index - 1));
                            numberIndex = -1;
                            number = 0;
                        }

                        if (character != '.')
                        {
                            symbols.Add(new Symbol(character.ToString(), rowIndex, index));
                        }
                    }
                }

                if (number > 0)
                {
                    // Line ends with number.
                    numbers.Add(new Number(number, rowIndex, numberIndex, line.Length - 1));
                }

                rowIndex++;
            }

            return (numbers, symbols);
        }

        private static Number[] GetAdjacentNumbers(Symbol s, Dictionary<int, IEnumerable<Number>> numbersByRows)
        {
            var previousRowNumbers = numbersByRows.Get(s.Row - 1);
            var sameRowNumbers = numbersByRows.Get(s.Row);
            var nextRowNumbers = numbersByRows.Get(s.Row + 1);
            var possiblyAdjacentNumbers = Utilities.CreateFlat<Number>(previousRowNumbers, sameRowNumbers, nextRowNumbers);

            var adjacentNumbers = possiblyAdjacentNumbers.Where(n => s.Index >= (n.MinIndex - 1) && s.Index <= (n.MaxIndex + 1)).ToArray();
            return adjacentNumbers;
        }
    }

    internal record Number(int Value, int Row, int MinIndex, int MaxIndex);
    internal record Symbol(string Value, int Row, int Index);
}
