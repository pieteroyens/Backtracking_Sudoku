/*
 * Deze klasse representeerd het veld dat wordt gebruikt door sudoku puzzels en unieke karakteristieken
 * die eraan gerelateerd zijn.
 * 
 * Een veld bestaad uit N * N vakken (squares). Een rij is aangegeven met een letter, beginnend bij 
 * 'A' oplopend tot letter N (waar N is de n'de letter van het alfabet). Kolommen zijn cijfers, beginnend
 * bij 1 oplopend tot N (waar N de breedte/hoogte is van het bord). Het veld komt er als volgt uit te zien 
 * voor een 9x9 bord:
 * 
 * A1 A2 A3 ... A9
 * B1 B2 B3 ... B9
 * .   .  . ... .
 * .   .  . ... .
 * .   .  . ... .
 * I1 I2 I3 ... I9
 *
 * Bij deze datastructuur hoort ook de zogeheten buren (peers) van een vak. Dit zijn alle vakken waarbij, volgens
 * sudoku regels, één uniek vak mee gemoeid is. Dat wil zeggen: alle vakken die op dezelfde rij en kolom staan, en 
 * in het zelfde vierkant zitten, exclusief het vak zelf. Elk vak heeft als zodanig 20 buren. 
 * Bijvoorbeeld, A1's buren: A2, A3, A4, A5, A6, A7, A8, A9 (horizontaal) | B1, C1, D1, E1, F1, G1 H1 I1 (verticaal) |
 * B2, B3, C2, C3 (zelfde vierkant).
 * 
 * Deze klasse is geïnspireerd door het artikel http://norvig.com/sudoku.html geschreven door Peter Norvig. 
 */

using System;
using System.Collections.Generic;
using System.Linq;

static class Grid
{
    #region member variables
    static public int gridSize;

    static public string rows;
    static public int[] columns;

    // slaat op welke letters er horizontaal in een vierkant staan.
    static private string[] boxHorizontal;
    // slaat op welke getallen er verticaal in een vierkant staan.
    static private int[][] boxVertical;

    // per vak, elk vak (inclusief zichzelf) wat met dit vak gemoeid is volgens sudoku regels.
    static private Dictionary<string, string[][]> units = new Dictionary<string, string[][]>();

    // elk uniek vak van het bord.
    static public string[] squares;
    // elke buur van elk uniek vak.
    static public Dictionary<string, HashSet<string>> peers = new Dictionary<string, HashSet<string>>();
    #endregion

    static Grid()
    {
        gridSize = Program.boardSize;

        columns = new int[gridSize];

        // bepaald dynamisch op basis van de grote van het bord hoeveel kolommen er zijn.
        for (int i = 0; i < columns.Length; i++)
        {
            columns[i] = i + 1;
        }

        // vul de rijen in, beginnend bij de letter 'A'
        rows = "";
        for (int i = 0; i < gridSize; i++)
        {
            rows += (char)(65 + i);
        }
        
        // grootte van elk vierkant op het bord
        int boxSize = (int)Math.Sqrt(gridSize);

        // vul de mogelijke rijen in voor een vierkant
        boxHorizontal = new string[boxSize];
        for (int i  = 0; i  < boxSize; i ++)
        {
            boxHorizontal[i] = rows.Substring(i * boxSize, boxSize);
        }

        // vul de mogelijke kolommen in voor een vierkant
        boxVertical = new int[boxSize][];
        for (int i = 0; i < boxSize; i++)
        {
            boxVertical[i] = new int[boxSize];

            for (int j = 0; j < boxSize; j++)
            {
                boxVertical[i][j] = i * boxSize + (j + 1);
            }
        }
        
        // maakt mbv cartesisch product alle vlakken in het veld.
        squares = Cartesian(rows, columns);
        // bepaal alle peers voor elk vlak van het veld.
        CreatePeers();
    }

    // rekent met behulp van LINQ het cartesisch product uit en zet deze om naar een array.
    private static string[] Cartesian(string A, int[] B)
    {
        return A.SelectMany(a => B.Select(b => a + b.ToString())).ToArray();
    }

    private static void CreatePeers()
    {
        // maak alle rijen en kolommen en vierkanten aan.
        string[][] rowUnits = columns.Select(digit => Cartesian(rows, new int[] { digit })).ToArray();
        string[][] columnUnits = rows.Select(r => Cartesian(r.ToString(), columns).ToArray()).ToArray();   
        string[][] boxUnits = boxHorizontal.SelectMany(rs => boxVertical.Select(cs => Cartesian(rs, cs).ToArray())).ToArray();
        // maak een combinatie van de drie
        string[][] unitList = rowUnits.Concat(columnUnits).Concat(boxUnits).ToArray();

        // voor elk vlak: maak een lijst met zowel de horizontale, verticale vlakken en
        // de vlakken die het vierkant delen.
        foreach (string square in squares)
        {
            List<string[]> tempUnits = new List<string[]>();

            units.Add(square, new string[2][]);

            foreach (string[] unit in unitList)
            {
                foreach (string squareUnit in unit)
                {
                    if (squareUnit == square)
                    {
                        tempUnits.Add(unit);
                    }
                }
            }
            units[square] = tempUnits.ToArray();
        }

        // voor elk vlak, vind alle buren van het vlak, exclusief zichzelf
        foreach (KeyValuePair<string, string[][]> squareUnits in units)
        {
            // verkrijg elke buur, inclusief all dublicaten.
            var allPeers = squareUnits.Value[0].Union(squareUnits.Value[1]).Union(squareUnits.Value[2]);
            // verwijder alle duplicaten.
            var hashPeers = new HashSet<string>(allPeers);
            // verwijder het vlak zelf.
            hashPeers.Remove(squareUnits.Key);
  
            // voeg het vlak en zijn buren toen aan de dictionary.
            peers.Add(squareUnits.Key, hashPeers);        
        }
    }
}

