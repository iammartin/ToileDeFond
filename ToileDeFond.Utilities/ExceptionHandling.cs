using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToileDeFond.Utilities
{
    public static class ExceptionHandling
    {
        public static void IsNotNull(object value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}
