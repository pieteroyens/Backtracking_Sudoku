/*
 * Datastructuur voor een bord dat doorzocht en opgelost kan worden.
 * De datastrutuur gebruikt een dictionary met als key een vlak, en als value
 * een lijst van getallen. Deze lijst van getallen representeerd het ingevulde of nog
 * in te vullen getal(len).
 */ 

using System.Collections.Generic;
using System.Linq;

class Board
{
    public Dictionary<string, List<int>> board;

    // maak een nieuw bord aan op basis van een lijst met waardes verkregen uit een file.
    public Board(List<int> boardValues)
    {
        BoardFunctions.CreateNumberList();
        board = FillEmptyBoard(boardValues);
    }

    // maak een deep copy van een bord.
    public Board(Board newBoard)
    {
        board = newBoard.board.ToDictionary(k => k.Key, k => k.Value.ToList<int>());
    }

    // vult een lege dictionairy in met waardes verkregen uit een file. Als er een getal anders
    // dan 0 voorkomt, wordt dat getal ingevuld bij het bijbehorende vlak van het grid. Als het getal
    // een 0 is, wordt er een lijst van alle mogelijke getallen ingevuld voor het vlak.
    private Dictionary<string, List<int>> FillEmptyBoard(List<int> boardValues)
    {
        Dictionary<string, List<int>> boardToFillIn = new Dictionary<string, List<int>>();

        for (int i = 0; i < Grid.squares.Length; i++)
        {
            // als het getal is ingevuld.
            if (boardValues[i] != 0)
            {
                boardToFillIn.Add(Grid.squares[i], new List<int>{ boardValues[i] } );
            }
            // zo niet dan zijn alle mogelijke getallen voor dit bord mogelijk in dit vlak.
            else
            {
                boardToFillIn.Add(Grid.squares[i], BoardFunctions.squareNumbers);
            }
        }

        return boardToFillIn;
    }
}
