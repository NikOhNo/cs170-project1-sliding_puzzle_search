using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class SearchAlgorithm
{
    protected Heuristic heuristic;
    protected HashSet<Board> visitedStates;
    public SearchAlgorithm(Heuristic heuristic)
    {
        this.heuristic = heuristic;
    }

    public SearchResult GeneralSearch(Puzzle puzzle)
    {
        // variables for data tracking
        var watch = System.Diagnostics.Stopwatch.StartNew();
        int expanded = 0;

        // setting up data structures
        PriorityQueue<SearchNode, int> queueingFunction = new();
        visitedStates = new();

        // creating initial node
        SearchNode initial = MakeNode(puzzle);
        expanded++;
        queueingFunction.Enqueue(initial, initial.TotalCost);
        visitedStates.Add(initial.BoardState);
        
        // exit early if not solvable
        if (!SanityCheck.isSolvable(puzzle)) return Result(watch, null, expanded, 0);

        while (true)
        {
            // no more nodes to visit
            if (queueingFunction.Count == 0) return Result(watch, null, expanded, queueingFunction.Count);

            SearchNode node = queueingFunction.Dequeue(); // setting next node

            if (Solved(node))
            {
                Debug.Log("FOUND SOLUTION YAY");
                return Result(watch, node, expanded, queueingFunction.Count);
            }

            List<SearchNode> expandedNodes = Expand(node);
            expanded += expandedNodes.Count;
            foreach (var expNode in expandedNodes)
            {
                queueingFunction.Enqueue(expNode, expNode.TotalCost);
            }
        }
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

    private SearchResult Result(System.Diagnostics.Stopwatch watch, SearchNode node, int expanded, int frontier)
    {
        watch.Stop();

        SearchResult result = new()
        {
            node = node,
            solved = node != null,
            depth = node == null ? 0 : node.Cost, 
            expandedNodes = expanded,
            frontierNodes = frontier,
            time = watch.ElapsedMilliseconds,
        };

        return result;
    }

    /// <summary>
    /// Creates all possible nodes from current node
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public List<SearchNode> Expand(SearchNode node)
    {
        List<SearchNode> newNodes = new();
        var moves = new (Action action, Func<Board, bool> moveFunction)[]
        {
            (Action.Up, board => board.MoveBlankUp()),
            (Action.Down, board => board.MoveBlankDown()),
            (Action.Left, board => board.MoveBlankLeft()),
            (Action.Right, board => board.MoveBlankRight())
        };

        // simulate all possible moves
        foreach (var (action, moveFunction) in moves)
        {
            Board newBoard = node.BoardState.Clone();
            if (moveFunction(newBoard)) // Check if move is possible
            {
                // dont add repeated states
                if (visitedStates.Contains(newBoard)) continue;

                newNodes.Add(new SearchNode(
                    newBoard,
                    node,
                    node.Cost + 1,
                    heuristic.Calculate(newBoard),
                    action));
                visitedStates.Add(newBoard);
            }
        }

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
