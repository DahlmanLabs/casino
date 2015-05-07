using System.Collections.Generic;
using System.Text;
using System.Linq;
using LocalCasino.Common.Constants;
using LocalCasino.Common.Enums;
using LocalCasino.Common.Exceptions;
using LocalCasino.Common.Utils;

namespace LocalCasino.Common.Logic
{
    public class CasinoController
    {
        private CardDeck _deck;
        private CardHand _tableHand;
        private const GameType _gameType = GameType.Texas;
        private GameRound _currentRound;
        private int _currentPlayerId;
        private int _currentDealerId;
        private int _currentPot;
        private readonly CardPlayer[] _players;

        public CasinoController(int nbrOfAi)
        {
            const int nbrOfHumans = 1;
            const int humanPlayerId = 0;

            if (nbrOfHumans + nbrOfAi < 4)
            {
                throw new CasinoException("A game must have at least 4 players!");
            }

            _currentRound = GameRound.Init;
            _currentDealerId = 0;

            // who starts when nbr of players is really low
            _currentPlayerId = 1;

            _tableHand = new CardHand(5);
            _deck = new CardDeck(1);
            _players = new CardPlayer[nbrOfHumans + nbrOfAi];

            _players[humanPlayerId] = new CardPlayer(0, PlayerType.Human, GameConstants.DefaultMarkerAmount);

            for (int i = 1; i < nbrOfHumans + nbrOfAi; i++)
            {
                _players[i] = new CardPlayer(i, PlayerType.AI, GameConstants.DefaultMarkerAmount);
            }
        }

        public int TotalNbrOfPlayers { get { return _players.Length; } }

        public GameType GameType { get { return _gameType; } }

        public GameRound CurrentRound { get { return _currentRound; } }

        public int NbrOfCardsInDeck { get { return _deck.Count; } }

        public int CurrentDealerId { get { return _currentDealerId; } }

        public CardHand TableHand { get { return _tableHand; } }

        public CardPlayer CurrentPlayer { get { return GetPlayer(_currentPlayerId); } }

        public int CurrentPot { get { return _currentPot; } }

        public GameState GameState
        {
            get
            {
                int bet = CurrentPlayer.CurrentBet;

                int nbrOfActivePlayers = _players.Count(p => p.PlayerState == PlayerState.Active);
                int nbrOfActiveOtherPlayers = _players.Count(p => p.PlayerState == PlayerState.Active && p.CurrentBet != bet);

           //     if (_currentPlayerId != _currentDealerId)
           //     {
           //         return GameState.BettingOngoing;
           //     }

                if (nbrOfActivePlayers == 0 || nbrOfActivePlayers == 1)
                {
                    return GameState.GameOver;
                }

                if (nbrOfActiveOtherPlayers == 0)
                {
                    return GameState.BettingDone;
                }
                else
                {
                    return GameState.BettingOngoing;
                }
            }
        }

        public string DebugState
        {
            get
            {
                var sb = new StringBuilder();
                for (int i = 0; i < _players.Length; i++)
                {
                    CardPlayer p = _players[i];
                    sb.Append(string.Format("Player {0}: ", i));
                    sb.AppendLine(p.DebugState);
                }

                return sb.ToString();
            }
        }

        private bool Deal(GameRound round)
        {
            if (round == GameRound.Init)
            {
                for (int i = 0; i < _players.Length; i++)
                {
                    CardPlayer player = _players[i];
                    player.DealCards(_deck.DrawCards(2));
                }
            }
            else
            {
                throw new CasinoException("Too many rounds!");
            }

            return true;
        }

        // Dealer=0, Big=1, Small=2

        public bool DealAndPlaceInitialBets()
        {
            bool ok = Deal(GameRound.Init);
            DealerPlays();

            if (ok && TotalNbrOfPlayers > 0)
            {
                CardPlayer player1 = _players[1];
                player1.PlaceBet(GameConstants.SmallBlind);
                _currentPlayerId++;
                if (TotalNbrOfPlayers > 1)
                {
                    CardPlayer player2 = _players[2];
                    player2.PlaceBet(GameConstants.BigBlind);
                    _currentPlayerId++;
                }
            }

            return ok;
        }

        public bool DealerPlays()
        {
            if (_currentRound == GameRound.Init)
            {
                // No cards to deal for this round
                _currentRound++;
                return false;
            }
            else if (_currentRound == GameRound.PreFlop)
            {
                _tableHand.AddRange(_deck.DrawCards(3));
            }
            else if (_currentRound == GameRound.Flop)
            {
                _tableHand.AddRange(_deck.DrawCards(1));
            }
            else if (_currentRound == GameRound.Turn)
            {
                _tableHand.AddRange(_deck.DrawCards(1));
            }
            else
            {
                throw new CasinoException("Unknown dealer action!");
            }

            _currentRound++;
            return true;
        }

        private CardPlayer GetHighestBettingPlayer(int playerId)
        {
            int highestBet = 0;
            int highestBetter = 0;

            for (int i = 0; i < TotalNbrOfPlayers; i++)
            {
                var p = GetPlayer(i);
                if (p.CurrentBet > highestBet && p.PlayerState == PlayerState.Active && i != playerId)
                {
                    highestBet = p.CurrentBet;
                    highestBetter = i;
                }
            }

            return GetPlayer(highestBetter);
        }

        public bool CurrentPlayerPlays()
        {
            var current = CurrentPlayer;
            if (current.PlayerType == PlayerType.AI && current.PlayerState == PlayerState.Active)
            {
                CardPlayer leader = GetHighestBettingPlayer(current.Id);

                if (current.Hand[0].Value == current.Hand[1].Value && current.Hand[1].Value > 8) // high pair
                {
                    if (!current.PlaceBet(leader.CurrentBet - current.CurrentBet + GameConstants.DefaultBet)) // try call+raise
                    {
                        if (!current.PlaceBet(leader.CurrentBet - current.CurrentBet)) // try call
                        {
                            CurrentPlayerFolds(); // else all-in or fold
                            return true;
                        }
                    }
                }
                else if ((current.Hand[0].Value == current.Hand[1].Value && current.Hand[1].Value > 8) || (current.Hand[0].Value + current.Hand[1].Value > 25)) // low pair or good cards
                {
                    if (leader.CurrentBet == current.CurrentBet)
                    {
                        CurrentPlayerChecks(); // check id possible
                        return true;
                    }
                    else if (!current.PlaceBet(leader.CurrentBet - current.CurrentBet))
                    {
                        CurrentPlayerFolds(); // fold
                        return true;
                    }
                }
                else
                {
                    if (leader.CurrentBet == current.CurrentBet)
                    {
                        CurrentPlayerChecks(); // check if possible
                        return true;
                    }
                    else
                    {
                        CurrentPlayerFolds();
                        return true;
                    }
                }
            }
            else if (current.PlayerType == PlayerType.Human && current.PlayerState == PlayerState.Active)
            {
                // manual action
            }

            _currentPlayerId = (_currentPlayerId + 1) % TotalNbrOfPlayers;
            return true;
        }

        public bool CurrentPlayerChecks()
        {
            if (CurrentPlayer.PlayerType == PlayerType.AI && CurrentPlayer.PlayerState == PlayerState.Active)
            {
                CurrentPlayer.Check();
            }
            else if (CurrentPlayer.PlayerType == PlayerType.Human && CurrentPlayer.PlayerState == PlayerState.Active)
            {
                CurrentPlayer.Check();
            }

            _currentPlayerId = (_currentPlayerId + 1) % TotalNbrOfPlayers;
            return true;
        }

        public bool CurrentPlayerFolds()
        {
            if (CurrentPlayer.PlayerType == PlayerType.AI && CurrentPlayer.PlayerState == PlayerState.Active)
            {
                CurrentPlayer.Fold();
            }
            else if (CurrentPlayer.PlayerType == PlayerType.Human && CurrentPlayer.PlayerState == PlayerState.Active)
            {
                CurrentPlayer.Fold();
            }

            _currentPlayerId = (_currentPlayerId + 1) % TotalNbrOfPlayers;
            return true;
        }

        public bool CurrentPlayerSkip()
        {
            _currentPlayerId = (_currentPlayerId + 1) % TotalNbrOfPlayers;
            return true;
        }

        public CardPlayer GetPlayer(int playerId)
        {
            if (playerId < TotalNbrOfPlayers)
            {
                return _players[playerId];
            }
            else
            {
                throw new CasinoException("No such player");
            }
        }

        private int GetMarkerValue(MarkerType marker)
        {
            if (marker == MarkerType.Black)
            {
                return 1000;
            }
            else if (marker == MarkerType.Green)
            {
                return 250;
            }
            else if (marker == MarkerType.Blue)
            {
                return 100;
            }
            else if (marker == MarkerType.Red)
            {
                return 50;
            }
            else
            {
                throw new CasinoException("Unknown marker type");
            }
        }

        public ProblemType HumanPlayerBets(MarkerType marker)
        {
            if (CurrentPlayer.PlayerType != PlayerType.Human)
            {
                return ProblemType.NotYourTurn;
            }

            int markerValue = GetMarkerValue(marker);
            if (CurrentPlayer.NbrOfMarkers >= markerValue)
            {
                CurrentPlayer.PlaceBet(markerValue);
                return ProblemType.Ok;
            }
            else
            {
                return ProblemType.NoMarkers;
            }
        }

        public void HumanPlayerUnBets(MarkerType value)
        {
            CurrentPlayer.UnPlaceBet(GetMarkerValue(value));
        }

        public bool CanPlayerCheck(int playerId)
        {
            var h = GetHighestBettingPlayer(playerId);

            return playerId == h.Id;
        }

        public bool IsPlayerRaising(int playerId)
        {
            var p = GetPlayer(playerId);
            var h = GetHighestBettingPlayer(playerId);

            if (p.CurrentBet > h.CurrentBet)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsPlayerCalling(int playerId)
        {
            var p = GetPlayer(playerId);
            var h = GetHighestBettingPlayer(playerId);

            if (p.CurrentBet == h.CurrentBet)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CollectAllBets()
        {
            for (int i = 0; i < TotalNbrOfPlayers; i++)
            {
                var p = GetPlayer(i);
                _currentPot += p.CurrentBet;
                p.ZeroBet();
            }
        }

        public WinningHand BestHand(int playerId)
        {
            CardHand p = GetPlayer(playerId).Hand;
            CardHand h = _tableHand;

            List<Card> cards = new List<Card>(p.Count + h.Count);
            List<WinningHand> results = new List<WinningHand>(p.Count + h.Count);


            cards.AddRange(p);
            cards.AddRange(h);


            IEnumerable<IGrouping<int, Card>> vgroups = cards.GroupBy(x => x.Value).Where(y => y.Count() > 1).OrderByDescending(z => z.Count());
            IEnumerable<IGrouping<CardSuit, Card>> sgroups = cards.GroupBy(x => x.Suit).Where(y => y.Count() == 5).OrderByDescending(z => z.Count());

            var min = cards.Min(x => x.Value);
            var max = cards.Max(x => x.Value);

            IEnumerable<int> diff = Enumerable.Range(min, max - min).Except(cards.Select(x => x.Value));

            WinningHand lead = WinningHand.NoHand;

            if (sgroups.Count() == 1)
            {
                lead= WinningHand.Flush;
            }


            if (!diff.Any() && (max - min == 4))
            {
                if (lead == WinningHand.Flush)
                {
                    lead = WinningHand.StraightFlush;
                }
                else
                {
                    lead=WinningHand.Straight;
                }
            }

            if (lead == WinningHand.NoHand && !vgroups.Any())
            {
                lead = WinningHand.HighCard;
            }

            foreach (var vg in vgroups)
            {

                if (vg.Count() == 4)
                {
                    lead = WinningHand.FourOfAKind;
                }
                else if (vg.Count() == 3)
                {
                    lead = WinningHand.ThreeOfAKind;
                }
                else if (vg.Count() == 2)
                {
                    if (lead == WinningHand.FourOfAKind)
                    {
                        lead = WinningHand.FourOfAKind;
                    }
                    else if (lead == WinningHand.ThreeOfAKind)
                    {
                        lead = WinningHand.FullHouse;
                    }
                    else if (lead == WinningHand.Pair)
                    {
                        lead = WinningHand.TwoPairs;
                    }
                    else
                    {
                        lead = WinningHand.Pair;
                    }
                }
            }



            return lead;
        }
    }
}