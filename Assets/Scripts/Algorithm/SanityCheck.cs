using System;
using UnityEngine;

// THIS CLASS FOR SANITY CHECKS WAS NOT WRITTEN BY ME
// ALL CODE COMES FROM: https://www.geeksforgeeks.org/check-instance-8-puzzle-solvable/
public class SanityCheck
{
    // C# program to check if a given
    // instance of 8 puzzle is solvable or not
    
    // A utility function to count
    // inversions in given array 'arr[]'
    static int getInvCount(int[] arr)
    {
        int inv_count = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int j = i + 1; j < 9; j++)
            {
                // Value 0 is used for empty space
                if (arr[i] > 0 && arr[j] > 0 && arr[i] > arr[j])
                {
                    inv_count++;
                }
            }
        }
           
        return inv_count;
    }

    // This function returns true
    // if given 8 puzzle is solvable.
    public static bool isSolvable(Puzzle puzzle)
    {
        // Count inversions in given 8 puzzle
        int invCount = getInvCount(puzzle.Board.Array);

        // return true if inversion count is even.
        return (invCount % 2 == 0);
    }
// This code is contributed by chandan_jnu
}
