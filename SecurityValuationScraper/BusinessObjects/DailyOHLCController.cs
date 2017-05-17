using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecurityValuationScraper.DataObjects.SecurityValuationTableAdapters;

namespace SecurityValuationScraper.BusinessObjects
{
    class DailyOHLCController
    {
        DailyOHLCDataTableAdapter ta;

        public DailyOHLCController()
        {
            ta = new DailyOHLCDataTableAdapter();
        }


        public void AddData(long secID, DateTime date, double? open, double? high, double? low, double? close, double? volume, double? adjclose)
        {
            ta.AddData(secID, date, open, high, low, close, volume, adjclose);
        }
    }
}
