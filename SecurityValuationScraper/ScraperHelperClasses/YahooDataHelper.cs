using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SecurityValuationScraper.BusinessObjects;

namespace SecurityValuationScraper.ScraperHelperClasses
{
    public class YahooDataHelper
    {
        public static string GetReportTypeValue(ReportType type)
        {
            switch (type)
            {
                case ReportType.FinancialStatement:
                    return "is";
                case ReportType.BalanceSheet:
                    return "bs";
                case ReportType.CashFlow:
                    return "cf";
                default:
                    return "";
            }
        }

        public static string GetReportFrequencyValue(ReportFrequency freq)
        {
            switch (freq)
            {
                case ReportFrequency.Quarterly:
                    return "";
                case ReportFrequency.Annual:
                    return "&annual";
                default:
                    return "";
            }
        }

        public static DateTime GetDate(string data, string format)
        {
            DateTime result = DateTime.ParseExact(data, format, null);
            return result;
        }

        public static string CleanData(string data)
        {
            return CleanData(data, false);
        }

        public static string CleanData(string data, bool removeSpaces)
        {
            if (data.IndexOf("spacer") > -1)
                return "";

            string cell = data.Replace("&nbsp;", string.Empty);
            cell = cell.Replace("<br>", string.Empty);
            cell = cell.Replace("<wbr>", string.Empty);
            cell = cell.Replace("\r", string.Empty);
            cell = cell.Replace("\n", string.Empty);
            cell = cell.Replace("\t", string.Empty);
            cell = cell.Replace("<span class=chr>-</span>", string.Empty);
            cell = cell.Replace("<span class=chr>", string.Empty).Replace("</span>", string.Empty);
            cell = cell.Replace("<span style=\"padding-left:18px;\">", string.Empty);
            cell = cell.Replace("<b>", string.Empty).Replace("</b>", string.Empty);
            cell = cell.Replace("<strong>", string.Empty).Replace("</strong>", string.Empty);
            //cell = cell.Replace("<spacer type=\"block\" width=\"5\" height=\"10\"/>", string.Empty);
            Match match = new Regex(@"<span id=""yfs_j10_\w+"">(?<Data>.*)", RegexOptions.IgnoreCase).Match(cell);
            if (match.Success)
                cell = match.Groups["Data"].Value;

            cell = cell.Trim();

            if (cell == "-") //dashed entry = 0
                cell = "0";

            if(removeSpaces)
                cell = cell.Replace(" ", "");

            return cell;
        }

        public static double FormatValue(string data)
        {
            if (data.IndexOf('T') > -1)
            {
                return double.Parse(data.Replace("T", string.Empty)) * 1000000000000;
            }
            else if (data.IndexOf('B') > -1)
            {
                return double.Parse(data.Replace("B",string.Empty)) * 1000000000;
            }
            else if (data.IndexOf('M') > -1)
            {
                return double.Parse(data.Replace("M", string.Empty)) * 1000000;
            }
            else if (data.IndexOf('K') > -1)
            {
                return double.Parse(data.Replace("K", string.Empty)) * 1000;
            }
            else if (data.IndexOf("N/A") > -1 || data.IndexOf("NaN") > -1)
            {
                return 0;
            }
            else if (data.IndexOf('%') > -1)
            {
                return double.Parse(data.Replace("%", string.Empty));
            }
            

            return double.Parse(data);
        }



        public static double ConvertValue(string value)
        {
            string result = value;
            result = result.Replace("$", string.Empty); //remove dollar sign
            Match isneg = Regex.Match(result, @"\((?<Value>\d+)\)"); //remove parathesis and negate the number
            if (isneg.Success)
                result = "-" + isneg.Groups["Value"].Value;

            return Convert.ToDouble(result);
        }
    }
}
