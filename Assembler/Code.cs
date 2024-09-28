using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    partial class Code
    {

        public string dest(string input)
        {   //3 bits return as a string
            string outBin = "";

            //Dictionary 라는 자료구조에서 Key로 Value를 찾는 방식 쓴다. 
            
            var destDiction = new Dictionary<string, string>()
            {
                { "null","000" },  { "M","001" },{ "D","010" },{ "DM","011" },
                { "A","100" },  { "AM","101" },{ "AD","110" },{ "ADM","111" }
            };
            outBin=destDiction[input];

            return outBin;
        }

        public string comp(string input)
        {   //7 bits return as a string
            string outBin = "";

            //Dictionary 라는 자료구조에서 Key로 Value를 찾는 방식 쓴다. 
           
            var compDiction = new Dictionary<string, string>()
            {
                { "0","0101010" },  { "1","0111111" },{ "-1","0111010" },{ "D","0001100" },{ "A","0110000" },{ "!D","0001101" },
                { "!A" ,"0110001" }, { "-D" ,"0001111" }, { "-A" ,"0110011" }, { "D+1" ,"0011111" }, { "A+1" ,"0110111" }, { "D-1" ,"0001110" },
                { "A-1" ,"0110010" }, { "D+A" ,"0000010" }, { "D-A" ,"0010011" }, { "A-D" ,"0000111" }, { "D&A" ,"0000000" }, { "D|A" ,"0010101" },
                { "M" ,"1110000" }, { "!M" ,"1110001" }, { "-M" ,"1110011" }, { "M+1" ,"1110111" }, { "M-1" ,"1110010" }, { "D+M" ,"1000010" },
                { "D-M" ,"1010011" }, { "M-D" ,"1000111" }, { "D&M" ,"1000000" }, { "D|M" ,"1010101" }
            };
            outBin = compDiction[input];

            return outBin;
        }
        public string jump(string input)
        {   //3 bits return as a string
            string outBin = "";

            //Dictionary 라는 자료구조에서 Key로 Value를 찾는 방식 쓴다. 

            var jumpDiction = new Dictionary<string, string>()
            {
                { "null","000" },  { "JGT","001" },{ "JEQ","010" },{ "JGE","011" },
                { "JLT","100" },  { "JNE","101" },{ "JLE","110" },{ "JMP","111" }
            };
            outBin = jumpDiction[input];

            return outBin;
        }
    }
}
