using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.InputSystem.InputAction;

public class Puzzle : MonoBehaviour
{
    [SerializeField] GameObject numberDisplayPrefab;
    [SerializeField] int size = 3;
    [SerializeField] float animationTime = 0.3f;

    public int PuzzleSize => size * size;

    Board board;
    int[] puzzle;
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
        if (context.performed) MoveBlank(rowOffset, colOffset);
    }

    public void MoveBlank(int rowOffset, int colOffset)
    {
        if (animationRoutine != null) return;

        int blankIndex = board.FindBlank();
        (int row, int col) blankCoord = board.Coord(blankIndex);
        (int row, int col) targetCoord = (blankCoord.row + rowOffset, blankCoord.col + colOffset);

        // Ensure move is within bounds
        if (targetCoord.row >= 0 && targetCoord.row < size && targetCoord.col >= 0 && targetCoord.col < size)
        {
            // Swap blank with target
            board.Swap(blankCoord, targetCoord);
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
        board = new();
        board.Initialize(size, puzzle);
        numberDisplays = new NumberDisplay[board.ArraySize];

        // Create Displays
        for (int i = 0; i < PuzzleSize; i++)
        {
            numberDisplays[i] = CreateDisplay(i);
        }
    }

    public void SetPuzzle(TMP_InputField input)
    {
        if (input.text.Length != PuzzleSize)
        {
            Debug.LogError($"INVALID PUZZLE: Puzzle is not of size {PuzzleSize}");
        }

        puzzle = new int[PuzzleSize];
        for (int i = 0; i < PuzzleSize; i++)
        {
            puzzle[i] = int.Parse(input.text[i].ToString());
        }
    }

    public void SetAnimationTime(TMP_InputField input)
    {
        animationTime = float.Parse(input.text);
    }

    //-- PRIVATE HELPERS

    private void ResetPuzzle()
    {
        board = null;
        DestroyDisplays();
    }

    private NumberDisplay CreateDisplay(int index)
    {
        int value = board.GetValue(index);
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

        NumberDisplay blankDisplay = numberDisplays[board.Index(blank)];
        NumberDisplay targetDisplay = numberDisplays[board.Index(target)];

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
        var temp = numberDisplays[board.Index(blank)];
        numberDisplays[board.Index(blank)] = targetDisplay;
        numberDisplays[board.Index(target)] = temp;

        animationRoutine = null;
    }

    private Vector2 CalculateDisplayPosition((int,int) coord)
    {
        return CalculateDisplayPosition(board.Index(coord));
    }

    private Vector2 CalculateDisplayPosition(int index)
    {
        Vector2 puzzleSize = rectTransform.sizeDelta;
        float dispWidth = puzzleSize.x / size;
        float dispHeight = puzzleSize.y / size;

        int row = board.Row(index);
        int column = board.Column(index);
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
