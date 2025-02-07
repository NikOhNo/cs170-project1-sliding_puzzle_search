using TMPro;
using UnityEngine;

public class ResultsDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text solved;
    [SerializeField] TMP_Text depth;
    [SerializeField] TMP_Text expanded;
    [SerializeField] TMP_Text frontier;
    [SerializeField] TMP_Text time;
    [SerializeField] TMP_Text path;

    public void SetDisplay(ResultsDisplayConfig config)
    {
        solved.text = $"Solved: {config.solved}";
        depth.text = $"Depth: {config.depth}";
        expanded.text = $"Expanded Nodes: {config.expandedNodes}";
        frontier.text = $"Frontier Nodes: {config.frontierNodes}";
        time.text = $"Time to Answer: {config.time}";
        path.text = $"Path: {config.path}";
    }
}
