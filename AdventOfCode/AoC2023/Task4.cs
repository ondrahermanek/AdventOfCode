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
    internal class Task4 : IAocTask
    {
        public async Task<List<string>> Run()
        {
            var lines = await Utilities.ReadFileAsync("Input4.txt");
            var cards = ParseCards(lines);
            var cardQuantities = cards.ToDictionary(c => c.Id, c => 1);

            foreach (var card in cards)
            {
                var score = card.TicketNumbers.Intersect(card.WinningNumbers).Count();
                var quantity = cardQuantities[card.Id];

                for (var i = 1; i <= score; i++)
                {
                    var nexCardId = card.Id + i;
                    cardQuantities[nexCardId] += quantity;
                }

            }

            var results = cardQuantities.OrderBy(kvp => kvp.Key).Select(kvp => $"CardId {kvp.Key} - Quantity {kvp.Value}");
            var result = cardQuantities.Values.Sum().ToString().ToEnumerable();

            return results.Concat(result).ToList();
        }

        private static List<(Card Card, int Score)> ComputeScores(IReadOnlyList<Card> cards)
        {
            return cards.Select(c =>
            {
                var score = c.TicketNumbers.Intersect(c.WinningNumbers).Count();

                return (Card: c, Score: score == 0 ? 0 : (int)Math.Pow(2, score - 1));
            }).ToList();
        }

        private IReadOnlyList<Card> ParseCards(IEnumerable<string> lines)
        {
            return lines.Select(l =>
            {
                var split = l.Split(':', '|');
                if (split.Length != 3) throw new InvalidDataException($"Line '{l}' has invalid format.");

                var cardSplit = split[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (cardSplit.Length != 2) throw new InvalidDataException($"Card '{split[0]}' has invalid format.");

                var ticketNumbers = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (ticketNumbers.Length == 0) throw new InvalidDataException($"Ticket numbers '{split[1]}' have invalid format.");

                var winningNumbers = split[2].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (winningNumbers.Length == 0) throw new InvalidDataException($"Winning numbers '{split[2]}' have invalid format.");

                return new Card(Id: Convert.ToInt32(cardSplit[1]), TicketNumbers: ticketNumbers, WinningNumbers: winningNumbers, Quantity: 1);
            }).ToList();
        }

        private record Card(int Id, string[] TicketNumbers, string[] WinningNumbers, int Quantity);
    }
}
