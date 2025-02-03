using TMPro;
using Unity.Jobs;
using UnityEngine;

public class NumberDisplay : MonoBehaviour
{
    TMP_Text text;
    RectTransform rectTransform;

    private void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetRTPosition(Vector2 position)
    {
        rectTransform.localPosition = position;
    }

    public void SetRTSize(Vector2 size)
    {
        rectTransform.sizeDelta = size;
    }

    public void DisplayNumber(int number)
    {
        if (number == 0)
        {
            text.text = string.Empty;
            return;
        }
        text.text = number.ToString();
    }
}
