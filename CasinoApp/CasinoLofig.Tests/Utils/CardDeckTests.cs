using System.Collections.Generic;
using System.Diagnostics;
using LocalCasino.Common.Utils;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace LocalCasino.Common.Tests.Utils
{
    [TestClass]
    public class CardDeckTests
    {
        [TestMethod]
        public void TestOneDeckNormally()
        {
            var cd = new CardDeck(1);
            Assert.AreEqual(52, cd.CardsLeft);

            Card c = cd.DrawCard();
            Assert.IsNotNull(c);
            Assert.AreEqual(51, cd.CardsLeft);

            IList<Card> cs = cd.DrawCards(2);
            Assert.IsNotNull(cs);
            Assert.AreEqual(2, cs.Count);
            Assert.AreEqual(49, cd.CardsLeft);

            IList<Card> mcs = cd.DrawCards(0);
            Assert.IsNotNull(mcs);
            Assert.AreEqual(0, mcs.Count);
            Assert.AreEqual(49, cd.CardsLeft);
        }

        [TestMethod]
        public void TestTwoDeckNormally()
        {
            var cd = new CardDeck(2);
            Assert.AreEqual(104, cd.CardsLeft);

            Card c = cd.DrawCard();
            Assert.IsNotNull(c);
            Assert.AreEqual(103, cd.CardsLeft);

            IList<Card> cs = cd.DrawCards(2);
            Assert.IsNotNull(cs);
            Assert.AreEqual(2, cs.Count);
            Assert.AreEqual(101, cd.CardsLeft);

            IList<Card> mcs = cd.DrawCards(0);
            Assert.IsNotNull(mcs);
            Assert.AreEqual(0, mcs.Count);
            Assert.AreEqual(101, cd.CardsLeft);
        }

        [TestMethod]
        public void TestOneDeckTooMany()
        {
            var cd = new CardDeck(1);
            Assert.AreEqual(52, cd.CardsLeft);

            IList<Card> mcs = cd.DrawCards(30);
            Assert.IsNotNull(mcs);
            Assert.AreEqual(30, mcs.Count);
            Assert.AreEqual(22, cd.CardsLeft);

            IList<Card> mcs2 = cd.DrawCards(30);
            Assert.IsNotNull(mcs2);
            Assert.AreEqual(22, mcs2.Count);
            Assert.AreEqual(0, cd.CardsLeft);
        }

        [TestMethod]
        public void TestZeroDeck()
        {
            var cd = new CardDeck(0);
            Assert.AreEqual(0, cd.CardsLeft);

            IList<Card> mcs = cd.DrawCards(1);
            Assert.IsNotNull(mcs);
            Assert.AreEqual(0, mcs.Count);
            Assert.AreEqual(0, cd.CardsLeft);
        }

        [TestMethod]
        public void TestOneDeckForUniqueness()
        {
            var cd = new CardDeck(1);
            Assert.AreEqual(52, cd.CardsLeft);
            Debug.WriteLine(cd.ToString());

            IDictionary<string, string> dic = new Dictionary<string, string>();
            CardHand mcs = cd.DrawCards(52);
            Assert.AreEqual(52, mcs.Count);
            Assert.AreEqual(0, cd.CardsLeft);

            foreach (var card in mcs)
            {
                if (!dic.ContainsKey(card.Id))
                {
                    dic.Add(card.Id, card.Id);
                }
                else
                {
                    Assert.Fail("Duplicate {0} was found, but not expected!", card.Id);
                }
            }

            Debug.WriteLine(mcs.ToString());
        }
    }
}