using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SecurityValuationScraper.BusinessObjects;

namespace SecurityValuationScraper
{
    public class GoogleDataHelper
    {

        public static string GetReportTypeValue(ReportType type)
        {
            switch (type)
            {
                case ReportType.FinancialStatement:
                    return "inc";
                case ReportType.BalanceSheet:
                    return "bal";
                case ReportType.CashFlow:
                    return "cas";
                default:
                    return "";
            }
        }

        public static string GetReportFrequencyValue(ReportFrequency freq)
        {
            switch (freq)
            {
                case ReportFrequency.Quarterly:
                    return "interim";
                case ReportFrequency.Annual:
                    return "annual";
                default:
                    return "";
            }
        }

    }
}
