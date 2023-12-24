using FuncSharp;
using Library;

namespace AoC2023
{
    internal class Task07 : IAocTask
    {
        private static readonly Dictionary<char, Card> Cards = new[]
        {
            new Card('2', 0),
            new Card('3', 1),
            new Card('4', 2),
            new Card('5', 3),
            new Card('6', 4),
            new Card('7', 5),
            new Card('8', 6),
            new Card('9', 7),
            new Card('T', 8),
            new Card('J', 9),
            new Card('Q', 10),
            new Card('K', 11),
            new Card('A', 12)
        }.ToDictionary(c => c.Height);
        
        public async Task<List<string>> Run()
        {
            var lines = await Utilities.ReadFileAsync("Input7.txt");
            var hands = GetHands(lines).ToArray();

            var orderedHands = hands.Order().ToArray();
            var results = orderedHands.Select((h, i) =>
            {
                Console.WriteLine($"Rank: {i + 1}, Hand: {h}");
                return (long)(h.Bid * (i + 1));
            });
            var result = results.Sum();

            return result.ToString().ToEnumerable().AsList();
        }

        private IEnumerable<Hand> GetHands(List<string> lines)
        {
            return lines.Select(l =>
            {
                var lineSplit = l.Split(" ");
                Utilities.CheckLenght(lineSplit, 2, "Expected split to 2 parts.");

                var cards = lineSplit[0].Select(c => Cards[c]).AsReadOnlyList();
                var bet = Convert.ToInt32(lineSplit[1]);
                return new Hand(cards, bet);
                
            });
        }

        private record Card(char Height, int Value) : IComparable<Card>
        {
            public int CompareTo(Card? other)
            {
                return other.MatchRef(o => Value.CompareTo(o.Value), _ => 1);
            }
        }

        private sealed class Hand : IComparable<Hand>
        {
            public Hand(IReadOnlyList<Card> cards, int bet)
            {
                Utilities.Check(cards != null && cards.Count == 5, "Expected 5 cards in hand.");

                Cards = cards!;
                Bid = bet;
                Type = GetType(Cards);
            }

            public IReadOnlyList<Card> Cards { get; }

            public int Bid { get; set; }

            public HandType Type { get; }

            private static HandType GetType(IReadOnlyList<Card> cards)
            {
                var cardCountsByHeight = cards.GroupBy(c => c.Height).ToDictionary(g => g.Key, g => g.Count());
                var maxSameCardCount = cardCountsByHeight.Values.Max();

                return cardCountsByHeight.Keys.Count switch
                {
                    5 => HandType.HightCard,
                    4 => HandType.TwoPair,
                    3 => maxSameCardCount == 3 ? HandType.ThreeOfAKind : HandType.TwoPair,
                    2 => maxSameCardCount == 4 ? HandType.FourOfAKind : HandType.FullHouse,
                    1 => HandType.FiveOfAKind,
                    _ => throw new InvalidOperationException($"Unexpected hand breakdown to {cardCountsByHeight.Keys.Count} cards.")
                };
            }

            public override string ToString()
            {
                return $"{Cards.Select(c => c.Height.ToString()).MkString("")} - {Type.ToString()} - Bid: {Bid}";
            }

            public int CompareTo(Hand? other)
            {
                if (other == null ) return 1;

                var typeComparison = Type.CompareTo(other.Type);
                if (typeComparison != 0)
                {
                    return typeComparison;
                }

                var cardComparisons = Cards.Zip(other.Cards, (c, o) => c.CompareTo(o));
                var firstNonZero = cardComparisons.FirstOption(c => c != 0);
                return firstNonZero.GetOrDefault();
            }
        }

        enum HandType
        {
            HightCard = 0,
            OnePair = 1,
            TwoPair = 2,
            ThreeOfAKind = 3,
            FullHouse = 4,
            FourOfAKind = 5,
            FiveOfAKind = 6
        }
    }
}
