using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Board
{
    int[] board;
    public int ArraySize => size * size;

    private int size;

    public void Initialize(int size)
    {
        this.size = size;
        board = new int[ArraySize];

        for (int i = 1; i < board.Length; i++)
        {
            board[i - 1] = i;
        }
        board[board.Length - 1] = 0;
    }


    public int GetValue((int,int) coord)
    {
        return GetValue(Index(coord));
    }
    public int GetValue(int index)
    {
        return board[index];
    }

    public void Swap((int, int) coord, (int, int) targetCoord)
    {
        (board[Index(coord)], board[Index(targetCoord)]) = (board[Index(targetCoord)], board[Index(coord)]);
    }


    public int Index((int row, int col) coord)
    {
        return (coord.row * size) + coord.col;
    }

    public int FindBlank()
    {
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == 0)
            {
                return i;
            }
        }

        // Should not execute: this is an error case
        throw new KeyNotFoundException("Blank not found in board");
    }

    public (int, int) Coord(int index)
    {
        return (Row(index), Column(index));
    }

    public int Row(int index)
    {
        return index / size;
    }

    public int Column(int index)
    {
        return index % size;
    }
}
