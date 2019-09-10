using System;

namespace KentWebApplication.Classes
{
    public static class Validation
    {
        public static bool IsNumeric(string value)
        {
            return double.TryParse(value, out double outValue);
        }

        public static bool IsDouble(string value)
        {
            return double.TryParse(value, out double outValue);
        }

        public static bool IsDate(string value)
        {   
            return DateTime.TryParse(value, out DateTime outValue);
        }
    }
}