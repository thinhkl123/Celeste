using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInput : Singleton<GameInput>
{
    public bool isJumpPressed;
    public bool isDashPressed;
    public bool isGrabPressed;

    private void Update()
    {
        isJumpPressed = (Input.GetKey(KeyCode.Space));
        isDashPressed = (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftShift));
        isGrabPressed = (Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.LeftControl));
    }
}
