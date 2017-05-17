using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SecurityValuationScraper.DataObjects;
using SecurityValuationScraper.DataObjects.SecurityValuationTableAdapters;

namespace SecurityValuationScraper.BusinessObjects
{
    public class SecurityController
    {
        SecuritiesTableAdapter ta;

        public SecurityController()
        {
            ta = new SecuritiesTableAdapter();
        }

        public Dictionary<string, Security> GetSecuritiesAsDictionary()
        {
            Dictionary<string, Security> securities = new Dictionary<string, Security>();

            SecuritiesTableAdapter ta = new SecuritiesTableAdapter();
            SecurityValuation.SecuritiesDataTable tblSecurities = ta.GetData();

            foreach (SecurityValuation.SecuritiesRow row in tblSecurities)
            {
                Security sec = new Security();
                sec.SecurityID = row.SecurityID;
                sec.CompanyName = row.Name;
                sec.Symbol = row.Symbol;
                sec.Exchange = row.Exchange;

                securities.Add(sec.Symbol, sec);
            }

            return securities;
        }

        public List<Security> GetSecuritiesAsList()
        {
            List<Security> securities = new List<Security>();

            SecuritiesTableAdapter ta = new SecuritiesTableAdapter();
            SecurityValuation.SecuritiesDataTable tblSecurities = ta.GetData();

            foreach (SecurityValuation.SecuritiesRow row in tblSecurities)
            {
                Security sec = new Security();
                sec.SecurityID = row.SecurityID;
                sec.CompanyName = row.Name;
                sec.Symbol = row.Symbol;
                sec.Exchange = row.Exchange;

                securities.Add(sec);
            }

            return securities;
        }

        public bool HasRequiredInfo(string symbol)
        {
            SecuritiesTableAdapter ta = new SecuritiesTableAdapter();
            SecurityValuation.SecuritiesDataTable dt = ta.GetDataBySymbol(symbol);

            if (dt.Rows.Count > 0)
            {
                return !string.IsNullOrEmpty(dt[0]["CompanyName"].ToString());
            }
            else
                return false;
        }

        public long Add(Security newsecurity)
        {
            return ta.Insert(newsecurity.CompanyName, newsecurity.Symbol, newsecurity.Exchange);
        }

        public bool Update(Security update, Security original)
        {

            ta.Update(update.CompanyName, update.Symbol, update.Exchange, original.SecurityID, original.CompanyName, original.Symbol, original.Exchange);
            return true;
        }

    }
}
