using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SecurityValuationScraper.BusinessObjects;
using SecurityValuationScraper.DataObjects;
using SecurityValuationScraper.DataObjects.SecurityValuationTableAdapters;

namespace SecurityValuationScraper.BusinessObjects
{
    public class DataController
    {
        public bool AddData(long secid, ReportType reporttype, int rptfrequency, DateTime rptdate, long lineitemid, double value)
        {
            int result = -1;
            switch (reporttype)
            {
                case ReportType.FinancialStatement:
                    FinancialStatementDataTableAdapter taFS = new FinancialStatementDataTableAdapter();
                    //if (taFS.IsLineItemCreated(secid, lineitemid, rptfrequency, rptdate) > 0)
                    //{    //update
                    //    result = taFS.Update(value, secid, lineitemid, rptfrequency, rptdate);
                    //}
                    //else
                    //{
                    //    result = taFS.Insert(secid, lineitemid, rptfrequency, rptdate, value);
                    //}
                    result = taFS.AddData(secid, lineitemid, rptfrequency, rptdate, value);
                    break;
                case ReportType.BalanceSheet:
                    BalanceSheetDataTableAdapter taBS = new BalanceSheetDataTableAdapter();
                    //if (taBS.IsLineItemCreated(secid, lineitemid, rptfrequency, rptdate) > 0)
                    //{    //update
                    //    result = taBS.Update(value, secid, lineitemid, rptfrequency, rptdate);
                    //}
                    //else
                    //{
                    //    result = taBS.Insert(secid, lineitemid, rptfrequency, rptdate, value);
                    //}
                    result = taBS.AddData(secid, lineitemid, rptfrequency, rptdate, value);
                    break;
                case ReportType.CashFlow:
                    CashFlowDataTableAdapter taCF = new CashFlowDataTableAdapter();
                    //if (taCF.IsLineItemCreated(secid, lineitemid, rptfrequency, rptdate) > 0)
                    //{    //update
                    //    result = taCF.Update(value, secid, lineitemid, rptfrequency, rptdate);
                    //}
                    //else
                    //{
                    //    result = taCF.Insert(secid, lineitemid, rptfrequency, rptdate, value);
                    //}
                    result = taCF.AddData(secid, lineitemid, rptfrequency, rptdate, value);
                    break;
            }

            return true;
        }
    }
}
