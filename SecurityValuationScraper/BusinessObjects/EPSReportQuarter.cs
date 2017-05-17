using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityValuationScraper
{
    public class EPSRQuarter
    {
        public EPSRQuarter(int quarter, double value)
        {
            _quarter = quarter;
            _value = value;
        }

        private int _quarter;

        public int Quarter
        {
            get { return _quarter; }
            set { _quarter = value; }
        }
        private double _value;

        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
