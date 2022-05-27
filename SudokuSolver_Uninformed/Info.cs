/*
 * Alle informatie over het oplossen van borden wordt in deze klasse opgeslagen
 * en berekend.  
 */

using System;
using System.Collections.Generic;
using System.Numerics;

static class Info
{
    #region member variables
    public static long totalTime;
    public static long avrageTime;

    public static int solvedBoards;
    public static int totalBoards;

    public static Tuple<int, long> fastestSolvedBoard;
    public static Tuple<int, long> slowestSolvedBoard;

    public static ulong recursiveBTcalls;
    public static ulong leastAmountOfCalls;
    public static ulong mostAmountOfCalls;
    public static ulong totalRecursiveBTcalls;
    #endregion

    static Info()
    {
        solvedBoards = 0;
        totalBoards = 0;

        totalTime = 0;
        avrageTime = 0;
        fastestSolvedBoard = new Tuple<int, long>(0, 9999999);
        slowestSolvedBoard = new Tuple<int, long>(0, 0);
        

        recursiveBTcalls = 0;
        totalRecursiveBTcalls = 0;
        leastAmountOfCalls = 99999999;
        mostAmountOfCalls = 0;
    }

    // als een bord is opgelost update deze methode de oplostijden.
    // Als het bord sneller of langzamer is opgelost dan andere borden
    // wordt hiet bekeken.
    public static void UpdateSolveTimeBoards(long elapsedTime)
    {
        if(elapsedTime < fastestSolvedBoard.Item2)
        {
            fastestSolvedBoard = new Tuple<int, long>(totalBoards, elapsedTime);
        }
        if(elapsedTime > slowestSolvedBoard.Item2)
        {
            slowestSolvedBoard = new Tuple<int, long>(totalBoards, elapsedTime);
        }
    }

    public static long AvarageSolveTime()
    {
        return totalTime / totalBoards;
    }

    // update het aantal back tracking aanroepen ten op zichte 
    // van andere borden.
    public static void UpdateBTCalls(ulong calls)
    {
        if (calls < leastAmountOfCalls)
        {
            leastAmountOfCalls = calls;
        }
        if (calls > mostAmountOfCalls)
        {
            mostAmountOfCalls = calls;
        }
    }

    // berekent het totaal aantal mogelijkheden voor het invullen van een bord.
    // Het gebruik van BigInteger is nog omdat het aantal mogelijkeheden snel
    // uit de hand zal lopen.
    public static void GetPossibilities(Board board)
    {
        BigInteger numberOfPossibiliets = new BigInteger(1);

        foreach(KeyValuePair<string, List<int>> square in board.board)
        {
            numberOfPossibiliets = BigInteger.Multiply(numberOfPossibiliets, square.Value.Count);
        }

        Console.WriteLine("number of possibilites: {0}", numberOfPossibiliets);
    }
}

