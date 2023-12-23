using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using FuncSharp;
using Library;

namespace AoC2023
{
    internal class Task05 : IAocTask
    {
        public async Task<List<string>> Run()
        {
            var lines = await Utilities.ReadFileAsync("Input5.txt");
            var seeds = GetSeeds(lines[0]).ToArray();
            CheckBlankLine(lines[1]);

            var mapsByOrder = GetMaps(lines.Skip(2)).GroupBy(m => m.Order).ToDictionary(g => g.Key, g => g.ToArray()); // TODO: order by source something?

            return seeds.SelectMany(s => FindDestination(s, mapsByOrder)).Min().ToString().ToEnumerable().AsList();
        }

        private IEnumerable<Seed> GetSeeds(string line)
        {
            var values = Utilities.ToLongs(line.Replace("seeds:", ""));
            if (values.Length % 2 == 1) throw new InvalidOperationException("Expected even count of seed values.");

            while (values.NonEmpty())
            {
                yield return new Seed(values[0], values[1]);

                values = values.Skip(2).ToArray();
            }
        }

        private IEnumerable<Map> GetMaps(IEnumerable<string> lines)
        {
            var enumerator = lines.GetEnumerator();
            enumerator.MoveNext();

            var seedToSoilMaps = GetMaps("seed-to-soil", 0, enumerator).ToArray();
            CheckBlankLineAndMoveNext(enumerator);

            var soilToFertilizerMaps = GetMaps("soil-to-fertilizer", 1, enumerator).ToArray();
            CheckBlankLineAndMoveNext(enumerator);

            var fertilizerToWaterMaps = GetMaps("fertilizer-to-water", 2, enumerator).ToArray();
            CheckBlankLineAndMoveNext(enumerator);

            var waterToLightMaps = GetMaps("water-to-light", 3, enumerator).ToArray();
            CheckBlankLineAndMoveNext(enumerator);

            var lightToTemperaturMap = GetMaps("light-to-temperature", 4, enumerator).ToArray();
            CheckBlankLineAndMoveNext(enumerator);

            var temperatureToHumidityMaps = GetMaps("temperature-to-humidity", 5, enumerator).ToArray();
            CheckBlankLineAndMoveNext(enumerator);

            var humidityToLocationMaps = GetMaps("humidity-to-location", 6, enumerator).ToArray();

            if (enumerator.MoveNext()) throw new InvalidOperationException("No more lines expected");

            return seedToSoilMaps.Concat(soilToFertilizerMaps, fertilizerToWaterMaps, waterToLightMaps, lightToTemperaturMap, temperatureToHumidityMaps, humidityToLocationMaps);
        }

        private IEnumerable<Map> GetMaps(string mapName, int order, IEnumerator<string> enumerator)
        {
            var line = enumerator.Current;
            if (!line.Contains(mapName)) throw new InvalidOperationException($"{mapName} map expected.");
            while (enumerator.MoveNext() && (line = enumerator.Current).NonEmpty())
            {
                var mapDefinition = Utilities.ToLongs(line);
                if (mapDefinition.Length != 3) throw new InvalidOperationException($"Unexpected definition '{line}' for map {mapName}.");

                var destinationStart = mapDefinition[0];
                var sourceStart = mapDefinition[1];
                var length = mapDefinition[2];

                yield return new Map(mapName, order, destinationStart, destinationStart + length - 1, sourceStart, sourceStart + length - 1, length);
            }
        }

        private IEnumerable<long> FindDestination(Seed seed, Dictionary<int, Map[]> mapsByOrder)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (long i = 0; i < seed.Count; ++i)
            {
                var seedNumber = seed.Start + i;
                var soil = FindDestination(seedNumber, mapsByOrder[0]);
                var fertilizer = FindDestination(soil, mapsByOrder[1]);
                var water = FindDestination(fertilizer, mapsByOrder[2]);
                var light = FindDestination(water, mapsByOrder[3]);
                var temp = FindDestination(light, mapsByOrder[4]);
                var humidity = FindDestination(temp, mapsByOrder[5]);
                var location = FindDestination(humidity, mapsByOrder[6]);

                //Console.WriteLine($"Seed {seedNumber} -> Soil {soil} -> Fertilizer {fertilizer} -> Water {water} -> Light {light} -> Temp {temp} -> Hum {humidity} -> Loc {location}.");

                yield return location;
            }

            Console.WriteLine($"Seed {seed.Start} computed in {stopWatch.Elapsed.TotalSeconds} s.");
        }

        private long FindDestination(long source, Map[] maps)
        {
            var matchingMaps = maps.Where(m => m.SourceStart <= source && source <= m.SourceEnd).ToArray();
            return matchingMaps.Length switch
            {
                0 => source,
                1 => matchingMaps[0].Get(source),
                _ => throw new InvalidOperationException($"Multiple matching maps found for source {source}.")
            };
        }

        private static void CheckBlankLineAndMoveNext(IEnumerator<string> enumerator)
        {
            CheckBlankLine(enumerator.Current);
            enumerator.MoveNext();
        }

        private static void CheckBlankLine(string line)
        {
            if (line.NonEmpty()) throw new InvalidOperationException("Blank line expected.");
        }

        private record Seed(long Start, long Count);

        private record Map(string Name, int Order, long DestinationStart, long DestinationEnd, long SourceStart, long SourceEnd, long Lenght)
        {
            public long Get(long source)
            {
                var diff = source - SourceStart;
                return DestinationStart + diff;

            }
        }
    }
}
