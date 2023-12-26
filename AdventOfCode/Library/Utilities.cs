using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using FuncSharp;

namespace Library
{
    public static class Utilities
    {
        public static async Task<List<string>> ReadFileAsync(string fileName)
        {
            string consoleProjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "Inputs", "2023");
            string fullPath = Path.Combine(consoleProjectPath, fileName);

            string fileContent = await File.ReadAllTextAsync(fullPath);
            return [.. fileContent.Split("\r\n")];
        }

        public static IReadOnlyList<T> CreateFlat<T>(params Option<IEnumerable<T>>[] values)
        {
            return values.Flatten().Flatten().ToReadOnlyList();
        }

        public static int[] ToInts(string line, string delimitter = " ", StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            return line.Split(delimitter, StringSplitOptions.RemoveEmptyEntries).Select(i => Convert.ToInt32(i)).ToArray();
        }

        public static long[] ToLongs(string line, string delimitter = " ", StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            return line.Split(delimitter, StringSplitOptions.RemoveEmptyEntries).Select(i => Convert.ToInt64(i)).ToArray();
        }

        public static int Multiply(IEnumerable<int> enumerable)
        {
            return enumerable.Aggregate(1, (accumulator, value) => accumulator = accumulator * value);
        }

        public static void Check(bool condition, string errorMessage)
        {
            if (!condition) throw new InvalidOperationException(errorMessage);
        }

        public static void CheckLenght(string[] array, int lenght, string errorMessage)
        {
            Check(array.Length == lenght, errorMessage);
        }

        public static T Check<T>(Option<T> option, string errorMessage)
        {
            Utilities.Check(option.NonEmpty, errorMessage);
            return option.Get();
        }
    }
}
