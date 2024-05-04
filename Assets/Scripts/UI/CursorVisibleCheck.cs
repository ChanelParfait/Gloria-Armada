using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorVisibleCheck : MonoBehaviour
{
    public bool shouldCursorBeVisible;

    // Update is called once per frame
    void Start()
    {
        Time.timeScale = 1;
        if (shouldCursorBeVisible == false && Cursor.visible == true)
        {
            Cursor.visible = false;
        }

        if (shouldCursorBeVisible == true && Cursor.visible == false)
        {
            Cursor.visible = true;
        }
    }

    void TurnCursorOn()
    {
        shouldCursorBeVisible = true;
    }

    void TurnCursorOff()
    {
        shouldCursorBeVisible = false;
    }
}
