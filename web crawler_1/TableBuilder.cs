using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace web_crawler_1
{
    class TableBuilder
    {
        public TableBuilder()
        {

        }

        public static DataTable CreateTable(String TableName)
        {
            DataTable newTable = new DataTable(TableName);
            return newTable;
        }

        public static void AddCol(DataTable targetTable, List<String> ColName_L)
        {
            foreach (String ColName in ColName_L)
            {
                DataColumn newCol = new DataColumn(ColName, typeof(System.Int32));
                targetTable.Columns.Add(newCol);
            }
        }

        public static void AddData(DataTable targetTable)
        {

        }

        public static void ShowTable(DataTable table)
        {
            foreach (DataColumn col in table.Columns)
            {
                Console.Write("{0,-14}", col.ColumnName);
            }
            Console.WriteLine();

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    if (col.DataType.Equals(typeof(DateTime)))
                        Console.Write("{0,-14:d}", row[col]);
                    else if (col.DataType.Equals(typeof(Decimal)))
                        Console.Write("{0,-14:C}", row[col]);
                    else
                        Console.Write("{0,-14}", row[col]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
