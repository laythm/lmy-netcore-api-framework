using System;

namespace Common.Helpers
{
    public static class PhoneNumberHelper
    {
        public static string GetFormattedNumberIfNumber(string value)
        {
            if (value != null)
            {
                value = value.Trim();

                double result;
                var isNumber = Double.TryParse(value, out result);

                if (isNumber)
                {
                    if (value[0] == '0')
                    {
                        value = value.Substring(1, value.Length - 1);
                    }
                }
            }

            return value;
        }
    }
}
