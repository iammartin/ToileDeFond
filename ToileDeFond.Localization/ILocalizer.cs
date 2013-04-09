using System.Globalization;

namespace ToileDeFond.Localization
{
    public interface ILocalizer
    {
        string GetLocalizedString(string resourneName, string groupeName = "global", CultureInfo culture = null);
    }
}