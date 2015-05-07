using LocalCasino.Common.Utils;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace LocalCasino.Common.Tests.Utils
{
    [TestClass]
    public class CardHandTests
    {
        [TestMethod]
        public void TestOneHandEmpty()
        {
            var ch = new CardHand(5);
            Assert.AreEqual(0, ch.CardsLeft);

            Card c = ch.PlayCard();
            Assert.IsNull(c);
            Assert.AreEqual(0, ch.CardsLeft);
        }

        [TestMethod]
        public void TestOneHandOneCards()
        {
            var ch = new CardHand(2);
            Assert.AreEqual(0, ch.CardsLeft);

            ch.Add(new Card("clubs_14"));
            Assert.AreEqual(1, ch.CardsLeft);
            
            Card c = ch.PlayCard();
            Assert.IsNotNull(c);
            Assert.AreEqual(0, ch.CardsLeft);
        }

        [TestMethod]
        public void TestOneHandSomeCards()
        {
            var ch = new CardHand(2);
            Assert.AreEqual(0, ch.CardsLeft);

            ch.Add(new Card("hearts_01"));
            ch.Add(new Card("diamonds_03"));
            ch.Add(new Card("spades_12"));
            Assert.AreEqual(3, ch.CardsLeft);

            Card c = ch.PlayCard();
            Assert.IsNotNull(c);
            Assert.AreEqual(2, ch.CardsLeft);
        }
    }
}