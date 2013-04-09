using System.Text.RegularExpressions;

namespace ToileDeFond.Utilities
{
    public static class RegularExpressions
    {
        public static readonly Regex Url = new Regex(@"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static readonly Regex HrefUrl = new Regex(@"href=""(?<url>.+?)""");
        public static readonly Regex Email = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
        public static readonly Regex Alpha = new Regex(@"^[a-zA-Z]+$");
        public static readonly Regex Numeric = new Regex(@"^[0-9]+$");
        public static readonly Regex AlphaNumeric = new Regex(@"^[a-zA-Z0-9]+$");
        public static Regex Guid = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);
    }
}