using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Board
{
    int[] board;
    public int ArraySize => Size * Size;
    public int[] Array => board;

    public int Size;

    /// <summary>
    /// Sets up the board and initial values based off the puzzle passed in.
    /// </summary>
    /// <param name="size"></param>
    /// <param name="puzzle"></param>
    public void Initialize(int size, int[] puzzle)
    {
        this.Size = size;
        board = new int[ArraySize];

        for (int i = 0; i < ArraySize; i++)
        {
            board[i] = puzzle[i];
        }
    }

    public bool MoveBlankUp() => MoveBlank(-1, 0);
    public bool MoveBlankDown() => MoveBlank(1, 0);
    public bool MoveBlankLeft() => MoveBlank(0, -1);
    public bool MoveBlankRight() => MoveBlank(0, 1);

    public bool MoveBlank(int rowOffset, int colOffset)
    {
        int blankIndex = FindBlank();
        (int row, int col) blankCoord = Coord(blankIndex);
        (int row, int col) targetCoord = (blankCoord.row + rowOffset, blankCoord.col + colOffset);

        // Ensure move is within bounds
        if (targetCoord.row >= 0 && targetCoord.row < Size && targetCoord.col >= 0 && targetCoord.col < Size)
        {
            // Swap blank with target
            Swap(blankCoord, targetCoord);
            return true;
        }
        return false;
    }

    public Board Clone()
    {
        Board newBoard = new();
        newBoard.Initialize(this.Size, (int[])this.board.Clone());
        return newBoard;
    }

    public int GetValue((int,int) coord)
    {
        return GetValue(Index(coord));
    }
    public int GetValue(int index)
    {
        return board[index];
    }

    /// <summary>
    /// Swaps value of coord and target coord within the array
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="targetCoord"></param>
    public void Swap((int, int) coord, (int, int) targetCoord)
    {
        (board[Index(coord)], board[Index(targetCoord)]) = (board[Index(targetCoord)], board[Index(coord)]);
    }

    public int Index((int row, int col) coord)
    {
        return (coord.row * Size) + coord.col;
    }

    public int FindValue(int value)
    {
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == value)
            {
                return i;
            }
        }

        // Should not execute: this is an error case
        throw new KeyNotFoundException($"Value not found in board: {value}");
    }

    /// <summary>
    /// Finds the index of where the blank is located on the board
    /// </summary>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
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
        return index / Size;
    }

    public int Column(int index)
    {
        return index % Size;
    }
}
