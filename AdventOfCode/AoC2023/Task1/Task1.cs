using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Library;
using FuncSharp;

namespace AoC2023
{
    internal class Task1 : IAocTask
    {
        private static readonly (string, int)[] Numbers = [
            ("one", 1), ("two", 2), ("three", 3), ("four", 4), ("five", 5), ("six", 6), ("seven", 7), ("eight", 8), ("nine", 9),
            ("1", 1), ("2", 2), ("3", 3), ("4", 4), ("5", 5), ("6", 6), ("7", 7), ("8", 8), ("9", 9)
        ];

        public async Task<List<string>> Run()
        {
            var lines = await Utilities.ReadFileAsync("Task1/Input1.txt");
            var results = lines.Select(l => ExtractNumbers(l));

            return [results.Sum().ToString()];
            //return results.Select(r => r.ToString()).ToList();
        }

        private static int ExtractNumbers(string line)
        {
            var firstOccurrenceIndexes = Numbers.Select(n => (Replacement: n, Index: line.IndexOf(n.Item1))).ToArray();
            var firstOccurrences = firstOccurrenceIndexes.Where(ri => ri.Index >= 0);
            var firstNumber = firstOccurrences.MinBy(ri => ri.Index).Replacement.Item2;

            var lastOccurrenceIndexes = Numbers.Select(n => (Replacement: n, Index: line.LastIndexOf(n.Item1))).ToArray();
            var lastOccurrences = lastOccurrenceIndexes.Where(ri => ri.Index >= 0);
            var lastNumber = lastOccurrences.MaxBy(ri => ri.Index).Replacement.Item2;

            return firstNumber * 10 + lastNumber;
        }
    }
}
