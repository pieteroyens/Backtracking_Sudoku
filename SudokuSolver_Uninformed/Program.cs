/* 
 * Program.cs
 * Deze klasse bevat de initialisatie van alle variabelen en klassen, en het algemene aansturen van het oplosproces.
 * Deze klasse houdt ook alles bij: of het gelukt is en alle data over de opgeloste borden
 */

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

class Program
{
    //Instellingen
    #region Instellingen
    //Stel de oplossingsmethode in: 1 is BT | 2 is BT+FC
    public static int solveMode = 2;
    //Kies het op te lossen tekstbestand
    public static string textFile = "25x25_1";
    //Stel de uitvouwmethode in: lr = links -> rechts | rl = rechts -> links | ds = domain size | mcv = Most-Constrained-Variable
    public static string searchMode = "mcv";
    #endregion

    //Member variables
    #region MemberVariables
    public static int boardSize;
    public static Search searcher;
    public static Stopwatch stopwatch;
    public static BoardParser parser;
    #endregion

    static void Main(string[] args)
    {
        //Initialisatie
        #region Initialisatie

        //Specificeer welk .txt bestand het programma moet uitlezen
        string readerFile = "../../../SudokuBoards/" + textFile + ".txt";
        StreamReader reader = new StreamReader(readerFile);

        //Start Stopwatch
        stopwatch = new Stopwatch();

        //Pak het onopgeloste bord
        parser = new BoardParser(reader);
        List<Board> unsolvedBoards = parser.getBoards();

        //Initialiseer de Search-klasse
        searcher = new Search();

        //Toon informatie over de manier van oplossen
        Console.WriteLine("Solving in {0} search mode now.", searchMode);
        Console.WriteLine("Solving in {0} solve mode now.", solveMode);

        #endregion

        //Start het oplossen, houdt bij of het gelukt is en sla de informatie op
        #region MainLoop
        foreach (Board board in unsolvedBoards)
        {
            Info.totalBoards++;
            Console.WriteLine("Unsolved Board {0}:", Info.totalBoards);
            parser.displayBoard(board);

            stopwatch.Start();
            Board solvedBoard = searcher.Solve(board, searchMode, solveMode);
            stopwatch.Stop();

            if (solvedBoard != null)
            {

                Console.WriteLine("Solved");
                Console.WriteLine();
                parser.displayBoard(solvedBoard);
                Info.solvedBoards++;
            }
            else
            {
                Console.WriteLine("Failed to solve board {0} in time.", Info.totalBoards);
            }


            Info.totalTime += stopwatch.ElapsedMilliseconds;
            Info.UpdateSolveTimeBoards(stopwatch.ElapsedMilliseconds);
            

            Info.totalRecursiveBTcalls += Info.recursiveBTcalls;
            Info.UpdateBTCalls(Info.recursiveBTcalls);
            Info.recursiveBTcalls = 0;

            stopwatch.Reset();
        }
        #endregion

        //Toon alle data over de opgeloste borden
        #region DisplayInfo
        Console.WriteLine("Done with solving all boards with search mode: {0}", searchMode);
        Console.WriteLine("Total time to solve {0} boards was: {1} milli seconds", Info.totalBoards, Info.totalTime);
        Console.WriteLine("The board that was solved the fastest was board {0}, in {1} milli seconds", Info.fastestSolvedBoard.Item1, Info.fastestSolvedBoard.Item2);
        Console.WriteLine("The board that was solved the slowest was board {0}, in {1} milli seconds", Info.slowestSolvedBoard.Item1, Info.slowestSolvedBoard.Item2);
        Console.WriteLine("The avrage time to solve a board was {0} milli seconds", Info.AvarageSolveTime());

        Console.WriteLine("Total Recursive BT calls: {0} ", Info.totalRecursiveBTcalls);
        Console.WriteLine("Least Recursive BT calls: {0} ", Info.leastAmountOfCalls);
        Console.WriteLine("Most Recursive BT calls: {0} ", Info.mostAmountOfCalls);
        Console.WriteLine("Average Recursive BT calls: {0} ", Info.totalRecursiveBTcalls / (ulong)Info.totalBoards);
        Console.WriteLine("Solved {0} out of {1} ", Info.solvedBoards, unsolvedBoards.Count);

        Console.ReadKey();

        #endregion
    }
}