using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SecurityValuationScraper.BusinessObjects
{
    public static class ExtractHelper
    {
        public static string CleanData(string data)
        {
            string cell = data.Replace("&nbsp;", string.Empty);
            cell = cell.Replace("<br>", string.Empty);
            cell = cell.Replace("<wbr>", string.Empty);
            cell = cell.Replace("\n", string.Empty);
            cell = cell.Replace("\t", string.Empty);
            cell = cell.Replace("<span class=chr>-</span>", string.Empty);
            cell = cell.Replace("<span class=chr>", string.Empty).Replace("</span>", string.Empty);
            cell = cell.Replace("<span style=\"padding-left:18px;\">", string.Empty);
            cell = cell.Replace("<b>", string.Empty).Replace("</b>", string.Empty);
            cell = cell.Trim();

            return cell;
        }


        public static Match GetFrequencyAndDate(string data)
        {
            Match result = new Regex(@"([0-9]?).*?([0-9]{4}-[0-9]{2}-[0-9]{2})", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(data);

            return result;
        }

        public static DateTime GetDate(string data, string format)
        {
            DateTime result = DateTime.ParseExact(data, format, null);
            return result;
        }

    }
}
