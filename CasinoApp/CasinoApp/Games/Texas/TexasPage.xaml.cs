using System;
using System.Collections.Generic;
using LocalCasino.Common;
using LocalCasino.Common.Enums;
using LocalCasino.Common.Exceptions;
using LocalCasino.Common.Logic;
using LocalCasino.Utils;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace LocalCasino.Games.Texas
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class TexasPage : LayoutAwarePage
    {
        public TexasPage()
        {
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 4);
            _timer.Tick += TimerTick;

            _controller = new CasinoController(4);
            _controller.DealAndPlaceInitialBets();
            UpdateTextsOnScreen();

            DealerCard1.Source = ImageHelper.GetCardBackImage();
            DealerCard1.Visibility = Visibility.Collapsed;
            DealerCard2.Visibility = Visibility.Collapsed;
            DealerCard3.Visibility = Visibility.Collapsed;
            DealerCard4.Visibility = Visibility.Collapsed;
            DealerCard5.Visibility = Visibility.Collapsed;

            var cards1 = new[] { PlayerCard1, Cpu1Card1, Cpu2Card1, Cpu3Card1, Cpu4Card1 };
            var cards2 = new[] { PlayerCard2, Cpu1Card2, Cpu2Card2, Cpu3Card2, Cpu4Card2 };

            for (int i = 0; i < _controller.TotalNbrOfPlayers; i++)
            {
                var pi = _controller.GetPlayer(i);
                cards1[i].Source = ImageHelper.GetCardImage(pi.Hand[0]);
                cards2[i].Source = ImageHelper.GetCardImage(pi.Hand[1]);
            }
        }

        private readonly CasinoController _controller;
        private readonly DispatcherTimer _timer;

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            CardPlayer p = _controller.CurrentPlayer;
            if (p.PlayerType == PlayerType.AI)
            {
                _timer.Start();
            }
            else if (p.PlayerType == PlayerType.Human)
            {
                // let human click
            }
            else
            {
                throw new CasinoException("Unknown player type");
            }
        }

        private void TimerTick(object sender, object e)
        {
            _timer.Stop();

            if (_controller.GameState == GameState.BettingOngoing)
            {
                CardPlayer p = _controller.CurrentPlayer;
                if (p.PlayerType == PlayerType.AI)
                {
                    _controller.CurrentPlayerPlays();
                    _timer.Start();
                }
                else if (p.PlayerType == PlayerType.Human && p.PlayerState == PlayerState.Active)
                {
                    // let human click
                }
                else if (p.PlayerType == PlayerType.Human && p.PlayerState != PlayerState.Active)
                {
                    _controller.CurrentPlayerSkip();
                    _timer.Start();
                }
                else
                {
                    throw new CasinoException("Unknown player type");
                }
            }
            else if (_controller.GameState == GameState.BettingDone)
            {
                StartNewRound();                
            }

            UpdateTextsOnScreen();
        }

        private void ButtonCheckClick(object sender, RoutedEventArgs e)
        {
            _controller.CurrentPlayerChecks();
            UpdateTextsOnScreen();

            _timer.Start();
        }

        private void ButtonFoldClick(object sender, RoutedEventArgs e)
        {
            _controller.CurrentPlayerFolds();
            UpdateTextsOnScreen();

            _timer.Start();
        }

        private void ButtonBetClick(object sender, RoutedEventArgs e)
        {
            _controller.CurrentPlayerPlays();
            UpdateTextsOnScreen();

            _timer.Start();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void PlaceMarketImage(MarkerType marker)
        {
            if (_controller.CurrentPlayer.PlayerType != PlayerType.Human)
            {
                ShowMessage("Not your turn");
                return;
            }

            Image img;
            if (marker == MarkerType.Black)
            {
                img = imageBlack;
            }
            else if (marker == MarkerType.Green)
            {
                img = ImageGreen;
            }
            else if (marker == MarkerType.Blue)
            {
                img = ImageBlue;
            }
            else if (marker == MarkerType.Red)
            {
                img = ImageRed;
            }
            else
            {
                throw new CasinoException("Unsupported marker color");
            }

            var clone = new Image();
            GridBets.Children.Add(clone);

            clone.Stretch = img.Stretch;
            clone.Source = img.Source;
            clone.Width = img.Width;
            clone.Height = img.Height;
            clone.Tag = marker;
            clone.HorizontalAlignment = HorizontalAlignment.Left;
            clone.VerticalAlignment = VerticalAlignment.Top;
            clone.Tapped += CloneOnTapped;

            if (marker == MarkerType.Black)
            {
                clone.Margin = new Thickness(1 + 0 * 40, 1, 0, 0);
            }
            else if (marker == MarkerType.Green)
            {
                clone.Margin = new Thickness(1 + 1 * 40, 1, 0, 0);
            }
            else if (marker == MarkerType.Blue)
            {
                clone.Margin = new Thickness(1 + 2 * 40, 1, 0, 0);
            }
            else if (marker == MarkerType.Red)
            {
                clone.Margin = new Thickness(1 + 3 * 40, 1, 0, 0);
            }
            else
            {
                throw new CasinoException("Unknown marker type");
            }

            UpdateTextsOnScreen();
        }

        private void CloneOnTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            var img = sender as Image;
            if (img != null)
            {
                var value = (MarkerType)img.Tag;

                _controller.HumanPlayerUnBets(value);
                GridBets.Children.Remove(img);
                UpdateTextsOnScreen();
            }
        }

        private void ShowMessage(string messageFmt, params string[] args)
        {
            var d = new MessageDialog(string.Format(messageFmt, args), "Message from the casino");
            d.ShowAsync();
        }

        private void PlaceBetMarker(MarkerType marker)
        {
            var r = _controller.HumanPlayerBets(marker);
            if (r == ProblemType.Ok)
            {
                PlaceMarketImage(marker);
            }
            else if (r == ProblemType.NoMarkers)
            {
                ShowMessage("You do not have any more markers");
            }
            else if (r == ProblemType.NotYourTurn)
            {
                ShowMessage("It is  not your turn");
            }
        }

        private void ImgBlueTapped(object sender, TappedRoutedEventArgs e)
        {
            PlaceBetMarker(MarkerType.Blue);
        }

        private void ImgGreenTapped(object sender, TappedRoutedEventArgs e)
        {
            PlaceBetMarker(MarkerType.Green);
        }

        private void ImgRedTapped(object sender, TappedRoutedEventArgs e)
        {
            PlaceBetMarker(MarkerType.Red);
        }

        private void ImgBlackTapped(object sender, TappedRoutedEventArgs e)
        {
            PlaceBetMarker(MarkerType.Black);
        }

        private void ButtonNextPlayerClick(object sender, RoutedEventArgs e)
        {
            StartNewRound();
        }

        private void StartNewRound()
        {
            _controller.CollectAllBets();
            CollectAllMarkers();

            if (_controller.DealerPlays())
            {
                if (_controller.CurrentRound == GameRound.Flop)
                {
                    DealerCard1.Source = ImageHelper.GetCardImage(_controller.TableHand[0]);
                    DealerCard1.Visibility = Visibility.Visible;
                    DealerCard2.Source = ImageHelper.GetCardImage(_controller.TableHand[1]);
                    DealerCard2.Visibility = Visibility.Visible;
                    DealerCard3.Source = ImageHelper.GetCardImage(_controller.TableHand[2]);
                    DealerCard3.Visibility = Visibility.Visible;
                }
                else if (_controller.CurrentRound == GameRound.Turn)
                {
                    DealerCard4.Source = ImageHelper.GetCardImage(_controller.TableHand[3]);
                    DealerCard4.Visibility = Visibility.Visible;
                }
                else if (_controller.CurrentRound == GameRound.River)
                {
                    DealerCard5.Source = ImageHelper.GetCardImage(_controller.TableHand[4]);
                    DealerCard5.Visibility = Visibility.Visible;
                }
            }

            UpdateTextsOnScreen();
        }

        private void CollectAllMarkers()
        {
            IList<Image> toRemove = new List<Image>();
            foreach (var child in GridBets.Children)
            {
                if (child is Image)
                {
                    toRemove.Add(child as Image);
                }
            }

            foreach (var image in toRemove)
            {
                GridBets.Children.Remove(image);
            }
        }

        private void UpdateTextsOnScreen()
        {
            var p = _controller.CurrentPlayer;
            TextCurrentPlayer.Text = string.Format("Current Player: {0} ({1},{2})", p.Id, p.PlayerType, p.PlayerState);
            TextTotalBets.Text = string.Format("Your bet: ${0}", (_controller.GetPlayer(0)).CurrentBet);
            TextCurrentPot.Text = string.Format("Total pot of gold: ${0}", _controller.CurrentPot);

            var bets = new[] { TextTotalBets, TextPlayer1Bet, TextPlayer2Bet, TextPlayer3Bet, TextPlayer4Bet };
            var pots = new[] { TextTotalMarkers, TextPlayer1, TextPlayer2, TextPlayer3, TextPlayer4 };
            var recks = new[] { RectBetting, GridCpu1, GridCpu2, GridCpu3, GridCpu4 };
            var grids = new[] { GridBets, GridPlayer1, GridPlayer2, GridPlayer3, GridPlayer4 };

            ButtonDone.IsEnabled = false;
            ButtonCheck.IsEnabled = false;
            ButtonFold.IsEnabled = false;
            ButtonCheck.Visibility = Visibility.Collapsed;
            ButtonFold.Visibility = Visibility.Collapsed;

            if (_controller.GameState == GameState.GameOver)
            {
                ButtonNext.Content = string.Format("Game Over");
            }
            else
            {
                ButtonNext.Content = string.Format("Current {0}", _controller.CurrentRound);
            }

            if (_controller.CurrentRound != GameRound.River)
            {
                ButtonNext.IsEnabled = true;
            }
            else
            {
                ButtonNext.IsEnabled = false;
            }

            for (int i = 0; i < _controller.TotalNbrOfPlayers; i++)
            {
                if (i == p.Id)
                {
                    if (p.PlayerType == PlayerType.Human && p.Id == i && p.PlayerState == PlayerState.Active)
                    {
                        if (p.CurrentBet > 0 && _controller.IsPlayerRaising(i))
                        {
                            ButtonDone.Content = "Raise";
                            ButtonDone.IsEnabled = true;
                        }
                        else if (p.CurrentBet > 0 && _controller.IsPlayerCalling(i))
                        {
                            ButtonDone.Content = "Call";
                            ButtonDone.IsEnabled = true;
                        }

                        if (p.CurrentBet == 0 && _controller.CanPlayerCheck(i))
                        {
                            ButtonCheck.IsEnabled = true;
                        }
                        else
                        {
                            ButtonCheck.IsEnabled = false;
                        }

                        TextPlaceBetsHere.Text = "Place bets here";
                        ButtonFold.IsEnabled = true;
                        ButtonCheck.Visibility = Visibility.Visible;
                        ButtonFold.Visibility = Visibility.Visible;
                    }
                    else if (p.PlayerType == PlayerType.Human && p.PlayerState == PlayerState.Folded)
                    {
                        TextPlaceBetsHere.Text = "You have folded";
                    }

                    bets[i].Foreground = new SolidColorBrush(Colors.White);
                    pots[i].Foreground = new SolidColorBrush(Colors.White);
                    recks[i].BorderBrush = new SolidColorBrush(Colors.White);
                    recks[i].BorderThickness = new Thickness(4);
                    grids[i].Background = new SolidColorBrush(Colors.White) { Opacity = 0.1 };
                }
                else
                {
                    bets[i].Foreground = new SolidColorBrush(Colors.DarkGray);
                    pots[i].Foreground = new SolidColorBrush(Colors.DarkGray);
                    recks[i].BorderBrush = new SolidColorBrush(Colors.DarkGray);
                    recks[i].BorderThickness = new Thickness(3);
                    grids[i].Background = new SolidColorBrush(Colors.White) { Opacity = 0 };
                }

                if (_controller.GetPlayer(i).PlayerState == PlayerState.Active)
                {
                    bets[i].Text = string.Format("Bet: ${0} - Best: {1}", _controller.GetPlayer(i).CurrentBet, _controller.BestHand(i));
                }
                else
                {
                    bets[i].Text = string.Format("Bet: {0}", _controller.GetPlayer(i).PlayerState);
                }
                
                if (i == _controller.CurrentDealerId)
                {
                    pots[i].Text = string.Format("Player {0}*: ${1}", i, _controller.GetPlayer(i).NbrOfMarkers);                    
                }
                else
                {
                    pots[i].Text = string.Format("Player {0}: ${1}", i, _controller.GetPlayer(i).NbrOfMarkers);                    
                }
            }
        }
    }
}
