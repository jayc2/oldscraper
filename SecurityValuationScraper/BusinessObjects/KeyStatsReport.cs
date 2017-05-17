using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityValuationScraper.BusinessObjects
{
    public class KeyStatsReport
    {
        private DateTime _timeStamp;

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }
        private List<LineItem> _lineItems;

        public List<LineItem> LineItems
        {
            get { return _lineItems; }
            set { _lineItems = value; }
        }
    }
}
