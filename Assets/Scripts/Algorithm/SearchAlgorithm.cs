using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

public class SearchAlgorithm
{
    protected Puzzle puzzle;
    public SearchAlgorithm(Puzzle puzzle)
    {
        this.puzzle = puzzle;
    }

    public bool Solved(SearchNode node)
    {
        // Check that every number is in correct position
        for (int i = 0; i < node.BoardState.ArraySize - 1; i++)
        {
            if (node.BoardState.GetValue(i) != i + 1)
            {
                return false;
            }
        }
        // Check that blank is in correct position (bottom right)
        if (node.BoardState.GetValue(node.BoardState.ArraySize - 1) != 0)
        {
            return false;
        }
        // Otherwise puzzle is solved
        return true;
    }

    public SearchNode GeneralSearch(Puzzle puzzle, PriorityQueue<SearchNode, int> queueingFunction)
    {
        SearchNode initial = MakeNode(puzzle);
        queueingFunction.Enqueue(initial, initial.TotalCost);

        while (true)
        {
            if (queueingFunction.Count == 0) return null;

            SearchNode node = queueingFunction.Dequeue();

            if (Solved(node))
            {
                return node;
            }
            
            //queueingFunction.Add()
        }
    }

    /// <summary>
    /// Creates all possible nodes from current node
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public List<SearchNode> Expand(SearchNode node)
    {
        List<SearchNode> newNodes = new();

        Board board = node.BoardState;



        return newNodes;
    }

    public SearchNode SimulateMove(Action<int, int> move)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates the initial node for the puzzle
    /// </summary>
    /// <param name="puzzle"></param>
    /// <returns></returns>
    public SearchNode MakeNode(Puzzle puzzle) 
    {
        SearchNode node = new(puzzle.Board, null, 0, 0, Action.None);
        return node;
    }
}
