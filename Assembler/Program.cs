﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Assembler
{
    class Program
    {   /*
    아래 과정에서 Parser/Code/SymbolTable 의 기능을 잘 사용한다.

    <Initial>
    1.Prog.asm파일 열기 >> Parser 생성자
    2.Symbol table만들기 >> SymbolTable 생성자
    3.Symbol table에 predefined symbol 추가 >> SymbolTable.addEntry()

    <First Pass>
    1.프로그램 줄을 하나씩 읽는다. >> Parser.advance()
    2.레이블만 심볼 테이블에 작성한다.  >> SymbolTable.addEntry()

    <Second Pass>
    file의 맨 처음부터 다시 시작한다. 
    1.명령어 받고 파싱 >> SymbolTable.addEntry() / Code.dest,comp,jump() 
	    -파일 한 줄을 읽고 명령어만 구분한 후 
	    -@명령어는 <symbol, value>로 테이블에 담는다. 그리고 바로 2진 코드로 변환
	    -D명령어는 3개의 필드(dest, comp, jmp)를 바로 2진 코드로 변환한다
    2.이를 string으로 output file로 출력한다. >>파일 출력 

        */

        /*
                        <작업 현황>
                1.SymbolTable.cs 완성
                2.Parser.cs 의 dest,comp,jump()코드 다시 수정해야함
                3.Code.cs 완성 

                @2
        같은 거는 숫자로 값을 넣어야하는 것인데
        바로 16으로 가버리는게 문제이다.

        D=A같은 명령어에서는 완전히 틀렸다. 수정 요망
        */


        static void Main(string[] args)
        {   //<Initial>
            string outFile = "C:\\Users\\c\\source\\repos\\Assembler\\Assembler\\Add.hack";
            string inputFile = "C:\\Users\\c\\source\\repos\\Assembler\\Assembler\\Add.asm";
            Parser parser = new Parser(inputFile);//1. Open.asm file/stream
            SymbolTable tb = new SymbolTable();//2. Make Symbol Table
            Code cd= new Code(); 

            tb.addEntry("R0", 0); //3. Add Predefined Symbol into Table
            tb.addEntry("R1", 1);
            tb.addEntry("R2", 2);
            tb.addEntry("R3", 3);
            tb.addEntry("R4", 4);
            tb.addEntry("R5", 5);
            tb.addEntry("R6", 6);
            tb.addEntry("R7", 7);
            tb.addEntry("R8", 8);
            tb.addEntry("R9", 9);
            tb.addEntry("R10", 10);
            tb.addEntry("R11", 11);
            tb.addEntry("R12", 12);
            tb.addEntry("R13", 13);
            tb.addEntry("R14", 14);
            tb.addEntry("R15", 15);
            tb.addEntry("SCREEN", 16384);
            tb.addEntry("KBD", 24576);
            tb.addEntry("SP", 0);
            tb.addEntry("LCL", 1);
            tb.addEntry("ARG", 2);
            tb.addEntry("THIS", 3);
            tb.addEntry("THAT", 4);
            int varStr = 16; //변수의 value 시작 값.

            //tb.showContain("address", "0");
            //tb.showAll();

            //<First Pass>

            while (parser.hasMoreLines() == true)
            {
                parser.advance(); //1. Read program code one by one
                Console.WriteLine("Instruction Type : {0}  A:1, C:2, L:3, etc:-1", parser.instructionType());

                if (parser.instructionType() == 3) //2. Read only label and add it into Table
                {   //읽은 label이 5번째로 읽은 명령어이면 address는 4이다. ROM은 0부터 시작하므로.
                    int num = parser.effectCount - 1;
                    parser.effectCount = parser.effectCount - 1;

                    string labelName = parser.symbol();//Label 의 () 제거
                    tb.addEntry(labelName,num); //label 테이블에 추가
                   
                }

            }
            tb.showAll();


            //<Second Pass>

            Parser parser2 = new Parser(inputFile);
            StreamWriter progBin = new StreamWriter(outFile);

            while (parser2.hasMoreLines() == true)
            {
                parser2.advance();            // 1. Read program code one by one again
                Console.WriteLine("Instruction Type : {0}  A:1, C:2, L:3, etc:-1", parser.instructionType());
                string valName = "";

                if (parser2.instructionType() == 1) //A-명령어
                {
                    //1.@뺀다.
                    valName = parser2.symbol();

                    //2.테이블에 같은 이름이 없다면 넣는다. 같은 이름 없으면 무시
                    if (tb.contains("symbol", valName) == false)
                    {
                        tb.addEntry(valName, varStr);
                        varStr = varStr + 1;// 변수 추가했으니 주소 1증가
                    }
                    //2-2.테이블에 같은 이름이 있더라도 아래 3. 단계에서 값을 읽어들여 문자열로 변환할 것이다.
                    //3.테이블에 있는  해당 변수의 address를 2진수로 바꾸어서 문자열로 변환
                    int binVal = tb.getAddress(valName);//해당 변수의 address
                    string binInst = Convert.ToString(binVal, 2);
                    binInst=binInst.PadLeft(16, '0'); //왼쪽에 빈자리를 0으로 채워 16비트 맞춰주는 함수


                    //4.파일 출력
                    try
                    {
                        progBin.WriteLine(binInst);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception" + e.Message);
                    }
                    finally
                    {
                        Console.WriteLine("A-INNSTRUCTION : File Out Finally");

                    }


                }
                else if (parser2.instructionType() == 2) //C-명령어
                {   //1.C-명령어 파트별로 분할 후, 2진수 변환
                    string destPart = parser2.dest();// ex) D    in "D=M;JMP"
                    string compPart = parser2.comp();// ex) M    in "D=M;JMP"
                    string jumpPart = parser2.jump();// ex) JMP  in "D=M;JMP"

                    string destBin= cd.dest(destPart);// Binary String : 3 Bits  b000
                    string compBin= cd.comp(compPart);// Binary String : 7 Bits  b000_0000
                    string jumpBin= cd.jump(jumpPart);// Binary String : 3 Bits  b000

                    //2.출력할 문자열 정리
                    string binInst="111"+destBin+compBin+jumpBin;

                    //3. 파일출력

                    try
                    {
                        progBin.WriteLine(binInst);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception" + e.Message);
                    }
                    finally
                    {
                        Console.WriteLine("C-INNSTRUCTION : File Out Finally");

                    }


                }
                else if (parser2.instructionType() == 3) //(label)은 Table의 그 값이 저장되어 있고 @label 일때 값이 출력된다. 즉, (label)을 읽을 때, 출력할 것은 없다.
                {
                    
                    parser2.effectCount = parser2.effectCount - 1;

                }


            }
            progBin.Close();
            

        }
    }
}