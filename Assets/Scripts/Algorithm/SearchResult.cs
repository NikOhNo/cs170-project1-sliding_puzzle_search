using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SearchResult
{
    public SearchNode node;
    public bool solved;
    public int depth;
    public int expandedNodes;
    public int frontierNodes;
    public float time;
    public string Path => ComputePath(node);

    public string ComputePath(SearchNode node)
    {
        string solution = string.Empty;

        // Climb up the tree to parent to write the path
        SearchNode currNode = node;
        while (currNode != null)
        {
            switch (currNode.Action)
            {
                case Action.Up:
                    solution = 'U' + solution;
                    break;
                case Action.Down:
                    solution = 'D' + solution;
                    break;
                case Action.Left:
                    solution = 'L' + solution;
                    break;
                case Action.Right:
                    solution = 'R' + solution;
                    break;
                default:
                    break;
            }

            currNode = currNode.Parent;
        }

        return solution;
    }
}
