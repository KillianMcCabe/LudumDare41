using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager
{
    // Apply requested cursor state
    public static void SetCursorState(CursorLockMode wantedMode)
    {
        Cursor.lockState = wantedMode;

        // Hide cursor when locking
        Cursor.visible = (CursorLockMode.Locked != wantedMode);
    }
}
