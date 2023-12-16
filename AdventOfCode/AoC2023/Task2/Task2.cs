using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FuncSharp;
using Library;
using Microsoft.VisualBasic;
using static AoC2023.Task2;

namespace AoC2023
{
    internal class Task2 : IAocTask
    {
        private static readonly Regex GameId = new("Game ([0-9]+)");
        private static readonly Regex RedCount = new("([0-9]+) red");
        private static readonly Regex GreenCount = new("([0-9]+) green");
        private static readonly Regex BlueCount = new("([0-9]+) blue");

        public async Task<List<string>> Run()
        {
            var games = await GetGames("Input2.txt");
            var minimalConfigs = games.Select(g =>
            {
                var minRedCount = g.Draws.Max(d => d.Red ?? 0);
                var minGreenCount = g.Draws.Max(d => d.Green ?? 0);
                var minBlueCount = g.Draws.Max(d => d.Blue ?? 0);

                return (
                    Game: g, 
                    MinCubes: new Cubes(Red: minRedCount, Green: minGreenCount, Blue: minBlueCount), 
                    Power: minRedCount * minGreenCount * minBlueCount
                );
            }).ToArray();

            var results = minimalConfigs.Select(c => $"Game: {c.Game.Id} - Power: {c.Power} - Minimal Cubes: {c.MinCubes.ToString()}");
            var sumResult = minimalConfigs.Sum(c => c.Power);
            return results.Concat(sumResult.ToString()).ToList();
        }

        public async Task<List<string>> Sample1()
        {
            var threshold = new Cubes(Red: 12, Green: 13, Blue: 14);
            var games = await GetGames("Input2.txt");
            var possibleGames = games.Where(g =>
            {
                var (validDraws, invalidDraws) = g.Draws.Partition(d =>
                    (d.Red is null || d.Red <= threshold.Red) &&
                    (d.Blue is null || d.Blue <= threshold.Blue) &&
                    (d.Green is null || d.Green <= threshold.Green)
                );

                var isValid = invalidDraws.IsEmpty();
                if (!isValid)
                {
                    Console.WriteLine($"Game: {g.Id} has {invalidDraws.Count} invalid draws: {string.Join("; ", invalidDraws.Select(d => d.ToString()))}.");
                }

                return isValid;
            }).ToArray();

            var results = possibleGames.Select(g => g.ToString());
            var sumResult = possibleGames.Sum(g => g.Id);
            return results.Concat(sumResult.ToString()).ToList();
        }

        private static async Task<Game[]> GetGames(string fileName)
        {
            var gameLogs = await Utilities.ReadFileAsync($"Task2/{fileName}");
            var parsedGames = gameLogs.Select(ToGame).ToArray();

            var errors = parsedGames.Select(r => r.Error).Flatten().ToArray();
            if (errors.NonEmpty())
            {
                foreach (var error in errors)
                {
                    Console.WriteLine($"Failed to extract game because {error.Error}. GameLog: {error.GameLog}.");
                }

                throw new InvalidOperationException();
            }

            var games = parsedGames.Select(r => r.Success).Flatten().ToArray();
            return games;
        }

        private static Try<Game, ParsingIsssue> ToGame(string gameLog)
        {
            var gameSplit = gameLog.Split(":");
            if (gameSplit.Length != 2)
            {
                return Try.Error<Game, ParsingIsssue>(new ParsingIsssue(gameLog, "Not a valid count of ':'."));
            }

            var gameIdMatch = GameId.Match(gameSplit[0]);
            if (!gameIdMatch.Success)
            {
                return Try.Error<Game, ParsingIsssue>(new ParsingIsssue(gameLog, "Failed to match game id."));
            }

            var drawSplit = gameSplit[1].Split("; ");
            if (drawSplit.IsEmpty())
            {
                return Try.Error<Game, ParsingIsssue>(new ParsingIsssue(gameLog, "Failed to parse draws."));
            }

            var gameId = Convert.ToInt32(gameIdMatch.Groups[1].Value);
            var draws = drawSplit.Select(draw => 
            {
                var redMatch = RedCount.Match(draw);
                var greenMatch = GreenCount.Match(draw);
                var blueMatch = BlueCount.Match(draw);

                return new Cubes(
                    Red: redMatch.Success ? Convert.ToInt32(redMatch.Groups[1].Value) : null,
                    Green: greenMatch.Success ? Convert.ToInt32(greenMatch.Groups[1].Value) : null,
                    Blue: blueMatch.Success ? Convert.ToInt32(blueMatch.Groups[1].Value) : null
                );

            }).ToArray();

            return Try.Success<Game, ParsingIsssue>(new Game(gameId, draws, gameLog));
        }

        internal record Game(int Id, Cubes[] Draws, string Source)
        {
            public override string ToString()
            {
                return $"Game: {Id}: {string.Join(";", Draws.Select(d => d.ToString()))} | Source: {Source}";
            }
        }


        internal record Cubes(int? Red, int? Green, int? Blue)
        {
            public override string ToString()
            {
                return string.Join(", ", GetParts());
            }

            private IEnumerable<string> GetParts()
            {
                if (Red is not null)
                {
                    yield return $"{Red} red";
                }

                if (Green is not null)
                {
                    yield return $"{Green} green";
                }

                if (Blue is not null)
                {
                    yield return $"{Blue} blue";
                }
            }
        }

        internal record ParsingIsssue(string GameLog, string Error);
    }

}
