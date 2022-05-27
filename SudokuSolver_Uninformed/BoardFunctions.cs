/*
 * Deze klasse biedt methodes die kunnen worden uitgevoerd op de bord klasse.
 * Ze bieden functionaliteiten die nodig zijn om correct met backtracking (+forwartd checking)
 * te kunnen zoeken.
 */

using System;
using System.Collections.Generic;
using System.Linq;

static class BoardFunctions
{
    // de lijst van te doorzoeken vlakken van een bord.
    public static List<string> squareList;
    // alle mogelijke getallen voor een vlak als een bord nog niet consistent is gemaakt.
    public static List<int> squareNumbers;

    static BoardFunctions()
    {
        squareNumbers = CreateNumberList();
    }

    // controleer of op het huidige vlak gekozen nummer een legale keuze is. 
    public static bool moveLegal(string square, Board board)
    {
        int value = board.board[square][0];

        HashSet<string> peers = Grid.peers[square];

        foreach (string peer in peers)
        {
            // controleer voor ieder vlak waar al een nummber gekozen is, of het 
            // nummer niet identiek is aan de net gekozen waarde.
            if (board.board[peer].Count == 1 && value == board.board[peer][0])
                return false;
        }

        return true;
    }

    // creeërt een lijst met mogelijk nummers die ingevuld kunnen in elk leeg vlak.
    // Dit gebeurd dynamisch afhankelijk van de breedte/hoogte van een bord. Dus een
    // bord van 16 bij 16 zou voor elk leeg vlak een lijst krijgen van {1, 2, ... 16}.
    public static List<int> CreateNumberList()
    {
        List<int> numberList = new List<int>();

        for (int i = 0; i < Program.boardSize; i++)
        {
            numberList.Add(i + 1);
        }

        return numberList;
    }

    // maakt een lijst aan van alle vlakken die het algoritmen langs moet gaan en
    // er waardes invullen. De volgorde waarin vlakken in deze lijst staan is de
    // volgorde waarin er gezocht zal worden. Deze methode sorteert standaard van
    // links naar rechts, dus van A1 tot I9. 
    public static void SquaresToSolve(Board board, string mode)
    {
        squareList = new List<string>();

        // elk vlak waarin meer dan één getal staat moet doorzocht worden.
        foreach (KeyValuePair<string, List<int>> square in board.board)
        {
            if (square.Value.Count > 1)
            {
                squareList.Add(square.Key);
            }
        }

        // als er van recht naar links gezocht moet worden, draai de lijst om.
        if (mode == "rl")
        {
            squareList.Reverse();
        }
        // als er geozocht moet worden op basis van domeingrootte.
        else if (mode == "ds" || mode == "mcv")
        {
            orderByDomainSize(board);
        }
    }

    // ordert de lijst van te doorzoeken vlakken op basis van de domeingrootte van elk vlak.
    // Deze ordening is oplopend van klein naar groot.
    public static void orderByDomainSize(Board board)
    {
        // gebruik een dictionary zodat we later LINQ kunnen gebruiken.
        Dictionary<string, int> squareDomainSize = new Dictionary<string, int>();

        // bepaal de domein groote voor elk vlak door te kijken hoevel mogelijke
        // getallen er ingevuld kunnen worden.
        foreach (string square in squareList)
        {
            if (board.board[square].Count > 1)
                squareDomainSize.Add(square, board.board[square].Count);
        }

        // haal een op domeingrootte geordende lijst uit de dictionary.
        var domainSizeOrderedList = (from pair in squareDomainSize
                                     orderby pair.Value ascending
                                     select pair).ToList();

        // we hebben de domeingrootte niet meer nodig, enkel de vlakken.
        squareList = (from square in domainSizeOrderedList select square.Key).ToList();
    }

    // vindt op basis van het huidig bezochte vlak, welk vlak er hierna moet worden onderzocht.
    public static string getNextPos(string currentPos)
    {
        string[] tempList = squareList.ToArray();

        int currentIndex = Array.IndexOf(tempList, currentPos);

        // we proberen een volgende positie te vinden.
        try
        {
            string nextPos = tempList[currentIndex + 1];
            return nextPos;
        }
        // als er geen posities meer over zijn om na te gaan.
        catch (System.IndexOutOfRangeException)
        {
            return null;
        }
    }

    // deze methode ordend dynamisch de te doorzoeken vlakken op basis van de 
    // Most Constrained Variable heuristiek.
    public static string MCV(string currentPos, Board board)
    {
        // alle mogelijk vlakken te doorzoeken exclusief de vlakken die al doorzocht zijn.
        List<string> possibleSquares = squareList.Except(Search.visitedSquares).ToList();
        
        // gebruik een dictionary zodat we later LINQ kunnen gebruiken.
        Dictionary<string, int> squareDomainSize = new Dictionary<string, int>();

        // bepaal de domein groote voor elk vlak door te kijken hoevel mogelijke
        // getallen er ingevuld kunnen worden.
        foreach (string square in possibleSquares)
        {
            squareDomainSize.Add(square, board.board[square].Count);
        }

        // haal een op domeingrootte geordende lijst uit de dictionary.
        var domainSizeOrderedList = (from pair in squareDomainSize
                                     orderby pair.Value ascending
                                     select pair).ToList();

        // we hebben de domeingrootte niet meer nodig, enkel de vlakken.
        possibleSquares = (from square in domainSizeOrderedList select square.Key).ToList();
        
        // probeer om een vlak terug te geven wat doorzocht moet worden.
        try
        {
            return possibleSquares[0];
        }
        // als er geen posities zijn om te doorlopen.
        catch (System.ArgumentOutOfRangeException)
        {
            return null;
        }
    }
}