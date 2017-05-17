using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

using SecurityValuationScraper.BusinessObjects;
using SecurityValuationScraper.DataObjects;
using SecurityValuationScraper.DataObjects.SecurityValuationTableAdapters;
using SecurityValuationScraper.ScraperHelperClasses;


namespace SecurityValuationScraper
{
    public class SecurityDownloadToken
    {
        public SecurityDownloadToken(string symbol, Dictionary<string, Security> securitiesList)
        {
            SecuritiesList = securitiesList;
            Symbol = symbol;
        }
        public Dictionary<string, Security> SecuritiesList;
        public string Symbol;
    }


    class Program
    {
        static void Main(string[] args)
        {
            //create dictionary object for info lookup
            Dictionary<string, Security> securitiesList = new SecurityController().GetSecuritiesAsDictionary();

            //create looping object for symbol list
            List<Security> securities = new SecurityController().GetSecuritiesAsList();

            //go go go
            for (int secCount = 0; secCount < securities.Count; secCount++)
            {

                //yahoo financial data scrap
                //yahoo balance sheets (quarterly)
                using (WebClient ybsScrap = new WebClient())
                {
                    ybsScrap.DownloadStringCompleted += new DownloadStringCompletedEventHandler(ybsScrap_DownloadStringCompleted);
                    ybsScrap.DownloadStringAsync(new Uri(string.Format("http://finance.yahoo.com/q/bs?s={0}", securities[secCount].Symbol)), new SecurityDownloadToken(securities[secCount].Symbol, securitiesList));
                }

                //yahoo income statements (quarterly)
                using (WebClient yisScrap = new WebClient())
                {
                    yisScrap.DownloadStringCompleted += new DownloadStringCompletedEventHandler(yisScrap_DownloadStringCompleted);
                    yisScrap.DownloadStringAsync(new Uri(string.Format("http://finance.yahoo.com/q/is?s={0}", securities[secCount].Symbol)), new SecurityDownloadToken(securities[secCount].Symbol, securitiesList));
                }

                //yahoo cash flow (quarterly)
                using (WebClient ycfScrap = new WebClient())
                {
                    ycfScrap.DownloadStringCompleted += new DownloadStringCompletedEventHandler(ycfScrap_DownloadStringCompleted);
                    ycfScrap.DownloadStringAsync(new Uri(string.Format("http://finance.yahoo.com/q/cf?s={0}", securities[secCount].Symbol)), new SecurityDownloadToken(securities[secCount].Symbol, securitiesList));
                }

                ////yahoo key statistics
                //using (WebClient yKeyStatsScrap = new WebClient())
                //{
                //    yKeyStatsScrap.DownloadStringCompleted += new DownloadStringCompletedEventHandler(yKeyStatsScrap_DownloadStringCompleted);
                //    yKeyStatsScrap.DownloadStringAsync(new Uri(string.Format("http://finance.yahoo.com/q/ks?s={0}", securities[secCount].Symbol)), new SecurityDownloadToken(securities[secCount].Symbol, securitiesList));
                //}

                //new scrap webclient...
                //historical closes in csv format
                //http://ichart.finance.yahoo.com/table.csv?s=XS&d=10&e=6&f=2008&g=d&a=0&b=1&c=1850&ignore=.csv 

            }

            Console.Read();
            
        }

        private static void YahooCashFlowExtract(string p, SecurityDownloadToken securityDownloadToken)
        {
            SecurityDownloadToken securityToken = securityDownloadToken;

            string content = p;
            Security security = new Security();

            Security hashedsec = new Security(securityToken.SecuritiesList[securityToken.Symbol]);

            if (!string.IsNullOrEmpty(hashedsec.CompanyName))
            {
                security = new Security(hashedsec);
            }
            else //update symbol
            {
                Console.WriteLine("Could not find symbol " + securityToken.Symbol + " for yahoo balance sheet");
            }

            Console.WriteLine(security.Symbol);

            //actual data
            ReportType currentType = ReportType.CashFlow;

            List<Report> reports = new List<Report>();

            //get line items
            Dictionary<string, LineItem> LineItemList = new LineItemController().GetLineItemsByReportType(currentType);

            string reportTypeDivMatchString = "<TABLE width=\"100%\" cellpadding=\"2\" cellspacing=\"0\" border=\"0\">(?<LineItems>.*?)</TABLE>";

            //get correct table
            Match datamatch = new Regex(reportTypeDivMatchString, RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(content);
            if (datamatch.Success)
            {
                //got the table we need to parse, skip to the rows
                MatchCollection rowmatches = new Regex(@"<tr.*?>(?<LineItem>.*?)</tr>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(datamatch.Groups["LineItems"].Value);
                for (int i = 0; i < rowmatches.Count; i++)
                {
                    if (rowmatches[i].Success)
                    {
                        //got each row
                        MatchCollection cellmatches = new Regex(@"<td.*?>(?<Data>.*?)</td>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(rowmatches[i].Groups["LineItem"].Value);

                        if (i == 0) //column headers
                        {
                            for (int x = 1; x < cellmatches.Count; x++) //only loop the date columns
                            {
                                string columndate = YahooDataHelper.CleanData(cellmatches[x].Groups["Data"].Value);
                                Report report = new Report();
                                report.ReportType = currentType;
                                report.ReportFrequency = (int)ReportFrequency.Quarterly;
                                report.ReportDate = YahooDataHelper.GetDate(columndate, "dd-MMM-yy");

                                reports.Add(report);
                            }
                        }
                        else //content
                        {
                            //yahoo uses <spacers> instead of cell padding to indent line items...
                            //detect a spacer and check cellmatches, if more then one use the second cell as the liTitle...

                            int liValueStart = 1;
                            if ((cellmatches[0].Groups["Data"].Value.IndexOf("spacer") > -1) && (cellmatches.Count > 1))
                                liValueStart = 2;

                            string liTitle = YahooDataHelper.CleanData(cellmatches[liValueStart-1].Groups["Data"].Value);
                            if (!string.IsNullOrEmpty(liTitle))
                            {
                                for (int x = liValueStart; x < cellmatches.Count; x++)
                                {
                                    string value = YahooDataHelper.CleanData(cellmatches[x].Groups["Data"].Value).Replace(",", string.Empty);

                                    if (!LineItemList.ContainsKey(liTitle))
                                    {
                                        Console.WriteLine("*****Line item " + liTitle + " not found*****");
                                    }
                                    else
                                    {
                                        LineItem liData = new LineItem(LineItemList[liTitle]);
                                        liData.Value = (!string.IsNullOrEmpty(value)) ? YahooDataHelper.ConvertValue(value) / 1000 : 0;

                                        reports[x - liValueStart].LineItems.Add(liData);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            security.Reports.AddRange(reports);

            //save completed security to database
            DataController datacontrol = new DataController();

            for (int rptcount = 0; rptcount < security.Reports.Count; rptcount++)
            {
                for (int itemcount = 0; itemcount < security.Reports[rptcount].LineItems.Count; itemcount++)
                {
                    datacontrol.AddData(security.SecurityID, security.Reports[rptcount].ReportType, security.Reports[rptcount].ReportFrequency, security.Reports[rptcount].ReportDate, security.Reports[rptcount].LineItems[itemcount].ID, security.Reports[rptcount].LineItems[itemcount].Value);
                }
            }
        }


        static void ycfScrap_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            SecurityDownloadToken securityToken = (SecurityDownloadToken)e.UserState;

            if (e.Error != null) //ERROR LOG AND RETURN
            {
                Console.WriteLine("Error getting yahoo cf page for symbol " + securityToken.Symbol);
                return;
            }

            string content = e.Result;
            Security security = new Security();

            Security hashedsec = new Security(securityToken.SecuritiesList[securityToken.Symbol]);

            if (!string.IsNullOrEmpty(hashedsec.CompanyName))
            {
                security = new Security(hashedsec);
            }
            else //update symbol
            {
                Console.WriteLine("Could not find symbol " + securityToken.Symbol + " for yahoo balance sheet");
            }

            Console.WriteLine("Parsing " + security.Symbol + " cash flow...");

            //actual data

            ReportType currentType = ReportType.CashFlow;

            List<Report> reports = new List<Report>();

            //get line items
            Dictionary<string, LineItem> LineItemList = new LineItemController().GetLineItemsByReportType(currentType);

            string reportTypeDivMatchString = "<TABLE width=\"100%\" cellpadding=\"2\" cellspacing=\"0\" border=\"0\">(?<LineItems>.*?)</TABLE>";

            //get correct table
            Match datamatch = new Regex(reportTypeDivMatchString, RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(content);
            if (datamatch.Success)
            {
                //got the table we need to parse, skip to the rows
                MatchCollection rowmatches = new Regex(@"<tr.*?>(?<LineItem>.*?)</tr>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(datamatch.Groups["LineItems"].Value);
                for (int i = 0; i < rowmatches.Count; i++)
                {
                    if (rowmatches[i].Success)
                    {
                        //got each row
                        MatchCollection cellmatches = new Regex(@"<td.*?>(?<Data>.*?)</td>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(rowmatches[i].Groups["LineItem"].Value);

                        if (i == 0) //column headers
                        {
                            for (int x = 1; x < cellmatches.Count; x++) //only loop the date columns
                            {
                                string columndate = YahooDataHelper.CleanData(cellmatches[x].Groups["Data"].Value);
                                Report report = new Report();
                                report.ReportType = currentType;
                                report.ReportFrequency = (int)ReportFrequency.Quarterly;
                                report.ReportDate = YahooDataHelper.GetDate(columndate, "d-MMM-yy");

                                reports.Add(report);
                            }
                        }
                        else //content
                        {
                            //yahoo uses <spacers> instead of cell padding to indent line items...
                            //detect a spacer and check cellmatches, if more then one use the second cell as the liTitle...

                            int liValueStart = 1;
                            if ((cellmatches[0].Groups["Data"].Value.IndexOf("spacer") > -1) && (cellmatches.Count > 1))
                                liValueStart = 2;

                            string liTitle = YahooDataHelper.CleanData(cellmatches[liValueStart - 1].Groups["Data"].Value);
                            if (!string.IsNullOrEmpty(liTitle))
                            {
                                for (int x = liValueStart; x < cellmatches.Count; x++)
                                {
                                    string value = YahooDataHelper.CleanData(cellmatches[x].Groups["Data"].Value).Replace(",", string.Empty);

                                    if (!LineItemList.ContainsKey(liTitle))
                                    {
                                        Console.WriteLine("*****Line item " + liTitle + " not found*****");
                                    }
                                    else
                                    {
                                        LineItem liData = new LineItem(LineItemList[liTitle]);
                                        liData.Value = (!string.IsNullOrEmpty(value)) ? YahooDataHelper.ConvertValue(value) / 1000 : 0;

                                        reports[x - liValueStart].LineItems.Add(liData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No cf data found for " + security.Symbol);
            }

            security.Reports.AddRange(reports);

            //save completed security to database
            DataController datacontrol = new DataController();

            for (int rptcount = 0; rptcount < security.Reports.Count; rptcount++)
            {
                for (int itemcount = 0; itemcount < security.Reports[rptcount].LineItems.Count; itemcount++)
                {
                    datacontrol.AddData(security.SecurityID, security.Reports[rptcount].ReportType, security.Reports[rptcount].ReportFrequency, security.Reports[rptcount].ReportDate, security.Reports[rptcount].LineItems[itemcount].ID, security.Reports[rptcount].LineItems[itemcount].Value);
                }
            }
        }


        static void yisScrap_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            SecurityDownloadToken securityToken = (SecurityDownloadToken)e.UserState;

            if (e.Error != null) //ERROR LOG AND RETURN
            {
                Console.WriteLine("Error getting yahoo is page for symbol " + securityToken.Symbol);
                return;
            }

            string content = e.Result;
            Security security = new Security();

            Security hashedsec = new Security(securityToken.SecuritiesList[securityToken.Symbol]);

            if (!string.IsNullOrEmpty(hashedsec.CompanyName))
            {
                security = new Security(hashedsec);
            }
            else //update symbol
            {
                Console.WriteLine("Could not find symbol " + securityToken.Symbol + " for yahoo balance sheet");
            }

            Console.WriteLine("Parsing " + security.Symbol + " income statement...");

            //actual data
            ReportType currentType = ReportType.FinancialStatement;

            List<Report> reports = new List<Report>();

            //get line items
            Dictionary<string, LineItem> LineItemList = new LineItemController().GetLineItemsByReportType(currentType);

            string reportTypeDivMatchString = "<TABLE width=\"100%\" cellpadding=\"2\" cellspacing=\"0\" border=\"0\">(?<LineItems>.*?)</TABLE>";

            //get correct table
            Match datamatch = new Regex(reportTypeDivMatchString, RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(content);
            if (datamatch.Success)
            {
                int numReports = 0;

                //got the table we need to parse, skip to the rows
                MatchCollection rowmatches = new Regex(@"<tr.*?>(?<LineItem>.*?)</tr>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(datamatch.Groups["LineItems"].Value);
                for (int i = 0; i < rowmatches.Count; i++)
                {
                    if (rowmatches[i].Success)
                    {
                        //got each row
                        MatchCollection cellmatches = new Regex(@"<td.*?>(?<Data>.*?)</td>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(rowmatches[i].Groups["LineItem"].Value);

                        if (i == 0) //column headers
                        {
                            for (int x = 1; x < cellmatches.Count; x++) //only loop the date columns
                            {
                                string columndate = YahooDataHelper.CleanData(cellmatches[x].Groups["Data"].Value);
                                if (!string.IsNullOrEmpty(columndate))
                                {
                                    Report report = new Report();
                                    report.ReportType = currentType;
                                    report.ReportFrequency = (int)ReportFrequency.Quarterly;
                                    report.ReportDate = YahooDataHelper.GetDate(columndate, "d-MMM-yy");

                                    numReports++; //if a company does not have dates in all headers it throws the loop off, this will insure that the columns read are correct.
                                    reports.Add(report);
                                }
                            }
                        }
                        else //content
                        {
                            //yahoo uses <spacers> instead of cell padding to indent line items...
                            //detect a spacer and check cellmatches, if more then one use the second cell as the liTitle...

                            int liValueStart = 1;
                            if ((cellmatches[0].Groups["Data"].Value.IndexOf("spacer") > -1) && (cellmatches.Count > 1))
                                liValueStart = 2;

                            string liTitle = YahooDataHelper.CleanData(cellmatches[liValueStart - 1].Groups["Data"].Value);
                            if (!string.IsNullOrEmpty(liTitle))
                            {
                                for (int x = liValueStart; x < cellmatches.Count; x++)
                                {
                                    string value = YahooDataHelper.CleanData(cellmatches[x].Groups["Data"].Value).Replace(",", string.Empty);

                                    if (!LineItemList.ContainsKey(liTitle))
                                    {
                                        Console.WriteLine("*****Line item " + liTitle + " not found*****");
                                    }
                                    else
                                    {
                                        LineItem liData = new LineItem(LineItemList[liTitle]);
                                        liData.Value = (!string.IsNullOrEmpty(value)) ? YahooDataHelper.ConvertValue(value) / 1000 : 0;

                                        reports[x - liValueStart].LineItems.Add(liData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No is data found for " + security.Symbol);
            }

            security.Reports.AddRange(reports);

            //save completed security to database
            DataController datacontrol = new DataController();

            for (int rptcount = 0; rptcount < security.Reports.Count; rptcount++)
            {
                for (int itemcount = 0; itemcount < security.Reports[rptcount].LineItems.Count; itemcount++)
                {
                    datacontrol.AddData(security.SecurityID, security.Reports[rptcount].ReportType, security.Reports[rptcount].ReportFrequency, security.Reports[rptcount].ReportDate, security.Reports[rptcount].LineItems[itemcount].ID, security.Reports[rptcount].LineItems[itemcount].Value);
                }
            }
        }


        static void ybsScrap_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            SecurityDownloadToken securityToken = (SecurityDownloadToken)e.UserState;

            if (e.Error != null) //ERROR LOG AND RETURN
            {
                Console.WriteLine("Error getting yahoo bs page for symbol " + securityToken.Symbol);
                return;
            }

            string content = e.Result;

            Security security = new Security();

            Security hashedsec = new Security(securityToken.SecuritiesList[securityToken.Symbol]);

            if (!string.IsNullOrEmpty(hashedsec.CompanyName))
            {
                security = new Security(hashedsec);
            }
            else //update symbol
            {
                Console.WriteLine("Could not find symbol " + securityToken.Symbol + " for yahoo balance sheet");
            }

            Console.WriteLine("Parsing " + security.Symbol + " balance sheet...");

            //actual data
            ReportType currentType = ReportType.BalanceSheet;

            List<Report> reports = new List<Report>();

            //get line items
            Dictionary<string, LineItem> LineItemList = new LineItemController().GetLineItemsByReportType(currentType);

            string reportTypeDivMatchString = "<TABLE width=\"100%\" cellpadding=\"2\" cellspacing=\"0\" border=\"0\">(?<LineItems>.*?)</TABLE>";

            //get correct table
            Match datamatch = new Regex(reportTypeDivMatchString, RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(content);
            if (datamatch.Success)
            {
                //got the table we need to parse, skip to the rows
                MatchCollection rowmatches = new Regex(@"<tr.*?>(?<LineItem>.*?)</tr>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(datamatch.Groups["LineItems"].Value);
                for (int i = 0; i < rowmatches.Count; i++)
                {
                    if (rowmatches[i].Success)
                    {
                        //got each row
                        //Console.WriteLine(matches[i].Groups[1].Value);

                        MatchCollection cellmatches = new Regex(@"<td.*?>(?<Data>.*?)</td>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(rowmatches[i].Groups["LineItem"].Value);

                        if (i == 0) //column headers
                        {
                            for (int x = 1; x < cellmatches.Count; x++) //only loop the date columns
                            {
                                string columndate = YahooDataHelper.CleanData(cellmatches[x].Groups["Data"].Value);
                                Report report = new Report();
                                report.ReportType = currentType;
                                report.ReportFrequency = (int)ReportFrequency.Quarterly;
                                report.ReportDate = YahooDataHelper.GetDate(columndate, "d-MMM-yy");

                                reports.Add(report);
                            }
                        }
                        else //content
                        {
                            //yahoo uses <spacers> instead of cell padding to indent line items...
                            //detect a spacer and check cellmatches, if more then one use the second cell as the liTitle...

                            int liValueStart = 1;
                            if ((cellmatches[0].Groups["Data"].Value.IndexOf("spacer") > -1) && (cellmatches.Count > 1))
                                liValueStart = 2;

                            string liTitle = YahooDataHelper.CleanData(cellmatches[liValueStart - 1].Groups["Data"].Value);
                            if (!string.IsNullOrEmpty(liTitle))
                            {
                                for (int x = liValueStart; x < cellmatches.Count; x++)
                                {
                                    string value = YahooDataHelper.CleanData(cellmatches[x].Groups["Data"].Value).Replace(",", string.Empty);

                                    if (!LineItemList.ContainsKey(liTitle))
                                    {
                                        Console.WriteLine("*****Line item '" + liTitle + "' not found for symbol " + security.Symbol + "*****");
                                    }
                                    else
                                    {
                                        LineItem liData = new LineItem(LineItemList[liTitle]);
                                        liData.Value = (!string.IsNullOrEmpty(value)) ? YahooDataHelper.ConvertValue(value) / 1000 : 0;

                                        reports[x - liValueStart].LineItems.Add(liData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No bs data found for " + security.Symbol);
            }

            security.Reports.AddRange(reports);

            //save completed security to database
            DataController datacontrol = new DataController();

            for (int rptcount = 0; rptcount < security.Reports.Count; rptcount++)
            {
                for (int itemcount = 0; itemcount < security.Reports[rptcount].LineItems.Count; itemcount++)
                {
                    datacontrol.AddData(security.SecurityID, security.Reports[rptcount].ReportType, security.Reports[rptcount].ReportFrequency, security.Reports[rptcount].ReportDate, security.Reports[rptcount].LineItems[itemcount].ID, security.Reports[rptcount].LineItems[itemcount].Value);
                }
            }
        }


        static void yKeyStatsScrap_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            SecurityDownloadToken securityToken = (SecurityDownloadToken)e.UserState;

            if (e.Error != null) //ERROR LOG AND RETURN
            {
                Console.WriteLine("Error getting yahoo page for symbol " + securityToken.Symbol);
                return;
            }

            string content = e.Result;

            if (content.IndexOf("There is no  data available for") > -1)
            {
                Console.WriteLine("No data found in yahoo for symbol " + securityToken.Symbol);
                return;
            }
            else if (content.IndexOf("Get Quotes Results") > -1)
            {
                Console.WriteLine("Multiple matches found in yahoo for symbol " + securityToken.Symbol);
                return;
            }

            Security security;

            if (!securityToken.SecuritiesList.ContainsKey(securityToken.Symbol))    //ERROR!!!
            {
                Console.WriteLine("Could not find symbol " + securityToken.Symbol);
                return;
            }
            else
                security = new Security(securityToken.SecuritiesList[securityToken.Symbol]);

            //get current price
            Match pricematch = new Regex("<td align=\"right\" valign=\"bottom\" nowrap>.*?<big><b>(?<Price>.+?)</b></big>", RegexOptions.IgnoreCase | RegexOptions.Multiline).Match(content);
            if (pricematch.Success)
            {
                double lastprice = double.Parse(pricematch.Groups["Price"].Value);
                DailyOHLCController dohlcController = new DailyOHLCController();
                dohlcController.AddData(security.SecurityID, DateTime.Today, null, null, null, lastprice, null, null);
            }
            else
            {
                Console.WriteLine("Could not find last price for symbol " + security.Symbol);
            }

            List<LineItem> listItems = new KeyStatsController().GetLineItemsAsList();
            KeyStatsController controller = new KeyStatsController();

            foreach (LineItem item in listItems)
            {
                string matchstr = string.Format("<tr><td class=\"yfnc_tablehead1\" width=\"75%\">{0}.*?</td><td class=\"yfnc_tabledata1\">(.*?)</td></tr>", item.TextMatch);
                Match dataMatch = new Regex(matchstr, RegexOptions.Singleline).Match(content);
                if (dataMatch.Success)
                {
                    double value = YahooDataHelper.FormatValue(YahooDataHelper.CleanData(dataMatch.Groups[1].Value));
                    controller.AddData(security.SecurityID, item.ID, DateTime.Today, value);
                }
                else
                {
                    Console.WriteLine("Could not find line item " + item.TextMatch + " for symbol " + security.Symbol);
                }
            }
        }
        
  

        static void googScrap_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            SecurityDownloadToken securityToken = (SecurityDownloadToken)e.UserState;

            if (e.Error != null) //ERROR LOG AND RETURN
            {
                Console.WriteLine("Error getting google page for symbol " + securityToken.Symbol);
                return;
            }

            string content = e.Result;
            Security security = new Security();

            //stock symbol, company name, exchange
            Match stockinfo = new Regex(@"<td colspan=2>.*?<h2>(.*?)</h2>(.*?)</td>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(content);
            if (stockinfo.Success)
            {

                string stocksym = ExtractHelper.CleanData(stockinfo.Groups[2].Value).Replace("(", string.Empty).Replace(")", string.Empty).Trim();
                string symbol = stocksym.Substring(stocksym.IndexOf(':') + 1);

                if (!securityToken.SecuritiesList.ContainsKey(symbol))    //ERROR!!!
                {
                    Console.Write("Could not find symbol " + securityToken.Symbol);
                    return;
                }

                Security hashedsec = new Security(securityToken.SecuritiesList[symbol]);

                if (string.IsNullOrEmpty(hashedsec.CompanyName))
                {
                    security = hashedsec;
                }
                else //update symbol
                {
                    security.Symbol = symbol;
                    security.CompanyName = ExtractHelper.CleanData(stockinfo.Groups[1].Value);
                    security.Exchange = stocksym.Substring(0, stocksym.IndexOf(':'));
                    security.SecurityID = hashedsec.SecurityID;
                    new SecurityController().Update(security, hashedsec);
                }
            }

            //actual data

            foreach (int reportTypeEnumVal in Enum.GetValues(typeof(ReportType)))
            {
                foreach (int reportFreqencyEnumVal in Enum.GetValues(typeof(ReportFrequency)))
                {
                    //detect report type
                    ReportType currentType = (ReportType)reportTypeEnumVal;
                    ReportFrequency currentFreq = (ReportFrequency)reportFreqencyEnumVal;

                    List<Report> reports = new List<Report>();

                    //get line items
                    Dictionary<string, LineItem> LineItemList = new LineItemController().GetLineItemsByReportType(currentType);

                    string reportTypeDivMatchString = string.Format(@"<div.*?id=""{0}{1}div"".*?>(.*?)</div>",
                        GoogleDataHelper.GetReportTypeValue(currentType),
                        GoogleDataHelper.GetReportFrequencyValue(currentFreq));



                    //get correct table
                    Match datamatch = new Regex(reportTypeDivMatchString, RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(content);
                    if (datamatch.Success)
                    {
                        //got the table we need to parse, skip to the rows
                        MatchCollection rowmatches = new Regex(@"<tr.*?>(.*?)</tr>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(datamatch.Groups[1].Value);
                        for (int i = 0; i < rowmatches.Count; i++)
                        {
                            if (rowmatches[i].Success)
                            {
                                //got each row
                                MatchCollection cellmatches = new Regex(@"<td.*?>(.*?)</td>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(rowmatches[i].Groups[1].Value);

                                if (i == 0) //column headers
                                {
                                    for (int x = 1; x < cellmatches.Count; x++) //only loop the date columns
                                    {
                                        string columndate = ExtractHelper.CleanData(cellmatches[x].Groups[1].Value);
                                        Match freqdate = ExtractHelper.GetFrequencyAndDate(columndate);
                                        Report report = new Report();
                                        report.ReportType = currentType;
                                        report.ReportFrequency = (int)currentFreq;
                                        report.ReportDate = Convert.ToDateTime(freqdate.Groups[2].Value);

                                        reports.Add(report);
                                    }
                                }
                                else //content
                                {
                                    string liTitle = ExtractHelper.CleanData(cellmatches[0].Groups[1].Value);
                                    if (!string.IsNullOrEmpty(liTitle))
                                    {
                                        for (int x = 1; x < cellmatches.Count; x++)
                                        {
                                            string value = ExtractHelper.CleanData(cellmatches[x].Groups[1].Value).Replace(",", string.Empty);

                                            if (!LineItemList.ContainsKey(liTitle))
                                            {
                                                Console.WriteLine("*****Line item " + liTitle + " not found*****");
                                            }
                                            else
                                            {
                                                LineItem liData = new LineItem(LineItemList[liTitle]);
                                                liData.Value = (!string.IsNullOrEmpty(value)) ? Convert.ToDouble(value) : 0;

                                                reports[x - 1].LineItems.Add(liData);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    security.Reports.AddRange(reports);


                }
            }

            //save completed security to database
            DataController datacontrol = new DataController();

            for (int rptcount = 0; rptcount < security.Reports.Count; rptcount++)
            {
                for (int itemcount = 0; itemcount < security.Reports[rptcount].LineItems.Count; itemcount++)
                {
                    datacontrol.AddData(security.SecurityID, security.Reports[rptcount].ReportType, security.Reports[rptcount].ReportFrequency, security.Reports[rptcount].ReportDate, security.Reports[rptcount].LineItems[itemcount].ID, security.Reports[rptcount].LineItems[itemcount].Value);
                }
            }


        }


    }
}
