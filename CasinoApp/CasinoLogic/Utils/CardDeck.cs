using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalCasino.Common.Constants;
using LocalCasino.Common.Exceptions;

namespace LocalCasino.Common.Utils
{
    /// <summary>
    /// Represents a deck of card with 52 cards represented by Id per nbrOfDecks specified
    /// </summary>
    public class CardDeck : List<string>
    {
        private static readonly Random random = new Random();
        private readonly string[] _hearts = { "Hearts_02", "Hearts_03", "Hearts_04", "Hearts_05", "Hearts_06", "Hearts_07", "Hearts_08", "Hearts_09", "Hearts_10", "Hearts_11_J", "Hearts_12_Q", "Hearts_13_K", "Hearts_14_A" };
        private readonly string[] _spades = { "Spades_02", "Spades_03", "Spades_04", "Spades_05", "Spades_06", "Spades_07", "Spades_08", "Spades_09", "Spades_10", "Spades_11_J", "Spades_12_Q", "Spades_13_K", "Spades_14_A" };
        private readonly string[] _diamonds = { "Diamonds_02", "Diamonds_03", "Diamonds_04", "Diamonds_05", "Diamonds_06", "Diamonds_07", "Diamonds_08", "Diamonds_09", "Diamonds_10", "Diamonds_11_J", "Diamonds_12_Q", "Diamonds_13_K", "Diamonds_14_A" };
        private readonly string[] _clubs = { "Clubs_02", "Clubs_03", "Clubs_04", "Clubs_05", "Clubs_06", "Clubs_07", "Clubs_08", "Clubs_09", "Clubs_10", "Clubs_11_J", "Clubs_12_Q", "Clubs_13_K", "Clubs_14_A" };

        public CardDeck(int nbrOfDecks)
            : base(nbrOfDecks * GameConstants.CardsPerDeck)
        {
            for (int i = 0; i < nbrOfDecks; i++)
            {
                AddRange(_hearts);
                ShuffleList(this);

                AddRange(_spades);
                ShuffleList(this);

                AddRange(_diamonds);
                ShuffleList(this);

                AddRange(_clubs);
                ShuffleList(this);

                Verify(i + 1);
            }
        }

        public int CardsLeft
        {
            get { return base.Count; }
        }

        private static void ShuffleList<T>(IList<T> list)
        {
            if (list.Count > 1)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    T tmp = list[i];
                    int randomIndex = random.Next(i + 1);

                    //Swap elements
                    list[i] = list[randomIndex];
                    list[randomIndex] = tmp;
                }
            }
        }

        public Card DrawCard()
        {
            string id = base[0];
            base.RemoveAt(0);
            return new Card(id);
        }

        private void Verify(int nbrOfDecks)
        {
            var gr = this.GroupBy(x => x).Where(x => x.Count() == nbrOfDecks);
            if (gr.Count() != GameConstants.CardsPerDeck)
            {
                throw new CasinoException("Invalid deck detected");
            }
        }

        public CardHand DrawCards(int nbrOfCards)
        {
            var cards = new CardHand(nbrOfCards);
            for (int i = 0; i < nbrOfCards; i++)
            {
                if (CardsLeft > 0)
                {
                    string id = base[0];
                    base.RemoveAt(0);
                    cards.Add(new Card(id));
                }
            }

            return cards;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var card in this)
            {
                sb.Append(card);
                sb.Append(", ");
            }

            return sb.ToString();
        }
    }
}
