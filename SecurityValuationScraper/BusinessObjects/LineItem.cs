using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecurityValuationScraper.BusinessObjects
{
    public class LineItem
    {
        public LineItem(LineItem li)
        {
            this.ID = li.ID;
            this.Name = li.Name;
            this.TextMatch = li.TextMatch;
            this.Value = li.Value;
        }

        public LineItem()
        {
            
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

        private long _lineItemID;

        public long ID
        {
            get { return _lineItemID; }
            set { _lineItemID = value; }
        }

        private double _value;

        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }


        
    }
}
