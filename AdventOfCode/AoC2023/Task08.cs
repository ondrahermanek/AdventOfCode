using System.Linq;
using System.Text.RegularExpressions;
using FuncSharp;
using Library;

namespace AoC2023
{
    internal class Task08 : IAocTask
    {
        private static readonly Regex NodeRegex = new Regex(@"([A-Z]{3}) = \(([A-Z]{3}), ([A-Z]{3})\)");
        private const string StartNode = "AAA";
        private const string EndNode = "ZZZ";

        public async Task<List<string>> Run()
        {
            var lines = await Utilities.ReadFileAsync("Input8.txt");
            Utilities.Check(lines.Count > 4, "Expected at least 4 lines on input.");

            var instructions = lines[0].ToArray();
            Utilities.Check(lines[1].IsEmpty(), "2nd line is expected to be blank line.");

            var nodesByIdentifier = GetNodes(lines.Skip(2)).ToDictionary(n => n.Identifier);

            var stepCount = 0;
            var instructionCount = instructions.Length;
            var currentNode = Utilities.Check(nodesByIdentifier.Get(StartNode), $"Failed to find node with '{StartNode}' identifier");
            while (true)
            {
                var instruction = instructions[stepCount % instructionCount];
                var nextNodeIdentifier = instruction switch
                {
                    'L' => currentNode.Left,
                    'R' => currentNode.Right,
                    _ => throw new InvalidOperationException($"Unexpected instruction '{instruction}'.")
                };

                var nextNode = Utilities.Check(nodesByIdentifier.Get(nextNodeIdentifier), $"Failed to find node with '{nextNodeIdentifier}' identifier");
                Console.WriteLine($"Step {stepCount}: {currentNode} => {instruction} => {nextNode}");

                ++stepCount;
                currentNode = nextNode;

                if (currentNode.Identifier == EndNode)
                {
                    break;
                }

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
