using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SecurityValuationScraper.BusinessObjects;
using SecurityValuationScraper.DataObjects;
using SecurityValuationScraper.DataObjects.SecurityValuationTableAdapters;

namespace SecurityValuationScraper.BusinessObjects
{
    public class LineItemController
    {
        public LineItemController()
        {

        }

        public Dictionary<string, LineItem> GetLineItemsByReportType(ReportType type)
        {
            Dictionary<string, LineItem> lineItems = new Dictionary<string, LineItem>();

            switch (type)
            {
                case ReportType.FinancialStatement:
                {
                    FinancialStatementLineItemsTableAdapter ta = new FinancialStatementLineItemsTableAdapter();

                    SecurityValuation.FinancialStatementLineItemsDataTable tblFSLineItems = ta.GetData();

                    foreach (SecurityValuation.FinancialStatementLineItemsRow row in tblFSLineItems)
                    {
                        LineItem i = new LineItem();
                        i.ID = row.LineItemID;
                        i.Name = row.LineItemName;
                        i.TextMatch = row.LineItemHtmlTitle;

                        lineItems.Add(i.TextMatch, i);
                    }

                    break;
                }
                case ReportType.BalanceSheet:
                {
                    BalanceSheetLineItemsTableAdapter ta = new BalanceSheetLineItemsTableAdapter();

                    SecurityValuation.BalanceSheetLineItemsDataTable tblBSLineItems = ta.GetData();

                    foreach (SecurityValuation.BalanceSheetLineItemsRow row in tblBSLineItems)
                    {
                        LineItem i = new LineItem();
                        i.ID = row.LineItemID;
                        i.Name = row.LineItemName;
                        i.TextMatch = row.LineItemHtmlTitle;

                        lineItems.Add(i.TextMatch, i);
                    }

                    break;
                }
                case ReportType.CashFlow:
                {
                    CashFlowLineItemsTableAdapter ta = new CashFlowLineItemsTableAdapter();
                    SecurityValuation.CashFlowLineItemsDataTable tblCFLineItems = ta.GetData();

                    foreach (SecurityValuation.CashFlowLineItemsRow row in tblCFLineItems)
                    {
                        LineItem i = new LineItem();
                        i.ID = row.LineItemID;
                        i.Name = row.LineItemName;
                        i.TextMatch = row.LineItemHtmlTitle;

                        lineItems.Add(i.TextMatch, i);
                    }

                    break;
                }
                default:
                    break;
            }

            return lineItems;
        }

    }
}
