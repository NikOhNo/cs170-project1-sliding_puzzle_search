using System.Data;
using UnityEngine;

public class A_Manhattan : Hueristic
{
    private int ManhattanDistance(SearchNode node)
    {
        int distance = 0;
        Board board = node.BoardState;
        for (int i = 0; i < board.ArraySize; i++)
        {
            // coord where value should be
            (int row, int col) correctCoord = board.Coord(i);

            // coord where value currently is
            int currentIndex = board.FindValue(i + 1);
            (int row, int col) currentCoord = board.Coord(currentIndex);

            // Add absolute difference of rows and columns to distance
            distance += Mathf.Abs(currentCoord.row - correctCoord.row);
            distance += Mathf.Abs(currentCoord.col - correctCoord.col);
        }
        // find where blank coord should be
        (int row, int col) blankCoord = board.Coord(board.FindBlank());
        (int row, int col) correctBlankCoord = (board.Size - 1, board.Size - 1);

        // add blank coords distance
        distance += Mathf.Abs(blankCoord.row - correctBlankCoord.row);
        distance += Mathf.Abs(blankCoord.col - correctBlankCoord.col);

        return distance;
    }
}
