using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    partial class Parser
    {
        public string labelName = "";
        public int labelVal = -1;
        private string line = "";//한 줄씩 읽은 후, 그 값을 저장시킬 변수
        private string result = "";//advance()에서 만든 공백등을 무시한 알짜 문자열
        private string pubResult = "";//변환할때나 접근하는 알짜 문자열
        private int instType = 0;
        private int progLength = 0; //프로그램 파일 줄 수
        private int lineCount = 0; //Parser의 객체 생성 후, 읽어들인 줄의 수
        public int effectCount = 0; //명령어의 ROM 주소; 필요없는 줄 다 지웠을 때 줄의 갯수
        private string fileAddr = "";//읽을 파일명

        private const int A_INSTRUCTION = 1;
        private const int C_INSTRUCTION = 2;
        private const int L_INSTRUCTION = 3;

        StreamReader progFile;


        public Parser(string codeFile)
        {
            //Parser 생성자
            //Opens the input file/stream and gets ready to parse it
            fileAddr = codeFile;
            progFile = new StreamReader(fileAddr);
            progLength = File.ReadAllLines(fileAddr).Length;


        }
        ~Parser() { //소멸자는 가비지 컬렉터가 알아서 처리해줌
            progFile.Close();
        }


        public void advance() // advance안에서 다른 메소드를 활용하는 방식이다.
        {//실질적으로 파일에서 한 줄씩 읽어들이고
         //Skips whitespace and comments
         //Reads the next instruction from the input, and makes it the current instruction
         //This method should be called only if hasMoreLines is true
         //Initially there is no current instruction

            line = "";
            result = "";
            pubResult = "";
            string readProg = "";

            readProg = progFile.ReadLine();
            lineCount = lineCount + 1; //Parser의 객체 생성 후, 읽어들인 줄의 수

            if (readProg == null) //null 일때
            {
                return;
            }
            else if (readProg == "") {
                return;
            }
            else
            {
                line = readProg; //null 값이면 여기서 함수 종료. 
            }            
            Console.Write("ReadLine  :"+line);
            Console.WriteLine("###");


            //공백무시 : Replace 함수 사용
            string strNoSpace = "";
            strNoSpace=line.Replace(" ", "");
            Console.Write("NoSpace   :"+strNoSpace);
            Console.WriteLine("###");




            //코멘트 무시 : Split 함수로 구분자 기준 분할.
            string[] strArr =strNoSpace.Split('/');

            if (strArr[0] == null)
            {
                return;

            }
            else if (strArr[0] == "")
            {
                return;

            }
            else {
                result = strArr[0];//알짜문자열
                pubResult = result;
                effectCount = effectCount + 1;//실질적인 명령어 줄의 갯수

                Console.Write("NoComment :" + result);
                Console.WriteLine("###");
                Console.WriteLine("Effective Instruction Length : " + effectCount);



            }


        }
        public Boolean hasMoreLines()
        {
            //Are there more lines in the input?

            if (lineCount < progLength) // 프로그램 덜 읽었다.
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public int instructionType()
        {   
            //리턴값 : A_INSTRUCTION : 1, C_INSTRUCTION : 2, L_INSTRUCTION : 3, 그 외 : -1
            //A_INS... 1 :@xxx
            //C_INS... 2 :dest=comp;jump
            //L_INS... 3 :(xxx)


            instType = 0;
            if (result.Contains("@")) { //A명령어이면 @
                //D=A
                //M=D
                //0;JMP
                //dest=comp;jmp
                
                
                instType = A_INSTRUCTION;
                return instType;

            }
            else if (result.Contains("=") || result.Contains(";")) {//C 명령어 이면 등호 혹은 세미콜론
                instType = C_INSTRUCTION;
                return instType;

            }
            else if (result.Contains("(") && result.Contains(")")) {//Label이면
                instType = L_INSTRUCTION;
                return instType;

            }
            else {

                return -1;
            }


        }
        public string symbol()
        {
            //If the current instruction is (xxx), returns the symbol xxx.
            //If the current instruction is @xxx, returns the symbol or decimal xxx (as a string)
            //Should me called only if instructionType is A_INSTRUCTION or L_INSTRUCTION

            string outVal = pubResult.Replace("(", "");
            outVal = outVal.Replace(")", "");
            outVal = outVal.Replace("@", "");

            return outVal;
        }
        public string dest()
        {   //Returns the symbolic dest part of the cuurent Instruction
            //Should be called only if instructionType is C_INSTRUCTION
            // D=M;JMP 생각해봐  JUMP는 COMP를 조건으로 한다.  D;JMP의 D는 COMP다. 
            //DEST값이 없을 수 도 있다. null도 있으니까.

            string outDest = "";
            //ex) D;JMP     >> ;만 있을때, null
            //ex) D=D+1     >> =만 있을때, =로 구분
            //ex) D=D+1;JMP >> 둘 다 있을때, =로 구분
            if (pubResult.Contains("=")) {//=가 있을 때
                string[] strArr = pubResult.Split('=');
                outDest = strArr[0];
            }
            else if (pubResult.Contains(";")) { //;만 있을떄 

                outDest = "null";
            }

       

      
            return outDest;
        }

        public string comp()
        {   //Returns the symbolic comp part of the cuurent Instruction
            //Should be called only if instructionType is C_INSTRUCTION

            string outComp = "";
            //ex) D;JMP     >> ;만 있을때, ;로 구분
            //ex) D=D+1     >> =만 있을때, =로 구분
            //ex) D=D+1;JMP >> 둘 다 있을때, =,;로 구분
            if (pubResult.Contains(";") && pubResult.Contains("="))// >> 둘 다 있을때, =,; 로 구분
            {
                string[] strArr = pubResult.Split(';');
                string outComp2 = strArr[0];
                string[] strArr2 = outComp2.Split('=');
                outComp = strArr2[1];
            }
            else if (pubResult.Contains(";")) // >> ; 만 있을때, ; 로 구분
            {
                string[] strArr = pubResult.Split(';');
                outComp = strArr[0];
            }
            else if (pubResult.Contains("="))// >> =만 있을때, =로 구분
            {
                string[] strArr = pubResult.Split('=');
                outComp = strArr[1];

            }


  
            return outComp;
        }
        public string jump()
        {   //Returns the symbolic jump part of the cuurent Instruction
            //Should be called only if instructionType is C_INSTRUCTION

            string outJump = "";
            //ex) D;JMP     >> ;만 있을때, ;로 구분
            //ex) D=D+1     >> =만 있을때, "null"
            //ex) D=D+1;JMP >> 둘 다 있을때, ;로 구분

            if (pubResult.Contains(";"))// ; 있을때
            {
                string[] strArr = pubResult.Split(';'); // D=M;JMP 생각해봐
                outJump = strArr[1];

            }
            else if (pubResult.Contains("="))//  >> = 만 있을때, "null"
            { 
                outJump = "null";
            
            
            }
   


            return outJump;
        }
    }
}
