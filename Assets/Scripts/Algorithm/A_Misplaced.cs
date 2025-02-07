using UnityEngine;

public class A_Misplaced : Hueristic
{
    private int MisplacedTile(SearchNode node)
    {
        int numMisplaced = 0;

        Board board = node.BoardState;
        // Get sum of every number in incorrect position
        for (int i = 0; i < board.ArraySize - 1; i++)
        {
            if (board.GetValue(i) != i + 1)
            {
                numMisplaced++;
            }
        }
        // Check that blank is in correct position (bottom right)
        if (board.GetValue(board.ArraySize - 1) != 0)
        {
            numMisplaced++;
        }
        
        return numMisplaced;
    }
}
