using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityValuationScraper.BusinessObjects
{
    public class KeyStatsLineItem
    {
        private long _lineItemID;

        public long ID
        {
            get { return _lineItemID; }
            set { _lineItemID = value; }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _textMatch;

        public string TextMatch
        {
            get { return _textMatch; }
            set { _textMatch = value; }
        }
        private double _value;

        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public KeyStatsLineItem()
        {
            
        }

        public KeyStatsLineItem(KeyStatsLineItem li)
        {
            this.ID = li.ID;
            this.Name = li.Name;
            this.TextMatch = li.TextMatch;
            this.Value = li.Value;
        }
    }
}
