using LocalCasino.Common;
using LocalCasino.Common.Enums;
using LocalCasino.Data;
using System;
using System.Collections.Generic;
using LocalCasino.Games.Texas;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace LocalCasino.Pages
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class GameDetailPage : LayoutAwarePage
    {
        public GameDetailPage()
        {
            InitializeComponent();
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
            // Allow saved page state to override the initial item to display
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var item = MenuDataSource.GetItem((String)navigationParameter);
            DefaultViewModel["Group"] = item.Group;
            DefaultViewModel["Items"] = item.Group.Items;
            flipView.SelectedItem = item;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var selectedItem = (MenuDataItem)flipView.SelectedItem;
            pageState["SelectedItem"] = selectedItem.UniqueId;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var mi = (MenuDataItem)flipView.SelectedItem;
            StartGameOfType(mi);
        }

        private void StartGameOfType(MenuDataItem item)
        {
            if (item != null)
            {
                GameType gameType;
                if (Enum.TryParse(item.UniqueId, out gameType))
                {
                    switch (gameType)
                    {
                        case GameType.Texas:
                            Frame.Navigate(typeof(TexasPage));
                            return;
                       
                    }
                }
            }
        }
    }
}
