using System.Linq;
using System.Text.RegularExpressions;
using FuncSharp;
using Library;

namespace AoC2023
{
    internal class Task08 : IAocTask
    {
        private static readonly Regex NodeRegex = new Regex(@"([A-Z0-9]{3}) = \(([A-Z0-9]{3}), ([A-Z0-9]{3})\)");

        public async Task<List<string>> Run()
        {
            var lines = await Utilities.ReadFileAsync("Input8.txt");
            Utilities.Check(lines.Count > 4, "Expected at least 4 lines on input.");

            var instructions = lines[0].ToArray();
            Utilities.Check(lines[1].IsEmpty(), "2nd line is expected to be blank line.");

            var nodesByIdentifier = GetNodes(lines.Skip(2)).ToDictionary(n => n.Identifier);

            var stepCount = 0;
            var instructionCount = instructions.Length;
            var currentNodes = nodesByIdentifier.Where(kvp => kvp.Key.EndsWith("A")).Select(kvp => kvp.Value).ToArray();
            while (currentNodes.Any(n => !n.Identifier.EndsWith("Z")))
            {
                var instruction = instructions[stepCount % instructionCount];

                Console.WriteLine($"Step {stepCount}:");
                var nextNodes = currentNodes.Select(n =>
                {
                    var nextNodeIdentifier = instruction switch
                    {
                        'L' => n.Left,
                        'R' => n.Right,
                        _ => throw new InvalidOperationException($"Unexpected instruction '{instruction}'.")
                    };
                    var nextNode = Utilities.Check(nodesByIdentifier.Get(nextNodeIdentifier), $"Failed to find node with '{nextNodeIdentifier}' identifier");
                    Console.WriteLine($"\t{n} => {instruction} => {nextNode}"); 

                    return nextNode;
                }).ToArray();

                ++stepCount;
                currentNodes = nextNodes;
            }

            return stepCount.ToString().ToEnumerable().AsList();
        }

        private IEnumerable<Node> GetNodes(IEnumerable<string> lines)
        {
            return lines.Select(l =>
            {
                var match = NodeRegex.Match(l);
                Utilities.Check(match.Success, $"Node '{l}' is not in expected format.");

                return new Node(match.Groups[1].ToString(), match.Groups[2].ToString(), match.Groups[3].ToString());
            });
        }

        private record Node(string Identifier, string Left, string Right);
    }
}
