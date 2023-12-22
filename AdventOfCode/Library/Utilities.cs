using System.ComponentModel.DataAnnotations;
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

        public static int[] ToInts(string line, string delimitter = "", StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            return line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(i => Convert.ToInt32(i)).ToArray();
        }

        public static long[] ToLongs(string line, string delimitter = "", StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            return line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(i => Convert.ToInt64(i)).ToArray();
        }
    }
}
