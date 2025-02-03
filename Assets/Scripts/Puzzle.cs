using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [SerializeField] GameObject numberDisplayPrefab;

    [SerializeField] int NumRows = 3;
    [SerializeField] int NumColumns = 3;

    int ArraySize => NumRows * NumColumns;

    int[] board;
    NumberDisplay[] numberDisplays;
    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    //-- PUBLIC METHODS

    public void MakePuzzle()
    {
        ResetPuzzle();
        board = new int[ArraySize];
        numberDisplays = new NumberDisplay[ArraySize];

        // Create Puzzle
        for (int i = 1; i < board.Length; i++)
        {
            board[i - 1] = i;
        }
        board[board.Length - 1] = 0;

        // Create Displays
        for (int i = 0; i < board.Length; i++)
        {
            numberDisplays[i] = CreateDisplay(i);
        }
    }

    public void SetWidth(TMP_InputField input)
    {
        NumRows = int.Parse(input.text);
    }
    public void SetHeight(TMP_InputField input)
    {
        NumColumns = int.Parse(input.text);
    }


    //-- PRIVATE HELPERS

    private int Row(int index)
    {
        return index / NumRows;
    }

    private int Column(int index)
    {
        return index % NumRows;
    }

    private void ResetPuzzle()
    {
        board = null;
        DestroyDisplays();
    }

    private NumberDisplay CreateDisplay(int index)
    {
        int value = board[index];
        NumberDisplay display = Instantiate(numberDisplayPrefab, this.transform).GetComponent<NumberDisplay>();

        // Calculate size relative to puzzle
        Vector2 puzzleSize = rectTransform.sizeDelta;
        float dispWidth = puzzleSize.x / NumRows;
        float dispHeight = puzzleSize.y / NumColumns;
        Vector2 displaySize = new Vector2(dispWidth - 2, dispHeight - 2);

        // Calculate position relative to puzzle
        int row = Row(index);
        int column = Column(index);
        float meanRow = NumRows / 2f;
        float meanColumn = NumColumns / 2f;
        float stepX = column - meanColumn + 0.5f;            // this is just like z-scores
        float stepY = row - meanRow + 0.5f;
        float positionX = stepX * dispWidth;
        float positionY = stepY * dispHeight * -1f;
        Vector2 displayPosition = new(positionX, positionY);

        // Set values of display
        display.SetRTSize(displaySize);
        display.SetRTPosition(displayPosition);
        display.DisplayNumber(value);

        return display;
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
