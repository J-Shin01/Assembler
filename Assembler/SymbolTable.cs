using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    partial class SymbolTable
    {
        DataTable symbolTable;
        public SymbolTable()
        {
            //SymbolTable 생성자
            //빈 symbolTable 생성
            symbolTable = new DataTable("symbolTb");
            symbolTable.Columns.Add("symbol", typeof(string));
            symbolTable.Columns.Add("address", typeof(int));
            //symbolTable.Rows.Add("xxx", 10);
        }
        public void addEntry(string symbol, int address)
        {
            //adds <symbol, address> to the table
            symbolTable.Rows.Add(symbol, address);
        }
        public Boolean contains(string item, string word)
        {
            //주어진 symbol이 symbolTable에 있는가?
            string str = null;
            string val = null;

            DataRow[] table = symbolTable.Select(item + " = '" + word + "'");



            foreach (DataRow row in table)
            {//DataRow는 되는데 DataRow[]는 ToString이 되는 것이 아니다.
                str = row["symbol"].ToString();
                val = row["address"].ToString();
                //Console.WriteLine(str+" "+val);
            }

            if (str == null)
            {
                Console.WriteLine("false");
                return false;
            }
            else
            {
                Console.WriteLine("true");
                return true;
            }



        }
        public int getAddress(string symbol)
        {
            //Returns the adress associated with the symbol     
            string val = null;

            DataRow[] table = symbolTable.Select("symbol = '" + symbol + "'");

            int outAddress = -1;
            foreach (DataRow row in table)
            {//DataRow는 되는데 DataRow[]는 ToString이 되는 것이 아니다.
                val = row["address"].ToString();
                outAddress = int.Parse(val);
                //Console.WriteLine(outAddress);

            }
            return outAddress;

        }
        public void showContain(string item, string word) {
            string str = null;
            string val = null;

            string stt = "< Showing a Part of Item >";
            Console.WriteLine($"{stt,9}");

            DataRow[] table = symbolTable.Select(item + " = '" + word + "'");



            foreach (DataRow row in table)
            {//DataRow는 되는데 DataRow[]는 ToString이 되는 것이 아니다.
                str = row["symbol"].ToString();
                val = row["address"].ToString();
                Console.WriteLine($"{str,9} | {val,-9}");
            }

        }
        public void showAll()
        {
            string str = null;
            string val = null;



            string stt = "< Showing All of Item >";
            Console.WriteLine($"{stt,9}");
            foreach (DataRow row in symbolTable.Rows)
            {//DataRow는 되는데 DataRow[]는 ToString이 되는 것이 아니다.
                str = row["symbol"].ToString();
                val = row["address"].ToString();
                Console.WriteLine($"{str,9} | {val,-9}");
            }

        }
    }
}
