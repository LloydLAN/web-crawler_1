﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Data;
using System.IO;


namespace web_crawler_1
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("請輸入目標股號：");
            String tartgetStock = Console.ReadLine();
            Console.WriteLine();
            Get_Company_Info(tartgetStock);
            Console.WriteLine();
            Console.WriteLine("股利政策");
            Console.WriteLine();
            Get_Company_Dividend(tartgetStock);
            Console.WriteLine("按任意鍵結束....");
            Console.ReadKey();
        }

        // 抓取ID公司名稱與產業別
        static void Get_Company_Info(String strID)
        {
            HtmlDocument doc;   
            HtmlNodeCollection nodes;
            HtmlWeb web = new HtmlWeb();
            String CompanyName, CompanyIndustry;

            String XpathCompanyName = "/html/body/table[2]/tbody/tr/td[3]/table/tbody/tr[2]/td[3]/table[2]/tbody/tr[1]/td[2]";
            String XpathCompanyIndustry = "/html/body/table[2]/tbody/tr/td[3]/table/tbody/tr[2]/td[3]/table[2]/tbody/tr[2]/td[2]";

            String UrlCompanyInfo = "https://goodinfo.tw/StockInfo/StockDetail.asp?STOCK_ID=";

            web.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";    // chrome
            web.OverrideEncoding = Encoding.GetEncoding(65001); // UTF-8

            doc = web.Load(UrlCompanyInfo + strID);

            // 公司名稱
            nodes = doc.DocumentNode.SelectNodes(Regex.Replace(XpathCompanyName, "/tbody([[]\\d[]])?",""));     // Why?
            CompanyName = nodes[0].ChildNodes[0].InnerText;

            // 產業別
            nodes = doc.DocumentNode.SelectNodes(Regex.Replace(XpathCompanyIndustry, "/tbody([[]\\d[]])?", ""));
            CompanyIndustry = nodes[0].ChildNodes[0].InnerText;

            Console.WriteLine(CompanyName);
            Console.WriteLine(CompanyIndustry);
        }

        // 抓取股利政策
        static void Get_Company_Dividend(String strID)
        {
            // 建立股利政策 Talbe
            DataTable DividendTable = new DataTable("Dividend");

            // Define all the columns once.
            DataColumn[] cols ={
                                  new DataColumn("年度",typeof(String)),
                                  new DataColumn("現金股利(盈餘)",typeof(String)),
                                  new DataColumn("現金股利(公積)",typeof(String)),
                                  new DataColumn("現金股利(合計)",typeof(String)),
                                  new DataColumn("股票股利(盈餘)",typeof(String)),
                                  new DataColumn("股票股利(公積)",typeof(String)),
                                  new DataColumn("股票股利(合計)",typeof(String)),
                                  new DataColumn("股利合計",typeof(String)),
                                  new DataColumn("最高股價",typeof(String)),
                                  new DataColumn("最低股價",typeof(String)),
                                  new DataColumn("年均股價",typeof(String)),
                                  new DataColumn("現金殖利率",typeof(String)),
                                  new DataColumn("股票殖利率",typeof(String)),
                                  new DataColumn("合計殖利率",typeof(String)) 
                              };
            DividendTable.Columns.AddRange(cols);

            // 抓取股利政策
            HtmlDocument doc;
            HtmlNodeCollection nodes;
            HtmlWeb web = new HtmlWeb();

            web.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";    // chrome
            web.OverrideEncoding = Encoding.GetEncoding(65001); // UTF-8

            String UrlCompanyDividend = "https://goodinfo.tw/StockInfo/StockDividendPolicy.asp?STOCK_ID=";
            doc = web.Load(UrlCompanyDividend + strID);

            String CompanyDividendData;
            // 取得表格內容
            String XpathDividendData = "/html/body/table[2]/tbody/tr/td[3]/div[2]/div/div/table/tbody[1]/tr";
            nodes = doc.DocumentNode.SelectNodes(Regex.Replace(XpathDividendData, "/tbody([[]\\d[]])?", ""));
            //nodes = doc.DocumentNode.SelectNodes("//table");

            List<Object[]> rows = new List<object[]>();
            
            //Console.WriteLine("nodes.Count: " + nodes.Count);
            foreach (HtmlNode node in nodes)
            {

                List<HtmlNode> tds = new List<HtmlNode>();
                foreach (HtmlNode td in node.ChildNodes)
                {
                    //Console.WriteLine("td.Name: " + td.Name);
                    //Console.WriteLine("td.InnerText: " + td.InnerText);
                    if (td.Name == "td")
                        tds.Add(td); 
                }

                // Console.WriteLine("tds.Count: " + tds.Count);
                /*
                for (int i = 0; i < tds.Count; i++)
                {
                    if(tds[i].HasChildNodes == false)
                        CompanyDividendData = "0"; 
                    else
                        CompanyDividendData = tds[i].InnerText;
                    Console.WriteLine(CompanyDividendData);
                }
                */
                Object[] row = new Object[] {
                    tds[0].InnerText, tds[1].InnerText, tds[2].InnerText, tds[3].InnerText, tds[4].InnerText, tds[5].InnerText, tds[6].InnerText, tds[7].InnerText, // 
                    tds[13].InnerText, tds[14].InnerText, tds[15].InnerText, tds[16].InnerText, tds[17].InnerText, tds[18].InnerText };
                rows.Add(row);
            }
            foreach (Object[] row in rows)
            {
                DividendTable.Rows.Add(row);
            }

            TableSample.ShowTable(DividendTable);
            SaveToCSV(DividendTable, "E:\\side project\\" + strID + "-DividendTable.csv");
        }

        public static void SaveToCSV(DataTable oTable, string FilePath)
        {
            string data = "";
            StreamWriter wr = new StreamWriter(FilePath, false, System.Text.Encoding.UTF8);
            foreach (DataColumn column in oTable.Columns)
            {
                data += column.ColumnName + ",";
            }
            data += "\n";
            wr.Write(data);
            data = "";

            foreach (DataRow row in oTable.Rows)
            {
                foreach (DataColumn column in oTable.Columns)
                {
                    data += row[column].ToString().Trim() + ",";
                }
                data += "\n";
                wr.Write(data);
                data = "";
            }
            data += "\n";

            wr.Dispose();   // 釋放由 TextWriter 物件使用的所有資源。
            wr.Close();     // 關閉目前的 StreamWriter 物件和基礎資料流。
        }
    }
}
