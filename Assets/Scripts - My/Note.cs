using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Note : MonoBehaviour
{
    public string note;
    [SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI text;

    public void ReadNote()
    {
        canvas.SetActive(true);
        text.text = note;
        Time.timeScale = 0;
    }
}
