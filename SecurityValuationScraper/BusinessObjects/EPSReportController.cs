using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SecurityValuationScraper.BusinessObjects;
using SecurityValuationScraper.DataObjects;
using SecurityValuationScraper.DataObjects.SecurityValuationTableAdapters;

namespace SecurityValuationScraper.BusinessObjects
{
    public class EPSReportController
    {
        DilutedEPSDataTableAdapter ta;

        public EPSReportController()
        {
            ta = new DilutedEPSDataTableAdapter();
        }


        public void AddData(long secID, int year, int quarter, double value)
        {
            ta.AddData(secID, year, quarter, value);
        }

    }
}
