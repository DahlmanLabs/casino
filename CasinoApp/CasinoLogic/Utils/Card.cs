using System;
using LocalCasino.Common.Enums;

namespace LocalCasino.Common.Utils
{
    public class Card
    {
        public string Id { get { return _id; } }

        public int Value { get { return _value; } }

        public CardSuit Suit { get { return _suit; } }

        private string _id;
        private CardSuit _suit;
        private int _value;

        public string DebugState
        {
            get { return string.Format("{0}({1})", Suit, Value); }
        }

        public Card(string id)
        {
            _id = id;
            if (_id.IndexOf('_') < 0)
            {
                throw new ArgumentException("Invalid format of card id");
            }

            _suit = (CardSuit)Enum.Parse(typeof(CardSuit), _id.Split('_')[0], true);
            _value = int.Parse(_id.Split('_')[1]);
            if (_value < 1 || _value > 14)
            {
                throw new ArgumentException("Illegal value for card");
            }
        }

        public override string ToString()
        {
            return _id;
        }
    }
}