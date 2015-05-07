using System;
using LocalCasino.Common.Utils;
using Windows.UI.Xaml.Media.Imaging;

namespace LocalCasino.Utils
{
    public static class ImageHelper
    {

        public static BitmapImage GetCardBackImage()
        {
            var bmImage = new BitmapImage(new Uri(UriHelper.GetCardBackUri()));
            return bmImage;
        }

        public static BitmapImage GetCardImage(Card card)
        {
            var bmImage = new BitmapImage(new Uri(UriHelper.GetCardUri(card.Suit, card.Value), UriKind.Absolute));
            return bmImage;
        }
    }
}