using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FuncSharp;
using Library;

namespace AoC2023
{
    internal class Task06 : IAocTask
    {
        public async Task<List<string>> Run()
        {
            var lines = await Utilities.ReadFileAsync("Input6.txt");
            if (lines.Count != 2) throw new InvalidOperationException("Expected 2 lines on input.");

            var nonNumbers = new Regex("[^0-9]*");
            var duration = nonNumbers.Replace(lines[0], "");
            var distance = nonNumbers.Replace(lines[1], "");

            var races = new Race(Convert.ToInt64(duration), Convert.ToInt64(distance)).ToEnumerable();

            /* part1
            var durations = Utilities.ToInts(lines[0].Replace("Time:", ""));
            var distances = Utilities.ToInts(lines[1].Replace("Distance:", ""));
            if (durations.Length != distances.Length) throw new InvalidOperationException("Expecteed same number of Distances and Durations.");


            var races = durations.Zip(distances, (duration, distance) => new Race(duration, distance)).ToArray();
            */

            var attempts = races.Select(r => (Race: r, Attempts: ComputeAttempts(r).OrderBy(a => a.Speed).ToArray())).ToArray();
            var successfullAttempts = attempts.Select(r => (
                Race: r.Race,
                SuccessfullAttemptCount: r.Attempts.Where(a => a.TotalDistance > r.Race.MaxDistance).Sum(sa => sa.Multiplier)
            )).ToArray();

            var results = successfullAttempts.Select(r => $"Race Duration: {r.Race.Duration}, Race Record {r.Race.MaxDistance} can be broken {r.SuccessfullAttemptCount} ways.");

            var result = Utilities.Multiply(successfullAttempts.Select(a => a.SuccessfullAttemptCount));
            return results.Concat(result.ToString()).ToList();
        }

        private IEnumerable<Attempt> ComputeAttempts(Race race)
        {
            for (long i = 1; i <= race.Duration / 2; ++i)
            {
                // The attempts are mirrored on both sides of i.
                var speed = i;
                var timeLeft = race.Duration - i;
                var multiplier = (speed != timeLeft) ? 2 : 1;
                yield return new Attempt(speed, timeLeft, speed * timeLeft, multiplier);
            }
        }

        private record Race(long Duration, long MaxDistance);

        private record Attempt(long Speed, long TimeLeft, long TotalDistance, int Multiplier);
    }
}
