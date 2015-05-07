using System;
using LocalCasino.Common.Enums;
using LocalCasino.Common.Utils;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace LocalCasino.Common.Tests.Utils
{
    [TestClass]
    public class UriHelperTests
    {
        [TestMethod]
        public void TestBackUris()
        {
            var i = UriHelper.GetCardBackUri();
            Assert.IsNotNull(i);
        }

        [TestMethod]
        public void TestValidCardUri1()
        {
            var i = UriHelper.GetCardUri(CardSuit.Hearts, 5);
            Assert.IsNotNull(i);
            Assert.AreEqual("ms-appx:/Assets/temp/Cards/Hearts/Hearts_05.png", i);
        }
        
        [TestMethod]
        public void TestValidCardUri2()
        {
            var i = UriHelper.GetCardUri(CardSuit.Hearts, 11);
            Assert.IsNotNull(i);
            Assert.AreEqual("ms-appx:/Assets/temp/Cards/Hearts/Hearts_11.png", i);
        }
        
        [TestMethod]
        public void TestValidCardUri3()
        {
            var i = UriHelper.GetCardUri(CardSuit.Diamonds, 6);
            Assert.IsNotNull(i);
            Assert.AreEqual("ms-appx:/Assets/temp/Cards/Diamonds/Diamonds_06.png", i);
        }
        
        [TestMethod]
        public void TestValidCardUri4()
        {
            var i = UriHelper.GetCardUri(CardSuit.Diamonds, 12);
            Assert.IsNotNull(i);
            Assert.AreEqual("ms-appx:/Assets/temp/Cards/Diamonds/Diamonds_12.png", i);
        }
        
        [TestMethod]
        public void TestValidCardUri5()
        {
            var i = UriHelper.GetCardUri(CardSuit.Clubs, 7);
            Assert.IsNotNull(i);
            Assert.AreEqual("ms-appx:/Assets/temp/Cards/Clubs/Clubs_07.png", i);
        }
        
        [TestMethod]
        public void TestValidCardUri6()
        {
            var i = UriHelper.GetCardUri(CardSuit.Clubs, 13);
            Assert.IsNotNull(i);
            Assert.AreEqual("ms-appx:/Assets/temp/Cards/Clubs/Clubs_13.png", i);
        }
        
        [TestMethod]
        public void TestValidCardUri7()
        {
            var i = UriHelper.GetCardUri(CardSuit.Spades, 8);
            Assert.IsNotNull(i);
            Assert.AreEqual("ms-appx:/Assets/temp/Cards/Spades/Spades_08.png", i);
        }

        [TestMethod]
        public void TestValidCardUri8()
        {
            var i = UriHelper.GetCardUri(CardSuit.Spades, 14);
            Assert.IsNotNull(i);
            Assert.AreEqual("ms-appx:/Assets/temp/Cards/Spades/Spades_14.png", i);
        }

        [TestMethod]
        public void TestInvalidCardUri2()
        {
            var i = UriHelper.GetCardUri(CardSuit.Hearts, 55);
            Assert.IsNotNull(i);
            Assert.AreEqual("ms-appx:/Assets/temp/Cards/Hearts/Hearts_55.png", i);
        }

        [TestMethod]
        public void TestInvalidCardUri()
        {
            var i = UriHelper.GetCardUri(CardSuit.Hearts, 10);
            Assert.IsNotNull(i);
            Assert.AreEqual("ms-appx:/Assets/temp/Cards/Hearts/Hearts_10.png", i);
        }
    }
}