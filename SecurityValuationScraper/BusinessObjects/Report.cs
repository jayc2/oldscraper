using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecurityValuationScraper.BusinessObjects
{
    public class Report
    {
        public Report()
        {
            _lineItems = new List<LineItem>();
        }

        private DateTime _reportDate;

        public DateTime ReportDate
        {
            get { return _reportDate; }
            set { _reportDate = value; }
        }
        private List<LineItem> _lineItems;

        internal List<LineItem> LineItems
        {
            get { return _lineItems; }
            set { _lineItems = value; }
        }

        private ReportType _reportType;

        public ReportType ReportType
        {
            get { return _reportType; }
            set { _reportType = value; }
        }

        private int _reportFrequency;

        public int ReportFrequency
        {
            get { return _reportFrequency; }
            set { _reportFrequency = value; }
        }
    }
}
