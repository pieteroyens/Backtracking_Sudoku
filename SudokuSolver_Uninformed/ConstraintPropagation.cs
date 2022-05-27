/*
 * Methodes voor het consistent van, en het propageren van constraints op borden
 * worden door deze klasse aangeboden. 
 */ 

using System.Collections.Generic;

static class ConstraintPropagation
{
    // Deze methode maakt een begin bord consistent. Er wordt naar elk vlak gekeken, en 
    // als en dit vlak maar één getal staat, wordt dit getal weg gehaald uit alle peers
    // van het vlak.
    public static void MakeStartingBoardConsitent(Board board)
    {
        foreach (string square in Grid.squares)
        {
            if (board.board[square].Count == 1)
            {
                int numberToRemove = board.board[square][0];

                foreach (string peer in Grid.peers[square])
                {
                    List<int> currentValues = new List<int>(board.board[peer]);
                    currentValues.Remove(numberToRemove);
                    board.board[peer] = new List<int>(currentValues);
                }
            }
        }

        Info.GetPossibilities(board);
    }


    // Forward checking ontvangt een positie/vlak op het bord. Op dit bord is bij ontvangst
    // voor één getal gekozen. Dit getal wordt uit zijn peers gehaald, en er wordt vervolgens 
    // gecontroleerd of de domeinen van deze peers niet leeg raken. Zo nee, is het geslaagd en 
    // kan er verder worden gezocht, zo nee, dan is Forward Checking mislukt en moet een ander
    // getal worden gekozen.
    public static bool ForwardChecking(Board board, string currentPosition)
    {
        int numberToRemove = board.board[currentPosition][0];

        foreach (string peer in Grid.peers[currentPosition])
        {
            List<int> currentValues = new List<int>(board.board[peer]);

            currentValues.Remove(numberToRemove);

            // controleer of een domein niet leeg is geraakt.
            if (currentValues.Count <= 0)
            {
                // forward checking is mislukt.
                return false;
            }

            // zet het nieuwe domein terug in het bord.
            board.board[peer] = new List<int>(currentValues);
        }
        // forward checking is geslaagd.
        return true;
    }
}
