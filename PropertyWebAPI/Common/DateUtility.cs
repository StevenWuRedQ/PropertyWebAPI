using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PropertyWebAPI.Common
{
    public class DateUtility
    {
        public static bool ParseDateTime(string date, out DateTime outputDate)
        {
            if (date != ""
                &&
                DateTime.TryParseExact(date,
                                        "yyyyMMdd",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None, out outputDate))
            {
                return true;
            }

            
            outputDate = DateTime.MinValue;
            
            return false;
        }
    }
}