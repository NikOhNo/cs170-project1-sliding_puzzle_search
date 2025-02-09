using TMPro;
using UnityEngine;

public class ErrorDisplay : MonoBehaviour 
{
    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text error;

    private void Awake()
    {
        CloseDisplay();
    }

    public void CloseDisplay()
    {
        panel.SetActive(false);
    }

    public void OpenDisplay(string message)
    {
        panel.SetActive(true);
        error.text = message;
    }
}
