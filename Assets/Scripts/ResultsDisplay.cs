using NUnit.Framework.Api;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ResultsDisplay : MonoBehaviour
{
    [SerializeField] Puzzle puzzle;
    [SerializeField] TMP_InputField animationInput;
    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text solved;
    [SerializeField] TMP_Text depth;
    [SerializeField] TMP_Text expanded;
    [SerializeField] TMP_Text frontier;
    [SerializeField] TMP_Text time;
    [SerializeField] TMP_Text path;

    private SearchResult config;

    private void Awake()
    {
        CloseDisplay();
    }

    public void WatchSolution()
    {
        CloseDisplay();
        StartCoroutine(WatchSolutionCoroutine(config.Path));
    }

    private IEnumerator WatchSolutionCoroutine(string path)
    {
        puzzle.SetAnimationTime(animationInput);
        for (int i = 0; i < path.Length; i++)
        {
            switch (path[i])
            {
                case 'U':
                    puzzle.MoveBlank(-1, 0);
                    break;
                case 'D':
                    puzzle.MoveBlank(1, 0);
                    break;
                case 'L':
                    puzzle.MoveBlank(0, -1);
                    break;
                case 'R':
                    puzzle.MoveBlank(0, 1);
                    break;
                default:
                    Debug.LogError("Unrecognized character in path!");
                    break;
            }
            yield return new WaitUntil(() => puzzle.AnimationComplete == true);
        }
    }

    public void SetDisplay(SearchResult config)
    {
        this.config = config;
        panel.SetActive(true);

        solved.text = $"Solved: {config.solved}";
        depth.text = $"Depth: {config.depth}";
        expanded.text = $"Expanded Nodes: {config.expandedNodes}";
        frontier.text = $"Frontier Nodes: {config.frontierNodes}";
        time.text = $"Time to Answer: {config.time}ms";
        path.text = $"Path: {config.Path}";
    }

    public void CloseDisplay()
    {
        panel.SetActive(false);
    }
}
