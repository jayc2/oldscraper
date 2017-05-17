using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityValuationScraper.BusinessObjects
{
    public class DailyOHLC
    {
        private DateTime _date;

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }
        private float _open;

        public float Open
        {
            get { return _open; }
            set { _open = value; }
        }
        private float _high;

        public float High
        {
            get { return _high; }
            set { _high = value; }
        }
        private float _low;

        public float Low
        {
            get { return _low; }
            set { _low = value; }
        }
        private float _close;

        public float Close
        {
            get { return _close; }
            set { _close = value; }
        }
        private float _volume;

        public float Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }
        private float _adjustedClose;

        public float AdjustedClose
        {
            get { return _adjustedClose; }
            set { _adjustedClose = value; }
        }
    }
}
