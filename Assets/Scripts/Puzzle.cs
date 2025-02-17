using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Puzzle : MonoBehaviour
{
    [SerializeField] ErrorDisplay errorDisplay;
    [SerializeField] ResultsDisplay resultsDisplay;
    [SerializeField] GameObject numberDisplayPrefab;
    [SerializeField] int size = 3;
    [SerializeField] float animationTime = 0.3f;
    [SerializeField] TMP_InputField animationInput;
    [SerializeField] TMP_InputField puzzleInput;
    [SerializeField] TMP_Dropdown heuristicDropdown;

    public int PuzzleSize => size * size;
    public Board Board;

    public bool AnimationComplete = false;

    int[] puzzle;
    SearchAlgorithm searchAlgorithm;
    NumberDisplay[] numberDisplays;
    RectTransform rectTransform;
    private Coroutine animationRoutine = null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    //-- PUBLIC METHODS

    public void MoveBlank(CallbackContext context, int rowOffset, int colOffset)
    {
        if (context.performed && Board != null) MoveBlank(rowOffset, colOffset);
    }

    public void MoveBlank(int rowOffset, int colOffset)
    {
        if (animationRoutine != null) return;

        int blankIndex = Board.FindBlank();
        (int row, int col) blankCoord = Board.Coord(blankIndex);
        (int row, int col) targetCoord = (blankCoord.row + rowOffset, blankCoord.col + colOffset);

        // Ensure move was made
        if (Board.MoveBlank(rowOffset, colOffset))
        {
            AnimationComplete = false;
            animationRoutine = StartCoroutine(AnimateSliding(blankCoord, targetCoord));
        }
    }

    // Input binding methods
    public void MoveBlankUp(CallbackContext context) => MoveBlank(context, -1, 0);
    public void MoveBlankDown(CallbackContext context) => MoveBlank(context, 1, 0);
    public void MoveBlankLeft(CallbackContext context) => MoveBlank(context, 0, -1);
    public void MoveBlankRight(CallbackContext context) => MoveBlank(context, 0, 1);


    public void MakePuzzle()
    {
        ResetPuzzle();

        if (!SetPuzzle(puzzleInput)) return;
        SetHeuristic(heuristicDropdown);

        Board = new();
        Board.Initialize(size, puzzle);
        numberDisplays = new NumberDisplay[Board.ArraySize];

        // Create Displays
        for (int i = 0; i < PuzzleSize; i++)
        {
            numberDisplays[i] = CreateDisplay(i);
        }
    }

    public void SolvePuzzle()
    {
        SearchAlgorithm searchAlgo = new(SetHeuristic(heuristicDropdown));
        SearchResult solution = searchAlgo.GeneralSearch(this);
        resultsDisplay.SetDisplay(solution);
    }

    private Heuristic SetHeuristic(TMP_Dropdown heuristicDropdown)
    {
        string heuristicString = heuristicDropdown.captionText.text;
        Heuristic heuristic = null;

        switch (heuristicString)
        {
            case "Uniform Cost":
                heuristic = new UniformCost();
                Debug.Log("Uniform Cost heuristic chosen");
                break;
            case "A* Manhattan":
                heuristic = new A_Manhattan();
                Debug.Log("A* Manhattan heuristic chosen");
                break;
            case "A* Misplaced":
                heuristic = new A_Misplaced();
                Debug.Log("A* Misplaced heuristic chosen");
                break;
            default:
                errorDisplay.OpenDisplay("Unrecognized heuristic in dropdown!");
                break;
        }

        return heuristic;
    }

    public bool SetPuzzle(TMP_InputField input)
    {
        if (input.text.Length != PuzzleSize)
        {
            errorDisplay.OpenDisplay($"INVALID PUZZLE: Puzzle string is not of size {PuzzleSize}");
            return false;
        }

        try
        {
            puzzle = new int[PuzzleSize];
            for (int i = 0; i < PuzzleSize; i++)
            {
                puzzle[i] = int.Parse(input.text[i].ToString());
            }
            return true;
        }
        catch (Exception e)
        {
            errorDisplay.OpenDisplay("INVALID PUZZLE CHARACTER DETECTED");
            return false;
        }
    }

    public void SetAnimationTime(TMP_InputField input)
    {
        animationTime = float.Parse(input.text);
        if (animationTime < 0)
        {
            errorDisplay.OpenDisplay("Animation Time less than 0. Setting value to 0.");
            animationTime = 0;
        }
    }

    //-- PRIVATE HELPERS

    private void ResetPuzzle()
    {
        Board = null;
        DestroyDisplays();
    }

    private NumberDisplay CreateDisplay(int index)
    {
        int value = Board.GetValue(index);
        NumberDisplay display = Instantiate(numberDisplayPrefab, this.transform).GetComponent<NumberDisplay>();

        // Calculate size relative to puzzle
        Vector2 puzzleSize = rectTransform.sizeDelta;
        float dispWidth = puzzleSize.x / size;
        float dispHeight = puzzleSize.y / size;
        Vector2 displaySize = new Vector2(dispWidth - 2, dispHeight - 2);

        // Calculate position relative to puzzle
        Vector2 displayPosition = CalculateDisplayPosition(index);

        // Set values of display
        display.SetRTSize(displaySize);
        display.SetRTPosition(displayPosition);
        display.DisplayNumber(value);

        return display;
    }

    private IEnumerator AnimateSliding((int, int) blank, (int, int) target)
    {
        Vector2 blankPos = CalculateDisplayPosition(blank);
        Vector2 targetPos = CalculateDisplayPosition(target);

        NumberDisplay blankDisplay = numberDisplays[Board.Index(blank)];
        NumberDisplay targetDisplay = numberDisplays[Board.Index(target)];

        // Animates the sliding
        float elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            blankDisplay.SetRTPosition(Vector2.Lerp(blankPos, targetPos, elapsedTime / animationTime));
            targetDisplay.SetRTPosition(Vector2.Lerp(targetPos, blankPos, elapsedTime / animationTime));

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blankDisplay.SetRTPosition(targetPos);
        targetDisplay.SetRTPosition(blankPos);

        // Swaps displays index in list
        var temp = numberDisplays[Board.Index(blank)];
        numberDisplays[Board.Index(blank)] = targetDisplay;
        numberDisplays[Board.Index(target)] = temp;

        AnimationComplete = true;
        animationRoutine = null;
    }

    private Vector2 CalculateDisplayPosition((int,int) coord)
    {
        return CalculateDisplayPosition(Board.Index(coord));
    }

    private Vector2 CalculateDisplayPosition(int index)
    {
        Vector2 puzzleSize = rectTransform.sizeDelta;
        float dispWidth = puzzleSize.x / size;
        float dispHeight = puzzleSize.y / size;

        int row = Board.Row(index);
        int column = Board.Column(index);
        float meanRow = size / 2f;
        float meanColumn = size / 2f;
        float stepX = column - meanColumn + 0.5f;            // this is just like z-scores
        float stepY = row - meanRow + 0.5f;
        float positionX = stepX * dispWidth;
        float positionY = stepY * dispHeight * -1f;
        Vector2 displayPosition = new(positionX, positionY);
        return displayPosition;
    }

    private void DestroyDisplays()
    {
        if (numberDisplays == null) return;

        foreach (var display in numberDisplays)
        {
            Destroy(display.gameObject);
        }
        numberDisplays = null;
    }
}
