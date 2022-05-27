/*
 * De functie van deze klasse is het uit lezen van sudoku borden uit .txt bestanden. 
 * Het zet de borden om met behulp van de Board klassen zodat ze opgelost kunnen worden.
 * 
 * In elk bestand moeten borden gescheiden zijn met Grid N. Grid geeft het volgende bord
 * aan wordt gebruikt door de code om te weten wanneer een bord stopt en een volgende begint.
 * 
 * Het leest de borden uit als strings, en zet deze om in lijsten van cijfers.
 * Deze lijsten van cijfers worden vervolgens gegeven aan de Board klasse zodat deze
 * een zoekbaar bord aanmaakt.
 * 
 * Het resultaat is een lijst van alle borden uit een .txt bestand
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


class BoardParser
{
    StreamReader reader;

    public BoardParser(StreamReader reader)
    {
        this.reader = reader;
    }

    // Leest alle borden uit een bestand uit en geeft een lijst
    // van zoekbare borden terug.
    public List<Board> getBoards()
    {
        List<string> board = new List<string>();
        List<Board> allBoards = new List<Board>();

        string temp;

        bool endOfFile = false;

        // zolang we niet op het einde van het bestand zijn,
        // zijn er nog borden over.
        while (!endOfFile)
        {
            bool readingBoard = true;

            while (readingBoard)
            {
                temp = reader.ReadLine();

                // als er geen regel is om te lezen, is het eind van het bestand bereikt
                // en moeten we stoppen met lezen.
                if (temp == null)
                {
                    endOfFile = true;
                    break;
                }
                else
                {
                    // als er geen "Grid" wordt gelezen zijn we nog bezig met het lezen
                    // van een bord.
                    if (!temp.Contains("G"))
                    {
                        board.Add(temp);
                    }
                    // als er wel "Grid" wordt gelezen
                    else
                    {
                        // we zijn aan het begin van het bord, dus we moeten doorlezen
                        if (board.Count == 0)
                        {
                            continue;
                        }
                        // we zijn klaar met het lezen van een bord
                        else
                            readingBoard = false;
                    }
                }
            }

            // De bord grote is gelijk aan hoeveel regels er zijn in een bord (bord is een perfect vierkant)
            Program.boardSize = board.Count;

            // Ze alle rijen van string om naar rijen van integers
            List<int> allBoardValues = getAllBoardValues(board, board.Count);

            // Creeër een nieuw bord en voeg deze toe aan de lijst met zoekbare borden.
            Board newBoard = new Board(allBoardValues);
            allBoards.Add(newBoard);

            // herstel de placeholder voor het bord uit het .txt bestand.
            board = new List<string>();
        }

        return allBoards;
    }

    private List<int> getAllBoardValues(List<string> board, int boardSize)
    {
        List<int> allBoardValues = new List<int>();

        // als het bord 9 bij 9 is, zijn er geen spaties tussen de getallen
        if (boardSize <= 9)
        {
            foreach (string numbers in board)
            {
                foreach (char number in numbers)
                    allBoardValues.Add((int)Char.GetNumericValue(number));
            }

            return allBoardValues;
        }
        // als het bord groter dan 9 bij 9 is, zijn er spaties en moeten deze eruit worden gehaald
        else
        {
            foreach (string numbers in board)
            {
                var rowValues = Regex.Matches(numbers, @"\d+").OfType<Match>().Select(m => Int32.Parse(m.Value)).ToArray();
                allBoardValues.AddRange(rowValues);
            }

            return allBoardValues;
        }
    }

    // Laat een bord zien in de console.
    public void displayBoard(Board board)
    {
        // de breedte van een vierkant in een bord
        int boxSize = (int)Math.Sqrt(Grid.gridSize);
        int dashesToPrint = 0;


        for (int i = 0; i < Grid.gridSize; i++)
        {
            for (int j = 0; j < Grid.gridSize; j++)
            {
                int currentValue;

                if (board.board[Grid.squares[i * Grid.gridSize + j]].Count == 1)
                    currentValue = board.board[Grid.squares[i * Grid.gridSize + j]][0];
                else
                    currentValue = 0;


                if (j % boxSize == 0 && j != 0)
                {
                    if (currentValue == 0)
                    {
                        Console.Write("| .  ");
                    }
                    else if (currentValue <= 9)
                    {
                        Console.Write("| " + currentValue + "  ");

                    }
                    else
                    {
                        Console.Write("| " + currentValue + " ");

                    }
                    dashesToPrint += 5;
                }
                else
                {
                    if (currentValue == 0)
                    {
                        Console.Write(".  ");
                    }
                    else if (currentValue <= 9)
                    {
                        Console.Write(currentValue + "  ");
                    }
                    else
                    {
                        Console.Write(currentValue + " ");
                    }
                    dashesToPrint += 3;
                }
            }

            Console.WriteLine();
            if ((i + 1) % boxSize == 0 && (i + 1) != Grid.gridSize)
            {
                for (int k = 0; k <= dashesToPrint - 3; k++)
                {
                    Console.Write("-");
                }
                Console.WriteLine();
            }
            dashesToPrint = 0;
        }
    }
}


