using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecurityValuationScraper.BusinessObjects
{
    public class Security
    {

        public Security(Security sec)
            : this()
        {
            this.SecurityID = sec.SecurityID;
            this.CompanyName = sec.CompanyName;
            this.Symbol = sec.Symbol;            
            this.Exchange = sec.Exchange;
        }

        public Security()
        {
            _reports = new List<Report>();
            _epsReports = new List<EPSReport>();
            _ksReports = new List<KeyStatsReport>();

        }

        private long _securityID;

        public long SecurityID
        {
            get { return _securityID; }
            set { _securityID = value; }
        }
        private string _symbol;

        public string Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }
        private string _exchange;

        public string Exchange
        {
            get { return _exchange; }
            set { _exchange = value; }
        }
        private string _companyName;

        public string CompanyName
        {
            get { return _companyName; }
            set { _companyName = value; }
        }

        private List<Report> _reports;

        internal List<Report> Reports
        {
            get { return _reports; }
            set { _reports = value; }
        }

        private List<EPSReport> _epsReports;

        public List<EPSReport> EpsReports
        {
            get { return _epsReports; }
            set { _epsReports = value; }
        }

        private List<KeyStatsReport> _ksReports;

        public List<KeyStatsReport> KeyStatsReports
        {
            get { return _ksReports; }
            set { _ksReports = value; }
        }

        private DailyOHLC _dailyOHLC;

        public DailyOHLC DailyOHLC
        {
            get { return _dailyOHLC; }
            set { _dailyOHLC = value; }
        }
    }
}
