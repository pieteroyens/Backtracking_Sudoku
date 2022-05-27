/*
 * Dit is de klasse waar het daadwerkelijke oplossen van de borden gebeurd.
 * Hier worden verschillende algoritmen ebruikt aan de hand van de gekozen instellingen.
 * Om ze op te lossen, ontvangt de Search klasse een bord met een oplosmodus. Hierbij kan gekozen
 * worden tussen backtracking of backtracking met forward search. De methode Solve is lost het bord op
 * en als een bord in 10 minuten opgelost kan worden geeft het het opgeloste bord terug, anders een null waarde.
 */

using System;
using System.Collections.Generic;


class Search
{
    #region member variabelen gebruikt in forward checking
    // een stack om de geschiedenis van borden bij te houden, gebruikt in forward checking.
    public static Stack<KeyValuePair<string, Board>> boardHistory = new Stack<KeyValuePair<string, Board>>();
    // een lijst van vlakken die zijn bezocht en waarvoor een getal is gekozen, gebruik in forward checking.
    public static List<string> visitedSquares = new List<string>();
    #endregion  

    Board boardToSearch;

    public Board Solve(Board board, string searchMode, int solveMode)
    {
        // het onopgeloste bord.
        boardToSearch = board;
        // haal alle getallen uit de mogelijk opties van vlakken op basis
        // van al ingevulde vlakken.
        ConstraintPropagation.MakeStartingBoardConsitent(board);

        // verkrijg alle vakken die niet ingevuld zijn in die moeten worden doorzcht.
        // Deze zijn geordend op basis van de zoek modus.
        BoardFunctions.SquaresToSolve(board, searchMode);

        // verkrijg het eerste vlak waarvoor een getal voor moet worden gekozen.
        string position = BoardFunctions.squareList[0];

        // of er wordt gezocht met BT of BT+FS.
        if (solveMode == 1)
        {
            if (backtrack(position))
                return boardToSearch;
        }
        else if (solveMode == 2)
        {
            if (backTrackForwarchChecking(position))
                return boardToSearch;
        }

        // een bord dat niet opgelost kan worden binnen 10 minuten.
        return null;
    }

    // de implementatie van het backtracking algoritme.
    public bool backtrack(string position)
    {
        Info.recursiveBTcalls++;
        
        // controleer of we nog in de 10 minuten grens zitten.
        if (Program.stopwatch.ElapsedMilliseconds >= 600000)
            return false;

        // als er geen vlakken meer zijn om te controleren, dan zijn alle vlakken goed ingevuld
        // en hebben we het bord dus opgelost.
        if (position == null)
            return true;

        // alle getallen die mogelijk kunnen worden ingevuld in het huidige vlak.
        List<int> numbersToCheck = new List<int>(boardToSearch.board[position]);

        // kijk voor elk nummer in het vlak of het een legale invulling is.
        foreach (int number in numbersToCheck)
        {
            // kies het huidige getal en vul het in het vlak in.
            boardToSearch.board[position] = new List<int> { number };

            // controleer of deze zet legaal is, zo ja, ga door naar het volgende vlak.
            if (BoardFunctions.moveLegal(position, boardToSearch))
            {
                if (backtrack(BoardFunctions.getNextPos(position)))
                    return true;
            }
        }
        
        // alle mogelijke getallen waren niet legaal in dit vlak, dus ALLE mogelijke getallen
        // weer terug.
        boardToSearch.board[position] = numbersToCheck;
        // ga nu terug naar het vorige vlak en probeer het volgende getal uit.
        return false; 
    }

    // de implementatie van het forward checking algoritme.
    public bool backTrackForwarchChecking(string position)
    {
        Info.recursiveBTcalls++;

        // controleer of we nog in de 10 minuten grens zitten.
        if (Program.stopwatch.ElapsedMilliseconds >= 600000)
            return false;

        // als er geen vlakken meer zijn om te controleren, dan zijn alle vlakken goed ingevuld
        // en hebben we het bord dus opgelost. We gaan hierna een ander bord oplossen, dus de bezochte
        // vlakken moet worden gereset voor het volgende bord.
        if (position == null)
        {
            visitedSquares = new List<string>();
            return true;
        }

        // alle getallen die mogelijk kunnen worden ingevuld in het huidige vlak.
        List<int> numbersToCheck = new List<int>(boardToSearch.board[position]); 

        // stop een kopie van het huidige bord zoals op de geschiedenis stack voordat nummer toegewezen worden.
        // Dit is een deep copy van het bord zodat er geen referenties bestaan in memory.
        Board previousBoard = new Board(boardToSearch);

        // als er sowieso maar een getal te kiezen in het vlak heeft het geen zin om het bord in de geschiedenis
        // te zetten.
        if (numbersToCheck.Count > 1)
        {
            boardHistory.Push(new KeyValuePair<string, Board>(position, previousBoard));
        }

        // het laatste getal dat ingevuld kan worden.
        int lastNumber = numbersToCheck[numbersToCheck.Count - 1];

        // voor elk getal dat ingevuld kan worden proberen we het getal in te vullen en constraints te propageren.s
        foreach (int number in numbersToCheck)
        {
            // kijk of een bord al niet op de geschiedenis stack staat, en willen het bord er ook niet opzetten
            // als we bij het laatste getal aangekomen zijn dat er geprobeerd kan worden. Als deze niet lukt moeten
            // we sowieso een stap terug in de geschiedenis.
            try
            {
                // en als we niet bij het laatste getal zijn van de nummers die we moeten checken
                if (numbersToCheck.Count > 1 && number != lastNumber && boardHistory.Peek().Key != position) 
                    boardHistory.Push(new KeyValuePair<string, Board>(position, previousBoard));
            }
            // voor het geval de geschiedenis van borden leeg is.
            catch (Exception e)
            {
                // en als we niet bij het laatste getal zijn van de nummers die we moeten checken
                if (numbersToCheck.Count > 1 && number != lastNumber) 
                    boardHistory.Push(new KeyValuePair<string, Board>(position, previousBoard));
            }


            // vul het getal in, in het huidige vlak.
            boardToSearch.board[position] = new List<int> { number };



            // maak het bord consistent en kijk of het bord nog geldig is.
            if (ConstraintPropagation.ForwardChecking(boardToSearch, position))
            {
                // voeg het huidige vlak toe aan bezochte vlakken.
                if (!visitedSquares.Contains(position))
                    visitedSquares.Add(position);

                // kies dynamisch het volgende vlak dat gekozen moet worden en voer daarop weer BT + FC uit.
                if (backTrackForwarchChecking(BoardFunctions.MCV(position, boardToSearch)))
                    return true;
            }
            // als het progaren niet goed gaat: herstel alle peers door het getal wat gekozen was en eruit
            // gehaald terug te zetten.
            else
            {
                // pak het bord zoals hij was voor de gemaakte keuze.
                KeyValuePair<string, Board> temp = boardHistory.Pop();
                // maak het te doorzoeken bord gelijk en zijn voorganger.
                boardToSearch = new Board(temp.Value);
            }
        }

        //Indien de tak niet uitkwam tot een geldige oplossing moet het huidige vlak weer terug komen
        // in de te doorzoeken vlakken.
        visitedSquares.Remove(position);

        // we komen niet uit, ga een stap terug.
        return false;
    }


}

