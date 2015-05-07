using System.Collections.Generic;
using System.Text;

namespace LocalCasino.Common.Utils
{
    public class CardHand : List<Card>
    {
        public CardHand(int nbrOfCards)
            : base(nbrOfCards)
        {
        }

        public int CardsLeft { get { return base.Count; } }

        public string DebugState
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(string.Format("Cards: {0}", CardsLeft)); 
                foreach (Card card in this)
                {
                    sb.Append(", ");
                    sb.Append(card.DebugState);
                }

                return sb.ToString();
            }
        }

        public Card PlayCard()
        {
            if (CardsLeft > 0)
            {
                Card c = base[0];
                base.RemoveAt(0);
                return c;
            }
            else
            {
                return null;
            }
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
