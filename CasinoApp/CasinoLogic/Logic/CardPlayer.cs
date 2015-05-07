using LocalCasino.Common.Enums;
using LocalCasino.Common.Utils;

namespace LocalCasino.Common.Logic
{
    public class CardPlayer
    {
        private readonly PlayerType _playerType;
        private PlayerState _playerState;
        private int _currentBet;
        private int _nbrOfMarkers;
        private readonly int _id;
        private readonly CardHand _hand;

        public CardPlayer(int id, PlayerType playerType, int nbrOfMarker)
        {
            _id = id;
            _currentBet = 0;
            _nbrOfMarkers = nbrOfMarker;
            _playerType = playerType;
            _playerState = PlayerState.Active;
            _hand = new CardHand(2);
        }

        public int Id { get { return _id; } }

        public int CurrentBet { get { return _currentBet; } }

        public int NbrOfMarkers { get { return _nbrOfMarkers; } }

        public PlayerType PlayerType { get { return _playerType; } }

        public PlayerState PlayerState { get { return _playerState; } }

        public CardHand Hand { get { return _hand; } }

        public string DebugState
        {
            get { return string.Format("Markers: {0}, Hand: {1}, State: {2}, Type: {3}", _nbrOfMarkers, _hand.DebugState, _playerState, _playerType); }
        }

        public void DealCards(CardHand cardHand)
        {
            _hand.Clear();
            _hand.AddRange(cardHand);
        }

        public bool PlaceBet(int bet)
        {
            if (_nbrOfMarkers >= bet)
            {
                _nbrOfMarkers = _nbrOfMarkers - bet;
                _currentBet = _currentBet + bet;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UnPlaceBet(int bet)
        {
            _nbrOfMarkers = _nbrOfMarkers + bet;
            _currentBet = _currentBet - bet;
        }

        public void ZeroBet()
        {
            _currentBet = 0;
        }

        public void Check()
        {
        }

        public void Fold()
        {
            _playerState = PlayerState.Folded;
        }
    }
}