using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LocalCasino.Common.Enums;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace LocalCasino.Data
{
    /// <summary>
    /// Base class for <see cref="MenuDataItem"/> and <see cref="MenuDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [WebHostHidden]
    public abstract class MenuDataCommon : LocalCasino.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public MenuDataCommon(string uniqueId, string title, string subtitle, string imagePath, string description)
        {
            _uniqueId = uniqueId;
            _title = title;
            _subtitle = subtitle;
            _description = description;
            _imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return _uniqueId; }
            set { SetProperty(ref _uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return _subtitle; }
            set { SetProperty(ref _subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (_image == null && _imagePath != null)
                {
                    _image = new BitmapImage(new Uri(MenuDataCommon._baseUri, _imagePath));
                }
                return _image;
            }

            set
            {
                _imagePath = null;
                this.SetProperty(ref _image, value);
            }
        }

        public void SetImage(string path)
        {
            _image = null;
            _imagePath = path;
            OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class MenuDataItem : MenuDataCommon
    {
        public MenuDataItem(string uniqueId, string title, string subtitle, string imagePath, string description, string content, MenuDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            _content = content;
            _group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        private MenuDataGroup _group;
        public MenuDataGroup Group
        {
            get { return _group; }
            set { SetProperty(ref _group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class MenuDataGroup : MenuDataCommon
    {
        public MenuDataGroup(string uniqueId, string title, string subtitle, string imagePath, string description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedGamesMainPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<MenuDataItem> _items = new ObservableCollection<MenuDataItem>();
        public ObservableCollection<MenuDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<MenuDataItem> _topItem = new ObservableCollection<MenuDataItem>();
        public ObservableCollection<MenuDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// MenuDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class MenuDataSource
    {
        private static readonly MenuDataSource _menuDataSource = new MenuDataSource();

        private readonly ObservableCollection<MenuDataGroup> _allGroups = new ObservableCollection<MenuDataGroup>();
        public ObservableCollection<MenuDataGroup> AllGroups
        {
            get { return _allGroups; }
        }

        public static IEnumerable<MenuDataGroup> GetGroups()
        {
            return _menuDataSource.AllGroups;
        }

        public static MenuDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _menuDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1)
            {
                return matches.First();
            }

            return null;
        }

        public static MenuDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _menuDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1)
            {
                return matches.First();
            }

            return null;
        }

        private MenuDataSource()
        {

            var group1 = new MenuDataGroup("CardGames",
                    "Card Games",
                    "Not including solitaires",
                    "Assets/temp/shutterstock-poker-hand-sq150.jpg",
                    "Come in and practice casino style card play");
            group1.Items.Add(new MenuDataItem(GameType.Texas.ToString(),
                    "Texas Hold'em",
                    "Play free with or without real friends",
                    "Assets/temp/shutterstock-poker-hand300x225.jpg",
                    "Play Texas Hold'em using standard european casino rules",
                    "A longer description that includes rules, winning hands, odds, or whatever. A longer description that includes rules, winning hands, odds, or whatever A longer description that includes rules, winning hands, odds, or whatever A longer description that includes rules, winning hands, odds, or whatever",
                    group1));
            group1.Items.Add(new MenuDataItem(GameType.BlackJack.ToString(),
                    "Black Jack",
                    "Try your luck with classic Black Jack.",
                    "Assets/MediumGray.png",
                    "Get 21 and beat the bank. Avoid dead mans hand",
                    "Blck Jack description",
                    group1));
            group1.Items.Add(new MenuDataItem("Baccarat",
                    "Baccarat",
                    "Play Baccarat like James Bond",
                    "Assets/DarkGray.png",
                    "Favoured by the rich and/or famous, loose much money quickly.",
                    "Baccarat, the game favoured by James Bond",
                    group1));
            AllGroups.Add(group1);

            var group2 = new MenuDataGroup("DiceGames",
                    "Dice Games",
                    "Collection of dice games",
                    "Assets/LightGray.png",
                    "Play craps and other variations of dice games");
            group2.Items.Add(new MenuDataItem("Craps",
                    "Craps",
                    "Most common dice game",
                    "Assets/temp/casino-marker300x225.png",
                    "Write something on craps and snake eyes etc.",
                    "Standard Craps",
                    group2));
           
            AllGroups.Add(group2);
        }
    }
}
