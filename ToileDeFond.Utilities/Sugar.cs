using System;

namespace ToileDeFond.Utilities
{
    public static class Sugar
    {
        public static string GetBinPath()
        {
            return string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath) ? AppDomain.CurrentDomain.BaseDirectory : AppDomain.CurrentDomain.RelativeSearchPath;
        }
    }
}