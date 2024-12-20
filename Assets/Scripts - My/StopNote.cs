using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopNote : MonoBehaviour
{
    public void Stop()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
