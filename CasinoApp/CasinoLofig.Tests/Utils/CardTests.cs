using System;
using LocalCasino.Common.Enums;
using LocalCasino.Common.Utils;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace LocalCasino.Common.Tests.Utils
{
    [TestClass]
    public class CardTests
    {
        [TestMethod]
        public void TestInvalidCard()
        {
            Assert.ThrowsException<ArgumentException>(() => { var c = new Card("1"); });
        }

        [TestMethod]
        public void TestCardGoodFormat()
        {
            var d = new Card("Hearts_1");
            Assert.AreEqual("Hearts_1", d.Id);
            Assert.AreEqual(CardSuit.Hearts, d.Suit);
            Assert.AreEqual(1, d.Value);
        }

        [TestMethod]
        public void TestCardInvalidFormat()
        {
            Assert.ThrowsException<ArgumentException>(() => { var c = new Card("Beads_1"); });
        }

        [TestMethod]
        public void TestCardToHigh()
        {
            Assert.ThrowsException<ArgumentException>(() => { var c = new Card("Hearts_55"); });
        }
    }
}