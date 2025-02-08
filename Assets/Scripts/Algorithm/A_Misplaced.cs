using UnityEngine;

public class A_Misplaced : Heuristic
{
    public override int Calculate(Board board)
    {
        return MisplacedTile(board);
    }

    private int MisplacedTile(Board board)
    {
        int numMisplaced = 0;

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
