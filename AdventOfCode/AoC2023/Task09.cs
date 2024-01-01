using FuncSharp;
using Library;

namespace AoC2023
{
    internal class Task09 : IAocTask
    {
        public async Task<List<string>> Run()
        {
            var lines = await Utilities.ReadFileAsync("Input9-1.txt");

            var parsedLines = lines.Select(l => Utilities.ToLongs(l)).ToArray();
            var rows = parsedLines.Select(l => Interpolate(l)).ToArray();

            var rowResults = rows.Select(r => r.ToString() + "\n");
            var result = rows.Sum(r => r.NewValue).ToString();
            return rowResults.Concat(result).AsList();
        }

        private Row Interpolate(long[] values)
        {
            var differences = values.Skip(1).Select((v, i) => v - values[i]).ToArray();
            if (differences.Sum() == 0)
            {
                return new Row(values, values.Last(), Interpolation: null);
            }
            else
            {
                var interpolation = Interpolate(differences);

                return new Row(values, values.Last() + interpolation.NewValue, interpolation);
            }
        }

        private record Row(long[] Values, long NewValue, Row? Interpolation)
        {
            public override string ToString()
            {
                var line = $"{Values.Select(v => v.ToString()).MkString("  ")} => {NewValue}";
                return Interpolation.MatchRef(i => $"{line}\n{i}", _ => line);
            }
        }
    }
}
