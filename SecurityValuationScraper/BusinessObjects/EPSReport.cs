using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecurityValuationScraper.BusinessObjects
{
    public class EPSReport
    {
        public EPSReport()
        {
            _quarters = new List<EPSRQuarter>();
        }

        private int _year;

        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }
        private List<EPSRQuarter> _quarters;

        public List<EPSRQuarter> Quarters
        {
            get { return _quarters; }
            set { _quarters = value; }
        }
    }
}
