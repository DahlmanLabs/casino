using System.Diagnostics;
using LocalCasino.Common.Constants;
using LocalCasino.Common.Enums;
using LocalCasino.Common.Logic;
using LocalCasino.Common.Utils;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace LocalCasino.Common.Tests.Logic
{
    [TestClass]
    public class CasinoControllerTests
    {

        // a deck last for 22 persons: 5 table + 3 burn + 2 per person (22 people)

        [TestMethod]
        public void TestControllerTexasGame()
        {
            var cc = new CasinoController(3);
            Assert.AreEqual(4, cc.TotalNbrOfPlayers);
            Assert.AreEqual(GameType.Texas, cc.GameType);
            Assert.AreEqual(GameRound.Init, cc.CurrentRound);
            Assert.AreEqual(52, cc.NbrOfCardsInDeck);
            Assert.AreEqual(0, cc.CurrentDealerId);
            Assert.AreEqual(1, cc.CurrentPlayer.Id);

            bool enoughCards = cc.DealAndPlaceInitialBets();
            Assert.AreEqual(GameConstants.DefaultMarkerAmount, cc.GetPlayer(cc.CurrentDealerId).NbrOfMarkers); // dealer placed no blind
            Assert.AreEqual(GameConstants.DefaultMarkerAmount, cc.GetPlayer(cc.CurrentPlayer.Id).NbrOfMarkers); // current (in this case last) player placed no blind
            Assert.AreEqual(GameConstants.DefaultMarkerAmount, cc.CurrentPlayer.NbrOfMarkers); // current (in this case last) player placed no blind
            Assert.AreEqual(0, cc.CurrentDealerId);
            Assert.AreEqual(3, cc.CurrentPlayer.Id);
            Assert.AreEqual(GameRound.PreFlop, cc.CurrentRound);

            Assert.AreEqual(true, enoughCards);
            Assert.AreEqual(52 - (4 * 2), cc.NbrOfCardsInDeck);

            for (int i = 3; i < cc.TotalNbrOfPlayers; i++)
            {
                Assert.AreEqual(i, cc.CurrentPlayer.Id);
                cc.CurrentPlayerPlays();
                Assert.AreEqual((i + 1) % cc.TotalNbrOfPlayers, cc.CurrentPlayer.Id);
            }

            Assert.AreEqual(GameRound.PreFlop, cc.CurrentRound);

            enoughCards = cc.DealerPlays();
            Assert.AreEqual(true, enoughCards);
            Assert.AreEqual(52 - (4 * 2 + 3), cc.NbrOfCardsInDeck);

            for (int i = 0; i < cc.TotalNbrOfPlayers; i++)
            {
                Assert.AreEqual(i, cc.CurrentPlayer.Id);
                cc.CurrentPlayerPlays();
                Assert.AreEqual((i + 1) % cc.TotalNbrOfPlayers, cc.CurrentPlayer.Id);
            }

            Assert.AreEqual(GameRound.Flop, cc.CurrentRound);

            enoughCards = cc.DealerPlays();
            Assert.AreEqual(true, enoughCards);
            Assert.AreEqual(52 - (4 * 2 + 3 + 1), cc.NbrOfCardsInDeck);

            for (int i = 0; i < cc.TotalNbrOfPlayers; i++)
            {
                Assert.AreEqual(i, cc.CurrentPlayer.Id);
                cc.CurrentPlayerPlays();
                Assert.AreEqual((i + 1) % cc.TotalNbrOfPlayers, cc.CurrentPlayer.Id);
            }

            Assert.AreEqual(GameRound.Turn, cc.CurrentRound);

            enoughCards = cc.DealerPlays();
            Assert.AreEqual(true, enoughCards);
            Assert.AreEqual(52 - (4 * 2 + 3 + 1 + 1), cc.NbrOfCardsInDeck);

            Debug.WriteLine(cc.DebugState);
        }

        [TestMethod]
        public void TestControllerTexasGameStraightFlush()
        {
            var cc = new CasinoController(3);
            Assert.AreEqual(4, cc.TotalNbrOfPlayers);
            Assert.AreEqual(GameType.Texas, cc.GameType);
            Assert.AreEqual(GameRound.Init, cc.CurrentRound);
            Assert.AreEqual(52, cc.NbrOfCardsInDeck);

            cc.TableHand.Add(new Card("Hearts_2"));
            cc.TableHand.Add(new Card("Hearts_3"));
            cc.TableHand.Add(new Card("Hearts_4"));

            cc.CurrentPlayer.Hand.Add(new Card("Hearts_5"));
            cc.CurrentPlayer.Hand.Add(new Card("Hearts_6"));

            var hand = cc.BestHand(cc.CurrentPlayer.Id);
            Assert.AreEqual(WinningHand.StraightFlush, hand);
        }

        [TestMethod]
        public void TestControllerTexasGameStraight()
        {
            var cc = new CasinoController(3);
            Assert.AreEqual(4, cc.TotalNbrOfPlayers);
            Assert.AreEqual(GameType.Texas, cc.GameType);
            Assert.AreEqual(GameRound.Init, cc.CurrentRound);
            Assert.AreEqual(52, cc.NbrOfCardsInDeck);

            cc.TableHand.Add(new Card("Hearts_2"));
            cc.TableHand.Add(new Card("Hearts_3"));
            cc.TableHand.Add(new Card("Diamonds_4"));

            cc.CurrentPlayer.Hand.Add(new Card("Spades_5"));
            cc.CurrentPlayer.Hand.Add(new Card("Clubs_6"));

            var hand = cc.BestHand(cc.CurrentPlayer.Id);
            Assert.AreEqual(WinningHand.Straight, hand);
        }

        [TestMethod]
        public void TestControllerTexasGameFullHouse()
        {
            var cc = new CasinoController(3);
            Assert.AreEqual(4, cc.TotalNbrOfPlayers);
            Assert.AreEqual(GameType.Texas, cc.GameType);
            Assert.AreEqual(GameRound.Init, cc.CurrentRound);
            Assert.AreEqual(52, cc.NbrOfCardsInDeck);

            cc.TableHand.Add(new Card("Hearts_2"));
            cc.TableHand.Add(new Card("Hearts_3"));
            cc.TableHand.Add(new Card("Diamonds_3"));

            cc.CurrentPlayer.Hand.Add(new Card("Spades_2"));
            cc.CurrentPlayer.Hand.Add(new Card("Clubs_2"));

            var hand = cc.BestHand(cc.CurrentPlayer.Id);
            Assert.AreEqual(WinningHand.FullHouse, hand);
        }

        [TestMethod]
        public void TestControllerTexasGameFlush()
        {
            var cc = new CasinoController(3);
            Assert.AreEqual(4, cc.TotalNbrOfPlayers);
            Assert.AreEqual(GameType.Texas, cc.GameType);
            Assert.AreEqual(GameRound.Init, cc.CurrentRound);
            Assert.AreEqual(52, cc.NbrOfCardsInDeck);

            cc.TableHand.Add(new Card("Hearts_2"));
            cc.TableHand.Add(new Card("Hearts_4"));
            cc.TableHand.Add(new Card("Hearts_5"));

            cc.CurrentPlayer.Hand.Add(new Card("Hearts_7"));
            cc.CurrentPlayer.Hand.Add(new Card("Hearts_8"));

            var hand = cc.BestHand(cc.CurrentPlayer.Id);
            Assert.AreEqual(WinningHand.Flush, hand);
        }

        [TestMethod]
        public void TestControllerTexasGameThreeOfKind()
        {
            var cc = new CasinoController(3);
            Assert.AreEqual(4, cc.TotalNbrOfPlayers);
            Assert.AreEqual(GameType.Texas, cc.GameType);
            Assert.AreEqual(GameRound.Init, cc.CurrentRound);
            Assert.AreEqual(52, cc.NbrOfCardsInDeck);

            cc.TableHand.Add(new Card("Hearts_2"));
            cc.TableHand.Add(new Card("Clubs_2"));
            cc.TableHand.Add(new Card("Diamonds_2"));

            cc.CurrentPlayer.Hand.Add(new Card("Hearts_9"));
            cc.CurrentPlayer.Hand.Add(new Card("Hearts_8"));

            var hand = cc.BestHand(cc.CurrentPlayer.Id);
            Assert.AreEqual(WinningHand.ThreeOfAKind, hand);
        }

        [TestMethod]
        public void TestControllerTexasGameTwoPairs()
        {
            var cc = new CasinoController(3);
            Assert.AreEqual(4, cc.TotalNbrOfPlayers);
            Assert.AreEqual(GameType.Texas, cc.GameType);
            Assert.AreEqual(GameRound.Init, cc.CurrentRound);
            Assert.AreEqual(52, cc.NbrOfCardsInDeck);

            cc.TableHand.Add(new Card("Hearts_2"));
            cc.TableHand.Add(new Card("Clubs_2"));
            cc.TableHand.Add(new Card("Diamonds_3"));

            cc.CurrentPlayer.Hand.Add(new Card("Hearts_3"));
            cc.CurrentPlayer.Hand.Add(new Card("Hearts_8"));

            var hand = cc.BestHand(cc.CurrentPlayer.Id);
            Assert.AreEqual(WinningHand.TwoPairs, hand);
        }

        [TestMethod]
        public void TestControllerTexasGameOnePairs()
        {
            var cc = new CasinoController(3);
            Assert.AreEqual(4, cc.TotalNbrOfPlayers);
            Assert.AreEqual(GameType.Texas, cc.GameType);
            Assert.AreEqual(GameRound.Init, cc.CurrentRound);
            Assert.AreEqual(52, cc.NbrOfCardsInDeck);

            cc.TableHand.Add(new Card("Hearts_2"));
            cc.TableHand.Add(new Card("Clubs_2"));
            cc.TableHand.Add(new Card("Diamonds_3"));

            cc.CurrentPlayer.Hand.Add(new Card("Hearts_10"));
            cc.CurrentPlayer.Hand.Add(new Card("Hearts_8"));

            var hand = cc.BestHand(cc.CurrentPlayer.Id);
            Assert.AreEqual(WinningHand.Pair, hand);
        }

        [TestMethod]
        public void TestControllerTexasGameHighCard()
        {
            var cc = new CasinoController(3);
            Assert.AreEqual(4, cc.TotalNbrOfPlayers);
            Assert.AreEqual(GameType.Texas, cc.GameType);
            Assert.AreEqual(GameRound.Init, cc.CurrentRound);
            Assert.AreEqual(52, cc.NbrOfCardsInDeck);

            cc.TableHand.Add(new Card("Hearts_2"));
            cc.TableHand.Add(new Card("Clubs_7"));
            cc.TableHand.Add(new Card("Diamonds_3"));

            cc.CurrentPlayer.Hand.Add(new Card("Hearts_12"));
            cc.CurrentPlayer.Hand.Add(new Card("Hearts_11"));

            var hand = cc.BestHand(cc.CurrentPlayer.Id);
            Assert.AreEqual(WinningHand.HighCard, hand);
        }

        [TestMethod]
        public void TestControllerTexasGamePair7CardHand()
        {
            var cc = new CasinoController(3);
            Assert.AreEqual(4, cc.TotalNbrOfPlayers);
            Assert.AreEqual(GameType.Texas, cc.GameType);
            Assert.AreEqual(GameRound.Init, cc.CurrentRound);
            Assert.AreEqual(52, cc.NbrOfCardsInDeck);

            cc.TableHand.Add(new Card("Hearts_2"));
            cc.TableHand.Add(new Card("Clubs_7"));
            cc.TableHand.Add(new Card("Diamonds_3"));
            cc.TableHand.Add(new Card("Clubs_3"));
            cc.TableHand.Add(new Card("Diamonds_4"));

            cc.CurrentPlayer.Hand.Add(new Card("Hearts_12"));
            cc.CurrentPlayer.Hand.Add(new Card("Hearts_11"));

            var hand = cc.BestHand(cc.CurrentPlayer.Id);
            Assert.AreEqual(WinningHand.Pair, hand);
        }
    }
}