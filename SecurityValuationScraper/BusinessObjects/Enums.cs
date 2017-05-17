using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityValuationScraper.BusinessObjects
{
    public enum ReportType
    {
        FinancialStatement,
        BalanceSheet,
        CashFlow,
    }

    public enum ReportFrequency
    {
        Quarterly = 3,
        Annual = 12
    }
}
