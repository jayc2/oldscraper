using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecurityValuationScraper.BusinessObjects;
using SecurityValuationScraper.DataObjects;
using SecurityValuationScraper.DataObjects.SecurityValuationTableAdapters;

namespace SecurityValuationScraper.BusinessObjects
{
    public class KeyStatsController
    {
        public KeyStatsController()
        {

        }

        public Dictionary<string, LineItem> GetLineItemsAsDictionary()
        {
            Dictionary<string, LineItem> lineItems = new Dictionary<string, LineItem>();

            KeyStatsLineItemsTableAdapter ta = new KeyStatsLineItemsTableAdapter();
            SecurityValuation.KeyStatsLineItemsDataTable tblKSLineItems = ta.GetData();

            foreach (SecurityValuation.KeyStatsLineItemsRow row in tblKSLineItems)
            {
                LineItem i = new LineItem();
                i.ID = row.LineItemID;
                i.Name = row.LineItemName;
                i.TextMatch = row.LineItemHtmlTitle;

                lineItems.Add(i.TextMatch, i);
            }

            return lineItems;
        }

        public List<LineItem> GetLineItemsAsList()
        {
            List<LineItem> lineItems = new List<LineItem>();

            KeyStatsLineItemsTableAdapter ta = new KeyStatsLineItemsTableAdapter();
            SecurityValuation.KeyStatsLineItemsDataTable tblKSLineItems = ta.GetData();

            foreach (SecurityValuation.KeyStatsLineItemsRow row in tblKSLineItems)
            {
                LineItem i = new LineItem();
                i.ID = row.LineItemID;
                i.Name = row.LineItemName;
                i.TextMatch = row.LineItemHtmlTitle;

                lineItems.Add(i);
            }

            return lineItems;
        }

        public void AddData(long securityID, long lineItemID, DateTime timeStamp, double lineItemValue)
        {
            KeyStatsDataTableAdapter ta = new KeyStatsDataTableAdapter();
            ta.AddData(securityID, lineItemID, timeStamp, lineItemValue);
        }
    }
}
