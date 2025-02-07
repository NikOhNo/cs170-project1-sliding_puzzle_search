using UnityEngine;

public class SearchNode
{
    public Board BoardState;
    public SearchNode Parent;
    public int Cost;
    public int Heuristic;
    public int TotalCost => Cost + Heuristic;
    public Action Action;

    public SearchNode(Board state, SearchNode parent, int cost, int heuristic, Action action)
    {
        BoardState = state;
        Parent = parent;
        Cost = cost;
        Heuristic = heuristic;
        Action = action;
    }
}

public enum Action
{
    Up, 
    Down,
    Left,
    Right,
    None
}