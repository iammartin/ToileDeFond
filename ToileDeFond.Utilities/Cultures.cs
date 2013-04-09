using System.Globalization;

namespace ToileDeFond.Utilities
{
    public static class Cultures
    {
        public const string FrenchCanadian = "fr-ca";
        public const string EnglishCanadian = "en-ca";
        public static readonly CultureInfo FrenchCanadianCulture = CultureInfo.GetCultureInfo(FrenchCanadian);
        public static readonly CultureInfo EnglishCanadianCulture = CultureInfo.GetCultureInfo(EnglishCanadian);
    }
}