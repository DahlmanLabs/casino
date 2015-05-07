using LocalCasino.Common.Enums;

namespace LocalCasino.Common.Utils
{
    public static class UriHelper
    {
        public static string GetCardBackUri()
        {
            return "ms-appx:/Assets/temp/Cards/backside.png";
        }

        public static string GetCardUri(CardSuit suit, int value)
        {
            return string.Format("ms-appx:/Assets/temp/Cards/{0}/{0}_{1:00}.png", suit.ToString(), value);
        }
    }
}